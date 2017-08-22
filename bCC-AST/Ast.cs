using System.Collections.Generic;

namespace bCC_AST
{
	public interface IAst
	{
	}

	public abstract class Statement : IAst
	{
	}

	public class StatementList : Statement
	{
		public readonly IList<Statement> Statements;

		public StatementList(params Statement[] statements)
		{
			Statements = statements;
		}
	}

	public abstract class Declaration : IAst
	{
	}

	public class FunctionDeclaration : Declaration
	{
	}

	public class VariableDeclaration : Declaration
	{
	}
}