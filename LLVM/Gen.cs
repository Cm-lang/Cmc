using System.IO;
using System.Text;
using bCC;
using bCC.Core;
using bCC.Expression;
using JetBrains.Annotations;
using Tools;
using static LLVM.GenAstHolder;

namespace LLVM
{
	public static class Gen
	{
		public static ulong GlobalVarCount;

		public static string Generate(
			[NotNull] params Declaration[] declarations)
		{
			var core = new Core();
			var builder = new StringBuilder();
			var analyzedDeclarations = core.Analyze(declarations);
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				var len = Constants.StringConstantLengths[i];
				builder.AppendLine(
					$"@.str{i}=private unnamed_addr constant [{len} x i8] c\"{str}\", align 1");
			}
			ulong varName = 0;
			foreach (var analyzedDeclaration in analyzedDeclarations)
			{
				GenAst(builder, analyzedDeclaration, ref varName);
				varName++;
			}
			return builder.ToString();
		}

		public static void RunLlvm(
			[NotNull] string outputFile,
			[NotNull] params Declaration[] declarations)
		{
			File.WriteAllText(outputFile, Generate(declarations));
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.ll -o {outputFile}");
		}
	}
}