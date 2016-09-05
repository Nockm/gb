using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Z80;

namespace Emu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Host
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public object getInput()
        {
            throw new NotImplementedException();
        }

        public void updateScreen(object bitmap)
        {
            throw new NotImplementedException();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Machine machine = new Machine(this);
            machine.Startup();
            machine.Run();
        }
    }
}
