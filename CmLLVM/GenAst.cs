using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using JetBrains.Annotations;
using LLVMSharp;

namespace CmLLVM
{
	public static partial class Gen
	{
		/// <summary>
		///  generate llvm ir by the given ast
		/// </summary>
		/// <param name="module"></param>
		/// <param name="builder"></param>
		/// <param name="element">the ast element waiting to be generated</param>
		public static void GenAst(
			LLVMModuleRef module,
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
					GenAstExpression(module, builder, expression);
					break;
				case Declaration declaration:
					GenAstDeclaration(module, builder, declaration);
					break;
				case Statement statement:
					GenAstStatement(module, builder, statement);
					break;
			}
		}
	}
}