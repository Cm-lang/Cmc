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
		public readonly IList<Statement> Statements = new List<Statement>();
	}

	public class FunctionDeclaration : Statement
	{
		
	}
}