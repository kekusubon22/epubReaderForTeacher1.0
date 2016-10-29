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
    /// ShowCaptureWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowCaptureWindow : Window
    {
        public ShowCaptureWindow()
        {
            InitializeComponent();
        }

        string[] epubCaptures = new string[1024];
        int nowImageNum = 0;
        int num = 0;

        public void ShowImage(string imagePath, string epubDirectory)
        {
            BitmapImage m_bitmap = null;

            // BitmapImageにファイルから画像を読み込む
            m_bitmap = new BitmapImage();
            m_bitmap.BeginInit();
            m_bitmap.UriSource = new Uri(imagePath);
            m_bitmap.EndInit();

            // Imageコントロールに表示
            image1.Source = m_bitmap;


            //imageのpathの配列を用意
            //保存先にimageが何枚保存されているか調べる
            System.Collections.ObjectModel.ReadOnlyCollection<string> files =
            Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                epubDirectory,
                "",
                false,
                Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly,
                new string[] { "*.png" });

            foreach (string f in files)
            {
                epubCaptures[num] = f;
                //MessageBox.Show(epubCaptures[num]);

                num++;
            }

            bool flag = false;
            int i = 0;
            while(!flag){
                if (epubCaptures[i].Equals(imagePath))
                {
                    flag = true;
                    nowImageNum = i;
                }
                i++;
            }
        }

        private void imageBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (nowImageNum > 0)
            {
                nowImageNum--;

                BitmapImage m_bitmap = null;
                // BitmapImageにファイルから画像を読み込む
                m_bitmap = new BitmapImage();
                m_bitmap.BeginInit();
                m_bitmap.UriSource = new Uri(epubCaptures[nowImageNum]);
                m_bitmap.EndInit();
                // Imageコントロールに表示
                image1.Source = m_bitmap;
            }
            else
            {
                MessageBox.Show("最初の記録です。", "ERROR!");
            }
        }

        private void imageNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (nowImageNum < num-1)
            {
                nowImageNum++;

                BitmapImage m_bitmap = null;
                // BitmapImageにファイルから画像を読み込む
                m_bitmap = new BitmapImage();
                m_bitmap.BeginInit();
                m_bitmap.UriSource = new Uri(epubCaptures[nowImageNum]);
                m_bitmap.EndInit();
                // Imageコントロールに表示
                image1.Source = m_bitmap;
            }
            else
            {
                MessageBox.Show("最後の記録です。", "ERROR!");
            }
        }


    }
}
