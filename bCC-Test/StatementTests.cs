using System;
using bCC;
using NUnit.Framework;
using Environment = bCC.Environment;

namespace bCC_Test
{
	[TestFixture]
	public class StatementTests
	{
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.DefaultMetaData,
					new Statement(MetaData.DefaultMetaData),
					new Statement(MetaData.DefaultMetaData)).Statements)
				stmt.PrintDumpInfo();
		}


		[Test]
		public void StatementTest2()
		{
			var stmt = new IfStatement(
				MetaData.DefaultMetaData,
				new BoolLiteralExpression(MetaData.DefaultMetaData, false),
				new StatementList(MetaData.DefaultMetaData)
			);
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
			var stmt2 = new IfStatement(
				MetaData.DefaultMetaData,
				new NullExpression(MetaData.DefaultMetaData),
				new StatementList(MetaData.DefaultMetaData)
			);
			stmt2.SurroundWith(new Environment());
			Console.WriteLine("");
			Console.WriteLine("");
			Assert.IsNotEmpty(Errors.ErrList);
			foreach (var s in Errors.ErrList)
				Console.WriteLine(s);
			stmt2.PrintDumpInfo();
		}
	}
}