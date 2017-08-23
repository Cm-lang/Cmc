using System.Collections.Generic;

namespace bCC
{
	public abstract class IAst
	{
		public MetaData MetaData;

		protected IAst(MetaData metaData)
		{
			MetaData = metaData;
		}
	}

	public class BcFile
	{
		public readonly IList<Declaration> Declarations;

		public BcFile(params Declaration[] declarations)
		{
			Declarations = declarations;
			AnalyzeDependencies();
		}

		/// <summary>
		/// When undefined dependencies appear, this method will give errors.
		/// TODO: check for local variables
		/// </summary>
		private void AnalyzeDependencies()
		{
			foreach (var declaration in Declarations)
			foreach (var dependency in declaration.FindDependencies())
				if (!Declarations.Contains(dependency))
					Errors.Add(declaration.MetaData.GetErrorHeader() + dependency + " undefined.");
		}
	}
}