using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public abstract class AIPlayer : Player {

        protected int maxDepth;
        protected Random rand = new Random();

        // variables for skipping printing function
        private bool skip = false;
        private bool skip50 = false;
        private int skipCounter = 0;
        private const int SKIP_NUM = 100;

        public AIPlayer(bool isWhite, int maxDepth) : base(isWhite) {
            this.maxDepth = maxDepth;
        }

        public override void Move() {
            skip = false;
            skip50 = false;
            skipCounter = 0;

            Move bestMove;
            int bestEvaluation;
            (bestEvaluation, bestMove) = GetBestMove(maxDepth, this);
            Console.WriteLine("AI move: " + bestMove);
            Console.WriteLine("Its evaluation: " + bestEvaluation);
            MakeMove(bestMove);
        }

        private void MakeMove(Move move) {
            switch (State) {
                case GameState.FirstStage:

                    OnValidFirstStageMove(move.FirstPos);

                    break;

                case GameState.SecondStage:

                    OnValidSecondStageMove(move.FirstPos, move.SecondPos);

                    break;

                case GameState.ThirdStage:

                    OnValidThirdStageMove(move.FirstPos, move.SecondPos);

                    break;

                case GameState.MillHasBeenArranged:

                    OnValidMillHasBeenArranged(move.FirstPos);

                    break;

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        protected abstract (int bestEvaluation, Move bestMove) GetBestMove(int currentDepth, Player currentPlayer);

        protected (List<Action> reverts, bool isMillHasBeenArrangedANextMove) MakeMoveReturnReverts(Move move, Player currentPlayer) {

            int firstPos = move.FirstPos;
            int secondPos = move.SecondPos;
            List<Action> reverts = new List<Action>();
            NodeState lastNodeState;

            switch (currentPlayer.State) {

                case GameState.FirstStage:

                    reverts.Add(() => Nodes[firstPos].SetEmpty());
                    Nodes[firstPos].SetColor(currentPlayer.IsWhite);

                    reverts.Add(() => currentPlayer.PawnsOnBoardNum--);
                    currentPlayer.PawnsOnBoardNum++;
                    if (--currentPlayer.PawnsInHandNum <= 0) {
                        reverts.Add(() => currentPlayer.ChangeGameState(GameState.FirstStage));
                        currentPlayer.ChangeGameState(GameState.SecondStage);
                    }
                    reverts.Add(() => currentPlayer.PawnsInHandNum++);

                    if (currentPlayer.IsNewMill(firstPos)) {
                        reverts.Add(() => currentPlayer.ChangeGameState(GameState.FirstStage));
                        currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        return (reverts, true);
                    }

                    return (reverts, false);

                case GameState.SecondStage:

                    lastNodeState = Nodes[firstPos].State;
                    reverts.Add(() => Nodes[firstPos].State = lastNodeState);
                    Nodes[firstPos].SetEmpty();

                    reverts.Add(() => Nodes[secondPos].SetEmpty());
                    Nodes[secondPos].SetColor(currentPlayer.IsWhite);

                    if (currentPlayer.IsNewMill(secondPos)) {
                        reverts.Add(() => currentPlayer.ChangeGameState(GameState.SecondStage));
                        currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        return (reverts, true);
                    }

                    return (reverts, false);

                case GameState.ThirdStage:

                    lastNodeState = Nodes[firstPos].State;
                    reverts.Add(() => Nodes[firstPos].State = lastNodeState);
                    Nodes[firstPos].SetEmpty();

                    reverts.Add(() => Nodes[secondPos].SetEmpty());
                    Nodes[secondPos].SetColor(currentPlayer.IsWhite);

                    if (IsNewMill(secondPos)) {
                        reverts.Add(() => currentPlayer.ChangeGameState(GameState.ThirdStage));
                        currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        return (reverts, true);
                    }

                    return (reverts, false);

                case GameState.MillHasBeenArranged:

                    lastNodeState = Nodes[firstPos].State;
                    reverts.Add(() => Nodes[firstPos].State = lastNodeState);
                    reverts.Add(() => currentPlayer.Enemy.PawnsOnBoardNum++);
                    currentPlayer.KillEnemysPawn(firstPos);

                    reverts.Add(() => currentPlayer.ChangeGameState(GameState.MillHasBeenArranged));
                    currentPlayer.ChangeGameState(currentPlayer.LastState);

                    if(currentPlayer.ChangeEnemyToThirdStageIfPossible()) {
                        GameState enemysPreviousState = currentPlayer.Enemy.LastState;
                        Player enemy = currentPlayer.Enemy;
                        reverts.Add(() => enemy.ChangeGameState(enemysPreviousState));
                    }

                    return (reverts, false);

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");

            }
        }

        protected List<Move> GetMoves(Player currentPlayer) {
            switch (currentPlayer.State) {
                case GameState.FirstStage:
                    return OnFirstStageMove(currentPlayer);

                case GameState.SecondStage:

                    return OnSecondStageMove(currentPlayer);

                case GameState.ThirdStage:

                    return OnThirdStageMove(currentPlayer);

                case GameState.MillHasBeenArranged:

                    return OnMillHasBeenArrangedMove(currentPlayer);

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        protected int EvaluateStatic() {
            return (PawnsInHandNum + PawnsOnBoardNum) - (Enemy.PawnsInHandNum + Enemy.PawnsOnBoardNum);
        }

        protected List<Move> OnFirstStageMove(Player currentPlayer) =>
            OnePositionMove(currentPlayer.FirstStageIsMoveValid);

        protected List<Move> OnSecondStageMove(Player currentPlayer) =>
            TwoPositionsMove(true, currentPlayer.SecondStageIsFirstMoveValid, currentPlayer.SecondStageIsSecondMoveValid);

        protected List<Move> OnThirdStageMove(Player currentPlayer) =>
            TwoPositionsMove(false, currentPlayer.ThirdStageIsFirstMoveValid, (_, secondPos) => currentPlayer.ThirdStageIsSecondMoveValid(secondPos));

        protected List<Move> OnMillHasBeenArrangedMove(Player currentPlayer) =>
            OnePositionMove(currentPlayer.IsMoveValidInMillArrangedState);

        protected List<Move> OnePositionMove(Func<int, bool> IsMoveValidCondition) {

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

        protected List<Move> TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid) {

            List<Move> moves = new List<Move>();

            int firstPos;
            int secondPos;
            foreach (Node firstNode in Nodes) {
                firstPos = firstNode.Id;
                if (IsFirstMoveValidCondition(firstPos)) {
                    List<Node> NodesToConsider = considerOnlyNeighboursAsSecondMove ? firstNode.GetNeighbours() : Nodes;
                    foreach (Node secondNode in NodesToConsider) {
                        secondPos = secondNode.Id;
                        if (IsSecondMoveValid(firstPos, secondPos)) {
                            moves.Add(new Move(firstPos, secondPos));
                        }
                    }
                }
            }

            return moves;
        }

        protected void Revert(List<Action> reverts) {
            foreach (Action revert in reverts) {
                revert();
            }
        }

        protected void PrintWithSkip(string message) {
            if (!skip && !skip50) {
                MillBoard.Print();
                Console.WriteLine(message);
                string command = Console.ReadLine();
                if (command == "s") {
                    skip = true;
                } else if (command == "d") {
                    skip50 = true;
                    skipCounter = 0;
                }
            } else if (skip50) {
                skipCounter++;
                if (skipCounter > SKIP_NUM) {
                    skip50 = false;
                }
            }
        }
    }
}
