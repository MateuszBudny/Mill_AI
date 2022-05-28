using System.Collections.Generic;
using System.Linq;

namespace Mill_AI
{
    public class Row
    {
        public List<Node> Nodes { get; set; }
        public bool IsStayedMill { get; set; }

        public Row(Node node1, Node node2, Node node3, bool isHorizontal)
        {
            IsStayedMill = false;

            Nodes = new List<Node> {
                node1,
                node2,
                node3
            };

            if(isHorizontal)
            {
                node1.HorizontalRow = this;
                node2.HorizontalRow = this;
                node3.HorizontalRow = this;
            }
            else
            {
                node1.VerticalRow = this;
                node2.VerticalRow = this;
                node3.VerticalRow = this;
            }
        }

        public bool IsNewMill()
        {
            if(IsStayedMill)
            {
                return false;
            }

            IsStayedMill = IsWholeRowInOneColor();
            return IsStayedMill;
        }

        public void CheckIfMillCrashed()
        {
            if(!IsStayedMill)
            {
                return;
            }

            IsStayedMill = IsWholeRowInOneColor();
        }

        private bool IsWholeRowInOneColor()
        {
            return Nodes.All(n => n.State == NodeState.White) || Nodes.All(n => n.State == NodeState.Black);
        }
    }
}
