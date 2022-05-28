using System;
using System.Collections.Generic;

namespace Mill_AI
{
    internal class MinimaxAI : AIPlayer
    {
        public int MaxDepth { protected get => maxDepth; set => maxDepth = value; }

        protected int maxDepth;

        public MinimaxAI(bool isWhite) : base(isWhite) { }

        public MinimaxAI(bool isWhite, int maxDepth) : base(isWhite)
        {
            MaxDepth = maxDepth;
        }

        protected override (int bestEvaluation, Move bestMove) GetBestMove(Player currentPlayer) =>
            Minimax(MaxDepth, currentPlayer);

        private (int bestEvaluation, Move bestMove) Minimax(int currentDepth, Player currentPlayer)
        {
            //PrintWithSkip("evaluate static: " + EvaluateStatic() + "\nAI in hands: " + PawnsInHandNum + "\nAI on board: " + PawnsOnBoardNum +
            //    "\nEnemy in hands: " + Enemy.PawnsInHandNum + "\nEnemy on board: " + Enemy.PawnsOnBoardNum + "\n" + GameOfMill.Instance.GetNameOfStage(currentPlayer.State));
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
                        (evaluation, _) = Minimax(currentDepth - 1, currentPlayer == this ? Enemy : this);
                    }
                    else
                    {
                        (evaluation, _) = Minimax(currentDepth, currentPlayer);
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
                        (evaluation, _) = Minimax(currentDepth - 1, currentPlayer == this ? Enemy : this);
                    }
                    else
                    {
                        (evaluation, _) = Minimax(currentDepth, currentPlayer);
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
                }

                return (minEvaluation, bestMoves.Count == 0 ? new Move() : bestMoves[rand.Next(bestMoves.Count)]);
            }
        }

        public override void InputParameters()
        {
            Console.Write("\nCalculations' max depth (4 or 5 is recommended as default):\n> ");
            string maxDepthInput = Console.ReadLine();
            if(!int.TryParse(maxDepthInput, out maxDepth))
            {
                OnIncorrentParametersInput();
            }
        }
    }
}
