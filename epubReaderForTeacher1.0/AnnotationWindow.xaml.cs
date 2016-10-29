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

using System.Drawing;
using System.Windows.Interop;

namespace epubReader3._9
{
    /// <summary>
    /// AnnotationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AnnotationWindow : Window
    {
        public AnnotationWindow()
        {
            InitializeComponent();
        }

        string epubPath1;
        string pageLeft1;
        string pageRight1;

        public void SetBackground(string epubPath, string pageLeft, string pageRight)
        {
            epubPath1 = epubPath.Replace(".epub","");
            pageLeft1 = pageLeft.Replace(epubPath1 + "\\OEBPS\\","");
            pageRight1 = pageRight.Replace(epubPath1 + "\\OEBPS\\", "");

            MessageBox.Show("epubPath1 = " + epubPath1 + "\n pageLeft1 = " + pageLeft1 + "\n pageRight1 = " + pageRight1);

            CaptureScreen(1);
            CaptureScreen(2);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            string k = null;

            if (pageLeft1 != "about:blank")
            {
                //保存先に左ページ.isfが何枚保存されているか調べる
                System.Collections.ObjectModel.ReadOnlyCollection<string> files1 =
                Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                    epubPath1,
                    pageLeft1,
                    false,
                    Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly,
                    new string[] { "*.isf" });


                int i = 0;
                foreach (string f in files1)
                {
                    MessageBox.Show(f);
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
            }


            //保存先に右ページ.isfが何枚保存されているか調べる
            System.Collections.ObjectModel.ReadOnlyCollection<string> files2 =
            Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                epubPath1,
                pageRight1,
                false,
                Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly,
                new string[] { "*.isf" });


            int j = 0;
            foreach (string f in files2)
            {
                MessageBox.Show(f);
                j++;
            }
            string l = null;
            if (j < 100)
            {
                l = "0" + j;
                if (j < 10)
                {
                    l = "0" + l;
                }
            }

            if (pageLeft1 != "about:blank")
            {
                //inkcanvas1を保存
                string savePath1 = epubPath1.Replace(".epub", "") + "\\" + pageLeft1 + "_" +k + ".isf";

                using (System.IO.FileStream fs = new System.IO.FileStream(savePath1, System.IO.FileMode.Create))
                {
                    inkCanvas1.Strokes.Save(fs);
                }
            }

            //inkcanvas2を保存
            string savePath2 = epubPath1.Replace(".epub", "") + "\\" + pageRight1 + "_" + l + ".isf";

            using (System.IO.FileStream fs = new System.IO.FileStream(savePath2, System.IO.FileMode.Create))
            {
                inkCanvas2.Strokes.Save(fs);
            }
        }

        public void CaptureScreen(int x)
        {
            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size canvasSize = new System.Drawing.Size();
            if (x == 1)
            {
                browserLeftTop = inkCanvas1.PointToScreen(new System.Windows.Point(0.0, 0.0));
                canvasSize.Height = (int)inkCanvas1.RenderSize.Height;
                canvasSize.Width = (int)inkCanvas1.RenderSize.Width;
            }
            else
            {
                browserLeftTop = inkCanvas2.PointToScreen(new System.Windows.Point(0.0, 0.0));
                canvasSize.Height = (int)inkCanvas2.RenderSize.Height;
                canvasSize.Width = (int)inkCanvas2.RenderSize.Width;
            }

            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
            rc.Size = canvasSize;

            //MessageBox.Show("とおりぬけーーー");

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
            }

            if (x == 1)
            {
                inkCanvas1.Background = ib;
            }
            else
            {
                inkCanvas2.Background = ib;
            }
        }

        private void StrokeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MarkerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            SelectColorWindow dialog = new SelectColorWindow();
            dialog.Owner = this;
            dialog.Show();
        }

    }
}
