using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Instruction
    {
        public Instruction(int cycles, string disassembly, Action action)
        {
            Cycles = cycles;
            Disassembly = disassembly;
            Execute = action;            
        }

        public int Cycles;
        public String Disassembly;
        public Action Execute;
    }
}
