using System.Text;
using Cmc.Decl;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenStatement
	{
		public static void GenAstStatement(
			[NotNull] StringBuilder builder,
			[NotNull] Statement element)
		{
			switch (element)
			{
				case ReturnStatement returnStatement:
					break;
				case ExpressionStatement expression:
					break;
				case StatementList statements:
					break;
				case Declaration declaration:
					break;
			}
		}
	}
}