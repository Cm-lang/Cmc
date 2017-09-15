using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.ObjectRegex;
using ParserTest.Bootstrap;

namespace ParserTest
{
	[TestClass]
	public class NaiveTest
	{
		[TestMethod]
		public void TestBootstrap()
		{
			// Initialize parser
			var parser = BootstrapParser.GenParser();
			var re = DefualtToken.Token();

			// Gen tokenized words
			var tokens = re.Matches(File.ReadAllText("../selfexamine.ebnf")).Select(i => i.ToString()).ToArray();

			// Parsing
			var meta = new MetaInfo();
			Console.WriteLine(parser.Stmt.Match(
				objs: tokens,
				partial: false,
				meta: ref meta
			).Dump());
		}
	}
}