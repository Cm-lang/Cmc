using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmcTest
{
	[TestClass]
	public class ReturnTests
	{
		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();

		[TestMethod]
		public void ReturnTest1()
		{
			const string var = "var";
			var block = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new VariableDeclaration(MetaData.Empty, var,
						new BoolLiteralExpression(MetaData.Empty, false)),
					new IfStatement(MetaData.Empty,
						new VariableExpression(MetaData.Empty, var),
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "0", false, 8))),
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "1", true, 8))))));
			block.SurroundWith(Environment.SolarSystem);
			block.PrintDumpInfo();
			Errors.PrintErrorInfo();
			Assert.IsTrue(0 != Errors.ErrList.Count);
		}

		[TestMethod]
		public void ReturnTest2()
		{
			const string var = "var";
			var block = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty,
					new VariableDeclaration(MetaData.Empty, var,
						new BoolLiteralExpression(MetaData.Empty, false)),
					new IfStatement(MetaData.Empty,
						new VariableExpression(MetaData.Empty, var),
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "23", false, 8))),
						new StatementList(MetaData.Empty,
							new ReturnStatement(MetaData.Empty,
								new IntLiteralExpression(MetaData.Empty, "45", false, 8))))));
			block.SurroundWith(Environment.SolarSystem);
			block.PrintDumpInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}
	}
}