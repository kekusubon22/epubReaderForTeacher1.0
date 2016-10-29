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
using System.Drawing;
using System.Windows.Interop;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// AddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddinWindow : Window
    {
        public AddinWindow()
        {
            InitializeComponent();
        }

        //追加教材を閲覧するWindow
        //WebKitのインスタンス
        public WebKitBrowser webBrowser1;

        //初期処理
        public void init(string addinType, string filePath)
        {
            if(addinType.Equals("administrator"))
            {
                //WebKitのインスタンスを作成する
                webBrowser1 = new WebKitBrowser();

                //WebKitのインスタンスをWindowsFormsHostに割り当てる
                windowsFormsHost1.Child = webBrowser1;

                //imageは使わないので非表示に
                image1.Visibility = System.Windows.Visibility.Hidden;

                //読み込ませる
                webBrowser1.Url = new Uri(filePath);
            }
            else
            {
                //WindowsFormsHostは使わないので非表示に
                windowsFormsHost1.Visibility = System.Windows.Visibility.Hidden;

                //image1に表示
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(filePath);
                bmp.EndInit();
                image1.Source = bmp;
            }

        }

        private void BlackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WhiteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void YellowButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ModeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Width1Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Width3Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Width20Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void addinCapture(string epubDirectory, string pageName)
        {
            //まず、保存先にページ名.pngが何枚保存されているか調べる
            string[] filesLeft = System.IO.Directory.GetFiles(epubDirectory, pageName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

            int i = 0;
            string k = "";
            foreach (string f in filesLeft)
            {
                //MessageBox.Show(f);
                i++;
            }
            if (i < 100)
            {
                k = "0" + i;
                if (i < 10)
                {
                    k = "0" + k;
                }
            }

            //ここからキャプチャの処理
            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size canvasSize = new System.Drawing.Size();

            //キャプチャ範囲の指定
            browserLeftTop = windowsFormsHost1.PointToScreen(new System.Windows.Point(0.0, 0.0));
            canvasSize.Height = (int)windowsFormsHost1.RenderSize.Height;
            canvasSize.Width = (int)windowsFormsHost1.RenderSize.Width;


            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
            rc.Size = canvasSize;

            ImageBrush ib = new ImageBrush();

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);

                    ib.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(
                        bmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

                //スクリーンショットの保存
                bmp.Save(epubDirectory + "\\" + pageName + "_" + k + ".png", System.Drawing.Imaging.ImageFormat.Png);

                //MessageBox.Show("すく諸完了");
            }
        }
    }
}