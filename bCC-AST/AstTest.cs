using System;
using NUnit.Framework;

namespace bCC_AST
{
	[TestFixture]
	public class AstTest
	{
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in new StatementList(new Statement(), new Statement()).Statements)
				Console.WriteLine(stmt);
		}
	}
}