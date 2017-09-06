using System.Collections.Generic;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using LLVM;

namespace CmcExec
{
	internal class Executable
	{
		public static void Main(string[] args)
		{
			Gen.RunLlvm(
				"out.exe",
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
		}
	}
}