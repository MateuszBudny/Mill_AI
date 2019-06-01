using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class MinimaxAI : AIPlayer {

        public MinimaxAI(bool isWhite, int maxDepth) : base(isWhite, maxDepth) { }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) =>
            Minimax(currentDepth, currentPlayer, reverts);

        private (int bestEvaluation, Move bestMove) Minimax(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            if (currentDepth == 0 || GameOfMill.Instance.HasPlayerLost(currentPlayer)) {
                //PrintWithSkip("evaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
                //"\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum);
                return (EvaluateStatic(), new Move());
            }

            (int bestEvaluation, Move bestMove) = EvaluateChildren(currentDepth, currentPlayer, reverts);
            //PrintWithSkip("evaluate children: " + bestEvaluation);

            return (bestEvaluation, bestMove);
        }

        protected override (int bestEvaluation, Move bestMove) OnePositionMove(Func<int, bool> IsMoveValidCondition, Func<int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            List<Move> bestMoves = new List<Move>();

            if (currentPlayer == this) {
                int maxEvaluation = int.MinValue;
                int pos;
                int evaluation;
                foreach (Node node in Nodes) {
                    pos = node.Id;
                    if (IsMoveValidCondition(pos)) {
                        evaluation = OnValid(pos);
                        if (evaluation == maxEvaluation) {
                            bestMoves.Add(new Move(pos));
                        }
                        if (evaluation > maxEvaluation) {
                            maxEvaluation = evaluation;
                            bestMoves.Clear();
                            bestMoves.Add(new Move(pos));
                        }
                        Revert(reverts);
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
                        evaluation = OnValid(pos);
                        if (evaluation == minEvaluation) {
                            bestMoves.Add(new Move(pos));
                        }
                        if (evaluation < minEvaluation) {
                            minEvaluation = evaluation;
                            bestMoves.Clear();
                            bestMoves.Add(new Move(pos));
                        }
                        Revert(reverts);
                    }
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }

        protected override (int bestEvaluation, Move bestMove) TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Func<int, int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            List<Move> bestMoves = new List<Move>();

            if (currentPlayer == this) {
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
                                Revert(reverts);
                            }
                        }
                    }
                }

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
                                Revert(reverts);
                            }
                        }
                    }
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }
    }
}
