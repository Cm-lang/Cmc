using System.Collections.Generic;
using Cmc.Core;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Decl
{
	/// <summary>
	///  return to here
	/// </summary>
	public class ReturnLabelDeclaration : Declaration
	{
		[NotNull] public readonly IList<ReturnStatement> StatementsUsingThis;

		public ReturnLabelDeclaration(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData, name)
		{
			StatementsUsingThis = new List<ReturnStatement>();
		}

		public override string ToString() => Name.Length == 0 ? GetHashCode().ToString() : Name;

		public override IEnumerable<string> Dump() => new[]
		{
			$"return label [{this}]\n"
		};
	}

	/// <summary>
	///  jump to here
	/// </summary>
	public class JumpLabelDeclaration : Declaration
	{
		[NotNull] public readonly IList<JumpStatement> StatementsUsingThis;

		public JumpLabelDeclaration(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData, name)
		{
			StatementsUsingThis = new List<JumpStatement>();
		}

		public override string ToString() => Name.Length == 0 ? GetHashCode().ToString() : Name;

		public override IEnumerable<string> Dump() => new[]
		{
			$"jump label [{this}]\n"
		};
	}
}