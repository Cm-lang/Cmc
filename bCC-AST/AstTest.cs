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
			foreach (var stmt in new StatementList(MetaData.DefaultMetaData,
				new Statement(MetaData.DefaultMetaData),
				new Statement(MetaData.DefaultMetaData)).Statements)
				Console.WriteLine(stmt);
		}
	}
}