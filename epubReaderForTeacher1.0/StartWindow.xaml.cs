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
using System.Windows.Shapes;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// StartWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            ContentRendered += (s, e) => { init(); };
        }

        private void init()
        {
            System.Threading.Thread.Sleep(5000);
            SelectEpubWindow dialog = new SelectEpubWindow();
            dialog.Show();
            this.Owner = dialog;
            this.Close();
        }
    }
}
