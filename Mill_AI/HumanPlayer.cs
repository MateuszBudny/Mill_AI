﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class HumanPlayer : Player {

        // here should be repeating the same move set as forbidden also
        // and double mills

        const bool AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES = true;

        public HumanPlayer(bool isWhite) : base(isWhite) {}

        public override void Move() {
            switch (State) {
                case GameState.FirstStage:

                    OnFirstStageMove("STAGE 1\nWhere should your pawn land? (left pawns: " + PawnsInHandNum + ")> ",
                        (pos) => {
                            Nodes[pos].SetColor(IsWhite);

                            PawnsOnBoard++;
                            if (--PawnsInHandNum <= 0) {
                                ChangeGameState(GameState.SecondStage);
                            }

                            if (IsNewMill(pos)) {
                                ChangeGameState(GameState.MillHasBeenArranged);
                                Move();
                            }
                        });

                    break;

                case GameState.SecondStage:

                    OnSecondStageMove((firstPos, secondPos) => {
                            Nodes[firstPos].SetEmpty();
                            Nodes[secondPos].SetColor(IsWhite);

                            if (IsNewMill(secondPos)) {
                                ChangeGameState(GameState.MillHasBeenArranged);
                                Move();
                            }
                        });

                    break;

                case GameState.ThirdStage:

                    OnThirdStageMove((firstPos, secondPos) => {
                            Nodes[firstPos].SetEmpty();
                            Nodes[secondPos].SetColor(IsWhite);

                            if (IsNewMill(secondPos)) {
                                ChangeGameState(GameState.MillHasBeenArranged);
                                Move();
                            }
                        });

                    break;

                case GameState.MillHasBeenArranged:

                    OnMillHasBeenArrangedMove("MILL HAS BEEN ARRANGED\nWhich enemy's pawn do you want to remove from board?> ",
                        (pos) => {
                            KillEnemysPawn(pos);
                            ChangeGameState(LastState);
                            ChangeEnemyToThirdStageIfPossible();
                        });

                    break;

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        protected override void OnePositionMove(string command, bool printBoard, Func<int, bool> IsMoveValidCondition, Action<int> OnValid) {
            if (printBoard) {
                MillBoard.Print();
            }

            while (AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES) {
                Console.Write(command);
                int pos = int.Parse(Console.ReadLine());
                if (IsMoveValidCondition(pos)) {
                    OnValid(pos);
                    return;
                } else {
                    OutputAboutWrongMove();
                }
            }
        }

        protected override void TwoPositionsMove(int stageNum, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Action<int, int> OnValid) {
            int firstPos;
            while (AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES) {
                Console.Write("STAGE " + stageNum + "\nWhich pawn do you want to move?> ");
                firstPos = int.Parse(Console.ReadLine());
                if (IsFirstMoveValidCondition(firstPos)) {
                    break;
                } else {
                    OutputAboutWrongMove();
                }
            }

            while (AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES) {
                Console.Write("Where should the " + firstPos + " pawn land?> ");
                int secondPos = int.Parse(Console.ReadLine());
                if (IsSecondMoveValid(firstPos, secondPos)) {
                    OnValid(firstPos, secondPos);
                    return;
                } else {
                    OutputAboutWrongMove();
                }
            }
        }

        private void OutputAboutWrongMove() {
            Console.WriteLine("Incorrect move!");
        }
    }
}