using System.Linq;
using System.Text;
using Cmc;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using JetBrains.Annotations;
using static System.StringComparison;
using static LLVM.GenAstHolder;
using static LLVM.GenExpression;
using static LLVM.TypeConverter;

namespace LLVM
{
	public static class GenDeclaration
	{
		private static void GenMain(
			[NotNull] StringBuilder builder,
			[NotNull] LambdaExpression lambda,
			ref ulong varName)
		{
			var retTypeName = lambda.GetExpressionType().Name;
			// FEATURE #35
			if (!string.Equals(retTypeName, "i32", Ordinal))
			{
				if (string.Equals(retTypeName, "nulltype", Ordinal))
					lambda.Body.Statements.Add(new ReturnStatement(lambda.MetaData,
						new IntLiteralExpression(lambda.MetaData, "0", true)));
			}
			else
			{
				var err = $"the main function must return i32 or null, but it's {retTypeName}";
				Errors.Add(err);
				throw new CompilerException(err);
			}
			Attr.MainFunctionIndex = Attr.GlobalFunctionCount;
			builder.AppendLine(
				$"define i32 @main() #{Attr.GlobalFunctionCount++} {{");
			GenAst(builder, lambda.Body, ref varName);
			if (!(lambda.Body.Statements.Last() is ReturnStatement))
				builder.AppendLine("  ret i32 0");
			builder.AppendLine("}");
		}

		public static void GenGlobVarDeclaration(
			[NotNull] StringBuilder builder,
			[NotNull] VariableDeclaration variable,
			ref ulong varName)
		{
			if (variable.Expression is LambdaExpression lambda)
			{
				if (string.Equals(variable.Name, "main", Ordinal))
					GenMain(builder, lambda, ref varName);
				else
				{
					builder.AppendLine(
						$"define @_cm_{variable.Name}_{variable.GetHashCode()}(" +
						string.Join(",",
							from i in lambda.ParameterList
							select ConvertType(i.Type)) +
						$") #{Attr.GlobalFunctionCount++} {{");
					// global functions doesn't need capturing, so much easier
				}
			}
			else
			{
				builder.Append($"@glob{varName}=global {ConvertType(variable.Type)} ");
				if (variable.Expression is IntLiteralExpression integer)
					builder.Append(
						$"{integer.Value}");
				else if (variable.Expression is BoolLiteralExpression boolean)
					builder.Append(
						$"{boolean.ValueToInt()}");
				else if (variable.Expression is StringLiteralExpression str)
					builder.Append(
						$"getelementptr inbounds ([{str.Length} x i8], [{str.Length} x i8]* " +
						$"@.str{str.ConstantPoolIndex}, i32 0, i32 0)");
				builder.AppendLine($", align {variable.Align}");
			}
			// TODO deal with other types
			variable.Address = varName;
			// TODO for complex initialization, generate a function to do this job
		}

		public static void GenAstDeclaration(
			[NotNull] StringBuilder builder,
			[NotNull] Declaration element,
			ref ulong varName)
		{
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"  ; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else if (element is VariableDeclaration variable)
			{
				if (variable.IsGlobal)
					GenGlobVarDeclaration(builder, variable, ref varName);
				else
				{
//					if (!variable.Used)
//					{
//						builder.AppendLine($"  ; unused declaration {variable.Name}, removed");
//						GenAstExpression(builder, variable.Expression, ref varName);
//						return;
//					}
					builder.AppendLine(
						$"  %var{varName} = alloca {ConvertType(variable.Type)}, align {variable.Align}");
					variable.Address = varName;
					GenAstExpression(builder, variable.Expression, ref varName);
					varName++;
				}
			}
		}
	}
}