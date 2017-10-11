﻿using System.Text;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using JetBrains.Annotations;
using LLVMSharp;
using static LLVM.GenDeclaration;
using static LLVM.GenExpression;
using static LLVM.GenStatement;

namespace LLVM
{
	public static class GenAstHolder
	{
		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="builder">the string builder used to append ir</param>
		/// <param name="element">the ast element waiting to be generated</param>
		public static void GenAst(
			LLVMBuilderRef builder,
			[NotNull] Ast element)
		{
			// convertion
			while (element is Statement statement && statement.ConvertedStatementList != null)
				element = statement.ConvertedStatementList;
			// optimization
			while (element.OptimizedStatementList != null && !Pragma.KeepAll)
				element = element.OptimizedStatementList;
			switch (element)
			{
				case Expression expression:
					GenAstExpression(builder, expression);
					break;
				case Declaration declaration:
					GenAstDeclaration(builder, declaration);
					break;
				case Statement statement:
					GenAstStatement(builder, statement);
					break;
			}
		}
	}
}