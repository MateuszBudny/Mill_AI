using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public class GameOfMill {

        public MillBoard Board { get; set; }
        public Player FirstPlayer { get; set; }
        public Player SecondPlayer { get; set; }
        public Player CurrentPlayer { get; set; }

        private GameOfMill() {}

        public void Run() {
            Console.Write(@"Game mode:
[1] Human vs. human
[2] Human (white/cyan) vs. AI (black/red)
[3] AI (white/cyan) vs. human (black/red)
[4] AI vs. AI

> ");
            string mode = Console.ReadLine();
            switch (mode) {
                case "1":
                    StartGame(true, true);
                    break;
                case "2":
                    StartGame(true, false);
                    break;
                case "3":
                    StartGame(false, true);
                    break;
                case "4":
                    StartGame(false, false);
                    break;
            } 
        }

        private void StartGame(bool isFirstPlayerHuman, bool isSecondPlayerHuman) {
            Board = new MillBoard();

            if (isFirstPlayerHuman) {
                FirstPlayer = new HumanPlayer(true);
            } else {
                FirstPlayer = new MinimaxAI(true, 4);
            }

            if (isSecondPlayerHuman) {
                SecondPlayer = new HumanPlayer(false);
            } else {
                SecondPlayer = new MinimaxAlphaBetaAI(false, 6);
            }

            FirstPlayer.Enemy = SecondPlayer;
            SecondPlayer.Enemy = FirstPlayer;

            CurrentPlayer = FirstPlayer;

            while (!IsGameOver(CurrentPlayer)) {
                Board.Print();
                Console.WriteLine("It's " + (CurrentPlayer.IsWhite ? "WHITE's" : "BLACK's") + " turn!");
                Console.WriteLine(GetNameOfStage(CurrentPlayer.State));
                Console.WriteLine("Pawns in hands: " + CurrentPlayer.PawnsInHandNum);
                Console.WriteLine("Pawns on board: " + CurrentPlayer.PawnsOnBoardNum);
                Console.WriteLine("Enemy's pawns in hands: " + CurrentPlayer.Enemy.PawnsInHandNum);
                Console.WriteLine("Enemy's pawns on board: " + CurrentPlayer.Enemy.PawnsOnBoardNum);
                CurrentPlayer.Move();

                CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
            }

            Board.Print();
            if (!IsDraw()) {
                Console.WriteLine("Game over! Winner: " + (CurrentPlayer.Enemy.IsWhite ? "WHITE!" : "BLACK!"));
            } else {
                Console.WriteLine("Draw!");
            }
        }

        public bool IsGameOver(Player player) => HasPlayerLost(player) || IsDraw();

        public bool HasPlayerLost(Player player) {
            return player.PawnsInHandNum == 0 && player.PawnsOnBoardNum == 2;
        }

        public bool IsDraw() {
            return FirstPlayer.PawnsInHandNum + FirstPlayer.PawnsOnBoardNum == 3 &&
                SecondPlayer.PawnsInHandNum + SecondPlayer.PawnsOnBoardNum == 3;
        }

        private string GetNameOfStage(GameState gameState) {
            switch(gameState) {
                case GameState.FirstStage:
                    return "STAGE 1";
                case GameState.SecondStage:
                    return "STAGE 2";
                case GameState.ThirdStage:
                    return "STAGE 3";
                case GameState.MillHasBeenArranged:
                    return "MILL HAS BEEN ARRANGED!";
                default:
                    return "UKNOWN STAGE";
            }
        }

        private static GameOfMill instance;

        public static GameOfMill Instance {
            get {
                if(instance == null) {
                    instance = new GameOfMill();
                }

                return instance;
            }

            private set {
                instance = value;
            }
        }
    }
}
