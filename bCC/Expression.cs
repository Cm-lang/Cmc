using System;
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
		[NotNull] public readonly string Value;

		public IntLiteralExpression(MetaData metaData, [NotNull] string value, bool isSigned, int length = 32)
			: base(metaData, new PrimaryType(metaData, $"{(isSigned ? "i" : "u")}{length}")) => Value = value;

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
		[NotNull] public readonly StatementList Body;
		[NotNull] public readonly IList<VariableDeclaration> ParameterList;
		private Type _type;
		[CanBeNull] private readonly Type _declaredType;

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
			Body.SurroundWith(Env);
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
		[NotNull] public readonly Expression Member;
		[NotNull] public readonly Expression Owner;

		public MemberAccessExpression(MetaData metaData, [NotNull] Expression owner, [NotNull] Expression member) :
			base(metaData)
		{
			Owner = owner;
			Member = member;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Owner.SurroundWith(Env);
			// TODO check if it's a valid member
			Member.SurroundWith(Env);
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
				Errors.Add($"{MetaData.GetErrorHeader()} [internal error] declaration is not a variable declaration");
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException();

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
			// TODO check parameter type
			var hisType = Receiver.GetExpressionType();
			if (hisType is LambdaType lambdaType) _type = lambdaType.RetType;
			else
				Errors.Add($"{MetaData.GetErrorHeader()}the function call receiver shoule be a function, not {hisType}.");
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