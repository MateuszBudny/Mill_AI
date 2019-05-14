using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_lista3 {
    class Mill {

        List<Node> nodes;
        List<Row> rows;

        public Mill() {
            InitBoard();
        }

        private void InitBoard() {
            nodes = new List<Node>();
            for(int i = 0; i < 24; i++) {
                nodes.Add(new Node(i));
            }

            // first half
            nodes[0].SetDown(nodes[9]);
            nodes[0].SetRight(nodes[1]);
            nodes[1].SetRight(nodes[2]);
            nodes[1].SetDown(nodes[4]);
            nodes[2].SetDown(nodes[14]);
            nodes[3].SetRight(nodes[4]);
            nodes[3].SetDown(nodes[10]);
            nodes[4].SetRight(nodes[5]);
            nodes[4].SetDown(nodes[7]);
            nodes[5].SetDown(nodes[13]);
            nodes[6].SetRight(nodes[7]);
            nodes[6].SetDown(nodes[11]);
            nodes[7].SetRight(nodes[8]);
            nodes[8].SetDown(nodes[12]);
            nodes[9].SetDown(nodes[21]);
            nodes[9].SetRight(nodes[10]);

            // second half
            nodes[10].SetRight(nodes[11]);
            nodes[10].SetDown(nodes[18]);
            nodes[11].SetDown(nodes[15]);
            nodes[12].SetDown(nodes[17]);
            nodes[12].SetRight(nodes[13]);
            nodes[13].SetDown(nodes[20]);
            nodes[13].SetRight(nodes[14]);
            nodes[14].SetDown(nodes[23]);
            nodes[15].SetRight(nodes[16]);
            nodes[16].SetDown(nodes[19]);
            nodes[16].SetRight(nodes[17]);
            nodes[18].SetRight(nodes[19]);
            nodes[19].SetRight(nodes[20]);
            nodes[19].SetDown(nodes[22]);
            nodes[21].SetRight(nodes[22]);
            nodes[22].SetRight(nodes[23]);

            nodes[17].State = NodeState.Black;
            nodes[19].State = NodeState.White;
        }

        public void Print() {
            int i = 0;

            GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write("\n");
            Console.Write("|        ||        |\n");
            Console.Write("|  "); GetNextNodeToString(ref i); Console.Write(" -- "); GetNextNodeToString(ref i); Console.Write(" -- "); GetNextNodeToString(ref i); Console.Write("  |\n");
            Console.Write("|  |     ||     |  |\n");
            Console.Write("|  |  "); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("  |  |\n");
            Console.Write("|  |  |      |  |  |\n");
            GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("    "); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("\n");
            Console.Write("|  |  |      |  |  |\n");
            Console.Write("|  |  "); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("-"); GetNextNodeToString(ref i); Console.Write("  |  |\n");
            Console.Write("|  |     ||     |  |\n");
            Console.Write("|  "); GetNextNodeToString(ref i); Console.Write(" -- "); GetNextNodeToString(ref i); Console.Write(" -- "); GetNextNodeToString(ref i); Console.Write("  |\n");
            Console.Write("|        ||        |\n");
            GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write("\n");
        }

        private void GetNextNodeToString(ref int i) {
            Console.ForegroundColor = nodes[i].GetFontColor();
            Console.Write(nodes[i++].ToString());
            Console.ResetColor();
        }
    }
}
