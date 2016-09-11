using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Log
    {
        /// <summary>
        /// Log of register states in human-readable format.
        /// </summary>
        string StateLog = "";

        /// <summary>
        /// Number of instructions processed so far.
        /// </summary>
        int InstructionCount = 0;

        /// <summary>
        /// If a sequence of N expected states is provided, it is used to verify correctness of the Registers for the first N instructions processed.
        /// </summary>
        public string[] ExpectedStates { get; set; }

        /// <summary>
        /// To be called every cycle when debugging. Compares current state against expected state.
        /// </summary>
        internal void Update(string currentState)
        {
            // Update the log.
            StateLog += currentState + "\n";

            // Verify correctness if an ExpectedState is provided for the Nth instruction processed.
            if (ExpectedStates != null && InstructionCount < ExpectedStates.Length)
            {
                string expectedState = ExpectedStates[InstructionCount];

                bool stateOk = String.Equals(expectedState, currentState);

                if (!stateOk)
                {
                    // If we reach here, we likely have a bug in our code.
                    throw new Exception() { Source = "Reached unexpected state!" };
                }
            }

            InstructionCount++;
        }
    }

    /// <summary>
    /// Represents the "Correct" sequence of Register States after each instruction is processed according to the bgb debugger (or whichever other reputable source).
    /// </summary>
    public class ExpectedStates
    {
        public static string[] Tetris = @"
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0100
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0101
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0150
AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=028B
AF=0080 BC=0013 DE=00D8 HL=014D SP=FFFE PC=028C
AF=0080 BC=0013 DE=00D8 HL=DFFF SP=FFFE PC=028F
".Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}
