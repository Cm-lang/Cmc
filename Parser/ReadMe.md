# EBNFParser for CSharp


### EBNF SelfExamination

[EBNF Grammar](../selfexamine.ebnf)

```bnf
Expr    ::= Or+
Or      ::= AtomExpr ('|' AtomExpr)* 

AtomExpr::= Atom Trailer 
Atom    ::= Name | Str | '[' Expr ']' | '(' Expr ')' 


Def    ::= '::='
Equals ::= Name Def Expr

Trailer::= ['*' | '+' | '{' Number{1 2} '}']

Name   ::= R'[a-zA-Z_][a-zA-Z0-9]*'
Str ::= R'[\w|\W]*'
Number ::= R'\d+'
```

\-----------------------------  
[Token for EBNF](./LanguageTest/SelfExaminationForEBNF/Token.cs)  
\-----------------------------

[Parser for EBNF](./LanguageTest/SelfExaminationForEBNF/Parser.cs)  

Here is the definition of `Atom`.

```c#
var atom = new Ast(
    ref compileClosure,
    "Atom", new BaseAst[]
    {
        DefualtToken.Name
    }, new BaseAst[]
    {
        DefualtToken.Str
    }, new BaseAst[]
    {
        DefualtToken.Lb,
        new LazyDef("Expr"),
        DefualtToken.Rb
    }, new BaseAst[]
    {
        DefualtToken.Lp,
        new LazyDef("Expr"),
        DefualtToken.Rp
    });
```

\-----------------------------  
### Core Codes

[Node.cs](./ObjectRegex/Node.cs)  
The results parsed are organized by `CSharp.ObjectRegex.Mode`.   
If you want to dump the results, just use `CSharp.ObjectRegex.Mode.Dump()`.  
Here is an example at [testres_csharp](../testres_csharp)
