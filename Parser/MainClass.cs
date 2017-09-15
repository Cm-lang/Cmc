using System;
using System.IO;

namespace Parser
{
	public static class MainClass
	{
		public static void Main(string[] args)
		{
			// Gen source
			var source = File.ReadAllText("../selfexamine.ebnf");
			var (testfunc, path) = (Test.Manage.GetTest(args[0]), args[1]);
			var res = testfunc(source);
			Console.WriteLine(res);
			File.WriteAllText(path, res);
		}
	}
}