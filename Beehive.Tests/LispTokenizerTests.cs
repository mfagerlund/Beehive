using System;
using System.Collections.Generic;
using System.Diagnostics;
using Beehive;
using Beehive.Lisp;
using NUnit.Framework;

namespace Beehive.Tests
{
    [TestFixture]
    public class LispTokenizerTests
    {
        [Test]
        public void CanTokenize()
        {
            LispParser lispParser = new LispParser();
            List<LispParser.Node> tokens = lispParser.Tokenize("(+ :x=12 :y=15)");
            foreach (LispParser.Node token in tokens)
            {
                Debug.WriteLine(token.ToString());
            }
        }

        [Test]
        public void CanParse()
        {
            TestParse("(+ 12 15 (goofy_balls 6 7))");
            TestParse("(+ 12 15)");
            TestParse("(+ 12.3 15)");
            TestParse("(MousePointerXY)");
            TestParse("(MousePointer.X)");
            TestParse("(yowsa 12.3 99 88 123.31)");
            TestParse("yowsa");
            TestParse("13.2");
            TestParse("\"show me your strings\"");
            TestParse("(print_error \"show me your strings\" 100 100)");
        }

        [Test]
        public void CanParseNamed()
        {
            TestParse(":yiki=13.2");
            TestParse(":baba_ka=(yowsa 1 2 3)");
            TestParse("(+ :x=12 :y=15 :k=(goofy_balls :z=6 :p=7))");
            TestParse("(yowsa :a=12.3 99 88 123.31)");
            TestParse(":s=\"show me your strings\"");
            TestParse("(print_error :text=\"show me your strings\" :x=100 :y=100)");
        }

        [Test]
        public void CanDetectErrors()
        {
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("*"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(*)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(*1 a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(82 a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(*a a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(:a=a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(=a a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(a :a)"));
            Assert.Throws<InvalidOperationException>(() => new LispParser().Parse("(a :a=)"));
        }

        private void TestParse(string code)
        {
            LispParser lispParser = new LispParser();
            LispParser.Node node = lispParser.Parse(code);
            Debug.WriteLine(" in:{0}\nout:{1}\n", code, node.ToCode());
        }
    }
}