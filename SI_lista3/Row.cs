using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_lista3 {
    class Row {
        public List<Node> Nodes { get; set; }

        public Row(Node node1, Node node2, Node node3, bool isHorizontal) {
            Nodes = new List<Node> {
                node1,
                node2,
                node3
            };

            if(isHorizontal) {
                node1.HorizontalRow = this;
                node2.HorizontalRow = this;
                node3.HorizontalRow = this;
            } else {
                node1.VerticalRow = this;
                node2.VerticalRow = this;
                node3.VerticalRow = this;
            }
        }

        public bool IsMill() {
            return Nodes.All(n => n.State == NodeState.White) || Nodes.All(n => n.State == NodeState.Black);
        }
    }
}
