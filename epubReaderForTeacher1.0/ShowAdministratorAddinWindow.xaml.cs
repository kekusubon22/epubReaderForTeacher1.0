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

namespace epubReader4._0_Dino
{
    /// <summary>
    /// ShowAdministratorAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowAdministratorAddinWindow : Window
    {
        public ShowAdministratorAddinWindow()
        {
            InitializeComponent();
        }

        //管理者が追加した教材の一覧を表示するWindow
        string addinDirectory;
        User user;

        //初期処理
        public void init(string addinDirectory, User user)
        {
            this.addinDirectory = addinDirectory;
            this.user = user;

            //ボタンを生成
            Button[] btn = new Button[1024];

            int j = 0; //グリッドの列要素の位置
            int k = 0; //グリッドの行要素の位置

            //Administratorディレクトリにファイルがいくつ保存されているか調べる
            string searchKeyword = "*";
            string[] files = System.IO.Directory.GetFiles(addinDirectory + "\\Administrator", searchKeyword, System.IO.SearchOption.TopDirectoryOnly);

            int i = 0;
            foreach (string f in files)
            {

                btn[i] = new Button() { Content = f };

                if (f.Contains(".png") || f.Contains(".jpg") || f.Contains(".bmp"))
                {
                    try
                    {
                        btn[i].Background = new ImageBrush(new BitmapImage(new Uri(f, UriKind.Relative)));
                    }
                    catch
                    {
                        btn[i].Background = new SolidColorBrush(Color.FromArgb(255, 200, 240, 190));
                    }
                }
                else
                {
                    btn[i].Background = new SolidColorBrush(Color.FromArgb(255, 200, 240, 190));
                }

                if (j < 5)
                {
                    ColumnDefinition cd1 = new ColumnDefinition() { Width = new GridLength(200) };
                    grid1.ColumnDefinitions.Add(cd1);
                    j++;
                }
                else
                {
                    RowDefinition rd1 = new RowDefinition() { Height = new GridLength(200) };
                    grid1.RowDefinitions.Add(rd1);
                    j = 0;
                    k++;
                }
                btn[i].Content = string.Format("{0}." + f.Replace(addinDirectory + "\\Administrator\\", ""), i + 1);
                Grid.SetColumn(btn[i], j-1);
                Grid.SetRow(btn[i], k);
                grid1.Children.Add(btn[i]);
                btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                btn[i].Width = double.NaN;  //Autoという意味
                btn[i].Height = double.NaN; //Autoという意味

                btn[i].Click += new RoutedEventHandler(btn_Click);
                i++;
            }
        }

        //それぞれのボタンを押したときの処理
        public void btn_Click(object sender, RoutedEventArgs e)
        {
            //senderからクリックしたファイル名を取得
            string fileName = sender.ToString();
            fileName = fileName.Replace("System.Windows.Controls.Button: ", "");
            int x = fileName.IndexOf(".");
            fileName = fileName.Remove(0, x + 1);

            //AddinWinndowで表示する
            AddinWindow aw = new AddinWindow();
            aw.Show();
            aw.init("administrator", addinDirectory + "\\Administrator" + fileName);

            this.Close();
        }
    }
}