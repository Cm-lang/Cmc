using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;

#pragma warning disable 659

namespace bCC_AST
{
	public abstract class Expression : IAst
	{
		public Environment Env;
		public abstract IList<Declaration> GetDependencies();
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

	/// <summary>
	/// A function is a variable with the type of lambda
	/// This is the class for anonymous lambda
	/// </summary>
	public class Lambda : AtomicExpression
	{
		public readonly StatementList Body;

		public override IList<Declaration> GetDependencies() =>
			Body.Statements.SelectMany(i => i.GetDependencies()).ToList();

		public override Type GetExpressionType()
		{
			throw new System.NotImplementedException();
		}

		public Lambda(MetaData metaData, StatementList body) : base(metaData)
		{
			Body = body;
		}
	}

	public class VariableExpression : AtomicExpression
	{
		public readonly string Name;

		public override IList<Declaration> GetDependencies() => new List<Declaration> {new Declaration(MetaData, Name)};

		public override Type GetExpressionType()
		{
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration) return variableDeclaration.Type;
			Errors.Add(MetaData.GetErrorHeader() + "Wtf");
			throw new CompilerException();
		}

		public VariableExpression(MetaData metaData, string name) : base(metaData)
		{
			Name = name;
		}
	}

	public class FunctionCallExpression : AtomicExpression
	{
		public readonly Expression Receiver;
		public readonly IList<Expression> ParameterList;

		public FunctionCallExpression(MetaData metaData, Expression receiver, IList<Expression> parameterList) :
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
			Errors.Add(MetaData.GetErrorHeader() + "the function call receiver shoule be a lambda. Currently it's " +
			           hisType + ".");
			throw new CompilerException();
		}
	}
}