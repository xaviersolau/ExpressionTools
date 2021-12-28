# ExpressionTools

ExpressionTools is a library that helps you to handle C# lambda expression.
It is written in C# and thanks to .Net Standard, it is cross platform.

The project is based on System.Linq.Expressions and uses the C# Roslyn compiler.

Don't hesitate to post issues, pull requests on the project or to fork and improve the project.

## Project dashboard

[![Build - CI](https://github.com/xaviersolau/ExpressionTools/actions/workflows/build-ci.yml/badge.svg)](https://github.com/xaviersolau/ExpressionTools/actions/workflows/build-ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/xaviersolau/ExpressionTools/badge.svg?branch=master)](https://coveralls.io/github/xaviersolau/ExpressionTools?branch=master)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

| Package                                    | Nuget.org | Pre-release |
|--------------------------------------------|-----------|-------------|
|**SoloX.ExpressionTools.Parser**            |[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.ExpressionTools.Parser.svg)](https://www.nuget.org/packages/SoloX.ExpressionTools.Parser)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.ExpressionTools.Parser.svg)](https://www.nuget.org/packages/SoloX.ExpressionTools.Parser)|
|**SoloX.ExpressionTools.Transform**         |[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.ExpressionTools.Transform.svg)](https://www.nuget.org/packages/SoloX.ExpressionTools.Transform)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.ExpressionTools.Transform.svg)](https://www.nuget.org/packages/SoloX.ExpressionTools.Transform)|


## License and credits

ExpressionTools project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Installation

You can checkout this Github repository or you can use the NuGet package:

**Install using the command line from the Package Manager:**
```bash
Install-Package SoloX.ExpressionTools.Parser -version 1.0.1-alpha.2
Install-Package SoloX.ExpressionTools.Transform -version 1.0.1-alpha.2
```

**Install using the .Net CLI:**
```bash
dotnet add package SoloX.ExpressionTools.Parser --version 1.0.1-alpha.2
dotnet add package SoloX.ExpressionTools.Transform --version 1.0.1-alpha.2
```

**Install editing your project file (csproj):**
```xml
<PackageReference Include="SoloX.ExpressionTools.Parser" Version="1.0.1-alpha.2" />
<PackageReference Include="SoloX.ExpressionTools.Transform" Version="1.0.1-alpha.2" />
```

## How to use it

Note that you can find code examples in this repository in this location: `src/examples`.

### Parse a C# Lambda expression

#### Simple

A few lines of code are actually needed to parse a textual C# lambda expression.
Let's say that we want to parse a simple lambda expression like `"(int x) => x + 1"` with x as an integer:

```csharp
// Set the expression to parse
var expToParse = "(int x) => x + 1";

// We need to create the parser.
var expressionParser = new ExpressionParser();

// We can just parse the expression.
var lambdaExpression = expressionParser.Parse(expToParse);

// Or we can parse the expression specifying the type of the lambda expression.
var expression = expressionParser.Parse<Func<int, int>>(expToParse);
```

The Parse method will return an expression tree (from `System.Linq.Expressions`) reflecting the given textual
C# lambda expression.

More precisely, The resulting expression object is based on `System.Linq.Expressions.LambdaExpression`.
It means that you can then use the `Compile` method to compute a `Delegate` in order to call the lambda at full speed.

#### Using a IParameterTypeResolver

Unfortunately, things can get a little bit more complicated if you want to parse the same expression without
specifying the parameter type giving us a expression like `"x => x + 1"`.
In this case you will need to provide to the ExpressionParser a `IParameterTypeResolver` that will resolve
the type of the parameter `x` as an integer:

```csharp
// Set the expression to parse
var expToParse = "x => x + 1";

// We need to create the parser with a DictionaryParameterTypeResolver that will resolve the
// parameter name using the given Dictionary.
var expressionParser = new ExpressionParser(
    parameterTypeResolver: new DictionaryParameterTypeResolver(new Dictionary<string, Type>()
    {
        { "x", typeof(int) },
    }));

// We can just parse the expression.
var expression = expressionParser.Parse<Func<int, int>>(expToParse);
```

#### Using a ITypeNameResolver

Let's now use a static method `Max` that is defined in `System.Math` with a full qualified name like
`"(double x, double y) => Math.Max(x, y)"`.
To support this use case, you will need to provide a `ITypeNameResolver` that will resolve the `Math` as `System.Math` class:

```csharp
// Set the expression to parse
var expToParse = "(double x, double y) => Math.Max(x, y)";

// We need to create the parser with a TypeNameResolver that will resolve type name with
// the given System.Math class.
var expressionParser = new ExpressionParser(
    typeNameResolver: new TypeNameResolver(typeof(Math)));

// We can just parse the expression.
var expression = expressionParser.Parse<Func<double, double, double>>(expToParse);
```

#### Using a IMethodResolver

Now we want our expression to use a static method `Max` that is defined in `System.Math` omitting the class name prefix like
`"(double x, double y) => Max(x, y)"`.
To support this use case, you will need to provide a `IMethodResolver` that will resolve the `Max` as `System.Math.Max`:

```csharp
// Set the expression to parse
var expToParse = "(double x, double y) => Max(x, y)";

// We need to create the parser with a StaticMethodResolver that will resolve methods with
// the System.Math class.
var expressionParser = new ExpressionParser(
    methodResolver: new StaticMethodResolver(typeof(Math)));

// We can just parse the expression.
var expression = expressionParser.Parse<Func<double, double, double>>(expToParse);
```

### Expression transformation

#### Inline C# Lambda expression

##### One parameter expression

In the case where we would like the replace the parameter `a` from one expression like `"a => a * 2"` by another
expression like `"b => b + 1"` resulting in `"b => (b + 1) * 2"`, we can use the `SingleParameterInliner`.

```csharp
// Setup the expressions to use as input
Expression<Func<int, int>> expressionToAmend = a => a * 2;
Expression<Func<int, int>> expressionToInline = b => b + 1;

// create the expression parameter in-liner.
var inliner = new SingleParameterInliner();

// Amend the given expression replacing parameter 'a' with the expression to in-line resulting in the
// lambda 'b => (b + 1) * 2'.
var inlinedExpression = inliner.Amend<Func<int, int>, Func<int, int>>(expressionToInline, expressionToAmend);
```

##### Multi parameter expression

In the case where we would like to amend an expression like `"(a, b) => a * b"` replacing the two parameter 'a' and 'b'
with two another expressions:

* using `"c => c + 1"` to replace 'a'
* and using `"d => d - 1"` to replace 'b'

all this resulting in `"(c, d) => (c + 1) * (d - 1)"`, we can use the `MultiParameterInliner`.

All it needs is a `ParameterResolver` that will be used to get the expression to in-line instead of the parameter itself:

```csharp
// Setup the expressions to use as input
Expression<Func<int, int>> expressionToAmend = (a, b) => a * b;
Expression<Func<int, int>> expressionToInlineForA = c => c + 1;
Expression<Func<int, int>> expressionToInlineForB = d => d - 1;

// create the expression in-liner.
var inliner = new ExpressionInliner();

// Setup the resolver telling that 'a' must be replaced by in-lined 'c => c + 1' lambda
// and that 'b' must be replaced by in-lined 'd => d - 1' lambda.
var resolver = new ParameterResolver()
    .Register("a", expressionToInlineForA)
    .Register("b", expressionToInlineForB);

// Amend the given expression replacing parameters resulting in the lambda '(c, d) => (c + 1) * (d - 1)'.
var inlinedExpression = inliner.Amend<Func<int, int>, Func<int, int>>(resolver, expressionToAmend);
```

#### Inline Constant expression

Let's say that we need to convert a Lambda Expression using an external variable like this:

```csharp
// Here is a variable we are going to use in a Lambda Expression.
var externalValue = 0.01d;

// The expression is multiplying the input with the variable 'externalValue'.
Expression<Func<double, double>> expToInline = i => i * externalValue;
```

The use case here is that we want to convert the expression in-lining the actual value of the variable like
 `i => i * 0.01d`.

It has never been simpler with the `ConstantInliner`: all we need is to call its `Amend` method:

```csharp
// Create the constant in-liner.
var inliner = new ConstantInliner();

// Amend the expToInline.
var exp = inliner.Amend(expToInline);

// That's yet, 'exp' is equal to 'i => i * 0.01d'
```

#### Resolve Property Name

In order to get the name of a property from a lambda like `i => i.MyProperty` we can use the `PropertyNameResolver`:

```csharp

// Create a instance of the resolver.
var resolver = new PropertyNameResolver();

// Resolve the property name of the given lambda.
var name = resolver.GetPropertyName<IMyType, string>(x => x.MyProperty);

// Here name is set to "MyProperty".

```
