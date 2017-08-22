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

	public class IfStatement : Statement
	{
		public readonly Expression Condition;
		public readonly StatementList IfStatementList;
		public readonly StatementList ElseStatementList;

		public IfStatement(
			MetaData metaData,
			Expression condition,
			StatementList ifStatementList,
			StatementList elseStatementList = null) : base(metaData)
		{
			Condition = condition;
			IfStatementList = ifStatementList;
			ElseStatementList = elseStatementList;
		}
	}
}