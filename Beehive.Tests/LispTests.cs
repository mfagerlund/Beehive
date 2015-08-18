using System;
using System.Diagnostics;
using Beehive.Lisp;
using FluentAssertions;
using NUnit.Framework;

namespace Beehive.Tests
{
    [TestFixture]
    public class LispTests
    {      
        [Test]
        public void Test()
        {
            string code = "(+ 7 10 (* 2 8))";
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            Debug.WriteLine(func.BuildParseTree().ToCode());
            Debug.WriteLine(func.TryEvaluateToFloat());
        }

        [Test]
        public void Test2()
        {
            string code = "(* 2 Pi (+ A B))";
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            Debug.WriteLine(func.BuildParseTree().ToCode());
            Debug.WriteLine(func.TryEvaluateToFloat());
        }

        [Test]
        public void Test3()
        {
            string code = "5";
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            Debug.WriteLine(func.BuildParseTree().ToCode());
            func.Should().BeOfType<IntConstant<Blackboard>>();
        }

        [Test]
        public void BoolTest()
        {
            string code = "(And True False)";
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            Debug.WriteLine(func.BuildParseTree().ToCode());
            Debug.WriteLine(func.TryEvaluateToBool());
        }

        [Test]
        public void MultiTest()
        {
            Test("(+ 5 5)", 10);
            Test("(* 2 10.5)", 21);
            Test("(/ 6 3)", 2);

            Test("(And True False)", false);
            Test("(And True True)", true);
            Test("(Or True False)", true);
            Test("Boo", true);
            Test("(Or Boo False)", true);
            Test("(And True Boo)", true);
            Test("(Not (And True Boo))", false);

            Test("(= 5 5)", true);
            Test("(> 10 2)", true);
            Test("(< 10 2)", false);
            Test("(> 10 10)", false);
            Test("(< 10 10)", false);
            Test("(>= 10 10)", true);
            Test("(<= 10 10)", true);
        }

        public void Test(string code, bool extected)
        {
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            bool result = func.TryEvaluateToBool();
            Debug.WriteLine(func.BuildParseTree().ToCode() + " => " + result);
            result.Should().Be(extected);
        }

        public void Test(string code, float extected)
        {
            LispCompiler<Blackboard> lispCompiler = new LispCompiler<Blackboard>();
            Blackboard blackboard = new Blackboard();
            LispOperator<Blackboard> func = lispCompiler.Compile(blackboard, code);
            float result = func.TryEvaluateToFloat();
            Debug.WriteLine(func.BuildParseTree().ToCode() + " => " + result);
            result.Should().Be(extected);
        }

        public class Blackboard : IBlackboard
        {
            public Blackboard()
            {
                A = 5;
                B = 10;
                Pi = 3.14159f;
                Boo = true;
            }

            public float A { get; set; }
            public float B { get; set; }
            public float Pi { get; set; }

            public bool Boo { get; set; }

            public Func<float> GetFloatFunction(string name)
            {
                // Todo: add reflection to dig into parameters? 
                // Todo: add reflection to supply all floats (skip boring ones).
                if (name == "A") { return () => A; }
                if (name == "B") { return () => B; }
                if (name == "Pi") { return () => Pi; }
                return null;
            }

            public Func<bool> GetBoolFunction(string name)
            {
                if (name == "Boo") { return () => Boo; }
                return null;
            }
        }
    }
}