﻿using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jace.Tests
{
    [TestClass]
    public class OptimizerTests
    {
        [TestMethod]
        public void TestOptimizerIdempotentFunction()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);            
            IList<Token> tokens = tokenReader.Read("test(var1, (2+3) * 500)");

            var functionRegistry = new FunctionRegistry<double>(true);
            functionRegistry.RegisterFunction("test", (Func<double, double, double>)((a, b) =>  a + b));

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Function optimizedFuction = (Function)optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedFuction.Arguments[1].GetType());
        }

        [TestMethod]
        public void TestOptimizerNonIdempotentFunction()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("test(500)");

            var functionRegistry = new FunctionRegistry<double>(true);
            functionRegistry.RegisterFunction("test", (Func<double, double>)(a => a), false, true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedFuction = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Function), optimizedFuction.GetType());
            Assert.AreEqual(typeof(IntegerConstant), ((Function)optimizedFuction).Arguments[0].GetType());
        }

        [TestMethod]
        public void TestOptimizerMultiplicationByZero()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var1 * 0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(0.0, ((FloatingPointConstant<double>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestOptimizerMultiplicationByZeroDecimal()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var1 * 0.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(0.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);
        }


        [TestMethod]
        public void TestBooleanOperationOptimizerDecimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("4 > 2");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestBooleanOperationOptimizerDecimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("4.0 > 2.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestBooleanOperationOptimizerDouble1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("4 > 2");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestBooleanOperationOptimizerDouble2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("4.0 > 2.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestBooleanOperation2OptimizerDecimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("(4 > 2) && var1");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestBooleanOperation2OptimizerDecimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("(4.0 > 2.0) && var1");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestBooleanOperation2OptimizerDouble1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("(4 > 2) && var1");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestBooleanOperation2OptimizerDouble2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("(4.0 > 2.0) && var1");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizerDecimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("0 && var_x");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(0.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestShortCircuitAndOptimizerDecimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("0.0 && var_x");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(0.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);
        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer2Decimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(0.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }


        [TestMethod]
        public void TestShortCircuitAndOptimizer2Decimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 0.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(0.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer2Double1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(0.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer2Double2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 0.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(0.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer3Decimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 1");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer3Decimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 1.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer3Double1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 1");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitAndOptimizer3Double2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x && 1.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(And), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizerDecimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("1 || var_x");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizerDecimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("1.0 || var_x");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizerDouble1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("1 || var_x");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizerDouble2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("1.0 || var_x");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer2Decimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 1");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer2Decimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 1.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<decimal>), optimizedOperation.GetType());
            Assert.AreEqual(1.0m, ((FloatingPointConstant<decimal>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer2Double1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 1");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer2Double2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 1.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(1.0, ((FloatingPointConstant<double>)optimizedOperation).Value);

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer3Decimal1()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Or), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer3Decimal2()
        {
            var optimizer = new Optimizer<decimal>(new Interpreter<decimal>(DecimalNumericalOperations.Instance), DecimalNumericalOperations.Instance);

            var tokenReader = new TokenReader<decimal>(CultureInfo.InvariantCulture, DecimalNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 0.0");

            var functionRegistry = new FunctionRegistry<decimal>(true);

            var astBuilder = new AstBuilder<decimal>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Or), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer3Double1()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Or), optimizedOperation.GetType());

        }

        [TestMethod]
        public void TestShortCircuitOrOptimizer3Double2()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance), DoubleNumericalOperations.Instance);

            var tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var_x || 0.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Or), optimizedOperation.GetType());

        }
    }
}
