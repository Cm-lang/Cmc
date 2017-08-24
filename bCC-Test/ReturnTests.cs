using bCC;
using NUnit.Framework;

namespace bCC_Test
{
	[TestFixture]
	public class ReturnTests
	{
		[Test]
		public void ReturnTest1()
		{
			var block = new LambdaExpression(MetaData.Empty,
				new StatementList(MetaData.Empty));
			block.PrintDumpInfo();
		}
	}
}