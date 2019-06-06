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

        protected Stack<List<Action>> MakeMoveReturnReverts(Move move, RoundData roundData) {

            int bestEvaluation;
            int firstPos = move.FirstPos;
            int secondPos = move.SecondPos;
            List<Action> newRevert = new List<Action>();
            NodeState lastNodeState;
            switch (roundData.CurrentPlayer.State) {

                case GameState.FirstStage:

                    newRevert.Add(() => Nodes[firstPos].SetEmpty());
                    Nodes[firstPos].SetColor(roundData.CurrentPlayer.IsWhite);

                    newRevert.Add(() => roundData.CurrentPlayer.PawnsOnBoardNum--);
                    roundData.CurrentPlayer.PawnsOnBoardNum++;
                    if (--roundData.CurrentPlayer.PawnsInHandNum <= 0) {
                        newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.FirstStage));
                        roundData.CurrentPlayer.ChangeGameState(GameState.SecondStage);
                    }
                    newRevert.Add(() => roundData.CurrentPlayer.PawnsInHandNum++);

                    if (roundData.CurrentPlayer.IsNewMill(firstPos)) {
                        newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.FirstStage));
                        roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        roundData.Reverts.Push(newRevert);
                        //(T)Activator.CreateInstance(typeof(T), new object[] { weight });
                        return roundData.Reverts;
                    }

                    roundData.Reverts.Push(newRevert);
                    //roundData.CurrentDepth--;
                    //roundData.CurrentPlayer = roundData.CurrentPlayer == this ? Enemy : this;
                    //(bestEvaluation, _) = GetBestMove(new RoundData(roundData));

                    //(bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts));

                    return roundData.Reverts;

                case GameState.SecondStage:

                    lastNodeState = Nodes[firstPos].State;
                    newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                    Nodes[firstPos].SetEmpty();

                    newRevert.Add(() => Nodes[secondPos].SetEmpty());
                    Nodes[secondPos].SetColor(roundData.CurrentPlayer.IsWhite);

                    if (roundData.CurrentPlayer.IsNewMill(secondPos)) {
                        newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.SecondStage));
                        roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        roundData.Reverts.Push(newRevert);
                        return roundData.Reverts;
                    }

                    roundData.Reverts.Push(newRevert);
                    (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                    return roundData.Reverts;

                case GameState.ThirdStage:

                    lastNodeState = Nodes[firstPos].State;
                    newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                    Nodes[firstPos].SetEmpty();

                    newRevert.Add(() => Nodes[secondPos].SetEmpty());
                    Nodes[secondPos].SetColor(roundData.CurrentPlayer.IsWhite);

                    if (IsNewMill(secondPos)) {
                        newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.ThirdStage));
                        roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                        roundData.Reverts.Push(newRevert);
                        return roundData.Reverts;
                    }

                    roundData.Reverts.Push(newRevert);
                    (bestEvaluation, _) = GetBestMove(new RoundData(roundData.CurrentDepth - 1, roundData.CurrentPlayer == this ? Enemy : this, roundData.Reverts, roundData.Alpha, roundData.Beta));
                    return roundData.Reverts;

                case GameState.MillHasBeenArranged:

                    lastNodeState = Nodes[firstPos].State;
                    newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                    newRevert.Add(() => roundData.CurrentPlayer.Enemy.PawnsOnBoardNum++);
                    roundData.CurrentPlayer.KillEnemysPawn(firstPos);

                    newRevert.Add(() => roundData.CurrentPlayer.ChangeGameState(GameState.MillHasBeenArranged));
                    roundData.CurrentPlayer.ChangeGameState(roundData.CurrentPlayer.LastState);

                    if(roundData.CurrentPlayer.ChangeEnemyToThirdStageIfPossible()) {
                        GameState enemysPreviousState = roundData.CurrentPlayer.Enemy.LastState;
                        newRevert.Add(() => roundData.CurrentPlayer.Enemy.ChangeGameState(enemysPreviousState));
                    }

                    roundData.Reverts.Push(newRevert);

                    return roundData.Reverts;

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");

            }
        }

        protected int EvaluateStatic() {
            return (PawnsInHandNum + PawnsOnBoardNum) - (Enemy.PawnsInHandNum + Enemy.PawnsOnBoardNum);
        }

        protected List<Move> OnFirstStageMove(Func<int, int> OnValid, RoundData roundData) =>
            OnePositionMove(roundData.CurrentPlayer.FirstStageIsMoveValid, OnValid, roundData);

        protected List<Move> OnSecondStageMove(Func<int, int, int> OnValid, RoundData roundData) =>
            TwoPositionsMove(true, roundData.CurrentPlayer.SecondStageIsFirstMoveValid, roundData.CurrentPlayer.SecondStageIsSecondMoveValid, OnValid, roundData);

        protected List<Move> OnThirdStageMove(Func<int, int, int> OnValid, RoundData roundData) =>
            TwoPositionsMove(false, roundData.CurrentPlayer.ThirdStageIsFirstMoveValid, (_, secondPos) => ThirdStageIsSecondMoveValid(secondPos), OnValid, roundData);

        protected List<Move> OnMillHasBeenArrangedMove(Func<int, int> OnValid, RoundData roundData) =>
            OnePositionMove(roundData.CurrentPlayer.IsMoveValidInMillArrangedState, OnValid, roundData);

        protected abstract List<Move> OnePositionMove(Func<int, bool> IsMoveValidCondition, Func<int, int> OnValid, 
            RoundData roundData);

        protected abstract List<Move> TwoPositionsMove(bool considerOnlyNeighboursAsSecondMove, Func<int, bool> IsFirstMoveValidCondition, 
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
