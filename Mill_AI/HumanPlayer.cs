using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class HumanPlayer : Player {

        // TODO: repeating the same move set as forbidden
        // TODO (?): double mills

        const bool AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES = true;

        public HumanPlayer(bool isWhite) : base(isWhite) {}

        public override void Move() {
            switch (State) {
                case GameState.FirstStage:

                    OnFirstStageMove("Where should your pawn land? (left pawns: " + PawnsInHandNum + ")> ",
                        (firstPos) => OnValidFirstStageMove(firstPos));

                    break;

                case GameState.SecondStage:

                    OnSecondStageMove((firstPos, secondPos) => OnValidSecondStageMove(firstPos, secondPos));

                    break;

                case GameState.ThirdStage:

                    OnThirdStageMove((firstPos, secondPos) => OnValidThirdStageMove(firstPos, secondPos));

                    break;

                case GameState.MillHasBeenArranged:

                    OnMillHasBeenArrangedMove("Which enemy's pawn do you want to remove from board?> ",
                        (firstPos) => OnValidMillHasBeenArranged(firstPos));

                    break;

                default:
                    throw new Exception("Your GameState is default. Something went wrong ¯\\_(ツ)_/¯ ");
            }
        }

        private void OnFirstStageMove(string command, Action<int> OnValid) => OnePositionMove(command, false,
            FirstStageIsMoveValid, OnValid);

        private void OnSecondStageMove(Action<int, int> OnValid) => TwoPositionsMove(SecondStageIsFirstMoveValid,
            SecondStageIsSecondMoveValid, OnValid);

        private void OnThirdStageMove(Action<int, int> OnValid) => TwoPositionsMove(ThirdStageIsFirstMoveValid, (_, secondPos) => ThirdStageIsSecondMoveValid(secondPos), OnValid);

        private void OnMillHasBeenArrangedMove(string command, Action<int> OnValid) => OnePositionMove(command, true, IsMoveValidInMillArrangedState, OnValid);

        private void OnePositionMove(string command, bool printBoard, Func<int, bool> IsMoveValidCondition, Action<int> OnValid) {
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

        private void TwoPositionsMove(Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Action<int, int> OnValid) {
            int firstPos;
            while (AS_LONG_AS_PLAYER_MAKES_INVALID_MOVES) {
                Console.Write("Which pawn do you want to move?> ");
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
