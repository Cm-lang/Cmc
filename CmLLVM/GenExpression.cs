using Cmc.Expr;
using JetBrains.Annotations;
using LLVMSharp;

namespace CmLLVM
{
	public static class GenExpression
	{
		public static void StoreAtomicExpression(
			LLVMBuilderRef builder,
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
			LLVMModuleRef module,
			LLVMBuilderRef builder,
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