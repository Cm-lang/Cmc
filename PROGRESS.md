
Please add feature request here.

Codes related to some implemented feature can be found by searching their number.

I.E. Want to find codes related to `lexical scoping` (the 4th one in the `scoping` section)?
[search "FEATURE \#18"](https://github.com/Cm-lang/Cmc/search?utf8=%E2%9C%93&q=%22FEATURE+%2318%22),
and you see the result.

## basic language features

+ [X] type aliases (#31)
+ [X] no functions (see [README](./README.md))
+ [ ] foreign function interfaces

## scoping

+ [X] find variables in outer scopes (#3)
+ [X] when new variables defined, a new scope starts (#4)
+ [X] find all variables matches the name (#5)
+ [X] lexical scoping (#18)

## types

+ [X] primitive types with name (#0) (tested)
+ [X] function type, consisting of the parameters' types and the return type (#6)
+ [X] types with multiple type parameter (#7)
+ [X] infer function's return type from the return statements (#12) (tested)
+ [X] infer variable's type by the assignment expression (#8) (tested)
+ [X] resolve types defined in the context (unknown types) (#30) (tested)
+ [X] `nulltype` can be assigned to variables with any type (#11) (tested)
+ [ ] an HM type system (extra)
+ [ ] pointers
+ [ ] raw arrays

## contracting (extra)

+ [ ] traits or interfaces
+ [ ] generic with boundaries

## variables

+ [X] initialize variable with null or given expression (#13)
+ [X] check for mutability (#21)
+ [X] check for assignment type (#14) (tested)
+ [X] validate lhs (#20)
+ [X] check if member exist (#29)
+ [ ] inline
+ [X] overloading (#33)
+ [ ] remove unused local variables but keep the expression (#36)

## inline

+ [ ] general inline
+ [ ] inline lambda parameters
+ [X] inline when directly invoke a lambda expression (#44)
+ [ ] keep returning currect when inline
+ [ ] keep label return when inline

## functions and lambdas
+ [X] check for parameter type (#32)
+ [X] the return type should be inferred, or `nulltype` (#19) (tested)
+ [X] return statements' types should be same (#24) (tested)
+ [ ] return statement exhaustiveness check (extra)
+ [X] defaultly no parameter (#22)
+ [X] y-combinator-like recur (#37)
+ [X] recur can invoke outside lambdas (#38) (tested)

## main function

+ [X] only `nulltype` and `i32` returning are allowed (#35)
+ [X] check for main function duplication (#40)

## structs

+ [X] mutual recursion detection (#34) (tested)
+ [ ] generic structs

## if

+ [X] check for condition type (#1)
+ [X] optional else branch (#2)
+ [X] when condition is constant value, delete redundant branch (#17)
+ [ ] if as expression (extra)
+ [ ] tenary operator

## low-level stuffs

+ [X] expression splitting (#42)
+ [X] temporary variables are not added to the env (#43)
+ [X] string pool (#38)

## int

+ [X] only 8, 16, 32, 64 are valid length (#26)
+ [X] signed and unsigned (#27)
+ [ ] check value is valid

## float

+ [X] only 32, 64 are valid length (#41)
+ [ ] check value is valid

## while

+ [X] check for condition type (#16)
+ [ ] do while
+ [X] jump statements (#25)

## errors

+ [X] when the declared/assignment type are mismatch (#9)
+ [X] display line number/file name (#10)

## debug

+ [X] display compilation information (#15)
+ [X] pretty print strings (#23)

<br/><br/><br/><br/><br/>

### Features given up

Those features will never be available.

+ types with single type parameter

