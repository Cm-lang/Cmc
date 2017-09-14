using System.Text;
using Cmc;
using Cmc.Decl;
using Cmc.Expr;
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
			switch (element)
			{
				case ReturnStatement returnStatement:
					var expr = (AtomicExpression) returnStatement.Expression;
					// expr should be an atomic expression
					builder.AppendLine(
						$"  ret {ConvertType(expr.GetExpressionType())} {expr.AtomicRepresentation()}");
					varName++;
					break;
				case ExpressionStatement expression:
					GenAst(builder, expression.Expression, ref varName);
					break;
				case StatementList statements:
					ulong localVarCount = 1;
					foreach (var statement in statements.Statements)
						GenAst(builder, statement, ref localVarCount);
					break;
				case Declaration declaration:
					GenAst(builder, declaration, ref varName);
					break;
			}
		}
	}
}