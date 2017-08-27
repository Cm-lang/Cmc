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
						new UnknownType(MetaData.Empty, "B")),
					new VariableDeclaration(MetaData.Empty, "var3", type:
						new UnknownType(MetaData.Empty, "u8"))
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

		[Test]
		public void MutualRecTest3()
		{
			var core = new Core();
			var defs = new[]
			{
				new StructDeclaration(MetaData.Empty, "A",
					new List<VariableDeclaration>(new[]
					{
						new VariableDeclaration(MetaData.Empty, "var1", type:
							new UnknownType(MetaData.Empty, "B")),
						new VariableDeclaration(MetaData.Empty, "var3", type:
							new UnknownType(MetaData.Empty, "u8"))
					})),
				new StructDeclaration(MetaData.Empty, "B",
					new List<VariableDeclaration>(new[]
					{
						new VariableDeclaration(MetaData.Empty, "var5", type:
							new UnknownType(MetaData.Empty, "C"))
					})),
				new StructDeclaration(MetaData.Empty, "C",
					new List<VariableDeclaration>(new[]
					{
						new VariableDeclaration(MetaData.Empty, "var7", type:
							new UnknownType(MetaData.Empty, "D"))
					})),
				new StructDeclaration(MetaData.Empty, "D",
					new List<VariableDeclaration>(new[]
					{
						new VariableDeclaration(MetaData.Empty, "var9", type:
							new UnknownType(MetaData.Empty, "A"))
					}))
			};
			var env = new Environment(Environment.SolarSystem);
			foreach (var structDeclaration in defs)
				env.Declarations.Add(structDeclaration);
			foreach (var structDeclaration in defs)
				structDeclaration.SurroundWith(env);
			foreach (var structDeclaration in defs)
				structDeclaration.PrintDumpInfo();
			core.CheckMutualRec(defs);
			Assert.IsNotEmpty(Errors.ErrList);
			Errors.PrintErrorInfo();
		}
	}
}