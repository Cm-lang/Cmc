# Cmc

![](https://avatars1.githubusercontent.com/u/31237156)

<!-- CI|Status
:---:|:---:
Travis CI|
-->

[![Build Status](https://travis-ci.org/Cm-lang/Cmc.svg?branch=master)](https://travis-ci.org/Cm-lang/Cmc)
[![](https://img.shields.io/badge/Cm--lang-Compiler-ff69b4.svg)](https://github.com/Cm-lang/Cmc)
[![](https://img.shields.io/badge/request-new%20features-blue.svg)](https://github.com/Cm-lang/Cm-Document/issues/new)
[![](https://img.shields.io/badge/backend-LLVM-ab51ba.svg)](http://llvm.org/)<br/>
[![NuGet](https://img.shields.io/nuget/dt/Cmc.svg)](https://www.nuget.org/packages/Cmc/)
[![NuGet](https://img.shields.io/nuget/v/Cmc.svg)](https://www.nuget.org/packages/Cmc/)<br/>
[![license](https://img.shields.io/github/license/Cm-lang/Cmc.svg)](https://github.com/Cm-lang/Cmc)

The compiler for the Cm programming language.

Cm is a statically typed native language (targeting LLVM and GLSL),
with first-class lambdas and powerful type inference.

There are no `function`s in this language, it treats them as lambda variables.
With compile-time inlining, functional style codes can run as fast as imperative style ones.

This language doesn't have subtyping,
it organize data with structs and extension methods.

Maybe there will be an optional H-M type system, and hole-oriented programming (like Idris).

## Document

+ [AST Design](./Cmc/Cm_AST_Design.yml)
+ [Progress](./PROGRESS.md)

## Used Project

+ [LLVMSharp](https://github.com/Microsoft/LLVMSharp)
