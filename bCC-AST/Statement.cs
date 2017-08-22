using System.Collections.Generic;
using System.Linq;

namespace bCC_AST
{
	public class Statement : IAst
	{
		public virtual IList<Declaration> GetDependencies() => new List<Declaration>();

		public Statement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class StatementList : Statement
	{
		public readonly IList<Statement> Statements;

		public override IList<Declaration> GetDependencies() =>
			Statements.SelectMany(statement => statement.GetDependencies()).ToList();

		public StatementList(MetaData metaData, params Statement[] statements) : base(metaData)
		{
			Statements = statements;
		}
	}
}