using System.Text;
using Cmc.Expr;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenExpression
	{
		public static void StoreAtomicExpression(
			[NotNull] StringBuilder builder,
			[NotNull] AtomicExpression expression)
		{
			switch (expression)
			{
				case IntLiteralExpression integer:
				case BoolLiteralExpression boolean:
				case VariableExpression variable:
					break;
			}
		}

		public static void GenAstExpression(
			[NotNull] StringBuilder builder,
			[NotNull] Expression element)
		{
			switch (element)
			{
				case StringLiteralExpression str:
					break;
				case AtomicExpression expression:
					break;
				case FunctionCallExpression functionCall:
					// function callee and parameters should already be splitted
					// into atomic expressions
					break;
			}
		}
	}
}