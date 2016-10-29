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
    /// PopupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();
        }

        //WebKitのインスタンス
        public WebKitBrowser popupBrowser;

        bool isStraightMode = false;
        string thisElementOwnerPageName; //ポップアップしている要素のもともとのファイル名
        string epubDirectory;

        //初期処理
        public void init( string epubDirectory1, int width, int height, string ownerPageName )
        {
            //ウインドウの大きさの指定
            this.Width = width;
            this.Height = height - 25;

            //WebKitのインスタンスを作成する
            popupBrowser = new WebKitBrowser();

            //WebKitのインスタンスをWindowsFormsHostに割り当てる
            windowsFormsHost1.Child = popupBrowser;

            //ページの読み込み
            popupBrowser.Url = new Uri(epubDirectory1 + "\\OEBPS\\popup.xhtml");

            popupBrowser.UseJavaScript = true;
            popupBrowser.Preferences.AllowPlugins = true;

            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.PageZoom = (float)1.5;

            thisElementOwnerPageName = ownerPageName;
            epubDirectory = epubDirectory1;
        }

        private void BlackButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "0", "0", "0" });
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });

        }

        private void WhiteButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "255", "255", "255" });
        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "255", "0", "0" });
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "0", "0", "255" });
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "0", "255", "0" });
        }

        private void YellowButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeColor", new Object[] { "255", "255", "0" });
        }

        private void ModeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            if (!isStraightMode) //直線モードにする
            {
                popupBrowser.GetScriptManager.CallFunction("changeStrokeMode", new Object[] { "straight" });
                ModeSwitchButton.Content = "自由線";
                isStraightMode = true;
            }
            else //自由線にする
            {
                popupBrowser.GetScriptManager.CallFunction("changeStrokeMode", new Object[] { "free" });
                ModeSwitchButton.Content = "直線";
                isStraightMode = false;

            }
        }

        private void Width1Button_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeWidth", new Object[] { "1" });
        }

        private void Width3Button_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeWidth", new Object[] { "3" });
        }

        private void Width20Button_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("changeWidth", new Object[] { "20" });
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("undo", new Object[] { });
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            popupBrowser.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            popupBrowser.GetScriptManager.CallFunction("clearStroke", new Object[] { });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            //1本でもストロークがあればスクリーンショットして閉じる
            int numberOfPath = (int)popupBrowser.GetScriptManager.CallFunction("getNumberOfPath", new Object[] { }).ToNumber();
            if (numberOfPath > 0)
            {
                PopupCapture(epubDirectory, thisElementOwnerPageName);
            }
            this.Close();
        }

        private void PopupCapture(string epubDirectory, string pageName)
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
