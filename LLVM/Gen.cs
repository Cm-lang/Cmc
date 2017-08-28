using System.Diagnostics;
using System.IO;
using bCC;
using bCC.Core;

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
			var cmd = new Process
			{
				StartInfo =
				{
					FileName = "cmd.exe",
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					UseShellExecute = false
				}
			};
			cmd.Start();

			cmd.StandardInput.WriteLine($"llc-4.0 {outputFile}");
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit();
//			Console.WriteLine(cmd.StandardOutput.ReadToEnd());
		}
	}
}