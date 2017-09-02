using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Expression;
using Cmc.Statement;
using LLVM;
using NUnit.Framework;

namespace LLVM_Test
{
	public class LlvmCommandLineTests
	{
		[SetUp]
		public void Init() => Errors.ErrList.Clear();

		[Test]
		public void CommandLineTest1()
		{
			Gen.RunLlvm(
				"out.exe",
				new VariableDeclaration(MetaData.Empty,
					"j", new StringLiteralExpression(MetaData.Empty, "boy next door")),
				new VariableDeclaration(MetaData.Empty,
					"main", new LambdaExpression(MetaData.Empty,
						new StatementList(MetaData.Empty,
							new ExpressionStatement(MetaData.Empty,
								new FunctionCallExpression(MetaData.Empty,
									new VariableExpression(MetaData.Empty, "puts"),
									new List<Expression>(new[]
									{
										new VariableExpression(MetaData.Empty, "j")
									}))), new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "0", true)))))
			);
		}
	}
}