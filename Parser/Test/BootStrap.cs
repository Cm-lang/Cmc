using System;
using System.Linq;
using Parser.LanguageTest.Bootstrap;
using Parser.ObjectRegex;

namespace Parser.Test
{
	public class AssertError : Exception
	{
		public AssertError(string info) : base(info)
		{
		}
	}

	public static class NaiveTest
	{
		public static string SelfExamination(string source)
		{
			// Initialize parser
			var parser = LanguageTest.Bootstrap.Parser.GenParser();
			var re = DefualtToken.Token();

			// Gen tokenized words
			var tokens = re.Matches(source).Select(i => i.ToString()).ToArray();

			// Parsing
			var meta = new MetaInfo();
			return parser.Stmt.Match(
				objs: tokens,
				partial: false,
				meta: ref meta
			).Dump();
		}
	}
}