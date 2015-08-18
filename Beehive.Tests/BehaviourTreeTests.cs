using System;
using System.Collections.Generic;
using Beehive;
using Beehive.BehaviorTrees;
using Beehive.Lisp;
using FluentAssertions;
using NUnit.Framework;

namespace Beehive.Tests
{
    [TestFixture]
    public class BehaviourTreeTests
    {
        [Test]
        public void SelectorWorks()
        {
            Counter<Blackboard> c1;
            Counter<Blackboard> c2;
            Counter<Blackboard> c3;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    new Selector<Blackboard>(
                        c1 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Failure)),
                        c2 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success)),
                        c3 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success))));

            behaviourTree.Tick().Should().Be(TaskState.Success);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(c1, TaskState.Failure, 1, 1, 1);
            Verify(c2, TaskState.Success, 1, 1, 1);
            Verify(c3, null, 0, 0, 0);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void UtilitySelectorWorks()
        {
            Counter<Blackboard> c1;
            Counter<Blackboard> c2;
            Counter<Blackboard> c3;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    new UtilitySelector<Blackboard>(
                        new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.5f), c2 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success))),
                        new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.75f), c1 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Failure))),
                        new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.1f), c3 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success)))));

            behaviourTree.Tick().Should().Be(TaskState.Success);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(c1, TaskState.Failure, 1, 1, 1);
            Verify(c2, TaskState.Success, 1, 1, 1);
            Verify(c3, null, 0, 0, 0);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void SequenceWorks()
        {
            Counter<Blackboard> c1;
            Counter<Blackboard> c2;
            Counter<Blackboard> c3;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    new Sequence<Blackboard>(
                        c1 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success)),
                        c2 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Failure)),
                        c3 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success))));

            behaviourTree.Tick().Should().Be(TaskState.Failure);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(c1, TaskState.Success, 1, 1, 1);
            Verify(c2, TaskState.Failure, 1, 1, 1);
            Verify(c3, null, 0, 0, 0);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void UtilitySequenceWorks()
        {
            Counter<Blackboard> c1;
            Counter<Blackboard> c2;
            Counter<Blackboard> c3;
            BehaviourTree<Blackboard> behaviourTree =
             new BehaviourTree<Blackboard>(
                "Base",
                 new UtilitySequence<Blackboard>(
                    new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.5f), c2 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Failure))),
                    new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.75f), c1 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success))),
                    new TaskUtility<Blackboard>(new FloatConstant<Blackboard>(0.5f), c3 = new Counter<Blackboard>(new FixedResultState<Blackboard>(TaskState.Success)))));

            behaviourTree.Tick().Should().Be(TaskState.Failure);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(c1, TaskState.Success, 1, 1, 1);
            Verify(c2, TaskState.Failure, 1, 1, 1);
            Verify(c3, null, 0, 0, 0);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void CouroutineTaskWorks()
        {
            Counter<Blackboard> c;
            Coroutine<Blackboard> co;
            CoroutineState state = new CoroutineState { TaskStateToReturn = TaskState.Running };
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    new Sequence<Blackboard>(
                        c = new Counter<Blackboard>(
                            co = new Coroutine<Blackboard>(b => Coroutine(b, state)))));

            behaviourTree.Tick().Should().Be(TaskState.Running);
            behaviourTree.Context.RunningTask.Should().Be(co);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(co);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(c);
            Verify(c, TaskState.Running, 1, 1, 0, true);
            state.Started.Should().BeTrue();
            state.Yields.Should().Be(1);
            state.FinallyWasCalled.Should().Be(false);

            behaviourTree.Tick().Should().Be(TaskState.Running);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(co);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(c);
            behaviourTree.Context.RunningTask.Should().Be(co);
            Verify(c, TaskState.Running, 0, 1, 0, true);
            state.Started.Should().BeTrue();
            state.Yields.Should().Be(2);
            state.FinallyWasCalled.Should().Be(false);

            state.TaskStateToReturn = TaskState.Failure;
            behaviourTree.Tick().Should().Be(TaskState.Failure);
            behaviourTree.Context.RunningTask.Should().BeNull();
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
            Verify(c, TaskState.Failure, 0, 1, 1, true);
            state.Started.Should().BeTrue();
            state.Yields.Should().Be(3);
            state.FinallyWasCalled.Should().Be(true);
        }

        public IEnumerator<TaskState> Coroutine(Blackboard blackboard, CoroutineState state)
        {
            state.Started = true;
            try
            {
                while (state.TaskStateToReturn == TaskState.Running)
                {
                    state.Yields++;
                    yield return TaskState.Running;
                }
                state.Yields++;
                yield return TaskState.Failure;
            }
            finally
            {
                state.FinallyWasCalled = true;
            }
        }

        [Test]
        public void TaskThatReturnsSuccess()
        {
            Counter<Blackboard> x;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    x = new Counter<Blackboard>(
                       new FixedResultState<Blackboard>(TaskState.Success)));

            behaviourTree.Tick().Should().Be(TaskState.Success);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(x, TaskState.Success, 1, 1, 1);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void TaskThatReturnsFailure()
        {
            Counter<Blackboard> x;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    x = new Counter<Blackboard>(
                        new FixedResultState<Blackboard>(TaskState.Failure)));

            behaviourTree.Tick().Should().Be(TaskState.Failure);
            behaviourTree.Context.RunningTask.Should().BeNull();
            Verify(x, TaskState.Failure, 1, 1, 1);
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void TaskThatReturnsRunningThenFailure()
        {
            Counter<Blackboard> x;
            FixedResultState<Blackboard> s;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    x = new Counter<Blackboard>(
                        s = new FixedResultState<Blackboard>(TaskState.Running)));

            behaviourTree.Tick().Should().Be(TaskState.Running);
            Verify(x, TaskState.Running, 1, 1, 0, true);
            behaviourTree.Context.RunningTask.Should().Be(s);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(s);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(x);
            s.ResultState = TaskState.Failure;

            behaviourTree.Tick().Should().Be(TaskState.Failure);
            Verify(x, TaskState.Failure, 0, 1, 1, true);
            behaviourTree.Context.RunningTask.Should().BeNull();
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void TaskThatReturnsFailureThenRunning()
        {
            Counter<Blackboard> x;
            FixedResultState<Blackboard> s;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    x = new Counter<Blackboard>(
                        s = new FixedResultState<Blackboard>(TaskState.Running)));

            behaviourTree.Tick().Should().Be(TaskState.Running);
            behaviourTree.Context.RunningTask.Should().Be(s);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(x);
            Verify(x, TaskState.Running, 1, 1, 0, true);
            s.ResultState = TaskState.Failure;

            behaviourTree.Tick().Should().Be(TaskState.Failure);
            Verify(x, TaskState.Failure, 0, 1, 1, true);
            behaviourTree.Context.RunningTask.Should().BeNull();
            behaviourTree.Context.CurrentlyRunning.Should().BeEmpty();
        }

        [Test]
        public void TaskThatReturnsRunningAndIsThenSuperceded()
        {
            Counter<Blackboard> targetCounter;
            Counter<Blackboard> preCounter;
            FixedResultState<Blackboard> pre;
            FixedResultState<Blackboard> target;
            BehaviourTree<Blackboard> behaviourTree =
                new BehaviourTree<Blackboard>(
                    "Base",
                    new Selector<Blackboard>(
                        preCounter = new Counter<Blackboard>(
                            pre = new FixedResultState<Blackboard>(TaskState.Failure)),
                        targetCounter = new Counter<Blackboard>(
                          target = new FixedResultState<Blackboard>(TaskState.Running))));

            behaviourTree.Tick().Should().Be(TaskState.Running);
            behaviourTree.Context.RunningTask.Should().Be(target);

            behaviourTree.Context.CurrentlyRunning.Should().Contain(targetCounter);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(target);

            Verify(preCounter, TaskState.Failure, 1, 1, 1, true);
            Verify(targetCounter, TaskState.Running, 1, 1, 0, true);

            pre.ResultState = TaskState.Running;
            behaviourTree.Tick().Should().Be(TaskState.Running);
            behaviourTree.Context.RunningTask.Should().Be(pre);
            Verify(preCounter, TaskState.Running, 1, 1, 0, true);
            Verify(targetCounter, TaskState.Failure, 0, 0, 1, true);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(preCounter);
            behaviourTree.Context.CurrentlyRunning.Should().Contain(pre);
        }

        private void Verify<TBb>(
            Counter<TBb> task,
            TaskState? expectedState,
            int expectedOnEnterCount,
            int expectedExecutionCount,
            int expectedOnExitCount,
            bool reset = false) where TBb : IBehaviourTreeBlackboard
        {
            if (expectedState.HasValue)
            {
                task.State.Should().Be(expectedState);
            }
            task.OnEnterCount.Should().Be(expectedOnEnterCount);
            task.OnExitCount.Should().Be(expectedOnExitCount);
            task.ExecuteCount.Should().Be(expectedExecutionCount);
            if (reset)
            {
                task.Reset();
            }
        }

        public class CoroutineState
        {
            public bool Started { get; set; }
            public TaskState TaskStateToReturn { get; set; }
            public int Yields { get; set; }
            public bool FinallyWasCalled { get; set; }
        }

        public class Blackboard : IBehaviourTreeBlackboard
        {
            public Blackboard()
            {
                Gooba = 12;
            }

            public float Gooba { get; set; }

            public Func<float> GetFloatFunction(string name)
            {
                // Todo: add reflection to dig into parameters? 
                // Todo: add reflection to supply all floats (skip boring ones).
                if (name == "Gooba") { return () => Gooba; }
                return null;
            }

            public Func<bool> GetBoolFunction(string name)
            {
                if (name == "IsTrue") { return () => true; }
                return null;
            }

            public Func<IEnumerator<TaskState>> GetCoRoutineFunc(string name)
            {
                if (name == "RunRunFail") { return RunRunFail; }
                return null;
            }

            private IEnumerator<TaskState> RunRunFail()
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                yield return TaskState.Failure;
            }
        }
    }
}
