using System.Collections.Generic;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using LLVM;

namespace CmcExec.ErrorSamples
{
	public class NoMainNoLambdef
	{
		public static void Run(string[] args)
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
								"local", new StringLiteralExpression(MetaData.Empty, "NullRefTest")),
							
							new ExpressionStatement(MetaData.Empty,
								new FunctionCallExpression(MetaData.Empty,
									new VariableExpression(MetaData.Empty, "print"),
									new List<Expression>(new[]
									{
										new VariableExpression(MetaData.Empty, "local")
									}))),
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "0", true))
							))),
				new VariableDeclaration(MetaData.Empty,
					"main", new LambdaExpression(MetaData.Empty,
						new StatementList(MetaData.Empty,
							new VariableDeclaration(MetaData.Empty,
								"j", new StringLiteralExpression(MetaData.Empty, "Hello, World")),
							new ExpressionStatement(MetaData.Empty,
								new FunctionCallExpression(MetaData.Empty,
									new VariableExpression(MetaData.Empty, "print"),
									new List<Expression>(new[]
									{
										new VariableExpression(MetaData.Empty, "j")
									}))),
							new ExpressionStatement(MetaData.Empty,
								new FunctionCallExpression( MetaData.Empty,
									new VariableExpression(MetaData.Empty, "myfunc"),
									new List<Expression>())),
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "0", true)))))
			);
		}
	}
}