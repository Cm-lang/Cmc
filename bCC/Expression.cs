﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static bCC.PrimaryType;

#pragma warning disable 659

namespace bCC
{
	public abstract class Expression : Ast
	{
		protected Expression(MetaData metaData) : base(metaData)
		{
		}

		[NotNull]
		public abstract Type GetExpressionType();

		[CanBeNull]
		public virtual VariableExpression GetLhsExpression() => null;
	}

	public abstract class AtomicExpression : Expression
	{
		protected AtomicExpression(MetaData metaData) : base(metaData)
		{
		}
	}

	public sealed class NullExpression : Expression
	{
		public NullExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() => new PrimaryType(MetaData, NullType);

		public override IEnumerable<string> Dump() => new[] {"null expression\n"};
	}

	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;

		protected LiteralExpression(MetaData metaData, [NotNull] Type type) : base(metaData) => Type = type;

		public override Type GetExpressionType() => Type;
	}

	public class IntLiteralExpression : LiteralExpression
	{
		public static readonly int[] AcceptableLength = {8, 16, 32, 64};

		[NotNull] public readonly string Value;

		public IntLiteralExpression(
			MetaData metaData,
			[NotNull] string value,
			// FEATURE #27
			bool isSigned,
			int length = 32)
			: base(metaData, new PrimaryType(metaData, $"{(isSigned ? "i" : "u")}{length}"))
		{
			Value = value;
			// FEATURE #26
			if (!AcceptableLength.Contains(length))
				Errors.Add($"{MetaData.GetErrorHeader()}integer length of {length} is not supported");
		}

		public override IEnumerable<string> Dump() => new[] {$"literal expression [{Value}]:\n"}
			.Concat(Type.Dump().Select(MapFunc));
	}

	public class BoolLiteralExpression : LiteralExpression
	{
		public readonly bool Value;

		public BoolLiteralExpression(MetaData metaData, bool value) : base(metaData, new PrimaryType(metaData, BoolType)) =>
			Value = value;

		public override IEnumerable<string> Dump() => new[] {$"bool literal expression [{Value}]:\n"};
	}

	public class StringLiteralExpression : LiteralExpression
	{
		private readonly string _debugRepresentation;
		public readonly string Value;

		public StringLiteralExpression(MetaData metaData, string value) : base(metaData,
			new PrimaryType(MetaData.Empty, StringType))
		{
			Value = value;
			// FEATURE #23
			_debugRepresentation = Value
				.Replace("\\", "\\\\")
				.Replace("\r", "\\r")
				.Replace("\b", "\\b")
				.Replace("\a", "\\a")
				.Replace("\f", "\\f")
				.Replace("\n", "\\n")
				.Replace("\t", "\\t")
				.Replace("\"", "\\\"");
		}

		public override IEnumerable<string> Dump() => new[]
				{$"string literal expression [{_debugRepresentation}]:\n"}
			.Concat(Type.Dump().Select(MapFunc));
	}

	/// <summary>
	///   A function is a variable with the type of lambda
	///   This is the class for anonymous lambda
	/// </summary>
	public class LambdaExpression : AtomicExpression
	{
		[CanBeNull] private readonly Type _declaredType;
		[NotNull] public readonly StatementList Body;
		[NotNull] public readonly IList<VariableDeclaration> ParameterList;
		private Type _type;

		public LambdaExpression(
			MetaData metaData,
			[NotNull] StatementList body,
			// FEATURE #22
			[CanBeNull] IList<VariableDeclaration> parameterList = null,
			[CanBeNull] Type returnType = null) : base(metaData)
		{
			Body = body;
			_declaredType = returnType;
			ParameterList = parameterList ?? new List<VariableDeclaration>();
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var variableDeclaration in ParameterList)
				variableDeclaration.SurroundWith(Env);
			var bodyEnv = new Environment(Env);
			foreach (var variableDeclaration in ParameterList)
				bodyEnv.Declarations.Add(variableDeclaration);
			Body.SurroundWith(bodyEnv);
			var retTypes = Body.FindReturnStatements().Select(i =>
			{
				i.WhereToJump = this;
				return i.Expression.GetExpressionType();
			}).ToList();
			// FEATURE #24
			if (retTypes.Any(i => !Equals(i, _declaredType ?? retTypes.First())))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}ambiguous return types:\n" +
					(_declaredType != null ? $"<{_declaredType}>" : "") +
					$"[{string.Join(",", retTypes.Select(i => i.ToString()))}]");
			// FEATURE #12
			var retType = _declaredType ?? (retTypes.Count != 0
				              ? retTypes.First()
				              // FEATURE #19
				              : new PrimaryType(MetaData, NullType));
			_type = new LambdaType(MetaData, ParameterList.Select(i => i.Type).ToList(), retType);
		}

		public override Type GetExpressionType() => _type;

		public override IEnumerable<string> Dump()
		{
			return new[]
				{
					"lambda:\n",
					"  type:\n"
				}
				.Concat(GetExpressionType().Dump().Select(MapFunc2))
				.Concat(new[] {"  parameters:\n"})
				.Concat(ParameterList.SelectMany(i => i.Dump().Select(MapFunc2)))
				.Concat(new[] {"  body:\n"})
				.Concat(Body.Dump().Select(MapFunc2));
		}
	}

	public class MemberAccessExpression : AtomicExpression
	{
		[NotNull] public readonly VariableExpression Member;
		[NotNull] public readonly Expression Owner;

		public MemberAccessExpression(
			MetaData metaData,
			[NotNull] Expression owner,
			[NotNull] VariableExpression member) :
			base(metaData)
		{
			Owner = owner;
			Member = member;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Owner.SurroundWith(Env);
			Member.SurroundWith(Env);
			// FEATURE #29
			if (!(Owner.GetExpressionType() is SecondaryType type) || !type.Struct.FieldList.Contains(Member.Declaration))
				Errors.Add(MetaData.GetErrorHeader() + "invalid member access expression");
		}

		public override Type GetExpressionType() => Member.GetExpressionType();

		public override VariableExpression GetLhsExpression() => Member.GetLhsExpression();
	}

	public class VariableExpression : AtomicExpression
	{
		[NotNull] public readonly string Name;
		private Type _type;
		public VariableDeclaration Declaration;

		public VariableExpression(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration)
			{
				Declaration = variableDeclaration;
				_type = Declaration.Type;
			}
			else
				Errors.Add($"{MetaData.GetErrorHeader()}{declaration} isn't a variable");
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException("unknown type");

		public override IEnumerable<string> Dump() => new[]
			{
				$"variable expression [{Name}]:\n",
				"  type:\n"
			}
			.Concat(_type.Dump().Select(MapFunc2));

		public override VariableExpression GetLhsExpression() => this;
	}

	public class FunctionCallExpression : AtomicExpression
	{
		[NotNull] public readonly IList<Expression> ParameterList;

		[NotNull] public readonly Expression Receiver;
		private Type _type;

		public FunctionCallExpression(MetaData metaData, [NotNull] Expression receiver,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			ParameterList = parameterList;
			Receiver = receiver;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Receiver.SurroundWith(Env);
			foreach (var expression in ParameterList) expression.SurroundWith(Env);
			var hisType = Receiver.GetExpressionType() as LambdaType;
			if (null != hisType)
			{
				_type = hisType.RetType;
				// FEATURE 
				for (var i = 0; i < ParameterList.Count; i++)
					if (!Equals(ParameterList[i].GetExpressionType(), hisType.ArgsList[i]))
						Errors.Add($"{MetaData.GetErrorHeader()}type mismatch: expected {hisType.ArgsList[i]}, " +
						           $"found {ParameterList[i].GetExpressionType()}");
			}
			else
				Errors.Add(
					$"{MetaData.GetErrorHeader()}the function call receiver shoule be a function," +
					$" not {Receiver.GetExpressionType()}.");
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException();

		public override IEnumerable<string> Dump()
		{
			return new[]
				{
					"function call expression:\n",
					"  receiver:\n"
				}
				.Concat(Receiver.Dump().Select(MapFunc2))
				.Concat(new[] {"  parameters:\n"})
				.Concat(ParameterList.SelectMany(i => i.Dump().Select(MapFunc2)))
				.Concat(new[] {"  type:\n"})
				.Concat(_type.Dump().Select(MapFunc));
		}
	}
}