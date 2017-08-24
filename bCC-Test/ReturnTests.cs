using bCC;
using NUnit.Framework;

namespace bCC_Test
{
	[TestFixture]
	public class ReturnTests
	{
		[SetUp]
		public void Init() => Errors.ErrList.Clear();

		[Test]
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
			block.SurroundWith(new Environment());
			block.PrintDumpInfo();
			Assert.IsNotEmpty(Errors.ErrList);
		}
		[Test]
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
			block.SurroundWith(new Environment());
			block.PrintDumpInfo();
			Assert.IsEmpty(Errors.ErrList);
		}
	}
}