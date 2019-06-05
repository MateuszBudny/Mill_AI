using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public class RoundData {
        public int CurrentDepth { get; set; }
        public Player CurrentPlayer { get; set; }
        public Stack<List<Action>> Reverts { get; set; }
        public int Alpha;
        public int Beta;

        public RoundData(int currentDepth, Player currentPlayer) {
            CurrentDepth = currentDepth;
            CurrentPlayer = currentPlayer;
            Reverts = new Stack<List<Action>>();
            Alpha = int.MinValue;
            Beta = int.MaxValue;
        }

        //public RoundData(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
        //    CurrentDepth = currentDepth;
        //    CurrentPlayer = currentPlayer;
        //    Reverts = reverts;
        //    Alpha = int.MinValue;
        //    Beta = int.MaxValue;
        //}

        public RoundData(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts, int alpha, int beta) {
            CurrentDepth = currentDepth;
            CurrentPlayer = currentPlayer;
            Reverts = reverts;
            Alpha = alpha;
            Beta = beta;
        }

        public RoundData(RoundData roundData) {
            CurrentDepth = roundData.CurrentDepth;
            CurrentPlayer = roundData.CurrentPlayer;
            Reverts = roundData.Reverts;
            Alpha = roundData.Alpha;
            Beta = roundData.Beta;
        }
    }
}
