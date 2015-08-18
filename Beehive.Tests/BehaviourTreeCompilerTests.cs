using System.Diagnostics;
using Beehive.BehaviorTrees;
using Beehive.Lisp;
using NUnit.Framework;

namespace Beehive.Tests
{
    [TestFixture]
    public class BehaviourTreeCompilerTests
    {
        [Test]
        public void CanSerializeToLisp()
        {
            BehaviourTree<BehaviourTreeTests.Blackboard> behaviourTree = CreateBehaviourTree();
            //BehaviourTreeLispSerializer<BehaviourTreeTests.Blackboard> serializer = new BehaviourTreeLispSerializer<BehaviourTreeTests.Blackboard>();
            Debug.WriteLine(behaviourTree.ToString());
        }

        [Test]
        public void CanSerializeToLisp2()
        {
            BehaviourTree<BehaviourTreeTests.Blackboard> behaviourTree = CreateBehaviourTree();

            //BehaviourTreeLispSerializer<BehaviourTreeTests.Blackboard> serializer = new BehaviourTreeLispSerializer<BehaviourTreeTests.Blackboard>();
            Debug.WriteLine(behaviourTree.BuildParseTree().ToCode());
        }

        [Test]
        public void CanDeserializeFromLisp()
        {
            BehaviourTree<BehaviourTreeTests.Blackboard> behaviourTree = CreateBehaviourTree();
            BehaviourTreeCompiler<BehaviourTreeTests.Blackboard> compiler = new BehaviourTreeCompiler<BehaviourTreeTests.Blackboard>();
            //string code = serializer.Convert(behaviourTree).ToCode();
            string code = behaviourTree.ToString();
            Debug.WriteLine(" in:" + code);
            LispParser lispParser = new LispParser();
            LispParser.Node parseTree = lispParser.Parse(code);
            //Debug.WriteLine("out:" + parseTree.ToCode());
            BehaviourTreeTests.Blackboard blackboard = new BehaviourTreeTests.Blackboard();
            BehaviourTree<BehaviourTreeTests.Blackboard> compiled = compiler.Compile(blackboard, parseTree);
            Debug.WriteLine("dep:" + compiled);
            compiled.Tick();
            compiled.Tick();
            compiled.Tick();
        }

        private static BehaviourTree<BehaviourTreeTests.Blackboard> CreateBehaviourTree()
        {
            BehaviourTree<BehaviourTreeTests.Blackboard> behaviourTree =
                new BehaviourTree<BehaviourTreeTests.Blackboard>(
                    "Base Tree",
                    new Sequence<BehaviourTreeTests.Blackboard>(
                        "My sequence",
                        new UtilitySelector<BehaviourTreeTests.Blackboard>(
                            new TaskUtility<BehaviourTreeTests.Blackboard>(
                                new Add<BehaviourTreeTests.Blackboard>(
                                    new BlackboardFloatFunction<BehaviourTreeTests.Blackboard>("Gooba", null),
                                    new FloatConstant<BehaviourTreeTests.Blackboard>(7)),
                                    new Fail<BehaviourTreeTests.Blackboard>()),
                            new TaskUtility<BehaviourTreeTests.Blackboard>(new FloatConstant<BehaviourTreeTests.Blackboard>(12), new Succeed<BehaviourTreeTests.Blackboard>())),
                        new AlwaysFail<BehaviourTreeTests.Blackboard>(new Succeed<BehaviourTreeTests.Blackboard>()),
                        //new BooleanToState<BehaviourTreeTests.Blackboard>(
                        //  new TrueConstant<BehaviourTreeTests.Blackboard>()),
                        new Fail<BehaviourTreeTests.Blackboard>(),
                        new Succeed<BehaviourTreeTests.Blackboard>(),
                        new KeepRunning<BehaviourTreeTests.Blackboard>(),
                        new Invert<BehaviourTreeTests.Blackboard>(new Fail<BehaviourTreeTests.Blackboard>()),
                        new UntilFail<BehaviourTreeTests.Blackboard>(new Fail<BehaviourTreeTests.Blackboard>()),
                        new UntilSuccess<BehaviourTreeTests.Blackboard>(new Fail<BehaviourTreeTests.Blackboard>()),
                        new KeepRunning<BehaviourTreeTests.Blackboard>(),
                        new NamedCoroutine<BehaviourTreeTests.Blackboard>("mybob", new NamedCoroutineCreator("RunRunFail", null)),
                        new Selector<BehaviourTreeTests.Blackboard>(
                            "My selector",
                            new Counter<BehaviourTreeTests.Blackboard>(
                                new FixedResultState<BehaviourTreeTests.Blackboard>(TaskState.Failure)),
                            new FixedResultState<BehaviourTreeTests.Blackboard>(TaskState.Success),
                            new FixedResultState<BehaviourTreeTests.Blackboard>(TaskState.Success))));
            return behaviourTree;
        }
    }
}