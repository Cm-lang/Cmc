using Cmc;
using Cmc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CmcTest.FunctionCallTests;

namespace CmcTest
{
	[TestClass]
	public class DumpCodeTest
	{
		[TestInitialize]
		public void Init()
		{
			Errors.ErrList.Clear();
			Pragma.KeepAll = false;
			Environment.Initialize();
		}

		[TestMethod]
		public void TestDump1()
		{
			var example = FuncCallAst1();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintCode();
		}

		[TestMethod]
		public void FuncCallTest2()
		{
			var example = FuncCallAst2();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintCode();
		}

		[TestMethod]
		public void FuncCallTest4()
		{
			var example = FuncCallAst4();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintCode();
		}

		[TestMethod]
		public void FuncCallTest5()
		{
			var example = FuncCallAst5();
			example.SurroundWith(Environment.SolarSystem);
			example.PrintCode();
		}
	}
}