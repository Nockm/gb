using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Instruction
    {
        public Instruction(string description, Action action)
        {
            Description = description;
            Execute = action;
        }

        public String Description;
        public Action Execute;
    }
}
