using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace bCC
{
	public abstract class Type : Ast
	{
		[NotNull] public readonly string Name;

		protected Type(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;
		public override string ToString() => Name;
	}

	/// <summary>
	/// FEATURE #0
	/// </summary>
	public class PrimaryType : Type
	{
		public PrimaryType(MetaData metaData, [NotNull] string name) : base(metaData, name)
		{
		}

		public override IEnumerable<string> Dump() => new[] {"primary type [" + Name + "]\n"};
	}

	/// <summary>
	/// FEATURE #7
	/// </summary>
	public class SecondaryType : Type
	{
		public override Environment Env
		{
			[NotNull] get => _env;
			set
			{
				_env = value;
				Container.Env = Env;
				Parameter.Env = Env;
			}
		}

		[NotNull] public readonly Type Container;
		[NotNull] public readonly Type Parameter;
		private Environment _env;

		public SecondaryType(MetaData metaData, [NotNull] Type container, [NotNull] Type parameter) :
			base(metaData, SecondaryTypeToString(container, parameter))
		{
			Container = container;
			Parameter = parameter;
		}

		[NotNull]
		public static string SecondaryTypeToString([NotNull] Type args, [NotNull] Type ret) => args + "<" + ret + ">";

		public override string ToString() => SecondaryTypeToString(Container, Parameter);

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
	/// FEATURE #6
	/// </summary>
	public class LambdaType : Type
	{
		public override Environment Env
		{
			[NotNull] get => _env;
			set
			{
				_env = value;
				foreach (var type in ArgsList) type.Env = Env;
				RetType.Env = Env;
			}
		}

		[NotNull] public readonly IList<Type> ArgsList;
		[NotNull] public readonly Type RetType;
		private Environment _env;

		public LambdaType(MetaData metaData, [NotNull] IList<Type> args, [NotNull] Type ret) :
			base(metaData, LambdaTypeToString(args, ret))
		{
			ArgsList = args;
			RetType = ret;
		}

		public override string ToString() => LambdaTypeToString(ArgsList, RetType);

		[NotNull]
		public static string LambdaTypeToString([NotNull] IList<Type> args, [NotNull] Type ret) =>
			string.Join(",", args) + "->" + ret;

		public override IEnumerable<string> Dump() => new[]
			{
				"lambda type:\n",
				"  parameters' types:\n"
			}
			.Concat(ArgsList.SelectMany(i => i.Dump().Select(MapFunc).Select(MapFunc)))
			.Concat(new[] {"  return type:\n"})
			.Concat(RetType.Dump().Select(MapFunc).Select(MapFunc));
	}
}