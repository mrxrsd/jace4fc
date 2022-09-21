# Jace4fc
Jace4fc is hard-fork of Jace.NET. There is one major difference between them, here we have decimal support.

Jace is a high performance calculation engine for the .NET platform. It stands for "Just Another Calculation Engine For Financial Calculations"


## What does it do?
Jace4fc can interprete and execute strings containing mathematical formulas. These formulas can rely on variables. If variables are used, values can be provided for these variables at execution time of the mathematical formula.

Jace can execute formulas in two modes: in interpreted mode and in a dynamic compilation mode. If dynamic compilation mode is used, Jace will create a dynamic method at runtime and will generate the necessary MSIL opcodes for native execution of the formula. If a formula is re-executed with other variables, Jace will take the dynamically generated method from its cache. It is recommended to use Jace in dynamic compilation mode.

## Examples
Jace4fc can be used in a couple of ways:

To directly execute a given mathematical formula using the provided variables:
```csharp
Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
variables.Add("var1", 2.5m);
variables.Add("var2", 3.4m);

var engine = CalculationEngine.New<decimal>();
double result = engine.Calculate("var1*var2", variables);
```

To build a .NET Func accepting a dictionary as input containing the values for each variable:
```csharp
var engine = CalculationEngine.New<double>()
Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariable)");

Dictionary<string, double> variables = new Dictionary<string, double>();
variables.Add("var1", 2);
variables.Add("otherVariable", 4.2);
	
double result = formula(variables);
```

To build a typed .NET Func:
```csharp
var engine = CalculationEngine.New<double>()
Func<int, double, double> formula = (Func<int, double, double>)engine.Formula("var1+2/(3*otherVariable)")
	.Parameter("var1", DataType.Integer)
    .Parameter("otherVariable", DataType.FloatingPoint)
    .Result(DataType.FloatingPoint)
    .Build();
	
double result = formula(2, 4.2);
```

Functions can be used inside the mathemical formulas. Jace.NET currently offers four functions accepting one argument (sin, cos, loge and log10) and one function accepting two arguments (logn).

```csharp
Dictionary<string, double> variables = new Dictionary<string, double>();
variables.Add("var1", 2.5);
variables.Add("var2", 3.4);

var engine = CalculationEngine.New<double>()
double result = engine.Calculate("logn(var1,var2)+4", variables);
```

## Features

### Basic Operations 

The following mathematical operations are supported:
* Addition: +
* Subtraction: -
* Multiplication: *
* Division: /
* Modulo: %
* Exponentiation: ^

### Boolean Operations

The following boolean operations are supported:

* Less than: <
* Less than or equal: <=
* More than: >
* More than or equal: >=
* Equal: ==
* Not Equal: !=

The boolean operations map true to 1.0 and false to 0.0. All functions accepting a condition will consider 0.0 as false and any other value as true.

```csharp
result = engine.Calculate("5 > 1")
// 1.0
```
### Scientific Notation

```csharp
result = engine.Calculate("1E-3*5+2")
// 2.005
```

### Variables

```csharp

var vars = new Dictionary<string, double>();
variables.Add("$a", 1.0);
variables.Add("B", 2.0);
variables.Add("c_c", 3.0);
variables.Add("d1", 3.4);
variables.Add("VaR_vAr", 10.0);


result = engine.Calculate("$a + B + c_c + d1 + 10 + VaR_vAr", vars)
// 30.0
```
- Can contains letters ( a-z | A-Z ), underscore ( _ ), dolar sign ( $ ) or a number ( 0-9 ).
- Cannot start with a number.
- Cannot start with underscore.

### Standard Constants

| Constant        |  Description | More Information |
| ------------- | -------|----|
| e |   Euler's number  | https://oeis.org/A001113 |
| pi |   Pi| https://oeis.org/A000796 |

```csharp
result = engine.Calculate("2*pi")
// 6.283185307179586
```

### Standard Functions

The following mathematical functions are out of the box supported:

| Function | Arguments       | Description         | More Information                                                                               |
| -------- | --------------- | ------------------- | ---------------------------------------------------------------------------------------------- |
| sin      | sin(x)          | Sine                | https://docs.microsoft.com/pt-br/dotnet/api/system.math.sin                                    |
| cos      | cos(x)          | Cosine              | https://docs.microsoft.com/pt-br/dotnet/api/system.math.cos                                    |
| asin     | asin(x)         | Arcsine             | https://docs.microsoft.com/pt-br/dotnet/api/system.math.asin                                   |
| acos     | acos(x)         | Arccosine           | https://docs.microsoft.com/pt-br/dotnet/api/system.math.acos                                   |
| tan      | tan(x)          | Tangent             | https://docs.microsoft.com/pt-br/dotnet/api/system.math.tan                                    |
| atan     | atan(x)         | Arctangent          | https://docs.microsoft.com/pt-br/dotnet/api/system.math.atan                                   |
| log      | log(x)          | Logarithm           | https://docs.microsoft.com/pt-br/dotnet/api/system.math.log                                    |
| sqrt     | sqrt(x)         | Square Root         | https://docs.microsoft.com/pt-br/dotnet/api/system.math.sqrt                                   |
| trunc    | trunc(x)        | Truncate            | https://docs.microsoft.com/pt-br/dotnet/api/system.math.trunc                                  |
| floor    | floor(x)        | Floor               | https://docs.microsoft.com/pt-br/dotnet/api/system.math.floor                                  |
| ceil     | ceil(x)         | Ceil                | https://docs.microsoft.com/pt-br/dotnet/api/system.math.ceil                                   |
| round    | round(x \[,y\]) | Round               | Rounds a number to a specified number of digits where 'x' is the number and 'y' is the digits. |
| random   | random(x)       | Random              | Generate a random double value between 0.0 and 1.0 where 'x' is the seed.                      |
| if       | if(a,b,c)       | Excel's IF Function | IF 'a' IS true THEN 'b' ELSE 'c'.                                                              |
| max      | max(x1,…,xn)    | Maximum             | Return the maximum number of a series.                                                         |
| min      | min(x1,…,xn)    | Minimum             | Return the minimum number of a series.                                                         |


```csharp

// Sin (ordinary function)
var vars = new Dictionary<string, double>();
variables.Add("a", 2.0);

ret = engine.Calculate("sin(100)+a", vars)
// 1.4936343588902412


// If
var vars = new Dictionary<string, double>();
variables.Add("a", 4.0);

ifresult = engine.Calculate("if(2+2==a, 10, 5)", varsIf)
// 10.0

// MAX
max = engine.Calculate("max(5,6,3,-4,5,3,7,8,13,100)")
// 100.0

```

### Custom Functions 

Custom functions allow programmers to add additional functions besides the ones already supported (sin, cos, asin, …). Functions are required to have a unique name. The existing functions cannot be overwritten.

```csharp
engine.AddFunction("addTwo", (Func<double, double>)((a) => a+2)):

result  := engine.Calculate("addTwo(2.0)", nil)
// 4.0

```

### Compile Time Constants

Variables as defined in a formula can be replaced by a constant value at compile time. This feature is useful in case that a number of the parameters don't frequently change and that the formula needs to be executed many times. Thusfore it is better because constants could be optimizated on 'Optimization phase'.


```csharp

var consts  = new Dictionary<string, double>{{"b", 1.0}};
var formula = engine.Build("a + b", consts); // It's the same as 'engine.Build("a+1")' but without dealing with string replace

double result = formula(new Dictionary<string, double>{{"a", 3.0 }});
// result will be 4.0
```

## Benchmark 

TBD

## More Information
For more information, you can read the following articles:
* http://pieterderycke.wordpress.com/2012/11/04/jace-net-just-another-calculation-engine-for-net/
* http://www.codeproject.com/Articles/682589/Jace-NET-Just-another-calculation-engine-for-NET


# Disclaimer:

This is my fork of Jace. It is basically the same as the original project but here we have decimal support and I've applied some bug fixes there are not in jace's main branch.

Original Project: [Jace](https://github.com/pieterderycke/Jace)
