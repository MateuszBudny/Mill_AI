using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_lista3 {
    public class GameOfMill {

        public MillBoard Board { get; set; }
        public Player FirstPlayer { get; set; }
        public Player SecondPlayer { get; set; }
        public Player CurrentPlayer { get; set; }

        private GameOfMill() {}

        public void Run() {
            Console.Write(@"Tryb gry:
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

            if(isFirstPlayerHuman) {
                FirstPlayer = new HumanPlayer(true);
            } else {
                FirstPlayer = new AIPlayer(true);
            }

            if(isSecondPlayerHuman) {
                SecondPlayer = new HumanPlayer(false);
            } else {
                SecondPlayer = new AIPlayer(false);
            }

            FirstPlayer.Enemy = SecondPlayer;
            SecondPlayer.Enemy = FirstPlayer;

            CurrentPlayer = FirstPlayer;

            while(!HasPlayerLost(CurrentPlayer)) {
                Board.Print();
                Console.WriteLine("Now it's " + (CurrentPlayer.IsWhite ? "WHITE's" : "BLACK's") + " turn!");
                CurrentPlayer.Move();

                CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
            }

            Console.WriteLine("Game over! Winner: " + (CurrentPlayer.Enemy.IsWhite ? "WHITE!" : "BLACK!"));
        }

        private bool HasPlayerLost(Player player) {
            return player.PawnsInHandNum == 0 && player.PawnsOnBoard == 2;
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
