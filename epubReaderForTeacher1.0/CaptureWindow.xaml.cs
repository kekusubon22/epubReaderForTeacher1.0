using System;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// CaptureWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private Point _position;
        private bool _trimEnable = false;
        string epubDirectory;
        string epubName;
        string pagePath;
        string[] epubImage = new string[1024];
        string savedFilePath;
        string subjectName;
        string unitName;
        bool needToClip;

        public CaptureWindow()
        {
            InitializeComponent();
        }

        public void setepubInfo(string epubDirectory, string epubName, string pagePath, string subjectName, string unitName, bool need)
        {
            this.epubDirectory = epubDirectory.Replace(".epub","");
            this.epubName = epubName;
            this.pagePath = pagePath;
            this.subjectName = subjectName;
            this.unitName = unitName;
            needToClip = need;
            for (int i = 0; i < epubImage.Length; i++) 
            {
                epubImage[i] = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // プライマリスクリーンサイズの取得
            var screen = System.Windows.Forms.Screen.PrimaryScreen;

            // ウィンドウサイズの設定
            this.Left = screen.Bounds.Left;
            this.Top = screen.Bounds.Top;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // ジオメトリサイズの設定
            this.ScreenArea.Geometry1 = new RectangleGeometry(new Rect(0, 0, screen.Bounds.Width, screen.Bounds.Height));
        }

        private void DrawingPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as System.Windows.Shapes.Path;
            if (path == null)
                return;

            // 開始座標を取得
            var point = e.GetPosition(path);
            _position = point;

            // マウスキャプチャの設定
            _trimEnable = true;
            this.Cursor = Cursors.Cross;
            path.CaptureMouse();
        }

        private void DrawingPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var path = sender as System.Windows.Shapes.Path;
            if (path == null)
                return;

            // 現在座標を取得
            var point = e.GetPosition(path);

            // マウスキャプチャの終了
            _trimEnable = false;
            this.Cursor = Cursors.Arrow;
            path.ReleaseMouseCapture();

            // 画面キャプチャ
            CaptureScreen(point);

            //画面を閉じる
            this.Close();
        }

        private void DrawingPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_trimEnable)
                return;

            var path = sender as System.Windows.Shapes.Path;
            if (path == null)
                return;

            // 現在座標を取得
            var point = e.GetPosition(path);

            // キャプチャ領域枠の描画
            DrawStroke(point);
        }

        private void DrawStroke(Point point)
        {
            // 矩形の描画
            var x = _position.X < point.X ? _position.X : point.X;
            var y = _position.Y < point.Y ? _position.Y : point.Y;
            var width = Math.Abs(point.X - _position.X);
            var height = Math.Abs(point.Y - _position.Y);
            this.ScreenArea.Geometry2 = new RectangleGeometry(new Rect(x, y, width, height));
        }

        private void CaptureScreen(Point point)
        {
            // 座標変換
            var start = PointToScreen(_position);
            var end = PointToScreen(point);

            // キャプチャエリアの取得
            var x = start.X < end.X ? (int)start.X : (int)end.X;
            var y = start.Y < end.Y ? (int)start.Y : (int)end.Y;
            var width = (int)Math.Abs(end.X - start.X);
            var height = (int)Math.Abs(end.Y - start.Y);
            if (width == 0 || height == 0)
                return;

            // スクリーンイメージの取得
            using (var bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            using (var graph = System.Drawing.Graphics.FromImage(bmp))
            {
                // 画面をコピーする
                graph.CopyFromScreen(new System.Drawing.Point(x, y), new System.Drawing.Point(), bmp.Size);

                // イメージの保存
                string folder = epubDirectory.Replace(".epub", "") + "\\LearningRecord";
                if( !Directory.Exists(folder) )
                {
                    Directory.CreateDirectory(folder);
                }

                string searchPageName = pagePath.Replace(epubDirectory + "\\OEBPS\\", "");
                searchPageName = searchPageName.Replace("image\\", "");

                //保存先に左ページ.右ページ.pngが何枚保存されているか調べる
                string[] files = System.IO.Directory.GetFiles(folder, searchPageName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

                int j = 0;
                foreach (string f in files)
                {
                    //MessageBox.Show(f);
                    epubImage[j] = f;

                    j++;
                }

                string k = null;
                if (j < 100)
                {
                    k = "0" + j;
                    if (j < 10)
                    {
                        k = "0" + k;
                    }
                }

                //すでに保存されている枚数に応じて番号をつけて保存
                bmp.Save(System.IO.Path.Combine(folder, searchPageName + "_" + k + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                savedFilePath = folder + "\\" + searchPageName + "_" + k + ".png";
                
            }

            //「おくる」ボタンをクリックしていた場合は、クリップボードにコピー
            if (needToClip)
            {
                MemoryStream data = new MemoryStream(File.ReadAllBytes(savedFilePath));
                WriteableBitmap bmpim = new WriteableBitmap(BitmapFrame.Create(data));
                data.Close();
                Clipboard.SetImage(bmpim);
                bool a = File.Exists(savedFilePath);
                //File.Delete(savedFilePath);
                
                //DinoMainWindow dinoMain = new DinoMainWindow();
                //dinoMain.Show();
                //dinoMain.pasteClipBorad(subjectName, unitName, savedFilePath);
            }
        }
    }
}