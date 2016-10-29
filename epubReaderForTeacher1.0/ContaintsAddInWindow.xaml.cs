using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WebKit;
using WebKit.JSCore;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// ContaintsAddInWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ContaintsAddInWindow : Window
    {
        public ContaintsAddInWindow()
        {
            InitializeComponent();
        }

        //WebKitのインスタンス
        public WebKitBrowser webBrowser1;

        public void init(string addinFilePath, string addinFilesDirectory)
        {
            //WebKitのインスタンスを作成する
            webBrowser1 = new WebKitBrowser();

            //WebKitのインスタンスをWindowsFormsHostに割り当てる
            windowsFormsHost1.Child = webBrowser1;

            //webBrowserに1ページ目から読み込ませる
            webBrowser1.Url = new Uri(addinFilePath);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
