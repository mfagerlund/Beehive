using System.Collections.Generic;
using System.Diagnostics;
using Beehive.BehaviorTrees;
using Beehive.Lisp;

namespace Beehive.Tests.MovieBuffs
{
    public class MovieBuffBlackboard : BehaviourReflectionTreeBlackboard<MovieBuffBlackboard>
    {
        public MovieBuff MovieBuff { get; set; }
        private bool _seatReserved;
        private int _bathroomTries = 3;

        public MovieBuffBlackboard(MovieBuff movieBuff)
            : base(null)
        {
            MovieBuff = movieBuff;
            Owner = this;
        }

        public bool IsAlarmOn { get { return MovieBuff.IsAlarmOn; } }
        public float BathroomNeed { get { return MovieBuff.BathroomNeed; } }
        public Location Location { get { return MovieBuff.Location; } }

        public IEnumerator<TaskState> GoToCinema()
        {
            if (MovieBuff.Location != Location.Cinema && MovieBuff.Location != Location.CinemaAtSeat)
            {
                MovieBuff.Location = Location.InTransit;
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at cinema!");
                MovieBuff.Location = Location.Cinema;
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
            if (MovieBuff.Location != Location.CinemaAtSeat)
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at selected tile!");
                MovieBuff.Location = Location.CinemaAtSeat;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> GoToExit()
        {
            if (MovieBuff.Location != Location.Exit)
            {
                yield return TaskState.Running;
                yield return TaskState.Running;
                Debug.WriteLine(" Arrived at exit!");
                MovieBuff.Location = Location.Exit;
                yield return TaskState.Success;
            }
        }

        public IEnumerator<TaskState> GoToBathroom()
        {
            if (MovieBuff.Location != Location.Bathroom)
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
                MovieBuff.Location = Location.Bathroom;
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
            MovieBuff.BathroomNeed = 0;
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

    }
}