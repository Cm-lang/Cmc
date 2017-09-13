using System;

namespace Parser.Test
{
	public static class Manage
	{
		public static Func<string, string> GetTest(string argv)
		{
			switch (argv)
			{
				case "se":
					// bootstrap
					return NaiveTest.SelfExamination;

				default:
					return NaiveTest.SelfExamination;
			}
		}
	}
}