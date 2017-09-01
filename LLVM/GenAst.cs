using System;
using System.Linq;
using System.Text;
using Cmc;
using Cmc.Core;
using Cmc.Expression;
using Cmc.Statement;
using JetBrains.Annotations;
using static System.StringComparison;
using static LLVM.TypeConverter;

namespace LLVM
{
	public static class GenAstHolder
	{
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
			if (element is EmptyStatement) return;
			// optimization
			if (element.OptimizedStatementList != null) element = element.OptimizedStatementList;
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
				if (variable.IsGlobal)
				{
					if (variable.Expression is LambdaExpression lambdaExpression)
					{
						if (string.Equals(variable.Name, "main", Ordinal))
						{
							var retTypeName = lambdaExpression.GetExpressionType().Name;
							if (!string.Equals(retTypeName, "i32", Ordinal))
								if (string.Equals(retTypeName, "nulltype", Ordinal))
									lambdaExpression.Body.Statements.Add(new ReturnStatement(lambdaExpression.MetaData,
										new IntLiteralExpression(lambdaExpression.MetaData, "0", true)));
								else
								{
									Errors.Add("the main function must return i32 or null");
									throw new CompilerException("");
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
							// TODO create an anonymous function
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
					builder.AppendLine(
						$"%{varName} = alloca {ConvertType(variable.Type)}, align {variable.Align}");
					variable.Address = varName;
					GenAst(builder, variable.Expression, ref varName);
					varName++;
				}
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
				ulong localVarCount = 1;
				foreach (var statement in statements.Statements)
				{
					GenAst(builder, statement, ref localVarCount);
					localVarCount++;
				}
			}
		}
	}
}