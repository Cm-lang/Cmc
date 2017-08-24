using System;
using System.Linq;
using bCC;
using NUnit.Framework;
using Environment = bCC.Environment;

namespace bCC_Test
{
	[TestFixture]
	public class TypeTests
	{
		[Test]
		public void TypeTest1()
		{
			var example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", true, 64);
			example.PrintDumpInfo();
			Assert.AreEqual("i64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", true);
			example.PrintDumpInfo();
			Assert.AreEqual("i32", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", false, 64);
			example.PrintDumpInfo();
			Assert.AreEqual("u64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.DefaultMetaData, "123456789", false, 64);
			example.PrintDumpInfo();
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
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)));
			example.SurroundWith(new Environment());
			example.PrintDumpInfo();
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
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)));
			example.SurroundWith(new Environment());
			example.PrintDumpInfo();
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
					type: new PrimaryType(MetaData.DefaultMetaData, "i8")),
				new ExpressionStatement(MetaData.DefaultMetaData, new VariableExpression(MetaData.DefaultMetaData, varName)));
			example.SurroundWith(new Environment());
			Console.WriteLine(string.Join("", example.Dump()));
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual("i8", (example.Statements.Last() as ExpressionStatement).Expression.GetExpressionType().ToString());
		}
	}

	[TestFixture]
	public class StatementTests
	{
		[Test]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.DefaultMetaData,
					new Statement(MetaData.DefaultMetaData),
					new Statement(MetaData.DefaultMetaData)).Statements)
				stmt.PrintDumpInfo();
		}


		[Test]
		public void StatementTest2()
		{
			var stmt = new IfStatement(
				MetaData.DefaultMetaData,
				new BoolLiteralExpression(MetaData.DefaultMetaData, false),
				new StatementList(MetaData.DefaultMetaData)
			);
			stmt.SurroundWith(new Environment());
			stmt.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
			var stmt2 = new IfStatement(
				MetaData.DefaultMetaData,
				new NullExpression(MetaData.DefaultMetaData),
				new StatementList(MetaData.DefaultMetaData)
			);
			stmt2.SurroundWith(new Environment());
			Console.WriteLine("");
			Console.WriteLine("");
			Assert.IsNotEmpty(Errors.ErrList);
			foreach (var s in Errors.ErrList)
				Console.WriteLine(s);
			stmt2.PrintDumpInfo();
		}
	}
}