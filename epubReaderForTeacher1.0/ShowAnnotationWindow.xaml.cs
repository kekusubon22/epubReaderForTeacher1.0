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

using System.IO;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// ShowAnnotationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowAnnotationWindow : Window
    {
        public ShowAnnotationWindow()
        {
            InitializeComponent();
        }
        string[] epubCapture = new string[1024];
        string epubDirectory;
        string recordDirectory;

        public void CreateCaptureButton(string epubDirectory, string epubName)
        {
            //MessageBox.Show(epubDirectory);
            //MessageBox.Show(epubName);
            this.epubDirectory = epubDirectory.Replace(".epub", "");
            recordDirectory = this.epubDirectory + "\\LearningRecord";
            if( !Directory.Exists(recordDirectory) )
            {
                Directory.CreateDirectory(recordDirectory);
            }

            int i = 0;
            for (i = 0; i < epubCapture.Length;i++) { }

            //保存先にimageが何枚保存されているか調べる
            System.Collections.ObjectModel.ReadOnlyCollection<string> files =
            Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                recordDirectory,
                "",
                false,
                Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly,
                new string[] { "*.png" });


            int num = 0;
            foreach (string f in files)
            {
                epubCapture[num] = f;
                //MessageBox.Show(epubCapture[num]);

                num++;
            }

            //ボタンを生成
            Button[] btn = new Button[num];

            int j=0; //グリッドの列要素の位置
            int k=0; //グリッドの行要素の位置

            grid2.ShowGridLines = true;

            for (i = 0; i < num; i++)
            {
                btn[i] = new Button() { Content = epubCapture[i].Replace(this.epubDirectory + "\\LearningRecord\\","") };
                if(System.IO.File.Exists(epubCapture[i]))
                {
                    btn[i].Background = new ImageBrush(new BitmapImage(new Uri(epubCapture[i], UriKind.Relative)));
                }
                else
                {
                    btn[i].Background = Brushes.White;
                }

                if (j < 5)
                {
                    if(j == 1)
                    {
                        ColumnDefinition cd = new ColumnDefinition() { Width = new GridLength(200) };
                        grid2.ColumnDefinitions.Add(cd);
                    }
                    ColumnDefinition cd2 = new ColumnDefinition() { Width = new GridLength(200) };
                    grid2.ColumnDefinitions.Add(cd2);
                    j++;
                }
                else
                {
                    RowDefinition rd2 = new RowDefinition() { Height = new GridLength(200) };
                    grid2.RowDefinitions.Add(rd2);
                    j=1;
                    k++;
                }

                //MessageBox.Show(i + 1 + "枚目を" + k + "行目、" + j + "列目に配置");
                Grid.SetColumn(btn[i], j);
                Grid.SetRow(btn[i], k);
                grid2.Children.Add(btn[i]);
                btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                btn[i].Width = double.NaN;  //Autoという意味
                btn[i].Height = double.NaN; //Autoという意味

                btn[i].Click += new RoutedEventHandler(btn_Click);
            }
            grid2.ShowGridLines = false;
        }

        public void btn_Click(object sender, RoutedEventArgs e)
        {
            string captureName = sender.ToString();
            captureName = captureName.Replace("System.Windows.Controls.Button: ", "");
            captureName = recordDirectory + "\\" + captureName;

            //MessageBox.Show(captureName);

            ShowCaptureWindow dialog = new ShowCaptureWindow();
            dialog.Show();
            this.Owner = dialog;
            dialog.ShowImage(captureName, recordDirectory);

            this.Close();
        }
    }
}
