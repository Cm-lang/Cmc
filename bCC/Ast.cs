using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC
{
	public abstract class Ast
	{
		[NotNull]
		public virtual Environment Env { [CanBeNull] get; [NotNull] set; }

		public MetaData MetaData;

		protected Ast(MetaData metaData)
		{
			MetaData = metaData;
		}
	}

	public class BcFile
	{
		[NotNull] public readonly IList<Declaration> Declarations;

		public BcFile([NotNull] params Declaration[] declarations)
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