using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.StringComparison;

#pragma warning disable 659

namespace bCC
{
	public abstract class Type : Ast
	{
		protected Type(MetaData metaData) : base(metaData)
		{
		}

		public abstract override string ToString();
		public abstract override bool Equals(object obj);

		public static IEnumerable<Type> FindCommon(IEnumerable<Type> list1, IEnumerable<Type> list2) =>
			list1.Where(list2.Contains).ToList();
	}

	/// <summary>
	///   FEATURE #0
	/// </summary>
	public class PrimaryType : Type
	{
		public const string StringType = "string";
		public const string NullType = "nulltype";
		public const string BoolType = "bool";
		[NotNull] public readonly string Name;

		public PrimaryType(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;

		public override IEnumerable<string> Dump() => new[] {$"primary type [{Name}]\n"};
		public override string ToString() => Name;

		public override bool Equals(object obj) => obj is PrimaryType type && string.Equals(type.Name, Name, Ordinal);
	}

	/// <summary>
	///   FEATURE #7
	/// </summary>
	public class SecondaryType : Type
	{
		[NotNull] public readonly string Container;
		public StructDeclaration Struct;

		public SecondaryType(
			MetaData metaData,
			[NotNull] string container,
			StructDeclaration @struct) :
			base(metaData)
		{
			Container = container;
			Struct = @struct;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			if (Struct == null) Struct = Env.FindDeclarationByName(Container) as StructDeclaration;
		}

		public override string ToString() => Container;

		public override bool Equals(object obj) =>
			obj is SecondaryType type && string.Equals(type.Container, Container, Ordinal);

		public override IEnumerable<string> Dump() => new[] {"secondary type[{Container}]:\n"};
	}

	/// <summary>
	///   FEATURE #6
	/// </summary>
	public class LambdaType : Type
	{
		[NotNull] public readonly IList<Type> ArgsList;
		[NotNull] public readonly Type RetType;

		public LambdaType(MetaData metaData, [NotNull] IList<Type> args, [NotNull] Type ret) :
			base(metaData)
		{
			ArgsList = args;
			RetType = ret;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var type in ArgsList) type.SurroundWith(Env);
			RetType.SurroundWith(Env);
		}

		public override string ToString() => LambdaTypeToString(ArgsList, RetType);

		public override bool Equals(object obj)
		{
			var ok = obj is LambdaType type && Equals(type.RetType, RetType) && Equals(type.ArgsList.Count, ArgsList.Count);
			if (!ok) return false;
			var lambdaType = (LambdaType) obj;
			return !ArgsList.Where((t, i) => !Equals(lambdaType.ArgsList[i], t)).Any();
		}

		[NotNull]
		public static string LambdaTypeToString([NotNull] IList<Type> args, [NotNull] Type ret) =>
			$"{string.Join(",", args)}->{ret}";

		public override IEnumerable<string> Dump()
		{
			return new[]
				{
					"lambda type:\n",
					"  parameters' types:\n"
				}
				.Concat(ArgsList.SelectMany(i => i.Dump().Select(MapFunc).Select(MapFunc)))
				.Concat(new[] {"  return type:\n"})
				.Concat(RetType.Dump().Select(MapFunc).Select(MapFunc));
		}
	}
}