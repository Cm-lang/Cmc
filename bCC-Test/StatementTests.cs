using System;
using bCC;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
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

		[SetUp]
		public void Init() => Errors.ErrList.Clear();

		[Test]
		public void StatementTest3()
		{
			Errors.ErrList.Clear();
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, var1,
					new BoolLiteralExpression(MetaData.DefaultMetaData, true)),
				new WhileStatement(MetaData.DefaultMetaData,
					new VariableExpression(MetaData.DefaultMetaData, var1),
					new StatementList(MetaData.DefaultMetaData,
						new AssignmentStatement(MetaData.DefaultMetaData,
							new VariableExpression(MetaData.DefaultMetaData, var1),
							new BoolLiteralExpression(MetaData.DefaultMetaData, false)))));
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.AreEqual(1, Errors.ErrList.Count);
			foreach (var s in Errors.ErrList) Console.WriteLine(s);
		}
	}
}