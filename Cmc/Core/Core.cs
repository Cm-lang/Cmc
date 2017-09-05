using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cmc.Decl;
using JetBrains.Annotations;

namespace Cmc.Core
{
	/// <summary>
	///     The core of this compiler
	///     input a file (multiple file will be supported later)
	/// </summary>
	public class Core
	{
		[NotNull] private readonly IDictionary<string, IEnumerable<string>> _mutualRecList;

		[NotNull] private readonly IDictionary<string, bool> _mutualRecMark;

		public Core()
		{
			_mutualRecMark = new ConcurrentDictionary<string, bool>();
			_mutualRecList = new ConcurrentDictionary<string, IEnumerable<string>>();
		}

		/// <summary>
		///     Do all static analyze jobs
		/// </summary>
		/// <param name="declarations">
		///     Parsed top-level declarations
		/// </param>
		/// <returns>the analyzed declarations (errors are given during this process)</returns>
		[NotNull]
		public Declaration[] Analyze(params Declaration[] declarations)
		{
			var planet = new Environment(Environment.SolarSystem);
			foreach (var declaration in declarations)
			{
				planet.Declarations.Add(declaration);
				if (declaration is VariableDeclaration variableDeclaration)
					variableDeclaration.IsGlobal = true;
			}
			CheckMutualRec(declarations);
			// TODO topo sort, dependency analyze
			foreach (var declaration in declarations)
				declaration.SurroundWith(planet);
			return declarations;
		}

		/// <summary>
		///     to check if there's mutual recursion definitions
		///     in the structs.
		///     FEATURE #34
		/// </summary>
		/// <param name="declarations"></param>
		public void CheckMutualRec([NotNull] IEnumerable<Declaration> declarations)
		{
			var structs = new List<string>();
			foreach (var keyValuePair in
				from i in declarations
				where i is StructDeclaration
				select
					new KeyValuePair<string, IEnumerable<string>>(
						i.Name,
						from q in ((StructDeclaration) i)
							.FieldList
						where !Environment.SolarSystem
							.Declarations
							.Select(qq => qq.Name)
							.Contains(q.Type.ToString())
						select q.Type.Name))
			{
				structs.Add(keyValuePair.Key);
				_mutualRecList.Add(keyValuePair);
				_mutualRecMark[keyValuePair.Key] = false;
			}
			foreach (var chain in
				from chain in structs.Select(CheckMutualRecRec)
				where null != chain
				select chain)
			{
				Errors.Add($"mutual recursion in structure definition is detected: [{chain}]");
				break;
			}
		}

		/// <summary>
		///     dfs
		/// </summary>
		/// <param name="declaration">current delaration</param>
		/// <returns>if not null, return the dependency chain</returns>
		[CanBeNull]
		private string CheckMutualRecRec(string declaration)
		{
			if (_mutualRecMark[declaration]) return declaration;
			_mutualRecMark[declaration] = true;
			foreach (var ret in
				from ret in _mutualRecList[declaration].Select(CheckMutualRecRec)
				where null != ret
				select ret)
				return $"{declaration}]-[{ret}";
			_mutualRecMark[declaration] = false;
			return null;
		}
	}
}