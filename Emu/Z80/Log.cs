using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Log
    {
        #region Testing
        /// <summary>
        /// Log of register states in human-readable format.
        /// </summary>
        string StateLog = "";

        /// <summary>
        /// Number of instructions processed so far.
        /// </summary>
        int InstructionCount = 0;

        /// <summary>
        /// To be called every cycle when debugging. Compares current state against expected state.
        /// </summary>
        internal void Write(string currentState)
        {
            StateLog += currentState + "\n";

            if (InstructionCount < ExpectedStates.Length)
            {
                string expectedState = ExpectedStates[InstructionCount];
                bool stateOk = String.Equals(expectedState, currentState);
                if (!stateOk)
                {
                    throw new Exception() { Source = "Reached unexpected state!" };
                }
            }

            InstructionCount++;
        }

        /// <summary>
        /// Sequence of expected register states on bootup for unit testing.
        /// </summary>
        public string[] ExpectedStates = @"
AF=0000 BC=0000 DE=0000 HL=0000 SP=0000 PC=0000
AF=0000 BC=0000 DE=0000 HL=0000 SP=FFFE PC=0003
AF=00F0 BC=0000 DE=0000 HL=0000 SP=FFFE PC=0004
AF=00F0 BC=0000 DE=0000 HL=9FFF SP=FFFE PC=0007
AF=00F0 BC=0000 DE=0000 HL=9FFE SP=FFFE PC=0008  
".Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        #endregion
    }
}
