using System;
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

		/// <summary>
		///  ambiguous main definition
		/// </summary>
		[TestMethod]
		public void CodeGenFailTest1()
		{
			OnlyMainCanBeDefined.Run(new[] {""});
			Assert.IsTrue(0 != Errors.ErrList.Count);
		}

		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();
	}
}