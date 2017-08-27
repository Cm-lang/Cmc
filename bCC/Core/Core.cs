using System;
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
		public void Compile(params Declaration[] declarations)
		{
			var planet = new Environment(SolarSystem);
			foreach (var declaration in declarations)
			{
				planet.Declarations.Add(declaration);
			}
			foreach (var declaration in declarations)
			{
				declaration.SurroundWith(planet);
			}
		}

		public IDictionary<string, bool> MutualRecMark =
			new ConcurrentDictionary<string, bool>();

		public IDictionary<string, IEnumerable<string>> MutualRecList =
			new ConcurrentDictionary<string, IEnumerable<string>>();

		/// <summary>
		///  to check if there's mutual recursion definitions
		///  in the structs.
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
				MutualRecList.Add(keyValuePair);
				MutualRecMark[keyValuePair.Key] = false;
			}
			foreach (var chain in structs
				.Select(CheckMutualRecRec)
				.Where(chain => null != chain))
				Errors.Add($"mutual recursion in structure definition is detected: [{chain}]");
		}

		/// <summary>
		///  dfs
		/// </summary>
		/// <param name="declaration">current delaration</param>
		/// <returns>if not null, return the dependency chain</returns>
		[CanBeNull]
		public string CheckMutualRecRec(string declaration)
		{
			if (MutualRecMark[declaration]) return declaration;
			MutualRecMark[declaration] = true;
			foreach (var ret in MutualRecList[declaration]
				.Select(CheckMutualRecRec)
				.Where(ret => null != ret))
				return $"{declaration}]-[{ret}";
			MutualRecMark[declaration] = false;
			return null;
		}
	}
}