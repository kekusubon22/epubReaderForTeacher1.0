﻿using System;
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

using System.Windows.Ink;
using System.IO;

namespace epubReaderForTeacher1._0
{
    /// <summary>
    /// PNGPopupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGPopupWindow : Window
    {
        public PNGPopupWindow()
        {
            InitializeComponent();
        }

        //ファイル操作に関する変数
        string popupPath;
        string thawPath;

        //アノテーションに関する変数
        System.Windows.Point startP = new System.Windows.Point();
        System.Windows.Point prevP = new System.Windows.Point();
        List<StrokeLine> strokeLines = new List<StrokeLine>();
        List<System.Windows.Point> points;
        System.Windows.Media.Color color = new System.Windows.Media.Color();
        DrawingAttributes inkDA = new DrawingAttributes();
        bool isFreeLine = true;
        bool dragging = false;
        int strokeId = 0;
        int counter = 0;

        //動作のログ
        List<LearningLog> learningLogs = new List<LearningLog>();

        //ユーザ情報を表すオブジェクト
        User user = new User();

        //初期処理
        public void init(string popupPath, string thawPath)
        {
            this.popupPath = popupPath;
            this.thawPath = thawPath;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(popupPath);
            bmp.EndInit();
            image1.Source = bmp;

            //色の初期値として黒を指定
            color = System.Windows.Media.Color.FromArgb(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
            inkDA.Color = color;

            //太さの初期値は5
            inkDA.Width = 3;
            inkDA.Height = 3;

            inkCanvas1.DefaultDrawingAttributes = inkDA;
            inkCanvas1.AllowDrop = true;

            //inkCanvas1にマウスイベントを設定
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseDown), true);
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseMove), true);
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseUp), true);
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;

            //前に描画した情報があれば読み込む
            try
            {
                RoadAnnotateRecord();
                drawAll();
            }
            catch
            {
                //MessageBox.Show("再読み込み失敗");
            }
        }

        //描く処理(マウス用)
        private void inkCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //UIElement el = sender as UIElement;

            ////自由線
            //if (isFreeLine)
            //{
            //    dragging = true;
            //    points = new List<System.Windows.Point>();
            //    points.Add(e.GetPosition(el));

            //    prevP = e.GetPosition(el);
            //}

            ////直線
            //else
            //{
            //    dragging = true;
            //    startP = e.GetPosition(el);
            //}
        }

        //描く処理(マウス用)
        private void inkCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            //UIElement el = sender as UIElement;

            ////自由線モード
            //if (dragging && isFreeLine)
            //{
            //    points.Add(e.GetPosition(el));
            //    counter++;

            //    //点の情報を集め、始点と現在の点をむすぶ
            //    StylusPointCollection spc = new StylusPointCollection();
            //    spc.Add(new StylusPoint(prevP.X, prevP.Y));
            //    spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
            //    Stroke stroke = new Stroke(spc, inkDA);
            //    inkCanvas1.Strokes.Add(stroke);

            //    prevP = e.GetPosition(el);
            //}

            ////直線モード
            //else if (!isFreeLine && dragging)
            //{
            //    inkCanvas1.Strokes.Clear();
            //    drawAll();

            //    //点の情報を集め、始点と現在の点をむすぶ
            //    StylusPointCollection spc = new StylusPointCollection();
            //    spc.Add(new StylusPoint(startP.X, startP.Y));
            //    spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
            //    Stroke stroke = new Stroke(spc, inkDA);
            //    inkCanvas1.Strokes.Add(stroke);

            //    counter++;
            //}
        }

        //描く処理(マウス用)
        private void inkCanvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //UIElement el = sender as UIElement;

            ////自由線のとき
            //if (isFreeLine && dragging)
            //{
            //    points.Add(e.GetPosition(el));

            //    //点の情報を集め、始点と現在の点をむすぶ
            //    StylusPointCollection spc = new StylusPointCollection();
            //    spc.Add(new StylusPoint(prevP.X, prevP.Y));
            //    spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
            //    Stroke stroke = new Stroke(spc, inkDA);
            //    inkCanvas1.Strokes.Add(stroke);

            //    //配列strokeLinesに追加
            //    StrokeLine strokeLine = new StrokeLine();
            //    strokeLine.SetId(strokeId);
            //    strokeLine.SetPoints(points);
            //    strokeLine.SetColor(color);
            //    strokeLine.SetWidth((int)inkDA.Width);
            //    strokeLine.SetDownNow(false);
            //    strokeLine.SetInSpace(false);
            //    strokeLine.SetEreasedTime(-1);
            //    strokeLines.Add(strokeLine);

            //    //動作ログに記録
            //    LearningLog log = new LearningLog();
            //    log.SetStrokeId(strokeId.ToString());
            //    log.SetBehavior("draw");
            //    learningLogs.Add(log);

            //    dragging = false;
            //    strokeId++;

            //    counter = 0;
            //}

            ////直線のとき
            //else if (!isFreeLine && dragging)
            //{
            //    strokeId++;

            //    inkCanvas1.Strokes.Clear();
            //    drawAll();

            //    //点の情報を集め、始点と現在の点をむすぶ
            //    StylusPointCollection spc = new StylusPointCollection();
            //    spc.Add(new StylusPoint(startP.X, startP.Y));
            //    spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
            //    Stroke stroke = new Stroke(spc, inkDA);
            //    inkCanvas1.Strokes.Add(stroke);

            //    //pointsに始点と現在の点を格納
            //    points = new List<System.Windows.Point>();
            //    points.Add(startP);
            //    points.Add(e.GetPosition(el));

            //    //配列strokeLinesについか
            //    StrokeLine strokeLine = new StrokeLine();
            //    strokeLine.SetId(strokeId);
            //    strokeLine.SetPoints(points);
            //    strokeLine.SetColor(color);
            //    strokeLine.SetWidth((int)inkDA.Width);
            //    strokeLine.SetDownNow(false);
            //    strokeLine.SetInSpace(false);
            //    strokeLine.SetEreased(false);

            //    strokeLines.Add(strokeLine);

            //    dragging = false;
            //    counter = 0;
            //}
        }

        //描く処理(ペン用)
        private void inkCanvas1_StylusDown(object sender, StylusDownEventArgs e)
        {
            UIElement el = sender as UIElement;

            //自由線
            if (isFreeLine)
            {
                dragging = true;
                points = new List<System.Windows.Point>();
                points.Add(e.GetPosition(el));

                prevP = e.GetPosition(el);
            }

            //直線
            else
            {
                dragging = true;
                startP = e.GetPosition(el);
            }
        }

        //描く処理(ペン用)
        private void inkCanvas1_StylusMove(object sender, StylusEventArgs e)
        {
            UIElement el = sender as UIElement;

            //自由線モード
            if (dragging && isFreeLine)
            {
                points.Add(e.GetPosition(el));
                counter++;

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(prevP.X, prevP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                prevP = e.GetPosition(el);
            }

            //直線モード
            else if (!isFreeLine && dragging)
            {
                inkCanvas1.Strokes.Clear();
                drawAll();

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(startP.X, startP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                counter++;
            }
        }

        //描く処理(ペン用)
        private void inkCanvas1_StylusUp(object sender, StylusEventArgs e)
        {
            UIElement el = sender as UIElement;

            //自由線のとき
            if (isFreeLine && dragging)
            {
                points.Add(e.GetPosition(el));

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(prevP.X, prevP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                //配列strokeLinesに追加
                StrokeLine strokeLine = new StrokeLine();
                strokeLine.SetId(strokeId);
                strokeLine.SetPoints(points);
                strokeLine.SetColor(color);
                strokeLine.SetWidth((int)inkDA.Width);
                strokeLine.SetDownNow(false);
                strokeLine.SetInSpace(false);
                strokeLine.SetEreasedTime(-1);
                strokeLines.Add(strokeLine);

                //動作ログに記録
                LearningLog log = new LearningLog();
                log.SetStrokeId(strokeId.ToString());
                log.SetBehavior("draw");
                learningLogs.Add(log);

                dragging = false;
                strokeId++;

                counter = 0;
            }

            //直線のとき
            else if (!isFreeLine && dragging)
            {
                strokeId++;

                inkCanvas1.Strokes.Clear();
                drawAll();

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(startP.X, startP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                //pointsに始点と現在の点を格納
                points = new List<System.Windows.Point>();
                points.Add(startP);
                points.Add(e.GetPosition(el));

                //配列strokeLinesについか
                StrokeLine strokeLine = new StrokeLine();
                strokeLine.SetId(strokeId);
                strokeLine.SetPoints(points);
                strokeLine.SetColor(color);
                strokeLine.SetWidth((int)inkDA.Width);
                strokeLine.SetDownNow(false);
                strokeLine.SetInSpace(false);
                strokeLine.SetEreased(false);

                strokeLines.Add(strokeLine);

                dragging = false;
                counter = 0;
            }
        }

        //色変更
        private void colorChange(byte a, byte r, byte g, byte b)
        {
            color = System.Windows.Media.Color.FromArgb(a, r, g, b);
            inkDA.Color = color;
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //黒
        private void blackButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
        }

        //赤
        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
        }

        //青
        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
        }

        //黄色
        private void yellowButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(0));
        }

        //直線・曲線切り替え
        private void strokeModeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFreeLine) //直線モードにする
            {
                strokeModeChangeButton.Content = "自由線";
                isFreeLine = false;

                inkCanvas1.EditingMode = InkCanvasEditingMode.None;
            }
            else //自由線モードにする
            {
                strokeModeChangeButton.Content = "直線";
                isFreeLine = true;

                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //太さ変更
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sl = sender as Slider;
            inkDA.Width = sl.Value;
            inkDA.Height = sl.Value;
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //すべて消去
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("線をすべて消しますか？", "かくにん", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                try
                {
                    for (int i = 0; i < strokeLines.Count(); i++)
                    {
                        //ストローク一つひとつに、erase = trueをセット
                        strokeLines[i].SetEreased(true);

                        //本処理で初めてその線が消える場合のみ、erasedTimeをセット
                        if (strokeLines[i].GetEreasedTime() == -1)
                        {
                            strokeLines[i].SetEreasedTime(learningLogs.Count + 1);
                        }
                    }

                    //動作ログに記録。全消去の時はidの欄をallとする
                    LearningLog log = new LearningLog();
                    log.SetStrokeId("all");
                    log.SetBehavior("erase");
                    learningLogs.Add(log);

                    //キャンバスをクリア
                    inkCanvas1.Strokes.Clear();
                }
                catch
                {

                }
            }
        }

        //すべての線を再描画
        private void drawAll()
        {
            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //消去済みでないストローク以外を再描画
                if (!sl.GetEreased())
                {
                    //線の色、幅を取得
                    DrawingAttributes DA = new DrawingAttributes();
                    DA.Color = sl.GetColor();
                    DA.Width = sl.GetWidth();
                    DA.Height = sl.GetWidth();
                    inkCanvas1.DefaultDrawingAttributes = DA;

                    //点の情報を集める
                    StylusPointCollection spc = new StylusPointCollection();
                    for (int j = 0; j < sl.GetPoints().Count; j++)
                    {
                        spc.Add(new StylusPoint(sl.GetPoints()[j].X, sl.GetPoints()[j].Y));
                    }
                    Stroke stroke = new Stroke(spc, DA);
                    inkCanvas1.Strokes.Add(stroke);
                }

            }

            //線のスタイルを戻す
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //ひとつ戻る
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //後ろからさかのぼって消えていない線を探す
                int i;
                for (i = strokeLines.Count - 1; i >= 0; i--)
                {
                    if (!strokeLines[i].GetEreased())
                    {
                        strokeLines[i].SetEreased(true);
                        strokeLines[i].SetEreasedTime(learningLogs.Count + 1);
                        inkCanvas1.Strokes.Clear();
                        drawAll();
                        break;
                    }
                }

                //動作ログに記録
                LearningLog log = new LearningLog();
                log.SetStrokeId(strokeLines[i].GetId().ToString());
                log.SetBehavior("erase");
                learningLogs.Add(log);
            }
            catch
            {
                MessageBox.Show("ERROR! 一つ戻るの処理過程でエラーが起きました。");
            }
        }

        //ストローク情報の保存
        private void SaveAnnotateRecord()
        {
            //動作ログの保存と、ストローク情報の保存
            if (!Directory.Exists(thawPath + "\\Strokes"))
            {
                Directory.CreateDirectory(thawPath + "\\Strokes");
            }
            if (!Directory.Exists(thawPath + "\\LearningLog"))
            {
                Directory.CreateDirectory(thawPath + "\\LearningLog");
            }

            //XmlSerializerオブジェクトを作成
            //オブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer strokeSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<StrokeLine>));
            System.Xml.Serialization.XmlSerializer logSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<LearningLog>));

            //MessageBox.Show(thawPath + "\\strokes\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\" , ""));

            //書き込むファイルを開く
            StreamWriter ssw = new StreamWriter(
                thawPath + "\\Strokes\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));
            StreamWriter lsw = new StreamWriter(
                thawPath + "\\LearningLog\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));

            //シリアル化しxmlファイルに保存
            strokeSerializer.Serialize(ssw, strokeLines);
            logSerializer.Serialize(lsw, learningLogs);

            //ファイルを閉じる
            ssw.Close();
            lsw.Close();
        }

        //ストローク情報の読み込み
        private void RoadAnnotateRecord()
        {
            if (!Directory.Exists(thawPath + "\\Strokes"))
            {
                Directory.CreateDirectory(thawPath + "\\Strokes");
            }
            if (!Directory.Exists(thawPath + "\\LearningLog"))
            {
                Directory.CreateDirectory(thawPath + "\\LearningLog");
            }

            //保存元のファイル名
            string strokeXmlPath = thawPath + "\\Strokes\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml";
            string logXmlPath = thawPath + "\\LearningLog\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml";

            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer strokeSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<StrokeLine>));
            System.Xml.Serialization.XmlSerializer logSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LearningLog>));


            //読み込むファイルを開く
            System.IO.StreamReader ssr = new System.IO.StreamReader(strokeXmlPath, new System.Text.UTF8Encoding(false));
            System.IO.StreamReader lsr = new System.IO.StreamReader(logXmlPath, new System.Text.UTF8Encoding(false));

            //XMLファイルから読み込み、逆シリアル化する
            strokeLines = (List<StrokeLine>)strokeSerializer.Deserialize(ssr);
            learningLogs = (List<LearningLog>)logSerializer.Deserialize(lsr);

            //ファイルを閉じる
            ssr.Close();
            lsr.Close();
        }

        //ストローク再生画面の表示
        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            PNGPopupReviewWindow pprw = new PNGPopupReviewWindow();
            pprw.Owner = this;
            pprw.Show();
            pprw.init(popupPath, strokeLines, learningLogs);
        }

        //キャプチャ一覧の表示
        private void showCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAnnotationWindow saw = new ShowAnnotationWindow();
            saw.Owner = this;

        }

        //閉じるボタン
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //ストローク情報の保存
            SaveAnnotateRecord();

            //すく所の処理


            this.Close();
        }


        ////紙面全体をキャプチャ
        //public void ImageCaptureAll()
        //{
        //    string k = null;
        //    string searchPageName = pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "");
        //    //MessageBox.Show(searchPageName);

        //    //保存先にページ.pngが何枚保存されているか調べる
        //    string[] files = System.IO.Directory.GetFiles(thawPath + "\\LearningRecord", searchPageName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

        //    int i = 0;
        //    foreach (string f in files)
        //    {
        //        i++;
        //    }
        //    if (i < 100)
        //    {
        //        k = "0" + i;
        //        if (i < 10)
        //        {
        //            k = "0" + k;
        //        }
        //    }

        //    //webBrowser1を保存
        //    string savePath = pageContent[currentPageNum].Replace("\\OEBPS\\image", "\\LearningRecord") + "_" + k + ".png";
        //    CaptureScreen(savePath);
        //}

        ////スクリーンショットの処理
        //public void CaptureScreen(string saveFileName)
        //{
        //    System.Windows.Point browserLeftTop = new System.Windows.Point();
        //    System.Drawing.Size canvasSize = new System.Drawing.Size();

        //    browserLeftTop = image1.PointToScreen(new System.Windows.Point(0.0, 0.0));
        //    canvasSize.Height = (int)image1.RenderSize.Height;
        //    canvasSize.Width = (int)image1.RenderSize.Width;


        //    System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
        //    rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
        //    rc.Size = canvasSize;

        //    ImageBrush ib = new ImageBrush();

        //    using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //    {
        //        using (Graphics g = Graphics.FromImage(bmp))
        //        {
        //            g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);

        //            ib.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
        //                bmp.GetHbitmap(),
        //                IntPtr.Zero,
        //                Int32Rect.Empty,
        //                BitmapSizeOptions.FromEmptyOptions());
        //        }

        //        //スクリーンショットの保存
        //        bmp.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Png);
        //    }
        //}


    }
}
