using System.Collections.Generic;
using System.Linq;
using bCC;
using NUnit.Framework;

namespace bCC_Test
{
	[TestFixture]
	public class StructTests
	{
		[Test]
		public void StructTest1()
		{
			var @struct = new StatementList(MetaData.Empty,
				new StructDeclaration(MetaData.Empty, "Person", new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "name", new StringLiteralExpression(MetaData.Empty, "ice")),
					new VariableDeclaration(MetaData.Empty, "gender", new IntLiteralExpression(MetaData.Empty, "123", false))
				})),
				new VariableDeclaration(MetaData.Empty, "var", type:
					new UnknownType(MetaData.Empty, "Person")));
			@struct.SurroundWith(Environment.TopEnvironment);
			@struct.PrintDumpInfo();
			var a = (VariableDeclaration) @struct.Statements.Last();
			Assert.AreEqual("Person", a.Type.ToString());
			Assert.IsTrue(a.Type is SecondaryType);
		}
	}
}