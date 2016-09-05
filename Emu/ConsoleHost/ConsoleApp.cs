using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80;

namespace ConsoleHost
{
    /// <summary>
    /// A Host with no input and no output, just to test the emulation on Mac OS X
    /// where WPF does not work!
    /// </summary>
    class ConsoleApp: Host
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Emu starting");
            ConsoleApp console = new ConsoleApp();
            Machine machine = new Machine(console);
            machine.Startup();
            machine.Run();
        }

        public object getInput()
        {
            return new object();
        }

        public void updateScreen(object bitmap)
        {
            
        }
    }
}
