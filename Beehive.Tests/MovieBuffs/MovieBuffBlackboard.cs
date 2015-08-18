using Beehive.BehaviorTrees;

namespace Beehive.Tests.MovieBuffs
{
    public class MovieBuffBlackboard : BehaviourReflectionTreeBlackboard<MovieBuff>
    {
        public MovieBuffBlackboard(MovieBuff movieBuff)
            : base(movieBuff)
        {
        }
    }
}