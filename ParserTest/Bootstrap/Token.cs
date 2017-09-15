using System.Text.RegularExpressions;
using Parser.ObjectRegex;

namespace ParserTest.Bootstrap
{
	public static class DefualtToken
	{
		public static readonly Liter Name = new Liter("[a-zA-Z_][a-zA-Z0-9]*", "Name");
		public static readonly Liter Number = new Liter(@"\d+", "Number");
		public static readonly Liter Str = new Liter(@"[R]{0,1}'[\w|\W]*?'", "Str");
		public static readonly Liter Newline = new Liter("\n", "NEWLINE");

		public static readonly Liter Lbb = Liter.ELiter("{", "LBB");
		public static readonly Liter Lb = Liter.ELiter("[", "LB");
		public static readonly Liter Lp = Liter.ELiter("(", "LP");

		public static readonly Liter Rbb = Liter.ELiter("}", "RBB");
		public static readonly Liter Rb = Liter.ELiter("]", "RB");
		public static readonly Liter Rp = Liter.ELiter(")", "RP");

		public static readonly Liter SeqStar = Liter.ELiter("*", "SeqStar");
		public static readonly Liter SeqPlus = Liter.ELiter("+", "SeqPlus");
		public static readonly Liter Def = Liter.ELiter("::=", "Def");
		public static readonly Liter OrSign = Liter.ELiter("|", "OrSign");

		public static Regex Token() => new Regex(
			string.Join("|",
				Name.TokenRule,
				Number.TokenRule,
				Str.TokenRule,
				Newline.TokenRule,
				Lbb.TokenRule,
				Rbb.TokenRule,
				Lp.TokenRule,
				Rp.TokenRule,
				Lb.TokenRule,
				Rb.TokenRule,
				SeqPlus.TokenRule,
				SeqStar.TokenRule,
				Def.TokenRule,
				OrSign.TokenRule)
		);
	}
}