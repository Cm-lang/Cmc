using System.Collections.Generic;

namespace bCC_AST
{
	public class Statement : IAst
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class StatementList : Statement
	{
		public readonly IList<Statement> Statements;

		public StatementList(MetaData metaData, params Statement[] statements) : base(metaData)
		{
			Statements = statements;
		}
	}
}