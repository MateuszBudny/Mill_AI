﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    public class Move {
        public int FirstPos { get; set; }
        public int SecondPos { get; set; }

        public Move() {
            FirstPos = -1;
            SecondPos = -1;
        }

        public Move(int firstPos) {
            FirstPos = firstPos;
            SecondPos = -1;
        }

        public Move(int firstPos, int secondPos) {
            FirstPos = firstPos;
            SecondPos = secondPos;
        }

        public override string ToString() {
            return "FirstPos: " + FirstPos + ", SecondPos: " + SecondPos;
        }
    }
}
