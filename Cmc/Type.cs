using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using JetBrains.Annotations;
using static System.StringComparison;
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
		public abstract override bool Equals([CanBeNull] object obj);

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
			switch (declaration)
			{
				case null:
					Gg();
					break;
				case TypeDeclaration typeDeclaration:
					typeDeclaration.UsageCount++;
					return typeDeclaration.Type;
				case StructDeclaration structDeclaration:
					structDeclaration.UsageCount++;
					return structDeclaration.Type;
			}
			var s = MetaData.GetErrorHeader() + Name + " is not a type";
			Errors.Add(s);
			throw new CompilerException(s);
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
		[NotNull] public const string StringType = ReservedWords.StringType;
		[NotNull] public const string NullType = ReservedWords.NullType;
		[NotNull] public const string BoolType = ReservedWords.BoolType;

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
			obj is PrimaryType type && string.Equals(type.Name, Name, Ordinal);
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
			if (null == Struct)
				Struct = Env.FindDeclarationByName(Name) as StructDeclaration;
			if (null == Struct)
				Errors.AddAndThrow($"{MetaData.GetErrorHeader()}cannot resolve type {Name}");
			else Struct.UsageCount++;
		}

		public override string ToString() => Name;

		public override bool Equals(object obj) =>
			obj is SecondaryType type && string.Equals(type.Name, Name, Ordinal);

		public override IEnumerable<string> Dump() =>
			new[] {$"secondary type[{this}]:\n"};
	}

	/// <summary>
	///  FEATURE #6
	/// </summary>
	public class LambdaType : Type
	{
		[NotNull] public readonly IList<Type> ParamsList;
		[NotNull] public Type RetType;

		public LambdaType(MetaData metaData, [NotNull] IList<Type> @params, [NotNull] Type ret) :
			base(metaData, LambdaTypeToString(@params, ret))
		{
			ParamsList = @params;
			RetType = ret;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			for (var i = 0; i < ParamsList.Count; i++)
			{
				var type = ParamsList[i];
				type.SurroundWith(Env);
				if (type is UnknownType unknownType) ParamsList[i] = unknownType.Resolve();
			}
			RetType.SurroundWith(Env);
			if (RetType is UnknownType wtf) RetType = wtf.Resolve();
		}

		public override string ToString() => LambdaTypeToString(ParamsList, RetType);

		public override bool Equals(object obj)
		{
			var ok = obj is LambdaType type && Equals(type.RetType, RetType) &&
			         Equals(type.ParamsList.Count, ParamsList.Count);
			if (!ok) return false;
			var lambdaType = (LambdaType) obj;
			return !ParamsList.Where((t, i) => !Equals(lambdaType.ParamsList[i], t)).Any();
		}

		[NotNull]
		public static string LambdaTypeToString([NotNull] IList<Type> args, [NotNull] Type ret) =>
			$"({string.Join(",", args)})->{ret}";

		public override IEnumerable<string> Dump() => new[]
			{$"lambda type [{this}]\n"};
	}
}