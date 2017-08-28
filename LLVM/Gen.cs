using System;
using bCC;
using bCC.Core;

namespace LLVM
{
	public class Gen
	{
		public void Generate(params Declaration[] declarations)
		{
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			// TODO run code gen
		}
	}
}