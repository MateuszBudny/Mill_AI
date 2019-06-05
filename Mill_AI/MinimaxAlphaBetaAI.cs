using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class MinimaxAlphaBetaAI : MinimaxAI {

        public MinimaxAlphaBetaAI(bool isWhite, int maxDepth) : base(isWhite, maxDepth) { }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(RoundData roundData) =>
             MinimaxAlphaBeta(new RoundData(roundData));

        private (int bestEvaluation, Move bestMove) MinimaxAlphaBeta(RoundData roundData) {
            if (roundData.CurrentDepth == 0 || GameOfMill.Instance.HasPlayerLost(roundData.CurrentPlayer)) {
                //PrintWithSkip("evaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
                //"\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum);
                return (EvaluateStatic(), new Move());
            }

            (int bestEvaluation, Move bestMove) = EvaluateChildren(new RoundData(roundData));
            //PrintWithSkip("evaluate children: " + bestEvaluation);

            return (bestEvaluation, bestMove);
        }

        protected override (int bestEvaluation, Move bestMove) OnePositionMove(Func<int, bool> IsMoveValidCondition, Func<int, int> OnValid, RoundData roundData) {
            List<Move> bestMoves = new List<Move>();

            if (roundData.CurrentPlayer == this) {
                int maxEvaluation = int.MinValue;
                int pos;
                int evaluation;
                foreach (Node node in Nodes) {
                    pos = node.Id;
                    if (IsMoveValidCondition(pos)) {
                        //Console.WriteLine("Player OnePositionMove, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta + ", Depth: " + roundData.CurrentDepth);
                        evaluation = OnValid(pos);
                        if (evaluation == maxEvaluation) {
                            bestMoves.Add(new Move(pos));
                        }
                        if (evaluation > maxEvaluation) {
                            maxEvaluation = evaluation;
                            bestMoves.Clear();
                            bestMoves.Add(new Move(pos));
                        }
                        roundData.Alpha = Math.Max(roundData.Alpha, evaluation);
                        Revert(roundData.Reverts);
                        if (roundData.Alpha >= roundData.Beta) {
                            //Console.WriteLine("Player, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta);
                            //Console.WriteLine("Player break!");
                            break;
                        }
                    }
                }

                return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            } else {
                int minEvaluation = int.MaxValue;
                int pos;
                int evaluation;
                foreach (Node node in Nodes) {
                    pos = node.Id;
                    if (IsMoveValidCondition(pos)) {
                       // Console.WriteLine("Enemy OnePositionMove, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta + ", Depth: " + roundData.CurrentDepth);
                        evaluation = OnValid(pos);
                        if (evaluation == minEvaluation) {
                            bestMoves.Add(new Move(pos));
                        }
                        if (evaluation < minEvaluation) {
                            minEvaluation = evaluation;
                            bestMoves.Clear();
                            bestMoves.Add(new Move(pos));
                        }
                        roundData.Beta = Math.Min(roundData.Beta, evaluation);
                        Revert(roundData.Reverts);
                        if (roundData.Alpha >= roundData.Beta) {
                            //Console.WriteLine("Enemy, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta);
                            //Console.WriteLine("Enemy break!");
                            break;
                        }
                    }
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }

        protected override (int bestEvaluation, Move bestMove) TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Func<int, int, int> OnValid, RoundData roundData) {
            List<Move> bestMoves = new List<Move>();

            if (roundData.CurrentPlayer == this) {
                int maxEvaluation = int.MinValue;
                int firstPos;
                int secondPos;
                int evaluation;
                foreach (Node firstNode in Nodes) {
                    firstPos = firstNode.Id;
                    if (IsFirstMoveValidCondition(firstPos)) {
                        List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes;
                        foreach (Node secondNode in NodesToConsider) {
                            secondPos = secondNode.Id;
                            if (IsSecondMoveValid(firstPos, secondPos)) {
                                evaluation = OnValid(firstPos, secondPos);
                                if (evaluation == maxEvaluation) {
                                    bestMoves.Add(new Move(firstPos, secondPos));
                                }
                                if (evaluation > maxEvaluation) {
                                    maxEvaluation = evaluation;
                                    bestMoves.Clear();
                                    bestMoves.Add(new Move(firstPos, secondPos));
                                }
                                roundData.Alpha = Math.Max(roundData.Alpha, evaluation);
                                Revert(roundData.Reverts);
                                if (roundData.Alpha >= roundData.Beta) {
                                    //Console.WriteLine("Player, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta);
                                    //Console.WriteLine("Player break!");
                                    goto PlayerAfterForeaches; // break out of nested foreaches
                                }
                            }
                        }
                    }
                }
                PlayerAfterForeaches:
                return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            } else {
                int minEvaluation = int.MaxValue;
                int firstPos;
                int secondPos;
                int evaluation;
                foreach (Node firstNode in Nodes) {
                    firstPos = firstNode.Id;
                    if (IsFirstMoveValidCondition(firstPos)) {
                        List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes;
                        foreach (Node secondNode in NodesToConsider) {
                            secondPos = secondNode.Id;
                            if (IsSecondMoveValid(firstPos, secondPos)) {
                                evaluation = OnValid(firstPos, secondPos);
                                if (evaluation == minEvaluation) {
                                    bestMoves.Add(new Move(firstPos, secondPos));
                                }
                                if (evaluation < minEvaluation) {
                                    minEvaluation = evaluation;
                                    bestMoves.Clear();
                                    bestMoves.Add(new Move(firstPos, secondPos));
                                }
                                roundData.Beta = Math.Min(roundData.Beta, evaluation);
                                Revert(roundData.Reverts);
                                if (roundData.Alpha >= roundData.Beta) {
                                    //Console.WriteLine("Enemy, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta);
                                    //Console.WriteLine("Enemy break!");
                                    goto EnemyAfterForeaches; // break out of nested foreaches
                                }
                            }
                        }
                    }
                }
                EnemyAfterForeaches: 
                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }
    }
}
