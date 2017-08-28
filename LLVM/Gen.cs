using System.IO;
using bCC;
using bCC.Core;
using Tools;

namespace LLVM
{
	public static class Gen
	{
		public static void Generate(
			string outputFile,
			params Declaration[] declarations)
		{
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			// TODO run code gen
			File.WriteAllText(outputFile, "");
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.ll -o {outputFile}");
		}

		public static void Main(string[] args)
		{
			Generate(
				"out"
			);
		}
	}
}