using System;
using Cmc;
using Cmc.Decl;
using JetBrains.Annotations;
using LLVMSharp;
using static System.StringComparison;
using Type = Cmc.Type;

namespace LLVM
{
	public static class GenDeclaration
	{
		public static void GenAstDeclaration(
			LLVMBuilderRef builder,
			[NotNull] Declaration element)
		{
			switch (element)
			{
				case TypeDeclaration typeDeclaration:
					break;
				case VariableDeclaration variable:
					if (variable.Type is LambdaType lambdaType)
						GenFunction(builder, variable, lambdaType);
					break;
			}
		}

		private static void GenFunction(
			LLVMBuilderRef builder,
			[NotNull] VariableDeclaration variable,
			[NotNull] LambdaType lambdaType)
		{
			var argc = (uint) lambdaType.ParamsList.Count;
			var args = new LLVMTypeRef[Math.Max(1, argc)];
			for (var i = 0; i < argc; i++)
			{
				args[i] = GetLlvmType(lambdaType.ParamsList[i]);
			}
		}

		private static LLVMTypeRef GetLlvmType([NotNull] Type lambdaTypeParam)
		{
			switch (lambdaTypeParam)
			{
				case PrimaryType primaryType:
					if (string.Equals(primaryType.Name, "f64", Ordinal))
						return LLVMSharp.LLVM.DoubleType();
					if (string.Equals(primaryType.Name, "f32", Ordinal))
						return LLVMTypeRef.FloatType();
					if (string.Equals(primaryType.Name, "i8", Ordinal))
						return LLVMTypeRef.Int8Type();
					if (string.Equals(primaryType.Name, "i16", Ordinal))
						return LLVMTypeRef.Int16Type();
					if (string.Equals(primaryType.Name, "i32", Ordinal))
						return LLVMTypeRef.Int32Type();
					if (string.Equals(primaryType.Name, "i64", Ordinal))
						return LLVMTypeRef.Int64Type();
					break;
				case SecondaryType secondaryType:
					break;
			}
			throw new NotImplementedException();
		}
	}
}