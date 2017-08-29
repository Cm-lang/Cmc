using System;
using System.IO;
using System.Linq;
using System.Text;
using bCC;
using bCC.Core;
using bCC.Expression;
using bCC.Statement;
using JetBrains.Annotations;
using Tools;

namespace LLVM
{
	public static class Gen
	{
		public static ulong GlobalVarCount;

		public static string Generate(params Declaration[] declarations)
		{
			var core = new Core();
			var builder = new StringBuilder();
			var analyzedDeclarations = core.Analyze(declarations);
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				builder.Append(
					$"@.str{i}=unnamed_addr constant [{str.Length - str.Count(c => c == '\\')} x i8] c\"{str}\"");
			}
			foreach (var analyzedDeclaration in analyzedDeclarations)
				GenAst(builder, analyzedDeclaration, true);
			return builder.ToString();
		}

		public static void RunLlvm(
			string outputFile,
			params Declaration[] declarations)
		{
			File.WriteAllText(outputFile, Generate(declarations));
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.ll -o {outputFile}");
		}

		/// <summary>
		///  C# cannot have a tenary operator in the string templete
		///  So I made this
		/// </summary>
		/// <param name="isGlobal">is it a global declaration</param>
		/// <returns>the prefix of the declaration</returns>
		public static char DetermineDeclarationPrefix(bool isGlobal) => isGlobal ? '@' : '%';

		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="builder">the string builder used to append ir</param>
		/// <param name="element">the ast element waiting to be generated</param>
		/// <param name="isGlobal">is it a global declaration?</param>
		[CanBeNull]
		public static string GenAst(
			StringBuilder builder,
			Ast element,
			bool isGlobal = false)
		{
			if (element is LiteralExpression expression)
			{
				var varName = $"{DetermineDeclarationPrefix(isGlobal)}{GlobalVarCount++}";
				if (expression is StringLiteralExpression str)
					builder.AppendLine(
						$"");
				else if (expression is IntLiteralExpression integer)
					builder.AppendLine(
						$"{varName}=alloca {integer.Type}, align {Math.Ceiling(integer.Length / 8.0)}");
				return varName;
			}
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else if (element is VariableDeclaration variable)
			{
				var exprVarName = GenAst(builder, variable.Expression);
				// use this var name
			}
			else if (element is ReturnStatement returnStatement)
			{
				var expr = returnStatement.Expression;
				var varName = GenAst(builder, expr);
				// TODO assign the value
			}
			return null;
		}

		public static void Main(string[] args)
		{
			Console.WriteLine(Generate(
				new VariableDeclaration(MetaData.Empty,
					"i", new IntLiteralExpression(MetaData.Empty, "1", true))
			));
		}
	}
}