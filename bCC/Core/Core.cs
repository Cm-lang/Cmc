using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static bCC.Core.Environment;

namespace bCC.Core
{
	/// <summary>
	///   The core of this compiler
	///   input a file (multiple file will be supported later)
	/// </summary>
	public class Core
	{
		private readonly IDictionary<string, IEnumerable<string>> _mutualRecList;

		private readonly IDictionary<string, bool> _mutualRecMark;

		public Core()
		{
			_mutualRecMark = new ConcurrentDictionary<string, bool>();
			_mutualRecList = new ConcurrentDictionary<string, IEnumerable<string>>();
		}

		public void Compile(params Declaration[] declarations)
		{
			var planet = new Environment(SolarSystem);
			foreach (var declaration in declarations)
				planet.Declarations.Add(declaration);
			foreach (var declaration in declarations)
				declaration.SurroundWith(planet);
		}

		/// <summary>
		///   to check if there's mutual recursion definitions
		///   in the structs.
		///   FEATURE #34
		/// </summary>
		/// <param name="declarations"></param>
		public void CheckMutualRec(IEnumerable<Declaration> declarations)
		{
			var structs = new List<string>();
			foreach (var keyValuePair in declarations
				.Where(i => i is StructDeclaration)
				.Select(i =>
					new KeyValuePair<string, IEnumerable<string>>(
						i.Name,
						((StructDeclaration) i)
						.FieldList
						.Where(q => !SolarSystem
							.Declarations
							.Select(qq => qq.Name)
							.Contains(q.Type.ToString()))
						.Select(j => j.Type.Name))))
			{
				structs.Add(keyValuePair.Key);
				_mutualRecList.Add(keyValuePair);
				_mutualRecMark[keyValuePair.Key] = false;
			}
			foreach (var chain in structs
				.Select(CheckMutualRecRec)
				.Where(chain => null != chain))
				Errors.Add($"mutual recursion in structure definition is detected: [{chain}]");
		}

		/// <summary>
		///   dfs
		/// </summary>
		/// <param name="declaration">current delaration</param>
		/// <returns>if not null, return the dependency chain</returns>
		[CanBeNull]
		private string CheckMutualRecRec(string declaration)
		{
			if (_mutualRecMark[declaration]) return declaration;
			_mutualRecMark[declaration] = true;
			foreach (var ret in _mutualRecList[declaration]
				.Select(CheckMutualRecRec)
				.Where(ret => null != ret))
				return $"{declaration}]-[{ret}";
			_mutualRecMark[declaration] = false;
			return null;
		}
	}
}