using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;

namespace Jace
{
    public interface IOptimizer<T>
    {
        Operation Optimize(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry);
    }

    public class Optimizer<T> : IOptimizer<T>
    {
        private readonly IExecutor<T> executor;
        private readonly INumericalOperations<T> numericalOperations;

        public Optimizer(IExecutor<T> executor, INumericalOperations<T> numericalOperations)
        {
            this.executor = executor;
            this.numericalOperations = numericalOperations;
        }

        public Operation Optimize(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry)
        {
            if (!operation.DependsOnVariables && 
                operation.IsIdempotent && 
                operation.GetType() != typeof(IntegerConstant) && 
                operation.GetType() != typeof(FloatingPointConstant<T>))
            {
                T result = executor.Execute(operation, functionRegistry, constantRegistry);
                return new FloatingPointConstant<T>(result);
            }
            else
            {
                if (operation.GetType() == typeof(Addition))
                {
                    Addition addition = (Addition)operation;
                    addition.Argument1 = Optimize(addition.Argument1, functionRegistry, constantRegistry);
                    addition.Argument2 = Optimize(addition.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Subtraction))
                {
                    Subtraction substraction = (Subtraction)operation;
                    substraction.Argument1 = Optimize(substraction.Argument1, functionRegistry, constantRegistry);
                    substraction.Argument2 = Optimize(substraction.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Multiplication))
                {
                    Multiplication multiplication = (Multiplication)operation;
                    multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry, constantRegistry);
                    multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry, constantRegistry);

                    if (
                        (multiplication.Argument1.GetType() == typeof(FloatingPointConstant<T>) && 
                         ((FloatingPointConstant<T>)multiplication.Argument1).Value.Equals(numericalOperations.Constants.Zero)) ||
                        (multiplication.Argument1.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)multiplication.Argument1).Value.Equals(0)) ||
                        (multiplication.Argument2.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)multiplication.Argument2).Value.Equals(0)) ||
                        (multiplication.Argument2.GetType() == typeof(FloatingPointConstant<T>) 
                         && ((FloatingPointConstant<T>)multiplication.Argument2).Value.Equals(numericalOperations.Constants.Zero))
                        )
                    {
                        return new FloatingPointConstant<T>(numericalOperations.Constants.Zero);
                    }
                }
                else if (operation.GetType() == typeof(Division))
                {
                    Division division = (Division)operation;
                    division.Dividend = Optimize(division.Dividend, functionRegistry, constantRegistry);
                    division.Divisor = Optimize(division.Divisor, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Exponentiation))
                {
                    Exponentiation division = (Exponentiation)operation;
                    division.Base = Optimize(division.Base, functionRegistry, constantRegistry);
                    division.Exponent = Optimize(division.Exponent, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(GreaterThan))
                {
                    var op = (GreaterThan)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);
                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(GreaterOrEqualThan))
                {
                    var op = (GreaterOrEqualThan)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);
                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(LessThan))
                {
                    var op = (LessThan)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);
                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(LessOrEqualThan))
                {
                    var op = (LessOrEqualThan)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);
                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(And))
                {
                    var op = (And)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);

                    if ((op.Argument1.GetType() == typeof(FloatingPointConstant<T>) && 
                        ((FloatingPointConstant<T>)op.Argument1).Value.Equals(numericalOperations.Constants.Zero)) ||
                        (op.Argument1.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)op.Argument1).Value.Equals(0))

                        )
                    {
                        return new FloatingPointConstant<T>(numericalOperations.Constants.Zero);
                    }


                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);

                    if ((op.Argument2.GetType() == typeof(FloatingPointConstant<T>) &&
                         ((FloatingPointConstant<T>)op.Argument2).Value.Equals(numericalOperations.Constants.Zero)) ||
                        (op.Argument2.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)op.Argument2).Value.Equals(0))

                    )
                    {
                        return new FloatingPointConstant<T>(numericalOperations.Constants.Zero);
                    }

                }
                else if (operation.GetType() == typeof(Or))
                {
                    var op = (Or)operation;
                    op.Argument1 = Optimize(op.Argument1, functionRegistry, constantRegistry);

                    if ((op.Argument1.GetType() == typeof(FloatingPointConstant<T>) &&
                        ((FloatingPointConstant<T>)op.Argument1).Value.Equals(numericalOperations.Constants.One)) ||
                        (op.Argument1.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)op.Argument1).Value.Equals(1))
                    )
                    {
                        return new FloatingPointConstant<T>(numericalOperations.Constants.One);
                    }


                    op.Argument2 = Optimize(op.Argument2, functionRegistry, constantRegistry);

                    if ((op.Argument2.GetType() == typeof(FloatingPointConstant<T>) &&
                         ((FloatingPointConstant<T>)op.Argument2).Value.Equals(numericalOperations.Constants.One)) ||
                        (op.Argument2.GetType() == typeof(IntegerConstant) &&
                         ((IntegerConstant)op.Argument2).Value.Equals(1))
                    )
                    {
                        return new FloatingPointConstant<T>(numericalOperations.Constants.One);
                    }

                }
                else if(operation.GetType() == typeof(Function))
                {
                    Function function = (Function)operation;
                    IList<Operation> arguments = function.Arguments.Select(a => Optimize(a, functionRegistry, constantRegistry)).ToList();
                    function.Arguments = arguments;
                }

                return operation;
            }
        }
    }
}
