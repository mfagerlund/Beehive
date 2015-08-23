using Beehive.BehaviorTrees;

namespace Beehive.Tests.MovieBuffs
{
    public class MovieBuff
    {
        private static BehaviourTreeCompiler<MovieBuffBlackboard> _compiler;

        public MovieBuff()
        {
            Blackboard = new MovieBuffBlackboard(this);
        }

        public bool IsAlarmOn { get; set; }
        public float BathroomNeed { get; set; }
        public Location Location { get; set; }
        public MovieBuffBlackboard Blackboard { get; set; }
        public BehaviourTree<MovieBuffBlackboard> Ai { get; set; }
        
        public void BuildAi(string code)
        {
            _compiler = _compiler ?? new BehaviourTreeCompiler<MovieBuffBlackboard>();
            Ai = _compiler.Compile(Blackboard, code);
        }

        public void Tick()
        {
            BathroomNeed += 0.05f;
            Ai.Tick();
        }
    }
}