# bC-Compiler

![](https://avatars1.githubusercontent.com/u/31237156)

<!-- CI|Status
:---:|:---:
Travis CI|
-->

[![Build Status](https://travis-ci.org/bC-Programming-Language-StandardCouncil/bC-Compiler.svg?branch=master)](https://travis-ci.org/bC-Programming-Language-StandardCouncil/bC-Compiler)
[![](https://img.shields.io/badge/request-new%20features-ff68b4.svg)](https://github.com/bC-Programming-Language-StandardCouncil/bC-Compiler/blob/master/PROGRESS.md)
[![](https://img.shields.io/badge/backend-LLVM-ab51ba.svg)](http://llvm.org/)<br/>
[![Gitter](https://badges.gitter.im/bC-Programming-Language-StandardCouncil/bC-Compiler.svg)](https://gitter.im/bC-Programming-Language-StandardCouncil/bC-Compiler?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![license](https://img.shields.io/github/license/bC-Programming-Language-StandardCouncil/bC-Compiler.svg)](https://github.com/bC-Programming-Language-StandardCouncil/bC-Compiler)

The compiler for bC-language (the name will be changed in the future).

bC is a statically typed native language (targeting LLVM and GLSL),
with first-class lambdas and powerful type inference.

There are no `function`s in this language, it treats them as lambda variables.
With compile-time inlining, functional style codes can run as fast as imperative style ones.

This language doesn't have subtyping,
it organize data with structs and extension methods.

Maybe there will be an optional H-M type system, and hole-oriented programming (like Idris).

## Document

[AST Design](../bCC/bC_AST_Design.yml)
