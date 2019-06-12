using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class MinimaxDeepeningAI : MinimaxAI {

        private double secondsToTimeout;
        private Stopwatch stopwatch = new Stopwatch();

        public MinimaxDeepeningAI(bool isWhite, double secondsToTimeout) : base(isWhite) {
            this.secondsToTimeout = secondsToTimeout;
        }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(int maxDepth, Player currentPlayer) {

            int bestEvaluation = int.MinValue;
            Move bestMove = null;
            int bestDepth = 0;
            int reachedDepth = 0;

            stopwatch.Restart();
            for (int d = 1; !IsTimeout(); d++) {
                (int evaluation, Move move) = MinimaxWithTimeout(d, this);
                if(!IsTimeout()) {
                    bestEvaluation = evaluation;
                    bestMove = move;
                    bestDepth = d;
                }
                reachedDepth = d;
            }

            Console.WriteLine("bestDepth: " + bestDepth);
            Console.WriteLine("reachedDepth: " + reachedDepth);
            Console.WriteLine("time: " + stopwatch.ElapsedSeconds());
            return (bestEvaluation, bestMove);
        }

        protected (int bestEvaluation, Move bestMove) MinimaxWithTimeout(int currentDepth, Player currentPlayer) {
            
            if(IsTimeout()) {
                return (currentPlayer == this ? int.MinValue : int.MaxValue, null);
            }
            //PrintWithSkip("evaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
            //    "\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum + "\n" + GameOfMill.Instance.GetNameOfStage(currentPlayer.State));
            if (currentDepth == 0 || GameOfMill.Instance.HasPlayerLost(currentPlayer)) {
                return (EvaluateStatic(), new Move());
            }

            List<Move> moves = new List<Move>();
            List<Action> reverts = new List<Action>();
            List<Move> bestMoves = new List<Move>();
            int evaluation;
            bool isMillHasBeenArrangedANextMove;

            moves = GetMoves(currentPlayer);

            if (currentPlayer == this) {

                int maxEvaluation = int.MinValue;

                foreach (Move move in moves) {
                    if(IsTimeout()) {
                        break;
                    }

                    (reverts, isMillHasBeenArrangedANextMove) = MakeMoveReturnReverts(move, currentPlayer);
                    if (!isMillHasBeenArrangedANextMove) {
                        (evaluation, _) = MinimaxWithTimeout(currentDepth - 1, currentPlayer == this ? Enemy : this);
                    } else {
                        (evaluation, _) = MinimaxWithTimeout(currentDepth, currentPlayer);
                    }

                    if (evaluation == maxEvaluation) {
                        bestMoves.Add(move);
                    }

                    if (evaluation > maxEvaluation) {
                        maxEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);
                }

                return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            } else {
                int minEvaluation = int.MaxValue;

                foreach (Move move in moves) {
                    if (IsTimeout()) {
                        break;
                    }

                    (reverts, isMillHasBeenArrangedANextMove) = MakeMoveReturnReverts(move, currentPlayer);
                    if (!isMillHasBeenArrangedANextMove) {
                        (evaluation, _) = MinimaxWithTimeout(currentDepth - 1, currentPlayer == this ? Enemy : this);
                    } else {
                        (evaluation, _) = MinimaxWithTimeout(currentDepth, currentPlayer);
                    }

                    if (evaluation == minEvaluation) {
                        bestMoves.Add(move);
                    }

                    if (evaluation < minEvaluation) {
                        minEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }

        private bool IsTimeout() => stopwatch.ElapsedSeconds() > secondsToTimeout;
    }
}
