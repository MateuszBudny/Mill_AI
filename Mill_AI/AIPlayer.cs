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
            (bestEvaluation, bestMove) = GetBestMove(new RoundData(maxDepth, this));
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

        protected abstract (int bestEvaluation, Move bestMove) GetBestMove(RoundData roundData);

        protected (int bestEvaluation, Move bestMove) EvaluateChildren(RoundData roundData) {

            int bestEvaluation;
            switch (roundData.CurrentPlayer.State) {

                case GameState.FirstStage:

                    return OnFirstStageMove(
                        (pos) => {
                            //if (roundData.CurrentPlayer == this) {
                            //    Console.WriteLine("Player OnFirstStageMove, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta + ", Depth: " + roundData.CurrentDepth);
                            //} else {
                            //    Console.WriteLine("Enemy OnFirstStageMove, Alpha: " + roundData.Alpha + ", Beta: " + roundData.Beta + ", Depth: " + roundData.CurrentDepth);
                            //}

                            List<Action> newRevert = new List<Action>();

                            newRevert.Add(() => Nodes[pos].SetEmpty());
                            Nodes[pos].SetColor(roundData.CurrentPlayer.IsWhite);

                            newRevert.Add(() => roundData.CurrentPlayer.PawnsOnBoardNum--);
                            roundData.CurrentPlayer.PawnsOnBoardNum++;
                            if (--roundData.CurrentPlayer.PawnsInHandNum <= 0) {
                                newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.FirstStage));
                                roundData.CurrentPlayer.ChangeGameState(GameState.SecondStage);
                            }
                            newRevert.Add(() => roundData.CurrentPlayer.PawnsInHandNum++);

                            if (roundData.CurrentPlayer.IsNewMill(pos)) {
                                newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.FirstStage));
                                roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                                roundData.Reverts.Push(newRevert);
                                //(T)Activator.CreateInstance(typeof(T), new object[] { weight });
                                (bestEvaluation, _) = GetBestMove(new RoundData(roundData));
                                return bestEvaluation;
                            }

                            roundData.Reverts.Push(newRevert);
                            //roundData.CurrentDepth--;
                            //roundData.CurrentPlayer = roundData.CurrentPlayer == this ? Enemy : this;
                            //(bestEvaluation, _) = GetBestMove(new RoundData(roundData));

                            //(bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts));

                            (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                            return bestEvaluation;
                        },
                        roundData);

                case GameState.SecondStage:

                    return OnSecondStageMove((firstPos, secondPos) => {
                        List<Action> newRevert = new List<Action>();

                        NodeState lastNodeState = Nodes[firstPos].State;
                        newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                        Nodes[firstPos].SetEmpty();

                        newRevert.Add(() => Nodes[secondPos].SetEmpty());
                        Nodes[secondPos].SetColor(roundData.CurrentPlayer.IsWhite);

                        if (roundData.CurrentPlayer.IsNewMill(secondPos)) {
                            newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.SecondStage));
                            roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                            roundData.Reverts.Push(newRevert);
                            (bestEvaluation, _) = GetBestMove(new RoundData(roundData));
                            return bestEvaluation;
                        }

                        roundData.Reverts.Push(newRevert);
                        (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                        return bestEvaluation;
                    },
                    roundData);

                case GameState.ThirdStage:

                    return OnThirdStageMove((firstPos, secondPos) => {
                        List<Action> newRevert = new List<Action>();

                        NodeState lastNodeState = Nodes[firstPos].State;
                        newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                        Nodes[firstPos].SetEmpty();

                        newRevert.Add(() => Nodes[secondPos].SetEmpty());
                        Nodes[secondPos].SetColor(roundData.CurrentPlayer.IsWhite);

                        if (IsNewMill(secondPos)) {
                            newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.ThirdStage));
                            roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                            roundData.Reverts.Push(newRevert);
                            (bestEvaluation, _) = GetBestMove(new RoundData(roundData));
                            return bestEvaluation;
                        }

                        roundData.Reverts.Push(newRevert);
                        (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                        return bestEvaluation;
                    },
                    roundData);

                case GameState.MillHasBeenArranged:

                    return OnMillHasBeenArrangedMove(
                        (pos) => {
                            List<Action> newRevert = new List<Action>();

                            NodeState lastNodeState = Nodes[pos].State;
                            newRevert.Add(() => Nodes[pos].State = lastNodeState);
                            newRevert.Add(() => roundData.CurrentPlayer.Enemy.PawnsOnBoardNum++);
                            roundData.CurrentPlayer.KillEnemysPawn(pos);

                            newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged));
                            roundData.CurrentPlayer.ChangeGameState(roundData.CurrentPlayer.LastState);

                            if(roundData.CurrentPlayer.ChangeEnemyToThirdStageIfPossible()) {
                                GameState enemysPreviousState = roundData.CurrentPlayer.Enemy.LastState;
                                newRevert.Add(() => roundData.CurrentPlayer.Enemy.ChangeGameState(enemysPreviousState));
                            }

                            roundData.Reverts.Push(newRevert);
                            (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                            return bestEvaluation;
                        },
                        roundData);

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        protected int EvaluateStatic() {
            return (PawnsInHandNum + PawnsOnBoardNum) - (Enemy.PawnsInHandNum + Enemy.PawnsOnBoardNum);
        }

        protected (int bestEvaluation, Move bestMove) OnFirstStageMove(Func<int, int> OnValid, RoundData roundData) =>
            OnePositionMove(roundData.CurrentPlayer.FirstStageIsMoveValid, OnValid, roundData);

        protected (int bestEvaluation, Move bestMove) OnSecondStageMove(Func<int, int, int> OnValid, RoundData roundData) =>
            TwoPositionsMove(true, roundData.CurrentPlayer.SecondStageIsFirstMoveValid, roundData.CurrentPlayer.SecondStageIsSecondMoveValid, OnValid, roundData);

        protected (int bestEvaluation, Move bestMove) OnThirdStageMove(Func<int, int, int> OnValid, RoundData roundData) =>
            TwoPositionsMove(false, roundData.CurrentPlayer.ThirdStageIsFirstMoveValid, (_, secondPos) => ThirdStageIsSecondMoveValid(secondPos), OnValid, roundData);

        protected (int bestEvaluation, Move bestMove) OnMillHasBeenArrangedMove(Func<int, int> OnValid, RoundData roundData) =>
            OnePositionMove(roundData.CurrentPlayer.IsMoveValidInMillArrangedState, OnValid, roundData);

        protected abstract (int bestEvaluation, Move bestMove) OnePositionMove(Func<int, bool> IsMoveValidCondition, Func<int, int> OnValid, 
            RoundData roundData);

        protected abstract (int bestEvaluation, Move bestMove) TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, 
            Func<int, int, bool> IsSecondMoveValid, Func<int, int, int> OnValid, RoundData roundData);

        protected void Revert(Stack<List<Action>> reverts) {
            foreach (Action revert in reverts.Pop()) {
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

        //private RoundData GetProperNewRoundData(RoundData roundData) {
        //    if (roundData.GetType() == typeof(RoundData)) {
        //        return new RoundData(roundData);
        //    } else if(roundData.GetType() == typeof(RoundDataMinimaxAlphaBeta)) {
        //        return new RoundDataMinimaxAlphaBeta(roundData);
        //    }

        //    Console.WriteLine("GetProperNewRoundData doesn't work as it should.");
        //    return null;
        //}
    }
}
