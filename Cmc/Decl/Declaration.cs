using System.Collections.Generic;
using Cmc.Core;
using Cmc.Stmt;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

#pragma warning disable 659

namespace Cmc.Decl
{
	public class Declaration : Statement
	{
		public readonly Modifier Modifier;
		[NotNull] public readonly string Name;
		public bool Used = false;

		public Declaration(
			MetaData metaData,
			[NotNull] string name,
			Modifier modifier) : base(metaData)
		{
			Name = name;
			Modifier = modifier;
		}
	}

	/// <summary>
	///     type aliases
	///     FEATURE #31
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		[NotNull] public readonly Type Type;

		public TypeDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] Type type,
			Modifier modifier = Modifier.Private)
			: base(metaData, name, modifier)
		{
			Type = type;
		}
	}

	public class StructDeclaration : Declaration
	{
		[NotNull] public readonly IList<VariableDeclaration> FieldList;
		[NotNull] public readonly Type Type;

		public StructDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] IList<VariableDeclaration> fieldList,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier)
		{
			FieldList = fieldList;
			Type = new SecondaryType(metaData, name, this);
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var internalEnv = new Environment(Env);
			foreach (var variableDeclaration in FieldList)
				variableDeclaration.SurroundWith(internalEnv);
		}
	}

	/// <summary>
	///     Probably useless
	/// </summary>
	public class Macro : Declaration
	{
		[NotNull] public string Content;

		public Macro(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] string content,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier)
		{
			Content = content;
		}

		public override IEnumerable<string> Dump() =>
			new[] {"macro(this shouldn't appear)\n"};
	}
}