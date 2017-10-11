using System;
using System.IO;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;

namespace LLVM
{
	public static class Attr
	{
		public static ulong GlobalFunctionCount;
		public static ulong MainFunctionIndex;

		public static void Restore()
		{
			GlobalFunctionCount = 0;
			MainFunctionIndex = 0;
		}
	}

	public static class Gen
	{
		[NotNull]
		public static string Generate(
			[NotNull] string moduleName,
			[NotNull] params Declaration[] declarations)
		{
			Attr.Restore();
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			var moduleRef = LLVMSharp.LLVM.ModuleCreateWithName(moduleName);
			var builder = LLVMSharp.LLVM.CreateBuilder();
			foreach (var analyzedDeclaration in analyzedDeclarations)
				GenAstHolder.GenAst(moduleRef, builder, analyzedDeclaration);
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				var len = Constants.StringConstantLengths[i];
			}
			LLVMSharp.LLVM.DumpModule(moduleRef);
			return "";
		}

		public static void RunLlvm(
			[NotNull] string moduleName,
			[NotNull] string outputFile,
			[NotNull] params Declaration[] declarations)
		{
			var generate = Generate(moduleName, declarations);
			if (Pragma.PrintDumpInfo) declarations.ToList().ForEach(i => i.PrintDumpInfo());
			Console.WriteLine(generate);
			File.WriteAllText($"{outputFile}.ll", generate);
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.o -o {outputFile}");
		}
	}
}