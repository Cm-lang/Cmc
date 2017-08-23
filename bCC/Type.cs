using System.Collections.Generic;
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
	public class SecondaryType : Type
	{
		public SecondaryType(MetaData metaData, [NotNull] string name) : base(metaData, name)
		{
		}

		public override IEnumerable<string> Dump() => new[] {"secondary type:\n", "  " + Name + "\n"};
	}

	/// <summary>
	/// FEATURE #7
	/// </summary>
	public class ThirdLevelType : Type
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

		public ThirdLevelType(MetaData metaData, [NotNull] Type container, [NotNull] Type parameter) :
			base(metaData, PrimaryTypeToString(container, parameter))
		{
			Container = container;
			Parameter = parameter;
		}

		[NotNull]
		public static string PrimaryTypeToString([NotNull] Type args, [NotNull] Type ret) => args + "<" + ret + ">";

		public override string ToString() => PrimaryTypeToString(Container, Parameter);
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
	}
}