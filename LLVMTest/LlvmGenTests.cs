﻿using System;
using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using LLVM;
using LLVMTest.ErrorSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLVMTest
{
	[TestClass]
	public class LlvmGenTests
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

		[TestMethod]
		public void LlvmGenTest1()
		{
			var res = Gen.Generate(
				new VariableDeclaration(MetaData.Empty,
					"i", new IntLiteralExpression(MetaData.Empty, "1", true)),
				new VariableDeclaration(MetaData.Empty,
					"j", new StringLiteralExpression(MetaData.Empty, "boy next door")),
				new VariableDeclaration(MetaData.Empty,
					"main", new LambdaExpression(MetaData.Empty,
						new StatementList(MetaData.Empty,
							new VariableDeclaration(MetaData.Empty,
								"j", new StringLiteralExpression(MetaData.Empty, "boy next door")),
							new ExpressionStatement(MetaData.Empty,
								new FunctionCallExpression(MetaData.Empty,
									new VariableExpression(MetaData.Empty, "print"),
									new List<Expression>(new[]
									{
										new VariableExpression(MetaData.Empty, "j")
									}))),
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "0", true)))))
			);
			Console.WriteLine(res);
		}

		[TestMethod]
		public void LlvmGenTest2()
		{
			var body = new StatementList(MetaData.Empty,
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
											new IntLiteralExpression(MetaData.Empty, "123", true, 8)
										})
									)
								}))
						}))));
			var res = Gen.Generate(
				IdDeclaration,
				new VariableDeclaration(MetaData.Empty,
					"main", new LambdaExpression(MetaData.Empty, body)));
			Console.WriteLine(res);
		}

		/// <summary>
		///  ambiguous main definition
		/// </summary>
		[TestMethod]
		public void CodeGenFailTest1()
		{
			Assert.ThrowsException<CompilerException>(() => OnlyMainCanBeDefined.Run(new[] {""}));
			Assert.IsTrue(0 != Errors.ErrList.Count);
		}

		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();
	}
}