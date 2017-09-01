using System.Text;
using Cmc;
using Cmc.Statement;
using JetBrains.Annotations;
using static LLVM.GenExpression;
using static LLVM.TypeConverter;

namespace LLVM
{
	public class GenStatement
	{
		public static void GenAstStatement(
			[NotNull] StringBuilder builder,
			[NotNull] Statement element,
			ref ulong varName)
		{
			if (element is ReturnStatement returnStatement)
			{
				var expr = returnStatement.Expression;
				GenAstExpression(builder, expr, ref varName);
				builder.AppendLine(
					$"  ret {ConvertType(expr.GetExpressionType())} %{varName}");
				varName++;
			}
			else if (element is StatementList statements)
			{
				ulong localVarCount = 1;
				foreach (var statement in statements.Statements)
				{
					GenAstStatement(builder, statement, ref localVarCount);
					localVarCount++;
				}
			}
			// this should rarely happen
			else if (element is Declaration declaration)
				GenDeclaration.GenAstDeclaration(builder, declaration, ref varName);
		}
	}
}