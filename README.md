# ExpressionTools

ExpressionTools is a library that helps you to handle C# lambda expression. It is written in C# and thanks to .Net Standard, it is cross platform.

The project is based on System.Linq.Expressions and uses the C# Roslyn compiler.

Don't hesitate to post issue, pull request on the project or to fork and improve the project.

Build: [![CircleCI](https://circleci.com/gh/xaviersolau/ExpressionTools.svg?style=svg)](https://circleci.com/gh/xaviersolau/ExpressionTools)

## License and credits

ExpressionTools project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Installation

You can checkout this Github repository or use the NuGet package that will be available soon.

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

In the case where we would like the replace the parameter `b` from one expression like `"b => b * 2"` by another
expression like `"a => a + 1"` resulting in `"a => (a + 1) * 2"`, we can use the `ExpressionInliner`.
All its need is a `ParameterResolver` that will be used to get the expression to in-line instead of the parameter itself:

```csharp
// Setup the expressions to use as input
Expression<Func<int, int>> expressionToInline = a => a + 1;
Expression<Func<int, int>> expression = b => b * 2;

// Setup the resolver telling that 'b' must be replaced by in-lined 'a => a + 1' lambda.
var resolver = new ParameterResolver()
    .Register("b", expressionToInline);

// create the expression in-liner.
var inliner = new ExpressionInliner(resolver);

// Amend the given expression replacing parameter 'b' resulting in the lambda 'a => (a + 1) * 2'.
var inlinedExpression = inliner.Amend<Func<int, int>, Func<int, int>>(expression);
```
