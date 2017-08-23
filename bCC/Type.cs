using System.Collections.Generic;

namespace bCC
{
	public abstract class Type : Ast
	{
		public readonly string Name;

		protected Type(MetaData metaData, string name) : base(metaData) => Name = name;
		public override string ToString() => Name;
	}

	/// <summary>
	/// FEATURE #0
	/// </summary>
	public class SecondaryType : Type
	{
		public SecondaryType(MetaData metaData, string name) : base(metaData, name)
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

		public ThirdLevelType(MetaData metaData, Type container, Type parameter) :
			base(metaData, PrimaryTypeToString(container, parameter))
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

		public LambdaType(MetaData metaData, IList<Type> args, Type ret) :
			base(metaData, LambdaTypeToString(args, ret))
		{
			ArgsList = args;
			RetType = ret;
		}

		public override string ToString() => LambdaTypeToString(ArgsList, RetType);
		public static string LambdaTypeToString(IList<Type> args, Type ret) => string.Join(",", args) + "->" + ret;
	}
}