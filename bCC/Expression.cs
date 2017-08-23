using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#pragma warning disable 659

namespace bCC
{
	public abstract class Expression : Ast
	{
		[NotNull]
		public abstract IList<Declaration> GetDependencies();

		[NotNull]
		public abstract Type GetExpressionType();

		protected Expression(MetaData metaData) : base(metaData)
		{
		}
	}

	public abstract class AtomicExpression : Expression
	{
		protected AtomicExpression(MetaData metaData) : base(metaData)
		{
		}
	}

	public sealed class NullExpression : Expression
	{
		public const string NullType = "nulltype";

		public NullExpression(MetaData metaData) : base(metaData)
		{
		}

		public override IList<Declaration> GetDependencies() => new List<Declaration>();
		public override Type GetExpressionType() => new SecondaryType(MetaData, NullType);
	}

	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;
		protected LiteralExpression(MetaData metaData, [NotNull] Type type) : base(metaData) => Type = type;
		public override IList<Declaration> GetDependencies() => new List<Declaration>();
		public override Type GetExpressionType() => Type;
	}

	public class IntLiteralExpression : LiteralExpression
	{
		[NotNull] public readonly string Value;

		public IntLiteralExpression(MetaData metaData, [NotNull] string value, bool isSigned, int length = 32)
			: base(metaData, new SecondaryType(metaData, (isSigned ? "i" : "u") + length)) => Value = value;
	}

	/// <summary>
	/// A function is a variable with the type of lambda
	/// This is the class for anonymous lambda
	/// </summary>
	public class Lambda : AtomicExpression
	{
		[NotNull] public readonly StatementList Body;
		private Type _type;
		private Environment _env;

		public override Environment Env
		{
			get => _env;
			[NotNull]
			set
			{
				_env = value;
				Body.Env = Env;
				// FEATURE #12
				_type = Body.Statements.Last() is ReturnStatement ret
					? ret.Expression.GetExpressionType()
					: new SecondaryType(MetaData, "void");
			}
		}

		public override IList<Declaration> GetDependencies() =>
			Body.Statements.SelectMany(i => i.GetDependencies()).ToList();

		public override Type GetExpressionType() => _type;

		public Lambda(MetaData metaData, StatementList body) : base(metaData)
		{
			Body = body;
		}
	}

	public class VariableExpression : AtomicExpression
	{
		[NotNull] public readonly string Name;

		public override IList<Declaration> GetDependencies() => new List<Declaration> {new Declaration(MetaData, Name)};

		public override Type GetExpressionType()
		{
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration) return variableDeclaration.Type;
			Errors.Add(MetaData.GetErrorHeader() + "Wtf");
			throw new CompilerException();
		}

		public VariableExpression(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;
	}

	public class FunctionCallExpression : AtomicExpression
	{
		[NotNull] public readonly Expression Receiver;
		[NotNull] public readonly IList<Expression> ParameterList;

		public FunctionCallExpression(MetaData metaData, [NotNull] Expression receiver,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			ParameterList = parameterList;
			Receiver = receiver;
		}

		public override IList<Declaration> GetDependencies() =>
			ParameterList.SelectMany(param => param.GetDependencies()).ToList();

		public override Type GetExpressionType()
		{
			var hisType = Receiver.GetExpressionType();
			if (hisType is LambdaType lambdaType) return lambdaType.RetType;
			Errors.Add(MetaData.GetErrorHeader() + "the function call receiver shoule be a function, not " +
			           hisType + ".");
			throw new CompilerException();
		}
	}
}