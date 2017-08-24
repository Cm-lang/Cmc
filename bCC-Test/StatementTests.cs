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
		/// <summary>
		/// simplest test
		/// </summary>
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.DefaultMetaData,
					new Statement(MetaData.DefaultMetaData),
					new Statement(MetaData.DefaultMetaData)).Statements)
				stmt.PrintDumpInfo();
		}

		/// <summary>
		/// check for condition type
		/// </summary>
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

		/// <summary>
		/// check for mutability
		/// </summary>
		[Test]
		public void StatementTest3()
		{
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
			Assert.IsNotEmpty(Errors.ErrList);
			foreach (var s in Errors.ErrList) Console.WriteLine(s);
		}

		/// <summary>
		/// type check (when it's correct)
		/// </summary>
		[Test]
		public void StatementTest4()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, var1,
					new BoolLiteralExpression(MetaData.DefaultMetaData, true), true),
				new WhileStatement(MetaData.DefaultMetaData,
					new VariableExpression(MetaData.DefaultMetaData, var1),
					new StatementList(MetaData.DefaultMetaData,
						new AssignmentStatement(MetaData.DefaultMetaData,
							new VariableExpression(MetaData.DefaultMetaData, var1),
							new BoolLiteralExpression(MetaData.DefaultMetaData, false)))));
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}

		/// <summary>
		/// type check (when it's nulltype)
		/// </summary>
		[Test]
		public void StatementTest5()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, var1,
					new BoolLiteralExpression(MetaData.DefaultMetaData, true), true),
				new WhileStatement(MetaData.DefaultMetaData,
					new VariableExpression(MetaData.DefaultMetaData, var1),
					new StatementList(MetaData.DefaultMetaData,
						new AssignmentStatement(MetaData.DefaultMetaData,
							new VariableExpression(MetaData.DefaultMetaData, var1),
							new NullExpression(MetaData.DefaultMetaData)))));
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}

		/// <summary>
		/// type check (when it's incorrect)
		/// </summary>
		[Test]
		public void StatementTest6()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, var1,
					new BoolLiteralExpression(MetaData.DefaultMetaData, true), true),
				new WhileStatement(MetaData.DefaultMetaData,
					new VariableExpression(MetaData.DefaultMetaData, var1),
					new StatementList(MetaData.DefaultMetaData,
						new AssignmentStatement(MetaData.DefaultMetaData,
							new VariableExpression(MetaData.DefaultMetaData, var1),
							new IntLiteralExpression(MetaData.DefaultMetaData, "123", true)))));
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.IsNotEmpty(Errors.ErrList);
		}
	}
}