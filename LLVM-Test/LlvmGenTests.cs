using System;
using LLVM;
using NUnit.Framework;

namespace LLVM_Test
{
	[TestFixture]
	public class LlvmGenTests
	{
		[Test]
		public void LlvmGenTest1()
		{
			Console.WriteLine(Gen.Generate(
				new VariableDeclaration(MetaData.Empty,
					"i", new IntLiteralExpression(MetaData.Empty, "1", true)),
				new VariableDeclaration(MetaData.Empty,
					"j", new StringLiteralExpression(MetaData.Empty, "boy next door"))
			))
		}
	}
}