using System.IO;
using bCC;
using bCC.Core;
using Tools;

namespace LLVM
{
	public class Gen
	{
		public void Generate(
			string outputFile,
			params Declaration[] declarations)
		{
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			// TODO run code gen
			File.WriteAllText(outputFile, "");
			CommandLine.RunCommand($"llc-4.0 {outputFile}");
		}
	}
}