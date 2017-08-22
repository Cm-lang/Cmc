using System.Collections.Generic;

namespace bCC_AST
{
	public class Environment
	{
		public readonly IList<Declaration> Declarations = new List<Declaration>();
		public readonly Environment Outer;

		public Environment(Environment outer)
		{
			Outer = outer;
		}

		public IList<Declaration> FindAllDeclarationsByName(string name)
		{
		}

		/// <summary>
		/// If there's no such declaration, this funciton will return null.
		/// </summary>
		/// <param name="name">the name of the required declaration</param>
		/// <returns></returns>
		public Declaration FindDeclarationByName(string name)
		{
			var env = this;
			do
			{
				foreach (var declaration in env.Declarations)
					if (declaration.Name == name) return declaration;
			} while ((env = env.Outer) != null);
			return null;
		}
	}
}