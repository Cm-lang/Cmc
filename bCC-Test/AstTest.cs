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

		/// <summary>
		/// var someVar = 123u8;
		/// someVar; // the type of this expression will be inferred as "u8".
		/// </summary>
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
			Console.WriteLine(string.Join("", example.Dump()));
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual("u8", (example.Statements.Last() as ExpressionStatement).Expression.GetExpressionType().ToString());
		}

		/// <summary>
		/// var someVar = null;
		/// someVar; // nulltype
		/// </summary>
		[Test]
		public void TypeInferenceTest2()
		{
			const string varName = "someOtherVar";
			var example = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, varName,
					new NullExpression(MetaData.DefaultMetaData)),
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)))
			{
				Env = new Environment()
			};
			Console.WriteLine(string.Join("", example.Dump()));
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual(NullExpression.NullType,
				(example.Statements.Last() as ExpressionStatement).Expression.GetExpressionType().ToString());
		}

		/// <summary>
		/// var otherVar: i8 = null;
		/// otherVar; // i8
		/// FEATURE #11
		/// </summary>
		[Test]
		public void TypeInferenceTest3()
		{
			const string varName = "otherVar";
			var example = new StatementList(MetaData.DefaultMetaData,
				new VariableDeclaration(MetaData.DefaultMetaData, varName,
					new NullExpression(MetaData.DefaultMetaData),
					type: new SecondaryType(MetaData.DefaultMetaData, "i8")),
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)))
			{
				Env = new Environment()
			};
			Console.WriteLine(string.Join("", example.Dump()));
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual("i8", (example.Statements.Last() as ExpressionStatement).Expression.GetExpressionType().ToString());
		}

		[Test]
		public void StatementTest2()
		{
//			var stmt = new IfStatement(MetaData.DefaultMetaData, new AtomicExpression(MetaData.DefaultMetaData),);
//			Console.WriteLine(stmt);
		}
	}
}