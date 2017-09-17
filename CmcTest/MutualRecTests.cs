using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmcTest
{
	[TestClass]
	public class MutualRecTests
	{
		public static StructDeclaration StructDef1() => new StructDeclaration(MetaData.Empty, "A",
			new List<VariableDeclaration>(new[]
			{
				new VariableDeclaration(MetaData.Empty, "var1", type:
					new UnknownType(MetaData.Empty, "A"))
			}));

		public static StructDeclaration StructDef2() => new StructDeclaration(MetaData.Empty, "A",
			new List<VariableDeclaration>(new[]
			{
				new VariableDeclaration(MetaData.Empty, "var1", type:
					new UnknownType(MetaData.Empty, "B")),
				new VariableDeclaration(MetaData.Empty, "var3", type:
					new UnknownType(MetaData.Empty, "u8"))
			}));

		public static StructDeclaration StructDef3() => new StructDeclaration(MetaData.Empty, "B",
			new List<VariableDeclaration>(new[]
			{
				new VariableDeclaration(MetaData.Empty, "var2", type:
					new UnknownType(MetaData.Empty, "A"))
			}));

		public static StructDeclaration[] StructDefs() => new[]
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
						new UnknownType(MetaData.Empty, "C")),
					new VariableDeclaration(MetaData.Empty, "var5", type:
						new UnknownType(MetaData.Empty, "string")),
					new VariableDeclaration(MetaData.Empty, "var5", type:
						new UnknownType(MetaData.Empty, "nulltype"))
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

		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();

		[TestMethod]
		public void MutualRecTest1()
		{
			var core = new Core();
			var def = StructDef1();
			var env = new Environment(Environment.SolarSystem);
			env.Declarations.Add(def);
			core.CheckMutualRec(new[] {def});
			Assert.IsTrue(0 != Errors.ErrList.Count);
			Errors.PrintErrorInfo();
		}

		[TestMethod]
		public void MutualRecTest2()
		{
			var core = new Core();
			var def = StructDef2();
			var def2 = StructDef3();
			var env = new Environment(Environment.SolarSystem);
			env.Declarations.Add(def);
			env.Declarations.Add(def2);
			core.CheckMutualRec(new[] {def, def2});
			Assert.IsTrue(0 != Errors.ErrList.Count);
			Errors.PrintErrorInfo();
		}

		[TestMethod]
		public void MutualRecTest3()
		{
			var core = new Core();
			var defs = StructDefs();
			var env = new Environment(Environment.SolarSystem);
			foreach (var structDeclaration in defs)
				env.Declarations.Add(structDeclaration);
//			foreach (var structDeclaration in defs)
//				structDeclaration.SurroundWith(env);
//			foreach (var structDeclaration in defs)
//				structDeclaration.PrintDumpInfo();
			core.CheckMutualRec(defs);
			Assert.IsTrue(0 != Errors.ErrList.Count);
			Errors.PrintErrorInfo();
		}
	}
}