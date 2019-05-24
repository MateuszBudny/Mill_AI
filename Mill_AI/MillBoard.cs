using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public class MillBoard {

        public List<Node> Nodes { get; set; }
        public List<Row> Rows { get; set; }

        public MillBoard() {
            InitBoard();
        }

        private void InitBoard() {
            Nodes = new List<Node>();
            for(int i = 0; i < 24; i++) {
                Nodes.Add(new Node(i));
            }

            // first half
            Nodes[0].SetDown(Nodes[9]);
            Nodes[0].SetRight(Nodes[1]);
            Nodes[1].SetRight(Nodes[2]);
            Nodes[1].SetDown(Nodes[4]);
            Nodes[2].SetDown(Nodes[14]);
            Nodes[3].SetRight(Nodes[4]);
            Nodes[3].SetDown(Nodes[10]);
            Nodes[4].SetRight(Nodes[5]);
            Nodes[4].SetDown(Nodes[7]);
            Nodes[5].SetDown(Nodes[13]);
            Nodes[6].SetRight(Nodes[7]);
            Nodes[6].SetDown(Nodes[11]);
            Nodes[7].SetRight(Nodes[8]);
            Nodes[8].SetDown(Nodes[12]);
            Nodes[9].SetDown(Nodes[21]);
            Nodes[9].SetRight(Nodes[10]);

            // second half
            Nodes[10].SetRight(Nodes[11]);
            Nodes[10].SetDown(Nodes[18]);
            Nodes[11].SetDown(Nodes[15]);
            Nodes[12].SetDown(Nodes[17]);
            Nodes[12].SetRight(Nodes[13]);
            Nodes[13].SetDown(Nodes[20]);
            Nodes[13].SetRight(Nodes[14]);
            Nodes[14].SetDown(Nodes[23]);
            Nodes[15].SetRight(Nodes[16]);
            Nodes[16].SetDown(Nodes[19]);
            Nodes[16].SetRight(Nodes[17]);
            Nodes[18].SetRight(Nodes[19]);
            Nodes[19].SetRight(Nodes[20]);
            Nodes[19].SetDown(Nodes[22]);
            Nodes[21].SetRight(Nodes[22]);
            Nodes[22].SetRight(Nodes[23]);

            // rows
            Rows = new List<Row> {

                // horizontal rows
                new Row(Nodes[0], Nodes[1], Nodes[2], true),
                new Row(Nodes[3], Nodes[4], Nodes[5], true),
                new Row(Nodes[6], Nodes[7], Nodes[8], true),
                new Row(Nodes[9], Nodes[10], Nodes[11], true),
                new Row(Nodes[12], Nodes[13], Nodes[14], true),
                new Row(Nodes[15], Nodes[16], Nodes[17], true),
                new Row(Nodes[18], Nodes[19], Nodes[20], true),
                new Row(Nodes[21], Nodes[22], Nodes[23], true),

                // vertical rows
                new Row(Nodes[0], Nodes[9], Nodes[21], false),
                new Row(Nodes[3], Nodes[10], Nodes[18], false),
                new Row(Nodes[6], Nodes[11], Nodes[15], false),
                new Row(Nodes[1], Nodes[4], Nodes[7], false),
                new Row(Nodes[16], Nodes[19], Nodes[22], false),
                new Row(Nodes[8], Nodes[12], Nodes[17], false),
                new Row(Nodes[5], Nodes[13], Nodes[20], false),
                new Row(Nodes[2], Nodes[14], Nodes[23], false)
            };

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
            GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write(" ----- "); GetNextNodeToString(ref i); Console.Write("\n\n\n");
        }

        private void GetNextNodeToString(ref int i) {
            Console.ForegroundColor = Nodes[i].GetFontColor();
            Console.Write(Nodes[i++].ToString());
            Console.ResetColor();
        }
    }
}
