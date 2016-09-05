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
            ConsoleApp console = new ConsoleApp();
            Machine machine = new Machine(console);
            machine.Startup();
            machine.Run();
        }

        public object GetInput()
        {
            return new object();
        }

        public void UpdateScreen(object bitmap)
        {
            
        }

        public void Startup()
        {
            Console.WriteLine("Emu starting");
        }
    }
}
