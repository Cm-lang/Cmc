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
		/// <param name="varName"></param>
		public static void GenAst(
			StringBuilder builder,
			Ast element,
			[CanBeNull] ulong? varName = null)
		{
			if (element is LiteralExpression expression)
			{
				if (expression is StringLiteralExpression str)
					builder.AppendLine(
						$"{varName}=alloca i8*, align 8");
				else if (expression is IntLiteralExpression integer)
					builder.AppendLine(
						$"store {varName}=alloca {integer.Type}, align {Math.Ceiling(integer.Length / 8.0)}");
			}
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else if (element is VariableDeclaration variable)
			{
				GenAst(builder, variable.Expression);
			}
			else if (element is ReturnStatement returnStatement)
			{
				var expr = returnStatement.Expression;
				GenAst(builder, expr);
				// TODO assign the value
			}
			else if (element is StatementList statements)
			{
				ulong localVarCount = 0;
				// TODO deal with other types
				foreach (var statement in statements.Statements)
					if (statement is VariableDeclaration declaration)
					{
						var rawType = declaration.Type.ToString();
						var align = declaration.Align;
						builder.AppendLine($"%{localVarCount++} = alloca {rawType}, align {align}");
					}
				foreach (var statement in statements.Statements)
					GenAst(builder, statement, localVarCount++);
			}
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