using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class AIPlayer : Player {

        private int maxDepth;

        public AIPlayer(bool isWhite, int maxDepth) : base(isWhite) {
            this.maxDepth = maxDepth;
        }

        public override void Move() {
            Move bestMove;
            (_, bestMove) = Minimax(maxDepth, this, new Stack<List<Action>>());
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

        private (int bestEvaluation, Move bestMove) Minimax(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            MillBoard.Print();

            if (currentDepth == 0 || GameOfMill.Instance.HasPlayerLost(currentPlayer)) {
                return (EvaluateStatic(), new Move());
            }

            (int bestEvaluation, Move bestMove) = EvaluateChildren(currentDepth, currentPlayer, reverts);
            return (bestEvaluation, bestMove);
        }

        private (int bestEvaluation, Move bestMove) EvaluateChildren(int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {

            int bestEvaluation;
            switch (currentPlayer.State) {
                case GameState.FirstStage:

                    return OnFirstStageMove(
                        (pos) => {
                            // TODO: maybe separate reverts from actions?
                            List<Action> newRevert = new List<Action>();

                            newRevert.Add(() => Nodes[pos].SetEmpty());
                            Nodes[pos].SetColor(currentPlayer.IsWhite);

                            newRevert.Add(() => currentPlayer.PawnsOnBoardNum--);
                            currentPlayer.PawnsOnBoardNum++;
                            if (--currentPlayer.PawnsInHandNum <= 0) {
                                newRevert.Add(() => currentPlayer.ChangeGameState(GameState.FirstStage));
                                currentPlayer.ChangeGameState(GameState.SecondStage);
                            }
                            newRevert.Add(() => currentPlayer.PawnsInHandNum++);

                            if (currentPlayer.IsNewMill(pos)) {
                                newRevert.Add(() => currentPlayer.ChangeGameState(GameState.FirstStage));
                                currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                                reverts.Push(newRevert);
                                (bestEvaluation, _) = Minimax(currentDepth, currentPlayer, reverts);
                                return bestEvaluation;
                            }

                            reverts.Push(newRevert);
                            (bestEvaluation, _) = Minimax(currentDepth - 1, (currentPlayer == this ? Enemy : this), reverts);
                            return bestEvaluation;
                        },
                        currentDepth, currentPlayer, reverts);

                case GameState.SecondStage:

                    return OnSecondStageMove((firstPos, secondPos) => {
                        List<Action> newRevert = new List<Action>();

                        NodeState lastNodeState = Nodes[firstPos].State;
                        newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                        Nodes[firstPos].SetEmpty();

                        newRevert.Add(() => Nodes[secondPos].SetEmpty());
                        Nodes[secondPos].SetColor(currentPlayer.IsWhite);

                        if (currentPlayer.IsNewMill(secondPos)) {
                            newRevert.Add(() => currentPlayer.ChangeGameState(GameState.SecondStage));
                            currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                            reverts.Push(newRevert);
                            (bestEvaluation, _) = Minimax(currentDepth, currentPlayer, reverts);
                            return bestEvaluation;
                        }

                        reverts.Push(newRevert);
                        (bestEvaluation, _) = Minimax(currentDepth - 1, (currentPlayer == this ? Enemy : this), reverts);
                        return bestEvaluation;
                    },
                    currentDepth, currentPlayer, reverts);

                case GameState.ThirdStage:

                    return OnThirdStageMove((firstPos, secondPos) => {
                        List<Action> newRevert = new List<Action>();

                        NodeState lastNodeState = Nodes[firstPos].State;
                        newRevert.Add(() => Nodes[firstPos].State = lastNodeState);
                        Nodes[firstPos].SetEmpty();

                        newRevert.Add(() => Nodes[secondPos].SetEmpty());
                        Nodes[secondPos].SetColor(currentPlayer.IsWhite);

                        if (IsNewMill(secondPos)) {
                            newRevert.Add(() => currentPlayer.ChangeGameState(GameState.ThirdStage));
                            currentPlayer.ChangeGameState(GameState.MillHasBeenArranged);

                            reverts.Push(newRevert);
                            (bestEvaluation, _) = Minimax(currentDepth, currentPlayer, reverts);
                            return bestEvaluation;
                        }

                        reverts.Push(newRevert);
                        (bestEvaluation, _) = Minimax(currentDepth - 1, (currentPlayer == this ? Enemy : this), reverts);
                        return bestEvaluation;
                    },
                    currentDepth, currentPlayer, reverts);

                case GameState.MillHasBeenArranged:

                    return OnMillHasBeenArrangedMove(
                        (pos) => {
                            List<Action> newRevert = new List<Action>();

                            NodeState lastNodeState = Nodes[pos].State;
                            newRevert.Add(() => Nodes[pos].State = lastNodeState);
                            newRevert.Add(() => currentPlayer.Enemy.PawnsOnBoardNum++);
                            currentPlayer.KillEnemysPawn(pos);

                            newRevert.Add(() => currentPlayer.ChangeGameState(GameState.MillHasBeenArranged));
                            currentPlayer.ChangeGameState(currentPlayer.LastState);

                            if(currentPlayer.ChangeEnemyToThirdStageIfPossible()) {
                                newRevert.Add(() => currentPlayer.Enemy.ChangeGameState(GameState.MillHasBeenArranged));
                            }

                            reverts.Push(newRevert);
                            (bestEvaluation, _) = Minimax(currentDepth - 1, (currentPlayer == this ? Enemy : this), reverts);
                            return bestEvaluation;
                        },
                        currentDepth, currentPlayer, reverts);

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        private int EvaluateStatic() {
            return (PawnsInHandNum + PawnsOnBoardNum) - (Enemy.PawnsInHandNum + Enemy.PawnsOnBoardNum);
        }

        protected (int bestEvaluation, Move bestMove) OnFirstStageMove(Func<int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) =>
            OnePositionMove(FirstStageIsMoveValid, OnValid, currentDepth, currentPlayer, reverts);

        protected (int bestEvaluation, Move bestMove) OnSecondStageMove(Func<int, int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) =>
            TwoPositionsMove(SecondStageIsFirstMoveValid, SecondStageIsSecondMoveValid, OnValid, currentDepth, currentPlayer, reverts);

        protected (int bestEvaluation, Move bestMove) OnThirdStageMove(Func<int, int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) =>
            TwoPositionsMove(ThirdStageIsFirstMoveValid, (_, secondPos) => ThirdStageIsSecondMoveValid(secondPos), OnValid, currentDepth, currentPlayer, reverts);

        protected (int bestEvaluation, Move bestMove) OnMillHasBeenArrangedMove(Func<int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) =>
            OnePositionMove(IsMoveValidInMillArrangedState, OnValid, currentDepth, currentPlayer, reverts);

        protected (int bestEvaluation, Move bestMove) OnePositionMove(Func<int, bool> IsMoveValidCondition, Func<int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            Move bestMove = new Move();

            if (currentPlayer == this) {
                int maxEvaluation = int.MinValue;
                int pos;
                int evaluation;
                foreach (Node node in Nodes) {
                    pos = node.Id;
                    if (IsMoveValidCondition(pos)) {
                        evaluation = OnValid(pos);
                        if(evaluation > maxEvaluation) {
                            maxEvaluation = evaluation;
                            bestMove.FirstPos = pos;
                        }
                        Revert(reverts);
                    }
                }

                return (maxEvaluation, bestMove);
            }

            else {
                int minEvaluation = int.MaxValue;
                int pos;
                int evaluation;
                foreach (Node node in Nodes) {
                    pos = node.Id;
                    if (IsMoveValidCondition(pos)) {
                        evaluation = OnValid(pos);
                        if (evaluation < minEvaluation) {
                            minEvaluation = evaluation;
                            bestMove.FirstPos = pos;
                        }
                        Revert(reverts);
                    }
                }

                return (minEvaluation, bestMove);
            }
        }

        protected (int bestEvaluation, Move bestMove) TwoPositionsMove(Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Func<int, int, int> OnValid, int currentDepth, Player currentPlayer, Stack<List<Action>> reverts) {
            Move bestMove = new Move();

            if (currentPlayer == this) {
                int maxEvaluation = int.MinValue;
                int firstPos;
                int secondPos;
                int evaluation;
                foreach (Node firstNode in Nodes) {
                    firstPos = firstNode.Id;
                    if (IsFirstMoveValidCondition(firstPos)) {
                        // TODO: in second stage take only those nodes, that are neighbours to first node.
                        foreach (Node secondNode in Nodes) {
                            secondPos = secondNode.Id;
                            if (IsSecondMoveValid(firstPos, secondPos)) {
                                evaluation = OnValid(firstPos, secondPos);
                                if(evaluation > maxEvaluation) {
                                    maxEvaluation = evaluation;
                                    bestMove.FirstPos = firstPos;
                                    bestMove.SecondPos = secondPos;
                                }
                                Revert(reverts);
                            }
                        }
                    }
                }

                return (maxEvaluation, bestMove);
            }

            else {
                int minEvaluation = int.MaxValue;
                int firstPos;
                int secondPos;
                int evaluation;
                foreach (Node firstNode in Nodes) {
                    firstPos = firstNode.Id;
                    if (IsFirstMoveValidCondition(firstPos)) {
                        foreach (Node secondNode in Nodes) {
                            secondPos = secondNode.Id;
                            if (IsSecondMoveValid(firstPos, secondPos)) {
                                evaluation = OnValid(firstPos, secondPos);
                                if (evaluation < minEvaluation) {
                                    minEvaluation = evaluation;
                                    bestMove.FirstPos = firstPos;
                                    bestMove.SecondPos = secondPos;
                                }
                                Revert(reverts);
                            }
                        }
                    }
                }

                return (minEvaluation, bestMove);
            }
        }

        protected void Revert(Stack<List<Action>> reverts) {
            foreach (Action revert in reverts.Pop()) {
                revert();
            }
        }
    }
}
