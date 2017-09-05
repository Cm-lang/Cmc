using System.Text;
using Cmc;
using Cmc.Stmt;
using JetBrains.Annotations;
using static LLVM.GenAstHolder;
using static LLVM.GenDeclaration;
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
				GenAst(builder, expr, ref varName);
				builder.AppendLine(
					$"  ret {ConvertType(expr.GetExpressionType())} %var{varName}");
				varName++;
			}
			else if (element is StatementList statements)
			{
				ulong localVarCount = 1;
				foreach (var statement in statements.Statements)
					GenAst(builder, statement, ref localVarCount);
			}
			// this should rarely happen
			else if (element is Declaration declaration)
				GenAst(builder, declaration, ref varName);
		}
	}
}