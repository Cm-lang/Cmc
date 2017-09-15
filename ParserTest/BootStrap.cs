using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.ObjectRegex;
using ParserTest.Bootstrap;

namespace ParserTest
{
	[TestClass]
	public class NaiveTest
	{
		[TestMethod]
		public void TestBootstrap()
		{
			const string code = @"

Expr    ::= Or+
Or      ::= AtomExpr ('|' AtomExpr)* 

AtomExpr::= Atom Trailer 
Atom    ::= Name | Str | '[' Expr ']' | '(' Expr ')' 


Def    ::= '::='
Equals ::= Name Def Expr

Trailer::= ['*' | '+' | '{' Number{1 2} '}']

Name   ::= R'[a-zA-Z_][a-zA-Z0-9]*'
Str    ::= R'[\w|\W]*'
Number ::= R'\d+'

";
			// Initialize parser
			var parser = BootstrapParser.GenParser();
			var re = DefualtToken.Token();

			// Gen tokenized words
			var tokens = re.Matches(code).Select(i => i.ToString()).ToArray();

			// Parsing
			var meta = new MetaInfo();
			Console.WriteLine(parser.Stmt.Match(
				objs: tokens,
				partial: false,
				meta: ref meta
			).Dump());
		}
	}
}