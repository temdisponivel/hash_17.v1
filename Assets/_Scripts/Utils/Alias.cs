using UnityEngine;
using System.Collections;
using Hash17.Blackboard_;
using Hash17.Terminal_;

namespace Hash17.Utils
{
    public static class Alias
    {
        public static Blackboard Board
        {
            get { return Blackboard.Instance; }
        }

        public static GameConfiguration GameConfig
        {
            get { return Board.GameConfiguration; }
        }

        public static Terminal Term
        {
            get { return Terminal.Instance; }
        }
    }
}
