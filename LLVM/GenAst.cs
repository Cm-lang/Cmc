using System.Text;
using Cmc;
using Cmc.Core;
using Cmc.Expr;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenAstHolder
	{
		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="builder">the string builder used to append ir</param>
		/// <param name="element">the ast element waiting to be generated</param>
		/// <param name="varName">local variable counter</param>
		public static void GenAst(
			[NotNull] StringBuilder builder,
			[NotNull] Ast element,
			ref ulong varName)
		{
			if (element is EmptyStatement) return;
			// optimization
			if (element.OptimizedStatementList != null) element = element.OptimizedStatementList;
			if (element is Expression expression)
				GenExpression.GenAstExpression(builder, expression, ref varName);
			else if (element is Declaration declaration)
				GenDeclaration.GenAstDeclaration(builder, declaration, ref varName);
			else if (element is Statement statement)
				GenStatement.GenAstStatement(builder, statement, ref varName);
		}
	}
}