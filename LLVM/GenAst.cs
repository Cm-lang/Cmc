using System.Text;
using Cmc;
using Cmc.Core;
using Cmc.Expression;
using Cmc.Statement;
using JetBrains.Annotations;
using static LLVM.TypeConverter;

namespace LLVM
{
	public static class GenAstHolder
	{
		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="builder">the string builder used to append ir</param>
		/// <param name="element">the ast element waiting to be generated</param>
		/// <param name="varName"></param>
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
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else 			else if (element is ReturnStatement returnStatement)
			{
				var expr = returnStatement.Expression;
				GenAst(builder, expr, ref varName);
				builder.AppendLine(
					$"ret {ConvertType(expr.GetExpressionType())} %{varName}");
				varName++;
			}
			else if (element is StatementList statements)
			{
				ulong localVarCount = 1;
				foreach (var statement in statements.Statements)
				{
					GenAst(builder, statement, ref localVarCount);
					localVarCount++;
				}
			}
		}
	}
}