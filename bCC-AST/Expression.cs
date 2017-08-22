using System.Collections.Generic;
using System.Linq;

namespace bCC_AST
{
	public abstract class Expression : IAst
	{
		public abstract IList<Declaration> GetDependencies();

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

	public class VariableExpression : AtomicExpression
	{
		public readonly string Name;

		public override IList<Declaration> GetDependencies() => new List<Declaration> {new Declaration(MetaData, Name)};

		public VariableExpression(MetaData metaData, string name) : base(metaData) => Name = name;
	}

	public class FunctionCallExpression : AtomicExpression
	{
		public readonly string Name;
		public readonly IList<Expression> ParameterList;

		public FunctionCallExpression(MetaData metaData, string name, IList<Expression> parameterList) : base(metaData)
		{
			ParameterList = parameterList;
			Name = name;
		}

		public override IList<Declaration> GetDependencies() =>
			ParameterList.SelectMany(param => param.GetDependencies()).ToList();
	}
}