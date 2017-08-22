using System;
using System.Collections;
using System.Collections.Generic;

namespace bCC_AST
{
	public abstract class Type
	{
		public readonly string Name;

		protected Type(string name) => Name = name;
	}

	public class SecondaryType : Type
	{
		public SecondaryType(string name) : base(name)
		{
		}
	}

	public class LambdaType : Type
	{
		public readonly IList<Type> ArgsList;
		public readonly Type RetType;

		public LambdaType(IList<Type> args, Type ret) : base(string.Join(",", args) + "->" + ret)
		{
			ArgsList = args;
			RetType = ret;
		}
	}
}