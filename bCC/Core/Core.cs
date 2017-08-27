using static bCC.Core.Environment;

namespace bCC.Core
{
	/// <summary>
	///   The core of this compiler
	///   input a file (multiple file will be supported later)
	/// </summary>
	public class Core
	{
		public void Compile(params Declaration[] declarations)
		{
			var planet = new Environment(SolarSystem);
			foreach (var declaration in declarations)
			{
				declaration.SurroundWith(planet);
			}
		}
	}
}