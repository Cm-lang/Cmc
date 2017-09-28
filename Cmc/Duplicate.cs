using System.Collections.Generic;
using Cmc.Core;
using JetBrains.Annotations;

namespace Cmc
{
	public class Duplicate : Ast
	{
		public Type ConvertedType;
		[NotNull] public string Name;

		public Duplicate(
			MetaData metaData,
			[NotNull] string name) : base(metaData)
		{
			Name = name;
		}

		public override IEnumerable<string> Dump() => new[] {$"dup: [{Name}]"};

		public override IEnumerable<string> DumpCode() => new[] {Name};
	}
}