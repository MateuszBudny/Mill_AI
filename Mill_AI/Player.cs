using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public enum GameState {
        FirstStage, // player has pawns in hand and puts them where he wants
        SecondStage, // player has no pawns in hand and moves them on board to adjacent nodes
        ThirdStage, // player has three pawns and can move his pawns where he wants
        MillHasBeenArranged, // player has arranged a mill, so now he kills one of his enemy's pawn 
    }

    public abstract class Player {
        public int PawnsInHandNum { get; set; }
        public int PawnsOnBoardNum { get; set; }
        public GameState State { get; set; }
        public GameState LastState { get; set; }
        protected MillBoard MillBoard => GameOfMill.Instance.Board; 
        protected List<Node> Nodes => MillBoard.Nodes;
        public Player Enemy { get; set; }
        public bool IsWhite { get; set; }

        public Player(bool isWhite) {
            PawnsInHandNum = 9;
            PawnsOnBoardNum = 0;
            State = GameState.FirstStage;
            LastState = GameState.FirstStage;
            IsWhite = isWhite;
        }

        public virtual void Move() {
            throw new NotImplementedException();
        }

        public virtual void ChangeGameState(GameState toState) {
            LastState = State;
            State = toState;
        }

        public bool IsMoveValidInMillArrangedState(int pos) => !IsNodeEmpty(pos) && !IsMoveValidNodeIsYourColor(pos) && !IsStayedMill(pos);

        public bool FirstStageIsMoveValid(int pos) => IsMoveValidToEmptyNode(pos);

        public bool SecondStageIsFirstMoveValid(int firstPos) => IsMoveValidNodeIsYourColor(firstPos) && HasNodeAnyEmptyNeighbours(firstPos);

        public bool SecondStageIsSecondMoveValid(int firstPos, int secondPos) => IsMoveValidStartAndEndPosSecondStage(firstPos, secondPos);

        public bool ThirdStageIsFirstMoveValid(int firstPos) => IsMoveValidNodeIsYourColor(firstPos);

        public bool ThirdStageIsSecondMoveValid(int secondPos) => IsMoveValidToEmptyNode(secondPos);

        protected void OnValidFirstStageMove(int firstPos) {
            Nodes[firstPos].SetColor(IsWhite);

            PawnsOnBoardNum++;
            if (--PawnsInHandNum <= 0) {
                ChangeGameState(GameState.SecondStage);
            }

            if (IsNewMill(firstPos)) {
                ChangeGameState(GameState.MillHasBeenArranged);
                Move();
            }
        }

        protected void OnValidSecondStageMove(int firstPos, int secondPos) {
            Nodes[firstPos].SetEmpty();
            Nodes[secondPos].SetColor(IsWhite);

            if (IsNewMill(secondPos)) {
                ChangeGameState(GameState.MillHasBeenArranged);
                Move();
            }
        }

        protected void OnValidThirdStageMove(int firstPos, int secondPos) {
            Nodes[firstPos].SetEmpty();
            Nodes[secondPos].SetColor(IsWhite);

            if (IsNewMill(secondPos)) {
                ChangeGameState(GameState.MillHasBeenArranged);
                Move();
            }
        }

        protected void OnValidMillHasBeenArranged(int firstPos) {
            KillEnemysPawn(firstPos);
            ChangeGameState(LastState);
            ChangeEnemyToThirdStageIfPossible();
        }

        private bool IsMoveValidToEmptyNode(int pos) {
            return IsPosInRange(pos) && IsNodeEmpty(pos);
        }

        private bool IsMoveValidStartAndEndPosSecondStage(int startPos, int endPos) {
            return IsPosInRange(endPos) && IsNodeEmpty(endPos) && AreNodesNeighbours(startPos, endPos);
        }

        private bool IsMoveValidNodeIsYourColor(int pos) {
            return IsPosInRange(pos) && IsColorEqual(pos);
        }

        private bool IsKillingValid(int pos) {
            return IsPosInRange(pos) && !IsColorEqual(pos);
        }

        private bool IsColorEqual(int pos) {
            if(IsWhite) {
                return Nodes[pos].State == NodeState.White;
            } else {
                return Nodes[pos].State == NodeState.Black;
            }
        }

        private bool IsNodeEmpty(int pos) {
            return Nodes[pos].State == NodeState.NotFilled;
        }

        private bool HasNodeAnyEmptyNeighbours(int pos) {
            return Nodes[pos].HasAnyEmptyNeighbours();
        }

        private bool IsPosInRange(int pos) {
            return pos >= 0 && pos <= 23;
        }

        private bool AreNodesNeighbours(int pos1, int pos2) {
            return Nodes[pos1].IsNodeNeighbour(Nodes[pos2]);
        }

        public bool IsNewMill(int pos) {
            return Nodes[pos].IsNewMill();
        }

        private bool IsStayedMill(int pos) {
            return Nodes[pos].IsStayedMill();
        }

        public void KillEnemysPawn(int pos) {
            Nodes[pos].SetEmpty();
            Enemy.PawnsOnBoardNum--;
        }

        public bool ChangeEnemyToThirdStageIfPossible() {
            if(Enemy.PawnsInHandNum == 0 && Enemy.PawnsOnBoardNum == 3) {
                Enemy.ChangeGameState(GameState.ThirdStage);
                return true;
            }

            return false;
        }
    }
}
