using System.IO;
using System.Text;
using bCC;
using bCC.Core;
using bCC.Expression;
using bCC.Statement;
using JetBrains.Annotations;
using Tools;
using static LLVM.TypeConverter;

namespace LLVM
{
	public static class Gen
	{
		public static ulong GlobalVarCount;

		public static string Generate(
			[NotNull] params Declaration[] declarations)
		{
			var core = new Core();
			var builder = new StringBuilder();
			var analyzedDeclarations = core.Analyze(declarations);
			for (var i = 0; i < Constants.StringConstants.Count; i++)
			{
				var str = Constants.StringConstants[i];
				var len = Constants.StringConstantLengths[i];
				builder.Append(
					$"@.str{i}=private unnamed_addr constant [{len} x i8] c\"{str}\", align 1\n");
			}
			foreach (var analyzedDeclaration in analyzedDeclarations)
			{
				ulong varName = 0;
				GenAst(builder, analyzedDeclaration, ref varName);
			}
			return builder.ToString();
		}

		public static void RunLlvm(
			[NotNull] string outputFile,
			[NotNull] params Declaration[] declarations)
		{
			File.WriteAllText(outputFile, Generate(declarations));
			CommandLine.RunCommand($"llc-4.0 {outputFile}.ll -filetype=obj");
			CommandLine.RunCommand($"gcc {outputFile}.ll -o {outputFile}");
		}

		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="builder">the string builder used to append ir</param>
		/// <param name="element">the ast element waiting to be generated</param>
		/// <param name="varName"></param>
		public static void GenAst(
			[NotNull] StringBuilder builder,
			[NotNull] Ast element,
			ref ulong varName)
		{
			if (element is LiteralExpression expression)
			{
				if (expression is StringLiteralExpression str)
					builder.AppendLine(
						$"store i8* getelementptr inbounds ([{str.Length} x i8]," +
						$"[{str.Length} x i8]* @.str{str.ConstantPoolIndex}, i32 0, i32 0)," +
						$"i8** %{varName}, align 8");
				else if (expression is IntLiteralExpression integer)
					builder.AppendLine(
						$"store {integer.Type} {integer.Value}, {integer.Type}* %{varName}, align {integer.Type.Align}");
			}
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else if (element is VariableDeclaration variable)
			{
				builder.AppendLine($"%{varName} = alloca {variable.Type}, align {variable.Align}");
				GenAst(builder, variable.Expression, ref varName);
			}
			else if (element is ReturnStatement returnStatement)
			{
				var expr = returnStatement.Expression;
				GenAst(builder, expr, ref varName);
				builder.AppendLine(
					$"ret {ConvertType(expr.GetExpressionType())} %{varName}");
				varName++;
			}
			else if (element is StatementList statements)
			{
				ulong localVarCount = 0;
				foreach (var statement in statements.Statements)
				{
					GenAst(builder, statement, ref localVarCount);
					localVarCount++;
				}
			}
		}
	}
}