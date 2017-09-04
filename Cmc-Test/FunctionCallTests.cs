using System;
using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Expr;
using Cmc.Stmt;
using NUnit.Framework;
using Environment = Cmc.Core.Environment;

namespace Cmc_Test
{
	[TestFixture]
	public class FunctionCallTests
	{
		[SetUp]
		public void Init() => Errors.ErrList.Clear();

		[Test]
		public void FuncCallTest1()
		{
			var example = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "id",
					new LambdaExpression(MetaData.Empty,
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new VariableExpression(MetaData.Empty, "a"))),
						new List<VariableDeclaration>(new[]
						{
							new VariableDeclaration(MetaData.Empty, "a", type:
								new UnknownType(MetaData.Empty, "i8"))
						}))),
				new VariableDeclaration(MetaData.Empty, "gg", type:
					new UnknownType(MetaData.Empty, "i8")),
				new AssignmentStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "gg"),
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "id"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "1", true)}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Console.WriteLine(string.Join("\n", Errors.ErrList));
			Assert.IsNotEmpty(Errors.ErrList);
		}

		[Test]
		public void FuncCallTest2()
		{
			var example = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "id",
					new LambdaExpression(MetaData.Empty,
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new VariableExpression(MetaData.Empty, "a"))),
						new List<VariableDeclaration>(new[]
						{
							new VariableDeclaration(MetaData.Empty, "a", type:
								new UnknownType(MetaData.Empty, "i8"))
						}))),
				new VariableDeclaration(MetaData.Empty, "gg", isMutable: true, type:
					new UnknownType(MetaData.Empty, "i8")),
				new AssignmentStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "gg"),
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "id"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "233", true, 8)}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Console.WriteLine(string.Join("\n", Errors.ErrList));
			Assert.IsEmpty(Errors.ErrList);
		}

		/// <summary>
		///  recur fail test
		/// </summary>
		[Test]
		public void FuncCallTest3()
		{
			var lambda = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new FunctionCallExpression(MetaData.Empty,
							new VariableExpression(MetaData.Empty, "recur"),
							new List<Expression>(new[] {new VariableExpression(MetaData.Empty, "a")})))),
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "a", type:
						new UnknownType(MetaData.Empty, "i8"))
				}));
			var example = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "recurFunc",
					lambda),
				new VariableDeclaration(MetaData.Empty, "gg", isMutable: true, type:
					new UnknownType(MetaData.Empty, "i8")),
				new AssignmentStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "gg"),
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "recurFunc"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "233", true, 8)}))));
			Assert.Throws<CompilerException>(() => { example.SurroundWith(Environment.SolarSystem); });
		}

		/// <summary>
		///  recur test
		/// </summary>
		[Test]
		public void FuncCallTest4()
		{
			var lambda = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new FunctionCallExpression(MetaData.Empty,
							new VariableExpression(MetaData.Empty, "recur"),
							new List<Expression>(new[] {new VariableExpression(MetaData.Empty, "a")})))),
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "a", type:
						new UnknownType(MetaData.Empty, "i8"))
				}), new UnknownType(MetaData.Empty, "i8"));
			var example = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "recurFunc",
					lambda),
				new VariableDeclaration(MetaData.Empty, "gg", isMutable: true, type:
					new UnknownType(MetaData.Empty, "i8")),
				new AssignmentStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "gg"),
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "recurFunc"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "233", true, 8)}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
		}

		/// <summary>
		///  recur test
		///  expression:
		///  { a: i32 -> { recur(a) }() }
		/// </summary>
		[Test]
		public void FuncCallTest5()
		{
			var lambda = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new FunctionCallExpression(MetaData.Empty,
							new LambdaExpression(MetaData.Empty,
								new StatementList(MetaData.Empty,
									new ReturnStatement(MetaData.Empty,
										new FunctionCallExpression(MetaData.Empty,
											new VariableExpression(MetaData.Empty, "recur"),
											new List<Expression>(new[] {new VariableExpression(MetaData.Empty, "a")})))
								), returnType:
								new UnknownType(MetaData.Empty, "i8")),
							new List<Expression>()))),
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "a", type:
						new UnknownType(MetaData.Empty, "i8"))
				}), new UnknownType(MetaData.Empty, "i8"));
			var example = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "recurFunc",
					lambda),
				new VariableDeclaration(MetaData.Empty, "gg", isMutable: true, type:
					new UnknownType(MetaData.Empty, "i8")),
				new AssignmentStatement(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "gg"),
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "recurFunc"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "233", true, 8)}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}
	}
}