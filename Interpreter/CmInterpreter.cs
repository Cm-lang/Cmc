using System.Collections.Generic;
using Cmc;
using Cmc.Core;
using Cmc.Decl;

namespace Interpreter
{
	public class CmInterpreter
	{
		public static Environment InitEnv()
		{
			var planet = new Environment(Environment.SolarSystem);
			planet.Declarations.Add(new VariableDeclaration(MetaData.Empty, "+",
				type: new LambdaType(MetaData.Empty,
					new List<Type>
					{
						new PrimaryType(MetaData.Empty, "i8"),
						new PrimaryType(MetaData.Empty, "i8")
					},
					new PrimaryType(MetaData.Empty, "i8"))));
			// TODO
			return planet;
		}
	}
}