using System;
using System.Linq;
using System.Text;
using Cmc;
using Cmc.Expression;
using Cmc.Statement;
using JetBrains.Annotations;
using static System.StringComparison;
using static LLVM.GenAstHolder;
using static LLVM.GenExpression;
using static LLVM.TypeConverter;

namespace LLVM
{
	public static class GenDeclaration
	{
		public static void GenAstDeclaration(
			[NotNull] StringBuilder builder,
			[NotNull] Declaration element,
			ref ulong varName)
		{
			if (element is TypeDeclaration typeDeclaration)
				builder.AppendLine($"; type alias: <{typeDeclaration.Name}> -> <{typeDeclaration.Type}>");
			else if (element is VariableDeclaration variable)
			{
				if (variable.IsGlobal)
				{
					if (variable.Expression is LambdaExpression lambdaExpression)
					{
						if (string.Equals(variable.Name, "main", Ordinal))
						{
							var retTypeName = lambdaExpression.GetExpressionType().Name;
							// FEATURE #35
							if (!string.Equals(retTypeName, "i32", Ordinal))
							{
								if (string.Equals(retTypeName, "nulltype", Ordinal))
									lambdaExpression.Body.Statements.Add(new ReturnStatement(lambdaExpression.MetaData,
										new IntLiteralExpression(lambdaExpression.MetaData, "0", true)));
							}
							else
							{
								Errors.Add(
									$"the main function must return i32 or null, but it's {retTypeName}");
								throw new CompilerException();
							}
							Attr.MainFunctionIndex = Attr.GlobalFunctionCount;
							builder.AppendLine(
								$"define i32 @main() #{Attr.GlobalFunctionCount++} {{");
							GenAst(builder, lambdaExpression.Body, ref varName);
							if (!(lambdaExpression.Body.Statements.Last() is ReturnStatement))
								builder.AppendLine("ret i32 0");
							builder.AppendLine("}");
						}
						else
						{
							// TODO create functions
							// global functions doesn't need capturing, so much easier
						}
					}
					else
					{
						builder.Append($"@{varName}=global {ConvertType(variable.Type)} ");
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
				else
				{
					if (!variable.Used)
					{
						builder.AppendLine($"; unused declaration {variable.Name}, removed");
						GenAstExpression(builder, variable.Expression, ref varName);
						return;
					}
					builder.AppendLine(
						$"%{varName} = alloca {ConvertType(variable.Type)}, align {variable.Align}");
					variable.Address = varName;
					GenAstExpression(builder, variable.Expression, ref varName);
					varName++;
				}
			}
		}
	}
}