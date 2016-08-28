using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Instruction
    {
        public Instruction(string disassembly, Action action)
        {
            Disassembly = disassembly;
            Execute = action;
        }

        public String Disassembly;
        public Action Execute;
    }
}
