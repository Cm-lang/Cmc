using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;
using static LLVM.GenAstHolder;
using System.Linq;

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
			[NotNull] params Declaration[] declarations)
		{
			Attr.Restore();
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				var len = Constants.StringConstantLengths[i];
			}
			return ""; // TODO
		}

		public static void RunLlvm(
			[NotNull] string outputFile,
			[NotNull] params Declaration[] declarations)
		{
			var generate = Generate(declarations);
			if (Pragma.PrintDumpInfo) declarations.ToList().ForEach(i => i.PrintDumpInfo());
			Console.WriteLine(generate);
			File.WriteAllText($"{outputFile}.ll", generate);
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.o -o {outputFile}");
		}
	}
}