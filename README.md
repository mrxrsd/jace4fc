# Jace4fc
Jace4fc is hard-fork of Jace.NET. Jace is a high performance calculation engine for the .NET platform. 

It stands for "Just Another Calculation Engine For Financial Calculations"


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

## More Information
For more information, you can read the following articles:
* http://pieterderycke.wordpress.com/2012/11/04/jace-net-just-another-calculation-engine-for-net/
* http://www.codeproject.com/Articles/682589/Jace-NET-Just-another-calculation-engine-for-NET


# Disclaimer:

This is my fork of Jace. It is basically the same as the original project but here we have decimal support and I've applied some bug fixes there are not in jace's main branch.

Original Project: [Jace](https://github.com/pieterderycke/Jace)
