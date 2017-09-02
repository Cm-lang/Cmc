using System;
using Cmc;
using Cmc.Core;
using Cmc.Expression;
using Cmc.Statement;
using NUnit.Framework;
using Environment = Cmc.Core.Environment;

namespace Cmc_Test
{
	[TestFixture]
	public class StatementTests
	{
		[SetUp]
		public void Init()
		{
			Errors.ErrList.Clear();
		}

		/// <summary>
		///     simplest test
		/// </summary>
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.Empty,
					new Statement(MetaData.Empty),
					new Statement(MetaData.Empty)).Statements)
				stmt.PrintDumpInfo();
		}

		/// <summary>
		///     check for condition type
		/// </summary>
		[Test]
		public void StatementTest2()
		{
			var stmt = new IfStatement(
				MetaData.Empty,
				new BoolLiteralExpression(MetaData.Empty, false),
				new StatementList(MetaData.Empty)
			);
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
			var stmt2 = new IfStatement(
				MetaData.Empty,
				new NullExpression(MetaData.Empty),
				new StatementList(MetaData.Empty)
			);
			stmt2.SurroundWith(Environment.SolarSystem);
			Console.WriteLine("");
			Console.WriteLine("");
			Assert.IsNotEmpty(Errors.ErrList);
			foreach (var s in Errors.ErrList)
				Console.WriteLine(s);
			stmt2.PrintDumpInfo();
		}

		/// <summary>
		///     check for mutability
		/// </summary>
		[Test]
		public void StatementTest3()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, var1,
					new BoolLiteralExpression(MetaData.Empty, true)),
				new WhileStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, var1),
					new StatementList(MetaData.Empty,
						new AssignmentStatement(MetaData.Empty,
							new VariableExpression(MetaData.Empty, var1),
							new BoolLiteralExpression(MetaData.Empty, false)))));
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsNotEmpty(Errors.ErrList);
			Errors.PrintErrorInfo();
		}

		/// <summary>
		///     type check (when it's correct)
		/// </summary>
		[Test]
		public void StatementTest4()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, var1,
					new BoolLiteralExpression(MetaData.Empty, true), true),
				new WhileStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, var1),
					new StatementList(MetaData.Empty,
						new AssignmentStatement(MetaData.Empty,
							new VariableExpression(MetaData.Empty, var1),
							new BoolLiteralExpression(MetaData.Empty, false)))));
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}

		/// <summary>
		///     type check (when it's nulltype)
		/// </summary>
		[Test]
		public void StatementTest5()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, var1,
					new BoolLiteralExpression(MetaData.Empty, true), true),
				new WhileStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, var1),
					new StatementList(MetaData.Empty,
						new AssignmentStatement(MetaData.Empty,
							new VariableExpression(MetaData.Empty, var1),
							new NullExpression(MetaData.Empty)))));
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}

		/// <summary>
		///     type check (when it's incorrect)
		/// </summary>
		[Test]
		public void StatementTest6()
		{
			const string var1 = "variableOne";
			var stmt = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, var1,
					new BoolLiteralExpression(MetaData.Empty, true), true),
				new WhileStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, var1),
					new StatementList(MetaData.Empty,
						new AssignmentStatement(MetaData.Empty,
							new VariableExpression(MetaData.Empty, var1),
							new IntLiteralExpression(MetaData.Empty, "123", true)))));
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsNotEmpty(Errors.ErrList);
			Errors.PrintErrorInfo();
		}
	}
}