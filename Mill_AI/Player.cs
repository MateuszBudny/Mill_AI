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

    public class Player {
        public int PawnsInHandNum { get; set; }
        public int PawnsOnBoard { get; set; }
        public GameState State { get; set; }
        public GameState LastState { get; set; }
        protected MillBoard MillBoard => GameOfMill.Instance.Board; 
        protected List<Node> Nodes => MillBoard.Nodes;
        public Player Enemy { get; set; }
        public bool IsWhite { get; set; }

        public Player(bool isWhite) {
            PawnsInHandNum = 9;
            PawnsOnBoard = 0;
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

        protected void OnFirstStageMove(string command, Action<int> OnValid) => OnePositionMove(command, false,
            FirstStageIsMoveValid, OnValid);

        protected void OnSecondStageMove(Action<int, int> OnValid) => TwoPositionsMove(2, SecondStageIsFirstMoveValid,
            SecondStageIsSecondMoveValid, OnValid);

        protected void OnThirdStageMove(Action<int, int> OnValid) => TwoPositionsMove(3, ThirdStageIsFirstMoveValid, (_, secondPos) => ThirdStageIsSecondMoveValid(secondPos), OnValid);

        protected void OnMillHasBeenArrangedMove(string command, Action<int> OnValid) => OnePositionMove(command, true, IsMoveValidInMillArrangedState, OnValid);

        protected virtual void OnePositionMove(string command, bool printBoard, Func<int, bool> IsMoveValidCondition, Action<int> OnValid) {
            throw new NotImplementedException();
        }

        protected virtual void TwoPositionsMove(int stageNum, Func<int, bool> IsFirstMoveValidCondition, Func<int, int, bool> IsSecondMoveValid, Action<int, int> OnValid) {
            throw new NotImplementedException();
        }

        private bool IsMoveValidInMillArrangedState(int pos) => !IsNodeEmpty(pos) && !IsMoveValidNodeIsYourColor(pos) && !IsStayedMill(pos);

        private bool FirstStageIsMoveValid(int pos) => IsMoveValidToEmptyNode(pos);

        private bool SecondStageIsFirstMoveValid(int firstPos) => IsMoveValidNodeIsYourColor(firstPos) && HasNodeAnyEmptyNeighbours(firstPos);

        private bool SecondStageIsSecondMoveValid(int firstPos, int secondPos) => IsMoveValidStartAndEndPosSecondStage(firstPos, secondPos);

        private bool ThirdStageIsFirstMoveValid(int firstPos) => IsMoveValidNodeIsYourColor(firstPos);

        private bool ThirdStageIsSecondMoveValid(int secondPos) => IsMoveValidToEmptyNode(secondPos);

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

        protected bool IsNewMill(int pos) {
            return Nodes[pos].IsNewMill();
        }

        private bool IsStayedMill(int pos) {
            return Nodes[pos].IsStayedMill();
        }

        protected void KillEnemysPawn(int pos) {
            Nodes[pos].SetEmpty();
            Enemy.PawnsOnBoard--;
        }

        protected void ChangeEnemyToThirdStageIfPossible() {
            if(Enemy.PawnsInHandNum == 0 && Enemy.PawnsOnBoard == 3) {
                Enemy.ChangeGameState(GameState.ThirdStage);
            }
        }
    }
}
