
Please add feature request here.

Codes related to some implemented feature can be found by searching their number.

I.E. Want to find codes related to `lexical scoping` (the 4th one in the `scoping` section)?
[search "FEATURE \#18"](https://github.com/bC-Programming-Language-StandardCouncil/bC-Compiler/search?utf8=%E2%9C%93&q=%22FEATURE+%2318%22&type=),
and you see the result.

## scoping

+ [X] find variables in outer scopes (#3)
+ [X] when new variables defined, a new scope starts (#4)
+ [X] find all variables matches the name (#5)
+ [X] lexical scoping (#18)

## types

+ [X] primitive types with name (#0)
+ [X] function type, consisting of the parameters' types and the return type (#6)
+ [X] types with multiple type parameter (#7)
+ [X] infer function's return type from the return statements (#12) (tested)
+ [X] infer variable's type by the assignment expression (#8) (tested)
+ [X] `nulltype` can be assigned to variables with any type (#11) (tested)
+ [ ] an HM type system (extra)

## variables

+ [X] initialize variable with null or given expression (#13)
+ [X] check for mutability (#21)
+ [X] check for assignment type (#14) (tested)
+ [X] validate lhs (#20)
+ [ ] check if member exist
+ [ ] inline

## functions and lambdas

+ [ ] inline
+ [ ] inline lambda parameters
+ [ ] check for parameter type
+ [ ] overloading
+ [X] the return type should be inferred, or `nulltype` (#19)
+ [X] return statements' types should be same (#24) (tested)
+ [ ] return statement exhaustiveness check (extra)
+ [X] defaultly no parameter (#22)

## if

+ [X] check for condition type (#1)
+ [X] optional else branch (#2)
+ [X] when condition is constant value, delete redundant branch (#17)
+ [ ] if as expression (extra)

## int

+ [X] only 8, 16, 32, 64 are valid length (#26)

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
