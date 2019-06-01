using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class Move {
        public int FirstPos { get; set; }
        public int SecondPos { get; set; }

        public Move() {
            FirstPos = -1;
            SecondPos = -1;
        }

        public override string ToString() {
            return "FirstPos: " + FirstPos + ", SecondPos: " + SecondPos;
        }
    }
}
