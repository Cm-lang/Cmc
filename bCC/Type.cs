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
		[NotNull] public readonly Type Container;
		[NotNull] public readonly Type Parameter;

		public SecondaryType(MetaData metaData, [NotNull] Type container, [NotNull] Type parameter) :
			base(metaData)
		{
			Container = container;
			Parameter = parameter;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Container.SurroundWith(Env);
			Parameter.SurroundWith(Env);
		}

		[NotNull]
		public static string SecondaryTypeToString([NotNull] Type args, [NotNull] Type ret) => $"{args}<{ret}>";

		public override string ToString() => SecondaryTypeToString(Container, Parameter);

		public override bool Equals(object obj) =>
			obj is SecondaryType type && Equals(type.Container, Container) && Equals(type.Parameter, Parameter);

		public override IEnumerable<string> Dump() => new[]
			{
				"secondary type:\n",
				"  container type:\n"
			}
			.Concat(Container.Dump().Select(MapFunc).Select(MapFunc))
			.Concat(new[] {"  parameter type:\n"})
			.Concat(Parameter.Dump().Select(MapFunc).Select(MapFunc));
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