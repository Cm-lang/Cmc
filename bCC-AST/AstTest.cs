using System;
using System.Linq;
using NUnit.Framework;

namespace bCC
{
	[TestFixture]
	public class AstTest
	{
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.DefaultMetaData,
					new Statement(MetaData.DefaultMetaData),
					new Statement(MetaData.DefaultMetaData)).Statements)
				Console.WriteLine(stmt);
		}

		[Test]
		public void TypeTest1()
		{
			var example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", true, 64);
			Assert.AreEqual("i64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", true);
			Assert.AreEqual("i32", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", false, 64);
			Assert.AreEqual("u64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", false, 64);
			Assert.AreEqual("u64", example.Type.ToString());
		}

		[Test]
		public void TypeInferenceTest1()
		{
			const string varName = "someVar";
			var example = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, varName,
					new IntLiteralExpression(MetaData.DefaultMetaData, "123", false, 8)),
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)))
			{
				Env = new Environment()
			};
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual("u8", (example.Statements.Last() as ExpressionStatement).Expression.GetExpressionType().ToString());
		}

		[Test]
		public void StatementTest2()
		{
//			var stmt = new IfStatement(MetaData.DefaultMetaData, new AtomicExpression(MetaData.DefaultMetaData),);
//			Console.WriteLine(stmt);
		}
	}
}