using System.Collections.Generic;
using Cmc.Core;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Decl
{
	public interface ILabel
	{
		LabelDeclaration GetLabel();
	}

	/// <summary>
	///  return to here
	/// </summary>
	public class ReturnLabelDeclaration : Declaration, ILabel
	{
		[NotNull] public readonly IList<ReturnStatement> StatementsUsingThis;

		public ReturnLabelDeclaration(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData, name)
		{
			StatementsUsingThis = new List<ReturnStatement>();
		}

		public sealed override string ToString() => Name.Length == 0 ? GetHashCode().ToString() : Name;
		public LabelDeclaration GetLabel() => new LabelDeclaration(MetaData, ToString());
		public override IEnumerable<string> Dump() => new[] {$"return label [{this}]\n"};
		public override IEnumerable<string> DumpCode() => new[] {$"rlabel:{this};\n"};
	}

	/// <summary>
	///  jump to here
	/// </summary>
	public class JumpLabelDeclaration : Declaration, ILabel
	{
		[NotNull] public readonly IList<JumpStatement> StatementsUsingThis;

		public JumpLabelDeclaration(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData, name)
		{
			StatementsUsingThis = new List<JumpStatement>();
		}

		public sealed override string ToString() => Name.Length == 0 ? GetHashCode().ToString() : Name;
		public LabelDeclaration GetLabel() => new LabelDeclaration(MetaData, ToString());
		public override IEnumerable<string> Dump() => new[] {$"jump label [{this}]\n"};
		public override IEnumerable<string> DumpCode() => new[] {$"jlabel:{this};\n"};
	}

	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(
			MetaData metaData,
			[NotNull] string name) : base(metaData, name)
		{
		}

		public override IEnumerable<string> Dump() => new[] {$"label [{Name}]\n"};
		public override IEnumerable<string> DumpCode() => new[] {$"label:{Name};\n"};
	}
}