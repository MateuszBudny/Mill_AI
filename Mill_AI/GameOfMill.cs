using System;

namespace Mill_AI
{
    public class GameOfMill
    {
        public static string INCORRENT_INPUT_MESSAGE = "Incorrect input! Press ENTER to try again...";

        public MillBoard Board { get; set; }
        public Player FirstPlayer { get; set; }
        public Player SecondPlayer { get; set; }
        public Player CurrentPlayer { get; set; }

        private GameOfMill() { }

        public void RunMainMenu()
        {
            Console.Write(@"Game mode:
[1] Human vs. human
[2] Human (white/cyan) vs. AI (black/red)
[3] AI (white/cyan) vs. human (black/red)
[4] AI vs. AI

> ");
            string mode = Console.ReadLine();
            switch(mode)
            {
                case "1":
                    StartGame(new HumanPlayer(true), new HumanPlayer(false));
                    break;
                case "2":
                    StartGame(new HumanPlayer(true), ChooseAIPlayer(false));
                    break;
                case "3":
                    StartGame(ChooseAIPlayer(true), new HumanPlayer(false));
                    break;
                case "4":
                    StartGame(ChooseAIPlayer(true), ChooseAIPlayer(false));
                    break;
                default:
                    Console.WriteLine(INCORRENT_INPUT_MESSAGE);
                    Console.ReadLine();
                    RunMainMenu();
                    return;
            }
        }

        private AIPlayer ChooseAIPlayer(bool isFirstPlayer)
        {
            string aiPlayerName = isFirstPlayer ? "first" : "second";
            Console.Write($"\nChoose {aiPlayerName} Player AI type:" + @"
[1] Minimax AI
[2] Minimax Deepening AI

> ");
            string chosenType = Console.ReadLine();
            AIPlayer aiPlayer;
            switch(chosenType)
            {
                case "1":
                    aiPlayer = new MinimaxAI(isFirstPlayer);
                    break;

                case "2":
                    aiPlayer = new MinimaxDeepeningAI(isFirstPlayer);
                    break;

                default:
                    Console.WriteLine(INCORRENT_INPUT_MESSAGE);
                    Console.ReadLine();
                    return ChooseAIPlayer(isFirstPlayer);
            }

            aiPlayer.InputParameters();

            return aiPlayer;
        }

        private void StartGame(Player firstPlayer, Player secondPlayer)
        {
            Board = new MillBoard();

            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;

            FirstPlayer.Enemy = SecondPlayer;
            SecondPlayer.Enemy = FirstPlayer;

            CurrentPlayer = FirstPlayer;

            GameLoop();
        }

        private void GameLoop()
        {
            while(!IsGameOver(CurrentPlayer))
            {
                Board.Print();
                PrintUI();
                CurrentPlayer.Move();

                CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
            }

            GameOver();
        }

        private void GameOver()
        {
            Board.Print();

            if(!IsDraw())
            {
                Console.WriteLine("Game over! Winner: " + (CurrentPlayer.Enemy.IsWhite ? "WHITE!" : "BLACK!"));
            }
            else
            {
                Console.WriteLine("Draw!");
            }
        }

        public bool IsGameOver(Player player) => HasPlayerLost(player) || IsDraw();

        public bool HasPlayerLost(Player player)
        {
            return player.PawnsInHandNum == 0 && player.PawnsOnBoardNum == 2;
        }

        public bool IsDraw()
        {
            return FirstPlayer.PawnsInHandNum + FirstPlayer.PawnsOnBoardNum == 3 &&
                SecondPlayer.PawnsInHandNum + SecondPlayer.PawnsOnBoardNum == 3;
        }

        public string GetNameOfStage(GameState gameState)
        {
            switch(gameState)
            {
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

        private void PrintUI()
        {
            Console.WriteLine("It's " + (CurrentPlayer.IsWhite ? "WHITE's" : "BLACK's") + " turn!");
            Console.WriteLine(GetNameOfStage(CurrentPlayer.State));
            Console.WriteLine("Pawns in hands: " + CurrentPlayer.PawnsInHandNum);
            Console.WriteLine("Pawns on board: " + CurrentPlayer.PawnsOnBoardNum);
            Console.WriteLine("Enemy's pawns in hands: " + CurrentPlayer.Enemy.PawnsInHandNum);
            Console.WriteLine("Enemy's pawns on board: " + CurrentPlayer.Enemy.PawnsOnBoardNum);
        }

        private static GameOfMill instance;

        public static GameOfMill Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GameOfMill();
                }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }
    }
}
