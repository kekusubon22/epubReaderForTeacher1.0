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
    /// SelectMyAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectMyAddinWindow : Window
    {
        public SelectMyAddinWindow()
        {
            InitializeComponent();
        }

        //自分のカメラロールから教材を追加するWindow
        string cameraRollDirectory;
        string myAddinDirectory;

        //初期処理
        public void init(string cameraRollDirectory, string myAddinDirectory, User user)
        {
            this.cameraRollDirectory = cameraRollDirectory;
            this.myAddinDirectory = myAddinDirectory;

            //ボタンを生成
            Button[] btn = new Button[1024];

            int j = 0; //グリッドの列要素の位置
            int k = 0; //グリッドの行要素の位置

            //カメラロールディレクトリに画像が何枚保存されているか調べる
            string searchKeyword = "*.jpg";
            string[] files = System.IO.Directory.GetFiles(cameraRollDirectory, searchKeyword, System.IO.SearchOption.TopDirectoryOnly);

            int i = 0;
            foreach (string f in files)
            {

                btn[i] = new Button() { Content = f };
                btn[i].Background = new ImageBrush(new BitmapImage(new Uri(f, UriKind.Relative)));

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
                btn[i].Content = string.Format("{0}." + f.Replace(cameraRollDirectory + "\\",""), i + 1);
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
            string picName = sender.ToString();
            picName = picName.Replace("System.Windows.Controls.Button: ", "");
            int x = picName.IndexOf(".");
            picName = picName.Remove(0, x + 1);

            try
            {
                //アドインのディレクトリに当該ファイルをコピー
                System.IO.File.Copy(cameraRollDirectory + "\\" + picName, myAddinDirectory + "\\" + picName);
                MessageBox.Show(picName + "を追加しました。");

            }

            catch
            {
                MessageBox.Show("既に追加されています。");
            }

            this.Close();
        }
    }
}
