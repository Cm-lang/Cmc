using System.Collections.Generic;
using bCC;
using bCC.Core;
using NUnit.Framework;

namespace bCC_Test
{
	[TestFixture]
	public class MutualRecTests
	{
		[SetUp]
		public void Init() => Errors.ErrList.Clear();

		[Test]
		public void MutualRecTest1()
		{
			var core = new Core();
			var def = new StructDeclaration(MetaData.Empty, "A",
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "var1", type:
						new UnknownType(MetaData.Empty, "A"))
				}));
			var env = new Environment(Environment.SolarSystem);
			env.Declarations.Add(def);
			def.SurroundWith(env);
			def.PrintDumpInfo();
			core.CheckMutualRec(new[] {def});
			Assert.IsNotEmpty(Errors.ErrList);
			Errors.PrintErrorInfo();
		}

		[Test]
		public void MutualRecTest2()
		{
			var core = new Core();
			var def = new StructDeclaration(MetaData.Empty, "A",
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "var1", type:
						new UnknownType(MetaData.Empty, "B"))
				}));
			var def2 = new StructDeclaration(MetaData.Empty, "B",
				new List<VariableDeclaration>(new[]
				{
					new VariableDeclaration(MetaData.Empty, "var2", type:
						new UnknownType(MetaData.Empty, "A"))
				}));
			var env = new Environment(Environment.SolarSystem);
			env.Declarations.Add(def);
			env.Declarations.Add(def2);
			def.SurroundWith(env);
			def2.SurroundWith(env);
			def.PrintDumpInfo();
			def2.PrintDumpInfo();
			core.CheckMutualRec(new[] {def, def2});
			Assert.IsNotEmpty(Errors.ErrList);
			Errors.PrintErrorInfo();
		}
	}
}