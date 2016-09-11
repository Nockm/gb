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
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0100
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0101
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0150
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=028B
AF=0080 BC=0013 DE=00D8 HL=014D SP=FFFE PC=028C
AF=0080 BC=0013 DE=00D8 HL=DFFF SP=FFFE PC=028F
".Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        #endregion
    }
}
