using System.Collections.Generic;
using Parser.ObjectRegex;

namespace Parser.LanguageTest.Bootstrap
{
	public class Parser
	{
		public Ast Expr;
		public Ast Or;
		public Ast Split;
		public Ast Atom;
		public Ast AtomExpr;
		public Ast Eq;
		public Ast Stmt;
		public Ast Trailer;
		public Dictionary<string, Ast> CompileClosure;

		public Parser(Ast expr, Ast or, Ast split,
			Ast atom, Ast atomExpr,
			Ast eq, Ast stmt,
			Ast trailer,
			Dictionary<string, Ast> compileClosure
		)
		{
			Expr = expr;
			Or = or;
			Split = split;
			Atom = atom;
			Eq = eq;
			AtomExpr = atomExpr;
			Stmt = stmt;
			Trailer = trailer;
			CompileClosure = compileClosure;
		}

		public static Parser GenParser()
		{
			var compileClosure = new Dictionary<string, Ast>();
			var expr = new Ast(
				compileClosure: ref compileClosure,
				name: "Expr",
				ebnf: new BaseAst[]
				{
					new Seq(
						compileClosure: ref compileClosure,
						atleast: 0,
						name: "Or+",
						ebnf: new BaseAst[]
						{
							new LazyDef("Or")
						}
					)
				}
			);

			var or = new Ast(
				compileClosure: ref compileClosure,
				name: "Or",
				ebnf: new BaseAst[]
				{
					new LazyDef("AtomExpr"),
					new Seq(
						compileClosure: ref compileClosure,
						atleast: 0,
						name: "('|' AtomExpr)*",
						ebnf: new BaseAst[]
						{
							DefualtToken.OrSign,
							new LazyDef("AtomExpr")
						}
					)
				}
			);
			var atomExpr = new Ast(
				compileClosure: ref compileClosure,
				name: "AtomExpr",
				ebnf: new BaseAst[]
				{
					new LazyDef("Atom"),
					new LazyDef("Trailer")
				}
			);
			var trailer = new Seq(
				ref compileClosure,
				"Trailer",
				0,
				ebnf: new[]
				{
					new BaseAst[]
					{
						DefualtToken.SeqStar
					},
					new BaseAst[]
					{
						DefualtToken.SeqPlus
					},
					new BaseAst[]
					{
						DefualtToken.Lbb,
						new Seq(
							compileClosure: ref compileClosure,
							atleast: 1,
							atmost: 2,
							name: "Number{1 2}",
							ebnf: new BaseAst[]
							{
								DefualtToken.Number
							}),
						DefualtToken.Rbb
					}
				}
			);
			var atom = new Ast(
				ref compileClosure,
				"Atom", new BaseAst[]
				{
					DefualtToken.Name
				}, new BaseAst[]
				{
					DefualtToken.Str
				}, new BaseAst[]
				{
					DefualtToken.Lb,
					new LazyDef("Expr"),
					DefualtToken.Rb
				}, new BaseAst[]
				{
					DefualtToken.Lp,
					new LazyDef("Expr"),
					DefualtToken.Rp
				});
			var eq = new Ast(
				compileClosure: ref compileClosure,
				name: "Eq",
				ebnf: new BaseAst[]
				{
					DefualtToken.Name,
					DefualtToken.Def,
					new LazyDef("Expr")
				}
			);
			var stmt = new Ast(
				compileClosure: ref compileClosure,
				name: "Stmt",
				ebnf: new BaseAst[]
				{
					new Seq(
						compileClosure: ref compileClosure,
						name: "Eq*",
						atleast: 0,
						ebnf: new BaseAst[]
						{
							new LazyDef("SPLIT"),
							new LazyDef("Eq"),
							new LazyDef("SPLIT")
						}
					)
				}
			);
			var split = new Seq(
				compileClosure: ref compileClosure,
				name: "SPLIT",
				atleast: 0,
				ebnf: new BaseAst[]
				{
					DefualtToken.Newline
				}
			);
			var namestore = new HashSet<string>();
			split.Compile(ref namestore);
			stmt.Compile(ref namestore);

			return new Parser(
				expr: expr,
				atom: atom,
				eq: eq,
				or: or,
				stmt: stmt,
				atomExpr: atomExpr,
				trailer: trailer,
				split: split,
				compileClosure: compileClosure
			);
		}
	}
}