using System;
using System.Diagnostics;

namespace Cmc.Core
{
	public static class CommandLine
	{
		/// <summary>
		///  Will select os automatically
		/// </summary>
		public static void RunCommand(string command)
		{
			Console.WriteLine(command);
			var cmd = new Process
			{
				StartInfo =
				{
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					UseShellExecute = false
				}
			};
			var platform = System.Environment.OSVersion.Platform;
			switch (platform)
			{
				case PlatformID.Xbox:
					break;
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.WinCE:
					cmd.StartInfo.FileName = "cmd.exe";
					break;
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					cmd.StartInfo.FileName = "sh";
					break;
				default:
					throw new ArgumentOutOfRangeException(platform.ToString(), "unknown os!");
			}
			cmd.Start();

			cmd.StandardInput.WriteLine(command);
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit();
			Console.WriteLine(cmd.StandardOutput.ReadToEnd());
		}
	}
}