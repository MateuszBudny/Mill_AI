using System;
using System.Collections.Generic;
using static Mill_AI.Tools;

namespace Mill_AI
{
    public enum NodeState
    {
        NotFilled,
        Black,
        White
    }

    public class Node
    {
        public int Id { get; set; }
        public NodeState State { get; set; }
        public Row HorizontalRow { get; set; }
        public Row VerticalRow { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Up { get; set; }
        public Node Down { get; set; }

        public Node(Node left, Node right, Node up, Node down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
            State = NodeState.NotFilled;
        }

        public Node(int id)
        {
            Id = id;
            Left = null;
            Right = null;
            Up = null;
            Down = null;
            State = NodeState.NotFilled;
        }

        public void SetLeft(Node node)
        {
            Left = node;
            node.Right = this;
        }

        public void SetRight(Node node)
        {
            Right = node;
            node.Left = this;
        }

        public void SetUp(Node node)
        {
            Up = node;
            node.Down = this;
        }

        public void SetDown(Node node)
        {
            Down = node;
            node.Up = this;
        }

        public void SetColor(bool isWhite)
        {
            State = isWhite ? NodeState.White : NodeState.Black;
        }

        public void SetEmpty()
        {
            State = NodeState.NotFilled;
            CheckIfMillCrashed();
        }

        public bool IsNodeNeighbour(Node node)
        {
            return Left == node || Right == node || Up == node || Down == node;
        }

        public bool HasAnyEmptyNeighbours()
        {
            return Left?.State == NodeState.NotFilled || Right?.State == NodeState.NotFilled ||
                Up?.State == NodeState.NotFilled || Down?.State == NodeState.NotFilled;
        }

        public bool IsNewMill()
        {
            return HorizontalRow.IsNewMill() || VerticalRow.IsNewMill();
        }

        public bool IsStayedMill()
        {
            return HorizontalRow.IsStayedMill || VerticalRow.IsStayedMill;
        }

        private void CheckIfMillCrashed()
        {
            HorizontalRow.CheckIfMillCrashed();
            VerticalRow.CheckIfMillCrashed();
        }

        public override string ToString()
        {
            return IntToStringWithZeros(Id);
        }

        public ConsoleColor GetFontColor()
        {
            switch(State)
            {
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

        public List<Node> GetNeighbours()
        {
            List<Node> neighbours = new List<Node>();

            if(Left != null)
            {
                neighbours.Add(Left);
            }
            if(Right != null)
            {
                neighbours.Add(Right);
            }
            if(Up != null)
            {
                neighbours.Add(Up);
            }
            if(Down != null)
            {
                neighbours.Add(Down);
            }

            return neighbours;
        }
    }
}
