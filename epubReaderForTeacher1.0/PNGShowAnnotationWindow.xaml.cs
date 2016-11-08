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
    /// PNGShowAnnotationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGShowAnnotationWindow : Window
    {
        public PNGShowAnnotationWindow()
        {
            InitializeComponent();
        }

        string[] files;
        int nowImageNum = 0;

        //初期処理
        public void init(string imagePath, string[] files, int x)
        {
            this.files = files;
            nowImageNum = x-1;

            BitmapImage m_bitmap = null;

            // BitmapImageにファイルから画像を読み込む
            m_bitmap = new BitmapImage();
            m_bitmap.BeginInit();
            m_bitmap.UriSource = new Uri(imagePath);
            m_bitmap.EndInit();

            // Imageコントロールに表示
            image1.Source = m_bitmap;
        }

        //戻るボタン
        private void imageBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (nowImageNum > 0)
            {
                nowImageNum--;

                BitmapImage m_bitmap = null;
                // BitmapImageにファイルから画像を読み込む
                m_bitmap = new BitmapImage();
                m_bitmap.BeginInit();
                m_bitmap.UriSource = new Uri(files[nowImageNum]);
                m_bitmap.EndInit();
                // Imageコントロールに表示
                image1.Source = m_bitmap;
            }
            else
            {
                MessageBox.Show("最初の記録です。", "ERROR!");
            }
        }

        //進むボタン
        private void imageNextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nowImageNum++;

                BitmapImage m_bitmap = null;
                // BitmapImageにファイルから画像を読み込む
                m_bitmap = new BitmapImage();
                m_bitmap.BeginInit();
                m_bitmap.UriSource = new Uri(files[nowImageNum]);
                m_bitmap.EndInit();

                // Imageコントロールに表示
                image1.Source = m_bitmap;
            }
            catch
            {
                MessageBox.Show("最後の記録です。", "ERROR!");
            }
        }
    }
}
