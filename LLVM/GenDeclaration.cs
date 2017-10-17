using System;
using System.Linq;
using Cmc;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;
using LLVMSharp;
using static System.StringComparison;
using Type = Cmc.Type;

namespace LLVM
{
	public static class GenDeclaration
	{
		public static void GenAstDeclaration(
			LLVMModuleRef module,
			LLVMBuilderRef builder,
			[NotNull] Declaration element)
		{
			switch (element)
			{
				case TypeDeclaration typeDeclaration:
					break;
				case VariableDeclaration variable:
					if (variable.Type is LambdaType lambdaType)
						GenFunction(module, builder, variable, lambdaType);
					break;
			}
		}

		private static void GenFunction(
			LLVMModuleRef module,
			LLVMBuilderRef builder,
			[NotNull] VariableDeclaration variable,
			[NotNull] LambdaType lambdaType)
		{
			var function = LLVMSharp.LLVM.AddFunction(module, variable.Name, GetLlvmType(lambdaType));
			LLVMSharp.LLVM.PositionBuilderAtEnd(builder, LLVMSharp.LLVM.AppendBasicBlock(function, "entry"));
			GenAstHolder.GenAst(module, builder, (LambdaExpression) variable.Expression);
			LLVMSharp.LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
		}

		private static LLVMTypeRef GetLlvmType([NotNull] Type lambdaTypeParam)
		{
			switch (lambdaTypeParam)
			{
				case PrimaryType primaryType:
					LLVMTypeRef ret;
					if (string.Equals(primaryType.Name, "f64", Ordinal))
						ret = LLVMSharp.LLVM.DoubleType();
					if (string.Equals(primaryType.Name, "f32", Ordinal))
						ret = LLVMTypeRef.FloatType();
					if (string.Equals(primaryType.Name, "i8", Ordinal))
						ret = LLVMTypeRef.Int8Type();
					if (string.Equals(primaryType.Name, "i16", Ordinal))
						ret = LLVMTypeRef.Int16Type();
					if (string.Equals(primaryType.Name, "i32", Ordinal))
						ret = LLVMTypeRef.Int32Type();
					if (string.Equals(primaryType.Name, "i64", Ordinal))
						ret = LLVMTypeRef.Int64Type();
					return ret;
				case LambdaType lambdaType:
					return LLVMTypeRef.FunctionType(GetLlvmType(lambdaType.RetType), (
						from type in lambdaType.ParamsList
						select GetLlvmType(type)
					).ToArray(), false);
				case SecondaryType secondaryType:
					break;
			}
			throw new NotImplementedException();
		}
	}
}