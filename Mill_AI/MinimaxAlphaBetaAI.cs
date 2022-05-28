using System;
using System.Collections.Generic;

namespace Mill_AI
{
    /// <summary>
    /// WIP! no errors, but this AI does stupid moves
    /// </summary>
    internal class MinimaxAlphaBetaAI : MinimaxAI
    {
        public MinimaxAlphaBetaAI(bool isWhite) : base(isWhite) { }

        public MinimaxAlphaBetaAI(bool isWhite, int maxDepth) : base(isWhite, maxDepth) { }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(Player currentPlayer) =>
            MinimaxAlphaBeta(MaxDepth, currentPlayer, int.MinValue, int.MaxValue);

        private (int bestEvaluation, Move bestMove) MinimaxAlphaBeta(int currentDepth, Player currentPlayer, int alpha, int beta)
        {
            //PrintWithSkip("Player: " + (currentPlayer == this ? "AI" : "Player") + "\nEvaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
            //    "\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum + "\n" + GameOfMill.Instance.GetNameOfStage(currentPlayer.State) + 
            //    "\nAlpha: " + alpha + "\nBeta: " + beta);
            if(currentDepth == 0 || GameOfMill.Instance.HasPlayerLost(currentPlayer))
            {
                return (EvaluateStatic(), new Move());
            }

            List<Move> moves = new List<Move>();
            List<Action> reverts = new List<Action>();
            List<Move> bestMoves = new List<Move>();
            int evaluation;
            bool isMillHasBeenArrangedANextMove;

            moves = GetMoves(currentPlayer);

            if(currentPlayer == this)
            {

                int maxEvaluation = int.MinValue;

                foreach(Move move in moves)
                {
                    (reverts, isMillHasBeenArrangedANextMove) = MakeMoveReturnReverts(move, currentPlayer);
                    if(!isMillHasBeenArrangedANextMove)
                    {
                        (evaluation, _) = MinimaxAlphaBeta(currentDepth - 1, currentPlayer == this ? Enemy : this, alpha, beta);
                    }
                    else
                    {
                        (evaluation, _) = MinimaxAlphaBeta(currentDepth, currentPlayer, alpha, beta);
                    }

                    if(evaluation == maxEvaluation)
                    {
                        bestMoves.Add(move);
                    }

                    if(evaluation > maxEvaluation)
                    {
                        maxEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);

                    alpha = Math.Max(alpha, evaluation);
                    if(beta <= alpha)
                    {
                        //Console.WriteLine("\n\nPRUNED\n\n");
                        break;
                    }
                }

                return (maxEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
            else
            {
                int minEvaluation = int.MaxValue;

                foreach(Move move in moves)
                {
                    (reverts, isMillHasBeenArrangedANextMove) = MakeMoveReturnReverts(move, currentPlayer);
                    if(!isMillHasBeenArrangedANextMove)
                    {
                        (evaluation, _) = MinimaxAlphaBeta(currentDepth - 1, currentPlayer == this ? Enemy : this, alpha, beta);
                    }
                    else
                    {
                        (evaluation, _) = MinimaxAlphaBeta(currentDepth, currentPlayer, alpha, beta);
                    }

                    if(evaluation == minEvaluation)
                    {
                        bestMoves.Add(move);
                    }

                    if(evaluation < minEvaluation)
                    {
                        minEvaluation = evaluation;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    Revert(reverts);

                    beta = Math.Min(beta, evaluation);
                    if(beta <= alpha)
                    {
                        //Console.WriteLine("\n\nPRUNED\n\n");
                        break;
                    }
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }
    }
}
