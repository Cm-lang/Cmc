#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parser.ObjectRegex
{
	public class ObjRegexError : Exception
	{
		public ObjRegexError(string info) : base(info)
		{
		}
	}

	public class StackOverFlowError : Exception
	{
		public StackOverFlowError(string info) : base(info)
		{
		}
	}

	public class NameError : Exception
	{
		public NameError(string info) : base(info)
		{
		}
	}

	public class CompileError : Exception
	{
		public CompileError(string info) : base(info)
		{
		}
	}

	public static class RegexTool
	{
		public static (string, Func<string, string>) ReMatch(string a, bool escape = false)
		{
			var tokenRule = escape ? Regex.Escape(a) : a;
			var re = new Regex(tokenRule);

			return (tokenRule, (string str) =>
				{
					if (str.Length.Equals(0))
						return null;
					var r = re.Match(str);
					if (r.Index != 0 || r.Length != str.Length)
						return null;
					return str;
				}
				);
		}
	}

	public class Mode : List<Mode>
	{
		public string Name;
		public string Value;

		public Mode SetName(string name)
		{
			Name = name;
			return this;
		}

		public Mode SetValue(string value)
		{
			Value = value;
			return this;
		}

		public string Dump(int i = 1)
		{
			var space = "\n" + new string(' ', 4 * i);
			var toDump = string.Join("",
				this.Select(
					mode => mode.Value != null
						? (mode.Value.Equals("\n")
							? $"{space}{mode.Name}['{@"\n"}']{space}"
							: $"{mode.Name}['{mode.Value}']{space}")
						: mode.Dump(i + 1)));
			return $"{Name}[{toDump}{space}]";
		}
	}

	public class MetaInfo
	{
		public int Count = 0;
		public int Rdx = 0;

		public int TraceLength = 0;
		public int HistoryLength = 0;
		public Stack<(int, string)> Trace = null;


		public Stack<(int, int, int)> History = null;

		protected (int, string) TracePop()
		{
			--TraceLength;
			return Trace.Pop();
		}

		public void TracePush((int, string) tp)
		{
			++TraceLength;
			Trace.Push(tp);
		}

		protected (int, int, int) HistoryPop()
		{
			--HistoryLength;
			return History.Pop();
		}

		protected void HistoryPush((int, int, int) tp)
		{
			++HistoryLength;
			History.Push(tp);
		}

		public MetaInfo(int count = 0, int rdx = 0, Stack<(int, string)> trace = null)
		{
			Count = count;
			Rdx = rdx;

			if (trace == null)
				Trace = new Stack<(int, string)>();
			else
				foreach (var unused in Trace = trace) ++TraceLength;

			History = new Stack<(int, int, int)>();
		}

		public MetaInfo Branch()
		{
			HistoryPush((Count, Rdx, TraceLength));
			return this;
		}

		public MetaInfo Rollback()
		{
			if (HistoryLength == 0)
				throw new StackOverFlowError("Pull nothing.");
			int traceLength;
			(Count, Rdx, traceLength) = HistoryPop();
			while (traceLength != TraceLength)
				TracePop();
			return this;
		}

		public MetaInfo Pull()
		{
			if (HistoryLength == 0)
				throw new StackOverFlowError("Pull nothing.");
			HistoryPop();
			return this;
		}

		public string DumpTrace() => string.Join(" ",
			Trace.Select((a, b) => $"({a}, {b})")
		);
	}

	public abstract class BaseAst
	{
		public string Name;
		public bool HasRecur;

		public abstract Mode Match(string[] objs, ref MetaInfo meta, bool partial = true);
	}

	public class LazyDef : BaseAst
	{
		public LazyDef(string name) => Name = name;

		public override Mode Match(string[] objs, ref MetaInfo meta, bool partial = true) =>
			throw new CompileError($"Ast {Name} Not compiled!");
	}

	public class Liter : BaseAst
	{
		public string TokenRule;

		public Func<string, string> F;

		public Liter(string i, string name = null, bool escape = false)
		{
			var (tokenRule, f) = RegexTool.ReMatch(i, escape);
			Name = name;
			HasRecur = false;
			TokenRule = tokenRule;
			F = f;
		}

		public override Mode Match(string[] objs, ref MetaInfo meta, bool partial = true)
		{
			var left = objs.Length - meta.Count;
			if (left == 0) return null;
			var r = F(objs[meta.Count]);
			if (r == null || !partial && left != 1) return null;
			if (r == "\n") meta.Rdx += 1;
			meta.Count += 1;

			return new Mode().SetName(Name).SetValue(r);
		}

		public static Liter ELiter(string i, string name = null) =>
			new Liter(i, name, true);
	}

	public class Ast : BaseAst
	{
		public List<List<BaseAst>> Possibilities;
		protected BaseAst[][] Cache;
		public Dictionary<string, Ast> CompileClosure;
		public bool Compiled;

		public Ast()
		{
		}

		public Ast(ref Dictionary<string, Ast> compileClosure,
			string name = null,
			params BaseAst[][] ebnf)
		{
			Name = name ?? throw new NameError("Name not found! Each kind of ast should have a identity name.");

			Cache = ebnf;
			if (compileClosure.Keys.Contains(name))
				throw new NameError("Name of Ast should be identified!");
			compileClosure[name] = this;
			Compiled = false;
			CompileClosure = compileClosure;
			Possibilities = new List<List<BaseAst>>();
		}

		public Ast Compile(ref HashSet<string> recurSearcher)
		{
			if (recurSearcher == null)
				recurSearcher = new HashSet<string> {Name};
			else
			{
				if (recurSearcher.Contains(Name))
				{
					HasRecur = true;
					Compiled = true;
				}
				else
					recurSearcher.Add(Name);
			}

			if (Compiled)
				return this;

			foreach (var es in Cache)
			{
				var possibility = new List<BaseAst>();
				Possibilities.Add(possibility);
				foreach (var e in es)
				{
					switch (e)
					{
						case Liter liter:
							possibility.Add(liter);
							break;
						case LazyDef _:
						{
							var refered = CompileClosure[e.Name];
							refered.Compile(ref recurSearcher);
							possibility.Add(refered);
							if (refered.HasRecur)
								HasRecur = true;
							break;
						}
						case Seq refered:
						{
							refered.Compile(ref recurSearcher);
							possibility.Add(refered);
							if (refered.HasRecur)
								HasRecur = true;
							break;
						}
						default:
							throw new CompileError("Unsolved Ast Type");
					}
				}
			}
#if DEBUG
	if (has_recur)
	Console.WriteLine("Found Recursive EBNF Node => "+name);
#endif

			Cache = null;
			Compiled = true;
			return this;
		}

		public override Mode Match(string[] objs, ref MetaInfo meta, bool partial = true)
		{
			if (meta == null)
			{
				throw new ArgumentNullException(nameof(meta));
			}
#if DEBUG
	Console.WriteLine($"{this.name} ");
#endif

			var res = new Mode().SetName(Name);
			var hasRes = false;

			foreach (var possibility in Possibilities)
			{
				meta.Branch(); // Make a branch in case of resetting.
				foreach (var thing in possibility)
				{
					var history = (meta.Count, thing.Name);
					Mode r;

					if (thing.HasRecur)
					{
						if (meta.Trace.Contains(history))
						{
							Console.WriteLine("Found Left Recursion. Dealed.");
							r = null;
						}
						else
						{
							meta.Trace.Push(history);
							r = thing.Match(objs, ref meta, true);
						}
					}
					else
					{
						// DEBUG View: the trace of parsing.
#if DEBUG
	meta.trace.Push(history);
#endif
						r = thing.Match(objs, ref meta, true);
					}

					if (r == null)
					{
						// Do not match current possibility, then rollback.
						res.Clear();
						meta.Rollback();
						goto ContinueForNewPossibility;
					}

					if (thing is Seq)
					{
						res.AddRange(r);
					}
					else
					{
						res.Add(r);
					}

#if DEBUG
	Console.WriteLine($"{thing.name} <= {r.Dump()}");
#endif
				}
#if DEBUG
	Console.WriteLine($"RETURN from {this.name} ");
#endif
				hasRes = true;
				break;
				ContinueForNewPossibility:
				;
			}
			if (!hasRes)
			{
#if DEBUG
	Console.WriteLine($"RETURN None from {this.name} ");
#endif
				return null;
			}
			meta.Pull();
			var left = objs.Length - meta.Count;
			if (partial || left == 0)
			{
#if DEBUG
	Console.WriteLine($"RETURN Some from {this.name}");
#endif
				return res;
			}
#if DEBUG
	Console.WriteLine($"RETURN None from {this.name} (No partial and do not match all)");
#endif
			return null;
		}
	}

	public class Seq : Ast
	{
		int _atleast;
		int _atmost;

		public Seq()
		{
		}

		public Seq(ref Dictionary<string, Ast> compileClosure,
			string name = null,
			int atleast = 1,
			int atmost = -1,
			params BaseAst[][] ebnf) : base(ref compileClosure, name, ebnf)
		{
			_atleast = atleast;
			_atmost = atmost;
		}

		public override Mode Match(string[] objs, ref MetaInfo meta, bool partial = true)
		{
			if (meta == null)
				throw new ArgumentNullException(nameof(meta));
			var res = new Mode().SetName(Name);
			var left = objs.Length - meta.Count;
			if (left == 0)
				return _atleast == 0 ? res : null;
			meta.Branch();
			Mode r;
			int idx = 0;
			if (_atmost > 0)
				while (true)
				{
					if (idx >= _atmost) break;
					r = base.Match(objs, ref meta, true);
					if (r == null)
						break;
					res.AddRange(r);
					++idx;
				}
			else
				while (true)
				{
					r = base.Match(objs, ref meta, true);
					if (r == null) break;
					res.AddRange(r);
					++idx;
				}
#if DEBUG
	Console.WriteLine($"{name} <= {res.Dump()}");
#endif
			if (idx < _atleast)
			{
				meta.Rollback();
				return null;
			}
			meta.Pull();
			return res;
		}
	}
}