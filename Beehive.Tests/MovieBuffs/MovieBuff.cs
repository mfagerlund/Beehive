using System.Collections.Generic;
using System.Diagnostics;
using Beehive.BehaviorTrees;

namespace Beehive.Tests.MovieBuffs
{
    public class MovieBuff
    {
        private static BehaviourTreeCompiler<MovieBuffBlackboard> _compiler;
        private bool _seatReserved;
        private int _bathroomTries = 3;
        private bool _messageThatItsTimeToGoHasBeenShown;

        public MovieBuff()
        {
            Blackboard = new MovieBuffBlackboard(this);
        }

        public bool IsAlarmOn { get; set; }
        public float BathroomNeed { get; set; }
        public Location Location { get; set; }
        public MovieBuffBlackboard Blackboard { get; set; }
        public BehaviourTree<MovieBuffBlackboard> Ai { get; set; }

        public IEnumerator<TaskState> GoToCinema()
        {
            if (Location != Location.Cinema && Location != Location.CinemaAtSeat)
            {
                Location = Location.InTransit;
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at cinema!");
                Location = Location.Cinema;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> ReserveSeat()
        {
            if (!_seatReserved)
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Reserved a seat!");
                _seatReserved = true;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> GoToSelectedTile()
        {
            if (Location != Location.CinemaAtSeat)
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at selected tile!");
                Location = Location.CinemaAtSeat;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> GoToExit()
        {
            if (Location != Location.Exit)
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at exit!");
                Location = Location.Exit;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> GoToBathroom()
        {
            if (Location != Location.Bathroom)
            {
                while (_bathroomTries-- > 0)
                {
                    Debug.WriteLine(" No bathroom found!");
                    yield return TaskState.Failure;
                }

                Debug.WriteLine(" Found bathroom!");

                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at bathroom!");
                Location = Location.Bathroom;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> UseBathroom()
        {
            Debug.WriteLine(" Using bathroom!");
            yield return TaskState.Running;
            yield return TaskState.Running;
            yield return TaskState.Running;
            Debug.WriteLine(" Done in bathroom!");
            BathroomNeed = 0;
        }

        public IEnumerator<TaskState> Exit()
        {
            Debug.WriteLine(" Exited!");
            yield return TaskState.Success;
        }

        public IEnumerator<TaskState> Panic()
        {
            Debug.WriteLine(" Panicking!");
            yield return TaskState.Success;
        }

        public IEnumerator<TaskState> StayInCenterOfSelectedTile()
        {
            yield return TaskState.Running;
        }

        public void BuildAi(string code)
        {
            _compiler = _compiler ?? new BehaviourTreeCompiler<MovieBuffBlackboard>();
            Ai = _compiler.Compile(Blackboard, code);
        }

        public void Tick()
        {
            BathroomNeed += 0.05f;

            if (BathroomNeed > 0.8 && !_messageThatItsTimeToGoHasBeenShown)
            {
                _messageThatItsTimeToGoHasBeenShown = true;
                Debug.WriteLine("Time to go to the bathroom!");
            }

            Ai.Tick();
        }
    }
}