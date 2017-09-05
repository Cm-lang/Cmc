using System.Text;
using Cmc.Expr;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenExpression
	{
		private static void StoreAtomicExpression(
			[NotNull] StringBuilder builder,
			[NotNull] AtomicExpression expression,
			ref ulong varName)
		{
			if (expression is IntLiteralExpression integer)
				builder.AppendLine(
					$"  store {integer.Type} {integer.Value}, {integer.Type}* %{varName}, align {integer.Type.Align}");
			else if (expression is BoolLiteralExpression boolean)
				builder.AppendLine(
					$"  store i8 {boolean.ValueToInt()}, i8* %{varName}, align 1");
			else if (expression is VariableExpression variable)
				builder.AppendLine(
					$"  store {variable.GetExpressionType()} %{}, {}, align {variable.GetExpressionType().Align}");
		}

		public static void GenAstExpression(
			[NotNull] StringBuilder builder,
			[NotNull] Expression element,
			ref ulong varName)
		{
			if (element is StringLiteralExpression str)
				builder.AppendLine(
					$"  store i8* getelementptr inbounds ([{str.Length} x i8]," +
					$"[{str.Length} x i8]* @.str{str.ConstantPoolIndex}, i32 0, i32 0)," +
					$"i8** %{varName}, align 8");
			else if (element is AtomicExpression expression)
				StoreAtomicExpression(builder, expression, ref varName);
			else if (element is FunctionCallExpression functionCall)
			{
				// TODO localize paramters
				// TODO localize function name
				builder.AppendLine(
					$"  %{varName} = call i32 @puts(i8* %6)");
				varName++;
			}
		}
	}
}