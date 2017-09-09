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
		public static ulong GlobalVarCount;

		public static string Generate(
			[NotNull] params Declaration[] declarations)
		{
			Attr.Restore();
			var core = new Core();
			var builder = new StringBuilder();
			var analyzedDeclarations = core.Analyze(declarations);
			builder.AppendLine("declare i32 @puts(i8*) #1");
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				var len = Constants.StringConstantLengths[i];
				builder.AppendLine(
					$"@.str{i}=private unnamed_addr constant [{len} x i8] c\"{str}\", align 1");
			}
			var varName = 0ul;
			foreach (var analyzedDeclaration in analyzedDeclarations)
			{
				GenAst(builder, analyzedDeclaration, ref varName);
				varName++;
			}
			for (var i = 0ul; i < Attr.GlobalFunctionCount; i++)
				if (i == Attr.MainFunctionIndex)
					builder.AppendLine(
						$"attributes #{i} = " +
						"{ noinline norecurse uwtable " +
						"\"correctly-rounded-divide-sqrt-fp-math\"=\"false\" " +
						"\"disable-tail-calls\"=\"false\" " +
						"\"less-precise-fpmad\"=\"false\" " +
						"\"no-frame-pointer-elim\"=\"true\" " +
						"\"no-frame-pointer-elim-non-leaf\" " +
						"\"no-infs-fp-math\"=\"false\" " +
						"\"no-jump-tables\"=\"false\" " +
						"\"no-nans-fp-math\"=\"false\" " +
						"\"no-signed-zeros-fp-math\"=\"false\" " +
						"\"no-trapping-math\"=\"false\" " +
						"\"stack-protector-buffer-size\"=\"8\" " +
						"\"target-cpu\"=\"x86-64\" " +
						"\"target-features\"=\"+fxsr,+mmx,+sse,+sse2,+x87\" " +
						"\"unsafe-fp-math\"=\"false\" " +
						"\"use-soft-float\"=\"false\" }");
				else
					// TODO add attributes
					builder.AppendLine(
						$"attributes #{i} = " +
						"{ " +
						" }");
			return builder.ToString();
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