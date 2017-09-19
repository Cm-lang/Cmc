using System;
using System.Collections.Generic;
using System.Linq;
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
	public class TypeTests
	{
		private const string VarName = "someVar";

		public static StatementList TypeInferenceAst1() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, VarName,
				new IntLiteralExpression(MetaData.Empty, "123", false, 8)),
			new ExpressionStatement(MetaData.Empty,
				new VariableExpression(MetaData.Empty, VarName)));

		public static StatementList TypeInferenceAst2() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, VarName,
				new NullExpression(MetaData.Empty)),
			new ExpressionStatement(MetaData.Empty,
				new VariableExpression(MetaData.Empty, VarName)));

		public static StatementList TypeInferenceAst3() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, VarName,
				new NullExpression(MetaData.Empty),
				type: new UnknownType(MetaData.Empty, "i8")),
			new ExpressionStatement(MetaData.Empty,
				new VariableExpression(MetaData.Empty, VarName)));

		public static VariableDeclaration TypeInferenceAst4() => new VariableDeclaration(MetaData.Empty, VarName,
			new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new ReturnStatement(MetaData.Empty,
						new IntLiteralExpression(MetaData.Empty, "0", true)))));

		[TestInitialize]
		public void Init()
		{
			Environment.Initialize();
			Errors.ErrList.Clear();
		}

		/// <summary>
		///     var someVar = 123u8;
		///     someVar; // the type of this expression will be inferred as "u8".
		/// </summary>
		[TestMethod]
		public void TypeInferenceTest1()
		{
			var example = TypeInferenceAst1();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual("u8", ((ExpressionStatement) example.Statements.Last())
				.Expression.GetExpressionType().ToString());
		}

		/// <summary>
		///     var someVar = null;
		///     someVar; // nulltype
		/// </summary>
		[TestMethod]
		public void TypeInferenceTest2()
		{
			var example = TypeInferenceAst2();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			Assert.AreEqual(PrimaryType.NullType,
				((ExpressionStatement) example.Statements.Last())
				.Expression.GetExpressionType().ToString());
		}

		/// <summary>
		///     var otherVar: i8 = null;
		///     otherVar; // i8
		///     FEATURE #11
		/// </summary>
		[TestMethod]
		public void TypeInferenceTest3()
		{
			var example = TypeInferenceAst3();
			example.SurroundWith(Environment.SolarSystem);
			Console.WriteLine(string.Join("", example.Dump()));
			Assert.AreEqual("i8", ((ExpressionStatement) example.Statements.Last())
				.Expression.GetExpressionType().ToString());
		}

		/// <summary>
		///     lambda type inference
		/// </summary>
		[TestMethod]
		public void TypeInferenceTest4()
		{
			var example = TypeInferenceAst4();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintDumpInfo();
			var type = (LambdaType) example.Type;
			Assert.IsNotNull(type);
			Assert.AreEqual("i32", type.RetType.ToString());
			Assert.IsTrue(0 == type.ParamsList.Count);
		}

		[TestMethod]
		public void TypeTest1()
		{
			LiteralExpression example = new IntLiteralExpression(MetaData.Empty, "123456789", true, 64);
			example.PrintDumpInfo();
			Assert.AreEqual("i64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.Empty, "123456789", true);
			example.PrintDumpInfo();
			Assert.AreEqual("i32", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.Empty, "123456789", false, 64);
			example.PrintDumpInfo();
			Assert.AreEqual("u64", example.Type.ToString());
			example = new IntLiteralExpression(MetaData.Empty, "123456789", false, 64);
			example.PrintDumpInfo();
			Assert.AreEqual("u64", example.Type.ToString());
			var example2 = new StringLiteralExpression(MetaData.Empty, "\"boy \\ next \\ door\n\t\"");
			example2.PrintDumpInfo();
			Assert.AreEqual(PrimaryType.StringType, example2.GetExpressionType().ToString());
			// var type = new SecondaryType(MetaData.Empty, "vec", new PrimaryType(MetaData.Empty, "i8"), example.Type);
			// type.SurroundWith(Environment.TopEnvironment);
			// type.PrintDumpInfo();
		}
	}
}