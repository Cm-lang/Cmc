using System;
using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Cmc.Core.Environment;

namespace CmcTest
{
	[TestClass]
	public class FunctionCallTests
	{
		/// <summary>
		///  id function
		///  let id = { a: i8 -> a }
		/// </summary>
		private static VariableDeclaration IdDeclaration =>
			new VariableDeclaration(MetaData.Empty, "id",
				new LambdaExpression(MetaData.Empty,
					new StatementList(MetaData.Empty,
						new ReturnStatement(MetaData.Empty,
							new VariableExpression(MetaData.Empty, "a"))),
					new List<VariableDeclaration>(new[]
					{
						new VariableDeclaration(MetaData.Empty, "a", type:
							new UnknownType(MetaData.Empty, "i8"))
					})));

		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();

		[TestMethod]
		public void FuncCallTest1()
		{
			var example = new StatementList(MetaData.Empty,
				IdDeclaration,
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
			Assert.IsTrue(0 != Errors.ErrList.Count);
		}

		[TestMethod]
		public void FuncCallTest2()
		{
			var example = new StatementList(MetaData.Empty,
				IdDeclaration,
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
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		/// <summary>
		///  recur fail test
		/// </summary>
		[TestMethod]
		public void FuncCallTest3()
		{
			var lambda = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new FunctionCallExpression(MetaData.Empty,
							new VariableExpression(MetaData.Empty, "recur"),
							new List<Expression>(new[]
							{
								new VariableExpression(MetaData.Empty, "a")
							})))),
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
						new List<Expression>(new[]
						{
							new IntLiteralExpression(MetaData.Empty, "233", true, 8)
						}))));
			Assert.ThrowsException<CompilerException>(() => { example.SurroundWith(Environment.SolarSystem); });
		}

		/// <summary>
		///  recur test
		/// </summary>
		[TestMethod]
		public void FuncCallTest4()
		{
			var lambda = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new FunctionCallExpression(MetaData.Empty,
							new VariableExpression(MetaData.Empty, "recur"),
							new List<Expression>(new[]
							{
								new VariableExpression(MetaData.Empty, "a")
							})))),
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
						new List<Expression>(new[]
						{
							new IntLiteralExpression(MetaData.Empty, "233", true, 8)
						}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
		}

		/// <summary>
		///  recur test
		///  expression:
		///  i8 { a: i32 -> i8 { recur(a) }() }
		/// </summary>
		[TestMethod]
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
											new List<Expression>(new[]
											{
												new VariableExpression(MetaData.Empty, "a")
											})))
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
						new List<Expression>(new[]
						{
							new IntLiteralExpression(MetaData.Empty, "233", true, 8)
						}))));
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		[TestMethod]
		public void ExpressionSplittingTest1()
		{
			var expr = new StatementList(MetaData.Empty,
				IdDeclaration,
				new ExpressionStatement(MetaData.Empty,
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "id"),
						new List<Expression>(new[]
						{
							new FunctionCallExpression(MetaData.Empty,
								new VariableExpression(MetaData.Empty, "id"),
								new List<Expression>(new[]
								{
									new FunctionCallExpression(MetaData.Empty,
										new VariableExpression(MetaData.Empty, "id"),
										new List<Expression>(new[]
										{
											new IntLiteralExpression(MetaData.Empty, "123", true)
										})
									)
								}))
						}))));
			expr.SurroundWith(Environment.SolarSystem);
			expr.PrintDumpInfo();
			Assert.IsNotNull(expr.ConvertedStatementList);
			expr.ConvertedStatementList.PrintDumpInfo();
		}
	}
}