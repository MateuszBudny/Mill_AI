using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class MinimaxAI : AIPlayer {

        public MinimaxAI(bool isWhite, int maxDepth) : base(isWhite, maxDepth) { }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(RoundData roundData) =>
            Minimax(roundData);

        private (int bestEvaluation, Move bestMove) Minimax(RoundData roundData) {
            if (roundData.CurrentDepth == 0 || GameOfMill.Instance.HasPlayerLost(roundData.CurrentPlayer)) {
                //PrintWithSkip("evaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
                //"\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum);
                return (EvaluateStatic(), new Move());
            }

            List<Move> moves = new List<Move>();
            Stack<List<Action>> reverts = new Stack<List<Action>>();
            List<Move> bestMoves = new List<Move>();
            int evaluation;

            moves = GetMoves();

            if(roundData.CurrentPlayer == this) {
                
                int maxEvaluation = int.MinValue;

                foreach (Move move in moves) {
                    reverts = MakeMoveReturnReverts(move, roundData);
                    (evaluation, _) = Minimax(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts));
                    
                    if(evaluation == maxEvaluation) {
                        bestMoves.Add(move);
                    }

                    if(evaluation > maxEvaluation) {
                        maxEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);
                }

                return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            } else {
                int minEvaluation = int.MinValue;

                foreach (Move move in moves) {
                    reverts = MakeMoveReturnReverts(move, roundData);
                    (evaluation, _) = Minimax(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts));

                    if (evaluation == minEvaluation) {
                        bestMoves.Add(move);
                    }

                    if (evaluation > minEvaluation) {
                        minEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }

            
            //(int bestEvaluation, Move bestMove) = EvaluateChildren(roundData);
            //PrintWithSkip("evaluate children: " + bestEvaluation);
        }

        protected List<Move> GetMoves() {

        }

        protected override List<Move> OnePositionMove(Func<int, bool> IsMoveValidCondition) {

            //if (roundData.CurrentPlayer == this) {
            //int maxEvaluation = int.MinValue;
            //int pos;
            //int evaluation;
            //foreach (Node node in Nodes) {
            //    pos = node.Id;
            //    if (IsMoveValidCondition(pos)) {
            //        evaluation = OnValid(pos);
            //        if (evaluation == maxEvaluation) {
            //            bestMoves.Add(new Move(pos));
            //        }
            //        if (evaluation > maxEvaluation) {
            //            maxEvaluation = evaluation;
            //            bestMoves.Clear();
            //            bestMoves.Add(new Move(pos));
            //        }
            //        Revert(roundData.Reverts);
            //    }
            //}

            //return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            // } else {
            //int minEvaluation = int.MaxValue;
            //int pos;
            //int evaluation;
            //foreach (Node node in Nodes) {
            //    pos = node.Id;
            //    if (IsMoveValidCondition(pos)) {
            //        evaluation = OnValid(pos);
            //        if (evaluation == minEvaluation) {
            //            bestMoves.Add(new Move(pos));
            //        }
            //        if (evaluation < minEvaluation) {
            //            minEvaluation = evaluation;
            //            bestMoves.Clear();
            //            bestMoves.Add(new Move(pos));
            //        }
            //        Revert(roundData.Reverts);
            //    }
            //}

            //return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            //}

            List<Move> moves = new List<Move>();

            int pos;
            foreach (Node node in Nodes) {
                pos = node.Id;
                if (IsMoveValidCondition(pos)) {
                    moves.Add(new Move(pos));
                }
            }

            return moves;
        }

        protected override List<Move> TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid) {
            //List<Move> bestMoves = new List<Move>();

            //if (roundData.CurrentPlayer == this) {
            //    int maxEvaluation = int.MinValue;
            //    int firstPos;
            //    int secondPos;
            //    int evaluation;
            //    foreach (Node firstNode in Nodes) {
            //        firstPos = firstNode.Id;
            //        if (IsFirstMoveValidCondition(firstPos)) {
            //            List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes; 
            //            foreach (Node secondNode in NodesToConsider) {
            //                secondPos = secondNode.Id;
            //                if (IsSecondMoveValid(firstPos, secondPos)) {
            //                    evaluation = OnValid(firstPos, secondPos);
            //                    if (evaluation == maxEvaluation) {
            //                        bestMoves.Add(new Move(firstPos, secondPos));
            //                    }
            //                    if (evaluation > maxEvaluation) {
            //                        maxEvaluation = evaluation;
            //                        bestMoves.Clear();
            //                        bestMoves.Add(new Move(firstPos, secondPos));
            //                    }
            //                    Revert(roundData.Reverts);
            //                }
            //            }
            //        }
            //    }

            //    return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            //} else {
            //    int minEvaluation = int.MaxValue;
            //    int firstPos;
            //    int secondPos;
            //    int evaluation;
            //    foreach (Node firstNode in Nodes) {
            //        firstPos = firstNode.Id;
            //        if (IsFirstMoveValidCondition(firstPos)) {
            //            List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes;
            //            foreach (Node secondNode in NodesToConsider) {
            //                secondPos = secondNode.Id;
            //                if (IsSecondMoveValid(firstPos, secondPos)) {
            //                    evaluation = OnValid(firstPos, secondPos);
            //                    if (evaluation == minEvaluation) {
            //                        bestMoves.Add(new Move(firstPos, secondPos));
            //                    }
            //                    if (evaluation < minEvaluation) {
            //                        minEvaluation = evaluation;
            //                        bestMoves.Clear();
            //                        bestMoves.Add(new Move(firstPos, secondPos));
            //                    }
            //                    Revert(roundData.Reverts);
            //                }
            //            }
            //        }
            //    }

            //    return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);

            List<Move> moves = new List<Move>();

            int firstPos;
            int secondPos;
            foreach(Node firstNode in Nodes) {
                firstPos = firstNode.Id;
                if(IsFirstMoveValidCondition(firstPos)) {
                    List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes;
                    foreach(Node secondNode in NodesToConsider) {
                        secondPos = secondNode.Id;
                        if(IsSecondMoveValid(firstPos, secondPos)) {
                            moves.Add(new Move(firstPos, secondPos));
                        }
                    }
                }
            }

            return moves;
        }
    }
}
