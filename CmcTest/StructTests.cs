using System.Linq;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
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
			var @struct = new StatementList(MetaData.Empty,
				new VariableDeclaration(MetaData.Empty, "var", type:
					new UnknownType(MetaData.Empty, "Person")));
			@struct.SurroundWith(Environment.SolarSystem);
			@struct.PrintDumpInfo();
			var a = (VariableDeclaration) @struct.Statements.Last();
			Assert.AreEqual("Person", a.Type.ToString());
			Assert.IsTrue(a.Type is SecondaryType);
		}
	}
}