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

using System.IO;

namespace epubReaderForTeacher1._0
{
    /// <summary>
    /// PNGShowAnnotationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGSelectAnnotationWindow : Window
    {
        public PNGSelectAnnotationWindow()
        {
            InitializeComponent();
        }

        //自分のキャプチャ一覧を表示する
        string[] files;
        string[] captureOwnerId;


        //要素ごとかユーザごとか
        bool isElement;

        //初期処理
        public void init(string[] files, string[] captureOwnerId, bool isElement)
        {
            this.files = files;
            this.captureOwnerId = captureOwnerId;
            this.isElement = isElement;

            if (files.Length == 0)
            {
                MessageBox.Show("きろくがありません。");
                this.Close();
            }

            //ボタンを生成
            Button[] btn = new Button[1024];

            //テキストボックスの生成
            TextBox[] txt = new TextBox[1024];

            int j = 0; //グリッドの列要素の位置
            int k = 1; //グリッドの行要素の位置

            int i = 0;
            foreach (string f in files)
            {
                //ボタン生成
                btn[i] = new Button() { Content = f };
                try
                {
                    btn[i].Background = new ImageBrush(new BitmapImage(new Uri(f, UriKind.Relative)));
                }
                catch
                {
                    btn[i].Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 255));
                }

                //テキストボックスの生成
                txt[i] = new TextBox() { Background = new SolidColorBrush( Color.FromArgb(255, 200, 200, 255) ), FontSize = 20, TextAlignment = TextAlignment.Center };
                txt[i].Text = captureOwnerId[i];

                //改行するかしないかの判定
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
                    RowDefinition rd2 = new RowDefinition() { Height = new GridLength(35) };
                    grid1.RowDefinitions.Add(rd2);

                    j = 1;
                    k++;
                    k++;
                }
                btn[i].Content = string.Format("{0}." + f, i + 1);
                Grid.SetColumn(btn[i], j - 1);
                Grid.SetRow(btn[i], k);
                grid1.Children.Add(btn[i]);
                btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                btn[i].Width = double.NaN;  //Autoという意味
                btn[i].Height = double.NaN; //Autoという意味
                btn[i].Click += new RoutedEventHandler(btn_Click);

                Grid.SetColumn(txt[i], j - 1);
                Grid.SetRow(txt[i], k + 1);
                grid1.Children.Add(txt[i]);
                txt[i].VerticalAlignment = VerticalAlignment.Stretch;
                txt[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                txt[i].Width = double.NaN;
                txt[i].Height = double.NaN;

                i++;
            }
        }

        //それぞれのボタンを押したときの処理
        public void btn_Click(object sender, RoutedEventArgs e)
        {
            //senderからクリックしたファイル名を取得
            string picPath = sender.ToString();
            picPath = picPath.Replace("System.Windows.Controls.Button: ", "");
            int x = picPath.IndexOf(".");
            picPath = picPath.Remove(0, x + 1);

            PNGShowAnnotationWindow pshaw = new PNGShowAnnotationWindow();
            pshaw.Show();
            pshaw.init(picPath, files, x);

            this.Close();
        }

        //更新ボタン
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (isElement)
            {
                ((PNGSelectHowToShowWindow)this.Owner).elementButton_Click(sender, e);
            }
            else
            {
                ((PNGSelectHowToShowWindow)this.Owner).userButton_Click(sender, e);
            }
            this.Close();
        }
    }
}