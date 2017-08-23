using System;
using System.Collections.Generic;
using System.Linq;

namespace bCC
{
	public class Environment
	{
		public readonly IList<Declaration> Declarations = new List<Declaration>();
		public readonly Environment Outer;

		public Environment(Environment outer = null)
		{
			Outer = outer;
		}

		public IList<Declaration> FindAllDeclarationsByName(string name)
		{
			var env = this;
			var list = new List<Declaration>();
			do list.AddRange(env.Declarations.Where(declaration => declaration.Name == name)); while ((env = env.Outer) != null);
			return list;
		}

		/// <summary>
		/// If there's no such declaration, this funciton will return null.
		/// </summary>
		/// <param name="name">the name of the required declaration</param>
		/// <returns></returns>
		public Declaration FindDeclarationByName(string name)
		{
			var env = this;
			// I don't know why I must declare a variable here
			// I think I can return the local object directly but it fails
			Declaration ret = null;
			do
			{
				foreach (var declaration in env.Declarations)
					if (string.Equals(declaration.Name, name, StringComparison.Ordinal))
					{
						Console.WriteLine(declaration.Name + ", " + name);
						ret = declaration;
						goto end;
					}
			} while ((env = env.Outer) != null);
			end:
			return ret;
		}
	}
}