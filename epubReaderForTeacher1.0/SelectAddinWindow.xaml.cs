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

namespace epubReaderForTeacher1._0
{
    /// <summary>
    /// SelectAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectAddinWindow : Window
    {
        public SelectAddinWindow()
        {
            InitializeComponent();
        }

        //管理者・児童のいずれかが追加した教材の一覧を表示するウインドウ
        string addinDirectory;
        string epubFileName;
        string nextDirectoryName;
        string target;
        User user;

        //初期処理
        public void init(string addinDirectory, string epubFileName, string nextDirectoryName, string target, User user)
        {
            this.addinDirectory = addinDirectory;
            this.epubFileName = epubFileName;
            this.nextDirectoryName = nextDirectoryName;
            this.target = target;
            this.user = user;

            //targetからどのディレクトリを調べるか判断する
            string searchKeyword;
            if( target.Equals("administrator") )
            {
                searchKeyword = "\\Administrator\\" + epubFileName.Replace(".epub", "") + "\\" + nextDirectoryName;
            }
            else
            {
                searchKeyword = "\\Student\\";
            }

            //ボタンを生成
            Button[] btn = new Button[1024];

            int j = 0; //グリッドの列要素の位置
            int k = 0; //グリッドの行要素の位置


            string[] files;
            try
            {
                //対象とするディレクトリにファイルがいくつ保存されているか調べる
                files = System.IO.Directory.GetFiles(addinDirectory + searchKeyword, "*", System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                MessageBox.Show("ファイルがありません。");
                return;
            }

            int i = 0;
            foreach (string f in files)
            {
                //targetがstudentのとき、表示させたいディレクトリ以外のものは除外する
                if(target.Equals("student") && !f.Contains( epubFileName.Replace(".epub", "") + "\\" + nextDirectoryName ))
                {
                    continue;
                }

                btn[i] = new Button() { Content = f };

                //追加教材が画像ならボタンの背景に設定
                if (f.Contains(".png") || f.Contains(".jpg") || f.Contains(".bmp") || f.Contains(".PNG") || f.Contains(".JPG") || f.Contains(".BMP"))
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

                //5個ごとに改行
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

                //targetによってボタンのテキストを変更
                if (target.Equals("administrator"))
                {
                    btn[i].Content = string.Format("{0}." + f.Replace(addinDirectory + "\\Administrator\\" + epubFileName.Replace(".epub", "") + "\\" + nextDirectoryName + "\\", ""), i + 1);
                }
                else
                {
                    btn[i].Content = string.Format("{0}." + f.Replace(addinDirectory + "\\Student\\", ""), i + 1);
                }

                Grid.SetColumn(btn[i], j - 1);
                Grid.SetRow(btn[i], k);
                grid1.Children.Add(btn[i]);
                btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                btn[i].Width = double.NaN;  //Autoという意味
                btn[i].Height = double.NaN; //Autoという意味

                btn[i].Click += new RoutedEventHandler(btn_Click);
                i++;
            }

            //該当するファイルが一つもなかったらメッセージを出す
            if (i == 0)
            {
                MessageBox.Show("ファイルがありません。");
                return;
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

            //targetによって開くパスが違う
            if (target.Equals("administrator"))
            {
                aw.init("administrator", addinDirectory + "\\Administrator\\" + epubFileName.Replace(".epub", "") + "\\" + nextDirectoryName + "\\" + fileName);
            }
            else
            {
                aw.init("administrator", addinDirectory + "\\Student\\" + fileName);
            }

            this.Close();
        }
    }
}