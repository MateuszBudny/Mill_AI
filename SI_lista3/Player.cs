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

        protected bool IsMoveValidToEmptyNode(int pos) {
            return IsPosInRange(pos) && IsNodeEmpty(pos);
        }

        protected bool IsMoveValidStartAndEndPosSecondStage(int startPos, int endPos) {
            return IsPosInRange(endPos) && IsNodeEmpty(endPos) && AreNodesNeighbours(startPos, endPos);
        }

        protected bool IsMoveValidNodeIsYourColor(int pos) {
            return IsPosInRange(pos) && IsColorEqual(pos);
        }

        protected bool IsKillingValid(int pos) {
            return IsPosInRange(pos) && !IsColorEqual(pos);
        }

        protected bool IsColorEqual(int pos) {
            if(IsWhite) {
                return Nodes[pos].State == NodeState.White;
            } else {
                return Nodes[pos].State == NodeState.Black;
            }
        }

        protected bool IsNodeEmpty(int pos) {
            return Nodes[pos].State == NodeState.NotFilled;
        }

        protected bool HasNodeAnyEmptyNeighbours(int pos) {
            return Nodes[pos].HasAnyEmptyNeighbours();
        }

        protected bool IsPosInRange(int pos) {
            return pos >= 0 && pos <= 23;
        }

        protected bool AreNodesNeighbours(int pos1, int pos2) {
            return Nodes[pos1].IsNodeNeighbour(Nodes[pos2]);
        }

        protected bool IsNewMill(int pos) {
            return Nodes[pos].IsNewMill();
        }

        protected bool IsStayedMill(int pos) {
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

        protected bool IsMoveValidInMillArrangedState(int pos) => !IsNodeEmpty(pos) && !IsMoveValidNodeIsYourColor(pos) && !IsStayedMill(pos);

        //protected Player GetEnemy() {
        //    if (GameOfMill.FirstPlayer == this) {
        //        return GameOfMill.SecondPlayer;
        //    }

        //    return GameOfMill.FirstPlayer;
        //}
    }
}
