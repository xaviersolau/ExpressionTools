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

The Parse method will return an expression tree (from System.Linq.Expressions) reflecting the given textual
C# lambda expression.

More precisely, The resulting expression object in based on System.Linq.Expressions.LambdaExpression.
It means that you can then use the Compile method in order to call lambda at full speed.

Unfortunately, things can get a little bit more complicated if you want to parse the same expression without
specifying the parameter type giving us a expression like `"x => x + 1"`.
In this case you will need to provide to the ExpressionParser a `IParameterTypeResolver` that will resolve
the type of the parameter `x` as an integer.

### Inline C# Lambda expression parameter

Coming soon...
