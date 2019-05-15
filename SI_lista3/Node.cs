using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SI_lista3.Tools;

namespace SI_lista3 {
    public enum NodeState {
        NotFilled,
        Black,
        White
    }

    class Node {
        public int Id { get; set; }
        public NodeState State { get; set; }
        public Row HorizontalRow { get; set; }
        public Row VerticalRow { get; set; }
        public Node Left { get; set; } 
        public Node Right { get; set; }
        public Node Up { get; set; }
        public Node Down { get; set; }

        public Node(Node left, Node right, Node up, Node down) {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
            State = NodeState.NotFilled;
        }

        public Node(int id) {
            Id = id;
            Left = null;
            Right = null;
            Up = null;
            Down = null;
            State = NodeState.NotFilled;
        }

        public void SetLeft(Node node) {
            Left = node;
            node.Right = this;
        }

        public void SetRight(Node node) {
            Right = node;
            node.Left = this;
        }

        public void SetUp(Node node) {
            Up = node;
            node.Down = this;
        }

        public void SetDown(Node node) {
            Down = node;
            node.Up = this;
        }

        public override string ToString() {
            return IntToStringWithZeros(Id);
        }

        public ConsoleColor GetFontColor() {
            switch (State) {
                case NodeState.NotFilled:
                    return ConsoleColor.White;
                case NodeState.White:
                    return ConsoleColor.Cyan;
                case NodeState.Black:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
