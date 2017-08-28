using System.Diagnostics;

namespace Tools
{
	public static class CommandLine
	{
		public static void RunCommand(string command)
		{
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

			cmd.StandardInput.WriteLine(command);
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit();
//			Console.WriteLine(cmd.StandardOutput.ReadToEnd());
		}
	}
}