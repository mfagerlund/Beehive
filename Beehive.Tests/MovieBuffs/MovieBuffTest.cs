using System;
using FluentAssertions;
using NUnit.Framework;

namespace Beehive.Tests.MovieBuffs
{
    [TestFixture]
    public class MovieBuffTest
    {
        private const string Code = @"
(BehaviourTree :RunningTaskHysterisis=10 
    (?
        (→  :if=IsAlarmOn /* THIS STUFF NEEDS TO WORK */     
            (→ 
                (GoToExit) 
                (Exit))
            (Panic :Message=""I can't find the exit!""))     
        (→  :if=(> BathroomNeed 0.8)
            (?
                (→ 
                    (GoToBathroom) 
                    (UseBathroom))
                (Panic :Message=""I can't find the bathroom!"")))
        (→
            (GoToCinema)
            (ReserveSeat)
            (GoToSelectedTile)
            (RunOnce 
              :Name=""Here's a name for you""
              (DebugLog (Format ""This seems {0} to be working {1}"" 1 5.5)))
            (StayInCenterOfSelectedTile))))";

        [Test]
        public void CanCompile()
        {
            MovieBuff movieBuff = Create();
            movieBuff.Should().NotBeNull();
            movieBuff.Ai.Should().NotBeNull();
            Console.WriteLine(movieBuff.Ai.ToString());
        }

        [Test]
        public void CanRun()
        {
            MovieBuff movieBuff = Create();
            for (int i = 0; i < 100; i++)
            {
                movieBuff.Tick();
                if (movieBuff.Ai.Context.RunningTask != null)
                {
                    Console.WriteLine(movieBuff.Ai.Context.RunningTask.DebugText);
                }

                if (movieBuff.Location == Location.Exit)
                {
                    break;
                }
            }
        }

        private MovieBuff Create()
        {
            MovieBuff movieBuff = new MovieBuff();
            movieBuff.BuildAi(Code);
            return movieBuff;
        }
    }
}