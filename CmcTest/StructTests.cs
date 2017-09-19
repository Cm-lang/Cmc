using System.Collections.Generic;
using System.Linq;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmcTest
{
	[TestClass]
	public class StructTests
	{
		[TestMethod]
		public void StructTest1()
		{
			var @struct = StructAst1();
			@struct.SurroundWith(Environment.SolarSystem);
			@struct.PrintDumpInfo();
			var a = (VariableDeclaration) @struct.Statements.Last();
			Assert.AreEqual("Person", a.Type.ToString());
			Assert.IsTrue(a.Type is SecondaryType);
		}

		public static StatementList StructAst1() => new StatementList(MetaData.Empty,
			new StructDeclaration(MetaData.Empty, "Person",
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "name",
						new StringLiteralExpression(MetaData.Empty, "ice")),
					new VariableDeclaration(MetaData.Empty, "gender",
						new IntLiteralExpression(MetaData.Empty, "123", false))
				})),
			new VariableDeclaration(MetaData.Empty, "var", type:
				new UnknownType(MetaData.Empty, "Person")));
	}
}