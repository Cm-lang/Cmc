using System;
using System.Collections.Generic;
using bCC;
using NUnit.Framework;
using Environment = bCC.Environment;

namespace bCC_Test
{
	[TestFixture]
	public class FunctionCallTests
	{
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
				new ExpressionStatement(MetaData.Empty,
					new FunctionCallExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, "id"),
						new List<Expression>(new[] {new IntLiteralExpression(MetaData.Empty, "1", true)}))));
			example.SurroundWith(Environment.Earth);
			example.PrintDumpInfo();
			Console.WriteLine(string.Join("\n", Errors.ErrList));
		}
	}
}