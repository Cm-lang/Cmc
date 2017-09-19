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
		public static VariableDeclaration IdDeclaration() =>
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

		public static StatementList FuncCallAst1() => new StatementList(MetaData.Empty,
			IdDeclaration(),
			new VariableDeclaration(MetaData.Empty, "gg", type:
				new UnknownType(MetaData.Empty, "i8")),
			new AssignmentStatement(MetaData.Empty,
				new VariableExpression(MetaData.Empty, "gg"),
				new FunctionCallExpression(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "id"),
					new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "1", true)}))));

		public static StatementList FuncCallAst2() => new StatementList(MetaData.Empty,
			IdDeclaration(),
			new VariableDeclaration(MetaData.Empty, "gg", isMutable: true, type:
				new UnknownType(MetaData.Empty, "i8")),
			new AssignmentStatement(MetaData.Empty,
				new VariableExpression(MetaData.Empty, "gg"),
				new FunctionCallExpression(MetaData.Empty,
					new VariableExpression(MetaData.Empty, "id"),
					new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "233", true, 8)}))));

		public static LambdaExpression LambdaAst1() => new LambdaExpression(MetaData.Empty,
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

		public static StatementList FuncCallAst3() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, "recurFunc", LambdaAst1()),
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

		public static LambdaExpression LambdaAst2() => new LambdaExpression(MetaData.Empty,
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

		public static StatementList FuncCallAst4() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, "recurFunc", LambdaAst2()),
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

		public static LambdaExpression LambdaAst3() => new LambdaExpression(MetaData.Empty,
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
										})),
									"inner")
							), returnType:
							new UnknownType(MetaData.Empty, "i8"),
							endLabel: new ReturnLabelDeclaration(MetaData.Empty, "inner")),
						new List<Expression>()))),
			new List<VariableDeclaration>(new[]
			{
				new VariableDeclaration(MetaData.Empty, "a", type:
					new UnknownType(MetaData.Empty, "i8"))
			}), new UnknownType(MetaData.Empty, "i8"));

		public static StatementList FuncCallAst5() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, "recurFunc", LambdaAst3()),
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

		[TestInitialize]
		public void Init()
		{
			Errors.ErrList.Clear();
			Pragma.KeepAll = false;
			Environment.Initialize();
		}

		[TestMethod]
		public void FuncCallTest1()
		{
			var example = FuncCallAst1();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Console.WriteLine(string.Join("\n", Errors.ErrList));
			Assert.IsTrue(0 != Errors.ErrList.Count);
		}

		[TestMethod]
		public void FuncCallTest2()
		{
			var example = FuncCallAst2();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Console.WriteLine(string.Join("\n", Errors.ErrList));
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		/// <summary>
		///  recur fail test
		/// </summary>
		[TestMethod]
		public void FuncCallTest3() =>
			Assert.ThrowsException<CompilerException>(() => { FuncCallAst3().SurroundWith(Environment.SolarSystem); });

		/// <summary>
		///  recur test
		/// </summary>
		[TestMethod]
		public void FuncCallTest4()
		{
			var example = FuncCallAst4();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
		}

		/// <summary>
		///  recur test
		///  expression:
		///  const recurFunc = i8 { a: i32 -> i8 { recur(a) }() }
		///  var gg: i8 = null
		///  gg = recurFunc(233u8)
		/// </summary>
		[TestMethod]
		public void FuncCallTest5()
		{
			var example = FuncCallAst5();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Errors.PrintErrorInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		[TestMethod]
		public void FuncCallTest5KeelAll()
		{
			Pragma.KeepAll = true;
			var example = FuncCallAst5();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Errors.PrintErrorInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		private static void ExpressionSplittingTestCore(IntLiteralExpression parameter, Action action)
		{
			var expr = new FunctionCallExpression(MetaData.Empty,
				new VariableExpression(MetaData.Empty, "id"),
				new List<Expression>(new[]
				{
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "id"),
						new List<Expression>(new[]
						{
							new FunctionCallExpression(MetaData.Empty,
								new VariableExpression(MetaData.Empty, "id"),
								new List<Expression>(new[] {parameter})
							)
						}))
				}));
			var core = new Core();
			var lambdaExpression = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						expr)));
			core.Analyze(
				IdDeclaration(),
				new VariableDeclaration(MetaData.Empty, "_", lambdaExpression));
//			Assert.IsNotNull(expr.ConvertedResult);
			Errors.PrintErrorInfo();
			action();
			lambdaExpression.Body.PrintDumpInfo();
		}

		[TestMethod]
		public void ExpressionSplittingTest1() =>
			ExpressionSplittingTestCore(new IntLiteralExpression(MetaData.Empty, "123", true),
				() => Assert.IsTrue(0 != Errors.ErrList.Count));

		[TestMethod]
		public void ExpressionSplittingTest2() =>
			ExpressionSplittingTestCore(new IntLiteralExpression(MetaData.Empty, "123", true, 8),
				() => Assert.IsTrue(0 == Errors.ErrList.Count));
	}
}