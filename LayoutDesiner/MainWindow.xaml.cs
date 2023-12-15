using System.Windows;
using ZmLib.Util.Config;
using ZmLib.Util.Logging;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LogConfig.InitLogger(
                    new TEXT_LOGGER(
                        "C:\\Users\\parksungjun\\source\\repos\\MapEditor\\log",
                        "Test"
                        )
                    );
            InitializeComponent();
        }
    }
}
