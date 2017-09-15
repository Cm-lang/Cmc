using System.Text;
using Cmc.Decl;
using JetBrains.Annotations;

namespace LLVM
{
	public static class GenDeclaration
	{

		public static void GenAstDeclaration(
			[NotNull] StringBuilder builder,
			[NotNull] Declaration element)
		{
			switch (element)
			{
				case TypeDeclaration typeDeclaration:
					break;
				case VariableDeclaration variable:
					break;
			}
		}
	}
}