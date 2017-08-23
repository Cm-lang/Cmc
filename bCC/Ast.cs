using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC
{
	public abstract class Ast
	{
		[NotNull]
		public virtual Environment Env { [CanBeNull] get; [NotNull] set; }

		public MetaData MetaData;

		protected Ast(MetaData metaData) => MetaData = metaData;

		[NotNull]
		public virtual string[] Dump() => new[] {ToString()};
	}
}