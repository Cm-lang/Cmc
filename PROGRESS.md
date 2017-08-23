
Please add feature request here.

## scoping

+ [X] find variables in outer scopes (#3)
+ [X] when new variables defined, a new scope starts (#4)
+ [X] find all variables matches the name (#5)

## types

+ [X] primitive types with name (#0)
+ [X] function type, consisting of the parameters' types and the return type (#6)
+ [X] types with single type parameter (#7)
+ [X] infer function's return type from the last return statement(#12)
+ [X] infer variable's type by the assignment expression (#8) (tested)
+ [X] `nulltype` can be assigned to variables with any type (#11)
+ [ ] an HM type system (extra)

## variables

+ [X] initialize variable with null or given expression (#13)
+ [ ] check for mutability
+ [X] check for assignment type (#14)

## functions

+ [ ] inline
+ [ ] check for parameter type

## if

+ [X] check for condition type (#1)
+ [X] optional else branch (#2)

## while

+ [ ] check for condition type
+ [ ] while
+ [ ] do while

## errors

+ [X] when the declared/assignment type are mismatch (#9)
+ [X] display line number/file name (#10)

## debug

+ [X] display compilation information (#15)
