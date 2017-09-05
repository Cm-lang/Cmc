using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

#pragma warning disable 659

namespace Cmc
{
	public abstract class Type : Ast
	{
		[NotNull] public readonly string Name;
		public int Align;

		protected Type(
			MetaData metaData,
			[NotNull] string name,
			int align = 8) : base(metaData)
		{
			Name = name;
			Align = align;
		}

		public abstract override string ToString();
		public abstract override bool Equals(object obj);

		public static IEnumerable<Type> FindCommon(IEnumerable<Type> list1, IEnumerable<Type> list2) =>
			list1.Where(list2.Contains).ToList();
	}

	public class UnknownType : Type
	{
		public UnknownType(MetaData metaData, [NotNull] string name) : base(metaData, name)
		{
		}

		/// <summary>
		///     FEATURE #30
		/// </summary>
		/// <returns>resolved type</returns>
		/// <exception cref="CompilerException">if unresolved</exception>
		[NotNull]
		public Type Resolve()
		{
			var declaration = Env.FindDeclarationByName(Name);
			if (null == declaration) Gg();
			if (declaration is TypeDeclaration typeDeclaration)
			{
				typeDeclaration.Used = true;
				return typeDeclaration.Type;
			}
			if (declaration is StructDeclaration structDeclaration)
			{
				structDeclaration.Used = true;
				return structDeclaration.Type;
			}
			Errors.Add(MetaData.GetErrorHeader() + Name + " is not a type");
			throw new CompilerException($"cannot resolve {Name}");
		}

		public void Gg() =>
			Errors.Add($"{MetaData.GetErrorHeader()}unresolved type: {MetaData}");

		public override string ToString() => Name;

		public override bool Equals(object obj)
		{
			Gg();
			throw new CompilerException("unknown type");
		}
	}

	/// <summary>
	///     FEATURE #0
	/// </summary>
	public class PrimaryType : Type
	{
		[NotNull] public const string StringType = "string";
		[NotNull] public const string NullType = "nulltype";
		[NotNull] public const string BoolType = "bool";

		public PrimaryType(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData, name)
		{
		}

		public PrimaryType(
			MetaData metaData,
			[NotNull] string name,
			int align = 8) :
			base(metaData, name, align)
		{
		}

		public override IEnumerable<string> Dump() => new[] {$"primary type [{this}]\n"};

		public override string ToString() => Name;

		public override bool Equals(object obj) =>
			obj is PrimaryType type && string.Equals(type.Name, Name, StringComparison.Ordinal);
	}

	/// <summary>
	///     FEATURE #7
	/// </summary>
	public class SecondaryType : Type
	{
		[CanBeNull] public StructDeclaration Struct;

		public SecondaryType(
			MetaData metaData,
			[NotNull] string name,
			[CanBeNull] StructDeclaration @struct = null) :
			base(metaData, name, 4)
		{
			Struct = @struct;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			if (null == Struct) Struct = Env.FindDeclarationByName(Name) as StructDeclaration;
			if (null != Struct)
			{
				Struct.Used = true;
				return;
			}
			Errors.Add($"{MetaData.GetErrorHeader()}cannot resolve type {Name}");
			throw new CompilerException();
		}

		public override string ToString() => Name;

		public override bool Equals(object obj) =>
			obj is SecondaryType type && string.Equals(type.Name, Name, StringComparison.Ordinal);

		public override IEnumerable<string> Dump() =>
			new[] {$"secondary type[{this}]:\n"};
	}

	/// <summary>
	///  FEATURE #6
	/// </summary>
	public class LambdaType : Type
	{
		[NotNull] public readonly IList<Type> ArgsList;
		[NotNull] public Type RetType;

		public LambdaType(MetaData metaData, [NotNull] IList<Type> args, [NotNull] Type ret) :
			base(metaData, LambdaTypeToString(args, ret))
		{
			ArgsList = args;
			RetType = ret;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			for (var i = 0; i < ArgsList.Count; i++)
			{
				var type = ArgsList[i];
				type.SurroundWith(Env);
				if (type is UnknownType unknownType) ArgsList[i] = unknownType.Resolve();
			}
			RetType.SurroundWith(Env);
			if (RetType is UnknownType wtf) RetType = wtf.Resolve();
		}

		public override string ToString() => LambdaTypeToString(ArgsList, RetType);

		public override bool Equals(object obj)
		{
			var ok = obj is LambdaType type && Equals(type.RetType, RetType) &&
			         Equals(type.ArgsList.Count, ArgsList.Count);
			if (!ok) return false;
			var lambdaType = (LambdaType) obj;
			return !ArgsList.Where((t, i) => !Equals(lambdaType.ArgsList[i], t)).Any();
		}

		[NotNull]
		public static string LambdaTypeToString([NotNull] IList<Type> args, [NotNull] Type ret) =>
			$"({string.Join(",", args)})->{ret}";

		public override IEnumerable<string> Dump() => new[]
			{$"lambda type [{this}]\n"};
	}
}