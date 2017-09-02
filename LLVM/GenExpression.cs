using System.Text;
using Cmc.Expression;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenExpression
	{
		public static void GenAstExpression(
			[NotNull] StringBuilder builder,
			[NotNull] Expression element,
			ref ulong varName)
		{
			// ReSharper disable once InvertIf
			if (element is LiteralExpression expression)
			{
				if (expression is StringLiteralExpression str)
					builder.AppendLine(
						$"  store i8* getelementptr inbounds ([{str.Length} x i8]," +
						$"[{str.Length} x i8]* @.str{str.ConstantPoolIndex}, i32 0, i32 0)," +
						$"i8** %{varName}, align 8");
				else if (expression is IntLiteralExpression integer)
					builder.AppendLine(
						$"  store {integer.Type} {integer.Value}, {integer.Type}* %{varName}, align {integer.Type.Align}");
			}
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