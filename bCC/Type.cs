using System.Collections.Generic;

namespace bCC
{
	public abstract class Type
	{
		public Environment Env;
		public readonly string Name;

		protected Type(string name) => Name = name;
		public override string ToString() => Name;
	}

	/// <summary>
	/// FEATURE #0
	/// </summary>
	public class SecondaryType : Type
	{
		public SecondaryType(string name) : base(name)
		{
		}
	}

	/// <summary>
	/// FEATURE #7
	/// </summary>
	public class ThirdLevelType : Type
	{
		public readonly Type Container;
		public readonly Type Parameter;

		public ThirdLevelType(Type container, Type parameter) : base(PrimaryTypeToString(container, parameter))
		{
			Container = container;
			Parameter = parameter;
		}

		public static string PrimaryTypeToString(Type args, Type ret) => args + "<" + ret + ">";

		public override string ToString() => PrimaryTypeToString(Container, Parameter);
	}

	/// <summary>
	/// FEATURE #6
	/// </summary>
	public class LambdaType : Type
	{
		public readonly IList<Type> ArgsList;
		public readonly Type RetType;

		public LambdaType(IList<Type> args, Type ret) : base(LambdaTypeToString(args, ret))
		{
			ArgsList = args;
			RetType = ret;
		}

		public override string ToString() => LambdaTypeToString(ArgsList, RetType);
		public static string LambdaTypeToString(IList<Type> args, Type ret) => string.Join(",", args) + "->" + ret;
	}
}