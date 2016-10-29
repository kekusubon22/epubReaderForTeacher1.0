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
using System.Windows.Navigation;
using System.Windows.Shapes;
   
using Microsoft.Win32;
using Ionic.Zip;
using Ionic.Zlib;
using System.Xml;
using System.IO;
using WebKit;
using WebKit.JSCore;
using System.Drawing;
using System.Windows.Interop;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //WebKitのインスタンス
        public WebKitBrowser webBrowser1;

        public string epubFileName = null;
        public string[] opfHref = new string[256];
        public string[] pageContent = new string[256];
        public string[] cssPath = new string[16];
        public int currentPageSingle = 0;
        public int pageLim = 0;
        public int size;
        string epubPath;
        bool drawingFlag = false; //現在お絵かきモードかどうかをします
        string subjectName;
        string[] unitName = new string[32];
        string notePath;
        string dinoNowOpening = "none";
        bool menuIsLeft = true;
        bool isEasyMode = false;
        string addinFilesDirectory;
        string addinFilePath = "0";

        public double slider;
        public double x = 0;

        //もどるボタン
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            pageBack(0);
            drawingFlag = false;
        }

        //すすむボタン
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            pageNext(0);
            drawingFlag = false;

        }

        //スライダーの値を変更したときの処理
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider = slider1.Value;

            x = ((double)pageLim / (double)slider1.Maximum) * slider;

            //MessageBox.Show(epubName + "のページ数は" + pageLim + "です．また，sliderの値は" + slider + "です．", "Debug19");
            //MessageBox.Show("pageLim / trackBar1.Maximum の値は" + pageLim / trackBar1.Maximum + "です．", "Debug20");
            //MessageBox.Show("x の値は" + x + "です．", "Debug21");

            currentPageSingle = (int)x;

            try
            {
                webBrowser1.Url = new Uri(pageContent[currentPageSingle]);
            }
            catch
            {
                webBrowser1.Url = new Uri("about:blank");
            }
        }

        //SelectEpubWindowから移ってきたときに行う処理 (初期処理)
        public void ePubClick(string epubName, int startPageNum, string dinoPageName)
        {
            dinoNowOpening = dinoPageName;

            //WebKitのインスタンスを作成する
            webBrowser1 = new WebKitBrowser();

            //WebKitのインスタンスをWindowsFormsHostに割り当てる
            windowsFormsHost1.Child = webBrowser1;

            epubFileName = epubName;
            for (int a = 0; a < 256; a++)
            {
                opfHref[a] = null;
                pageContent[a] = null;
            }

            int i = 0;
            int k = 0;
            int l = 0;

            epubPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ContentsData\\epub\\" + epubName;

            this.Title = epubName;
            currentPageSingle = startPageNum;

            //ここからcontent.opfを解析する
            String thawPath = epubPath.Replace(".epub", "");
            String opfPath = thawPath + "\\OEBPS\\content.opf";
            //MessageBox.Show("opfPath = " + opfPath, "debug5");

            if (File.Exists(opfPath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                // XmlDocumentオブジェクトを作成
                xmlDoc.Load(@opfPath);

                try
                {
                    // ルートの要素を取得
                    XmlElement xmlRoot = xmlDoc.DocumentElement;

                    // <item>要素をセット
                    XmlNodeList xmlNode = xmlRoot.GetElementsByTagName("item");

                    String strItemHref = "start.";
                    while (true)
                    {
                        try
                        {
                            // 取得した<item>要素はXmlNodeListなのでXmlElementにキャストする
                            XmlElement xmlName = (XmlElement)xmlNode.Item(i);

                            // <item>要素のhrefの属性値を取得します
                            strItemHref = xmlName.GetAttribute("href");
                            opfHref[i] = strItemHref;

                            if (opfHref[i].Contains(".css"))
                            {
                                cssPath[l] = thawPath + "\\OEBPS\\" + opfHref[i];
                                //MessageBox.Show("cssPath[" + l + "] = " + cssPath[l], "Debug7");
                                l++;
                            }

                            if (opfHref[i].Contains(".xhtml") || opfHref[i].Contains(".html"))
                            {
                                pageContent[k] = thawPath + "\\OEBPS\\" + opfHref[i];
                                //MessageBox.Show("pageContent[" + k + "] = " + pageContent[k], "Debug8");


                                //教科・単元名があれば取得
                                subjectName = xmlName.GetAttribute("subject");
                                unitName[k] = xmlName.GetAttribute("unit");

                                if (subjectName.Length == 0)
                                {
                                    //なければ教科名は「その他」
                                    subjectName = "その他";
                                }

                                if (unitName[k].Length == 0)
                                {
                                    //単元名の指定がなければ、電子書籍名を単元名とする
                                    unitName[k] = epubFileName;
                                }
                                //MessageBox.Show(pageContent[k] + " の単元名は " + unitName[k] + " です。");

                                k++;
                                //MessageBox.Show("<item>の属性href=" + strItemHref);
                            }
                        }
                        catch
                        {
                            break;
                        }
                        i++;
                    }
                }

                catch (System.Xml.XmlException Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
            else
            {
                MessageBox.Show("ERROR!ファイル" + opfPath + "は存在しません．", "ERROR!");
            }

            //ここから電子書籍を表示するコード
            //MessageBox.Show("ここから電子書籍を表示するコードです．", "Debug6");

            pageLim = k; //これがページの上限
            //MessageBox.Show("k = " + k, "Debug9");

            //webBrowserに1ページ目から読み込ませる
            webBrowser1.Url = new Uri(pageContent[currentPageSingle]);

            //倍率の設定
            adjustZoom();

            webBrowser1.UseJavaScript = true;
            webBrowser1.Preferences.AllowPlugins = true;
        }

        //次のページへ進むときの処理
        public void pageNext(int test)
        {

            currentPageSingle++;

            if (currentPageSingle < pageLim)
            {
                try
                {
                    webBrowser1.Url = new Uri(pageContent[currentPageSingle]);
                }
                catch
                {
                    webBrowser1.Url = new Uri("about:blank");
                }

                x = (double)slider1.Maximum / (double)pageLim * (double)currentPageSingle;
                slider1.Value = (int)x + 1;

                if (currentPageSingle == pageLim)
                {
                    slider1.Value = slider1.Maximum;
                }
            }
            else
            {
                MessageBox.Show("最後のページです．", "ERROR!");
                currentPageSingle--;
            }

            //倍率の設定
            adjustZoom();
        }

        //前のページへ戻る時の処理
        public void pageBack(int test)
        {
            currentPageSingle--;

            if (currentPageSingle > 0)
            {
                try
                {
                    webBrowser1.Url = new Uri(pageContent[currentPageSingle]);
                }
                catch
                {
                    webBrowser1.Url = new Uri("about:blank");
                }

                x = (double)slider1.Maximum / (double)pageLim * (double)currentPageSingle;
                slider1.Value = (int)x + 1;
            }
            else
            {
                MessageBox.Show("最初のページです．", "ERROR!");
                currentPageSingle++;
            }

            //倍率の設定
            adjustZoom();
        }

        //紙面を保存するときの処理(スクショする)
        private void SaveAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureWindow dialog = new CaptureWindow();
            dialog.Owner = this;
            dialog.setepubInfo(epubPath, epubFileName, pageContent[currentPageSingle], subjectName, unitName[currentPageSingle], false);
            dialog.Show();
        }

        //保存した書き込みの一覧を表示する
        private void ShowAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAnnotationWindow dialog = new ShowAnnotationWindow();
            dialog.Owner = this;
            dialog.Show();
            dialog.Title = epubFileName + "のかきこみ一覧";
            dialog.CreateCaptureButton(epubPath, epubFileName);
        }

        //アプリケーションを終了するときの処理
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //電子書籍一覧の画面に戻る
        private void indexButton_Click(object sender, RoutedEventArgs e)
        {
            SelectEpubWindow dialog = new SelectEpubWindow();
            dialog.Show();
        }

        //「かく」ボタンをクリックしたときの処理
        private void AnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!drawingFlag) //お絵かきモードにする
            {
                drawingFlag = true;
                AnnotationButton.Content = "やめる";
                AnnotationButton.Visibility = System.Windows.Visibility.Hidden;

                try
                {
                    webBrowser1.GetScriptManager.CallFunction("annotationChild", new Object[] { });
                }
                catch
                {

                }

                webBrowser1.GetScriptManager.CallFunction("openCanvas", new Object[] { });

                AnnotationToolsWindow dialog = new AnnotationToolsWindow();
                dialog.Owner = this;
                if (menuIsLeft)
                {
                    dialog.Top = 300;
                    dialog.Left = 10;
                }
                else
                {
                    dialog.Top = 300;
                    dialog.Left = this.Width - 90;
                }
                dialog.Show();
            }
            else //canvasを非表示にする
            {
                drawingFlag = false;
                AnnotationButton.Content = "かく";

                webBrowser1.GetScriptManager.CallFunction("closeAnnotation", new Object[] { });
            }     
        }

        //色変更
        public void ChangeColor(string r, string g, string b)
        {
            webBrowser1.GetScriptManager.CallFunction("openCanvas", new Object[] { });
            webBrowser1.GetScriptManager.CallFunction("changeColor", new Object[] { r, g, b });
        }

        //直線・曲線切り替え
        public void ChangeStrokeMode(string newMode)
        {
            webBrowser1.GetScriptManager.CallFunction("changeStrokeMode", new Object[] { newMode });
        }

        //太さ変更
        public void ChangeWidth(string newWidth)
        {
            webBrowser1.GetScriptManager.CallFunction("changeWidth", new Object[] { newWidth });
        }

        //ひとつもどる
        public void Undo()
        {
            webBrowser1.GetScriptManager.CallFunction("undo", new Object[] { });
        }

        //すべて消す
        public void ClearStroke()
        {
            webBrowser1.GetScriptManager.CallFunction("clearStroke", new Object[] { });
        }

        //あのてーしょんを非表示にする
        public void CloseCanvas()
        {
            drawingFlag = false;
            AnnotationButton.Content = "かく";

            webBrowser1.GetScriptManager.CallFunction("closeAnnotation", new Object[] { });

            AnnotationButton.Visibility = System.Windows.Visibility.Visible;    
        }

        //ブラウザ上にSVG pathがいくつあるかを確かめる
        public int getNumberOfPath()
        {
            int x1 = (int)webBrowser1.GetScriptManager.CallFunction("getNumberOfPath", new Object[] { }).ToNumber();

            //MessageBox.Show("SVG pathは" + x1 + "本描かれています");
            return x1;
        }

        //ポップアップ機能を使う(div classがselectedになっている要素のみで形成する新しいxhtmlファイルをつくる)
        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            string[] xhtml = new string[1024];
            string[] newXhtml = new string[1024];
            
            int i = 0;
            int j = 0;
            int bodyBeginingLine = -1; //xhtmlファイル内で<body>タグが始まる行
            int pw = 0; //ポップアップする要素の幅
            int ph = 0; //ポップアップする要素の高さ

            string popupString = webBrowser1.GetScriptManager.CallFunction("popup", new Object[] { }).ToString();
            //MessageBox.Show(popupString);

            if (popupString.Contains("width"))
            {
                int start = popupString.IndexOf("width"); //xhtml[i]の中でwidthの記述が始まる箇所
                int a = 0; //widthの数値の記述が始まる箇所
                int b = 0; //widthの数値の記述が終わる箇所
                int c = 0; //widthの記述の後に " が出てきた回数
                int index = start; //文字列の検索を始める箇所
                string dq = "\"";
                string cl = ":";
                string sc = ";";
                bool findWidth = false;

                while (!findWidth)
                {
                    if ( popupString[index] == dq[0] || popupString[index] == cl[0] || popupString[index] == sc[0] )
                    {
                        c++;
                        if (c == 1)
                        {
                            a = index + 1;
                            index++;
                        }
                        else if (c == 2)
                        {
                            b = index;
                            findWidth = true;
                        }
                    }
                    
                    else
                    {
                        index++;
                    }
                }
                //MessageBox.Show(popupString.Substring(a, b - a));
                pw = int.Parse(popupString.Substring(a, b-a).Replace("px", ""));
            }

            if (popupString.Contains("height"))
            {
                int start = popupString.IndexOf("height"); //xhtml[i]の中でheightの記述が始まる箇所
                int a = 0; //heightの数値の記述が始まる箇所
                int b = 0; //heightの数値の記述が終わる箇所
                int c = 0; //heightの記述の後に " が出てきた回数
                int index = start;
                string dq = "\"";
                string cl = ":";
                string sc = ";";
                bool findHeight = false;

                while (!findHeight)
                {
                    if (popupString[index] == dq[0] || popupString[index] == cl[0] || popupString[index] == sc[0])
                    {
                        c++;
                        if (c == 1)
                        {
                            a = index + 1;
                            index++;
                        }
                        else if (c == 2)
                        {
                            b = index;
                            findHeight = true;
                        }
                    }
                    else
                    {
                        index++;
                    }
                }
                //MessageBox.Show(popupString.Substring(a, b - a));
                ph = int.Parse(popupString.Substring(a, b-a).Replace("px", ""));
            }
            //MessageBox.Show("pw:" + pw + ", ph:" + ph);


            //ここからxhtmlを書き換える
            if (!popupString.Equals("null"))
            {
                using (StreamReader reader = new StreamReader(@epubPath.Replace(".epub", "") + "\\OEBPS\\popup.xhtml"))
                {
                    while (reader.Peek() > -1)
                    {
                        xhtml[i] = reader.ReadLine();
                        if (xhtml[i].Contains("<body>"))
                        {
                            bodyBeginingLine = i;
                        }

                        i++;
                    }
                    reader.Close();
                }

                for (j = 0; j <= bodyBeginingLine; j++)
                {
                    newXhtml[j] = xhtml[j];
                }

                //表示倍率を縦横どちらに合わせるか設定
                //float width;
                //float height;
                //float v;
                //if ( width < height )
                //{
                //v = width / height;
                //}
                //else
                //{
                //v = height / width;
                //}

                newXhtml[j] = "<div id='popupblock' style='position:fixed; top:75px; left:100px; z-index:0; width:" + (pw + 150) + "px; height:" + (ph + 150) + "px; zoom:1.5 background-color:#FFFFFF;' >";
                newXhtml[j + 1] = popupString;
                newXhtml[j + 2] = "</div></body></html>";

                using (StreamWriter writer = new StreamWriter(@epubPath.Replace(".epub", "") + "\\OEBPS\\popup.xhtml"))
                {
                    for (int k = 0; k <= j + 2; k++)
                    {
                        writer.WriteLine(newXhtml[k]);
                    }
                }

                string epubDirectory = epubPath.Replace(".epub", "");
                string pageName = pageContent[currentPageSingle].Replace(epubDirectory + "\\OEBPS\\", "");

                //別画面で表示する
                PopupWindow dialog = new PopupWindow();
                dialog.Owner = this;
                dialog.Top = 10;
                dialog.Left = 75;
                dialog.Show();
                dialog.init(epubDirectory, pw + 700, ph + 650, pageName);
            }

            else
            {
                MessageBox.Show("要素が選択されていないか、このページはPOPUPに対応していません。", "ERROR!");
            }

            //倍率の設定
            adjustZoom();

        }

        //スペーシング機能を使う (selectedになっている要素をさがし、したにすぺーすをあける)
        private void SpacingButton_Click(object sender, RoutedEventArgs e)
        {
            webBrowser1.GetScriptManager.CallFunction("spacing", new Object[] { });
            //SpacingButton.Background = new ImageBrush(new BitmapImage(new Uri("../../../icon/Spacing2.png", UriKind.Relative)));
        }

        //ブラウザ画面をキャプチャする
        public void BrowserCapture()
        {
            string k = null;
            string epubDirectory = epubPath.Replace(".epub", "");
            string searchPageName = pageContent[currentPageSingle].Replace(epubDirectory + "\\OEBPS\\", "");
            //MessageBox.Show(searchPageName);

            if (pageContent[currentPageSingle] != "about:blank" && searchPageName != "toc.xhtml")
            {
                //保存先にページ.pngが何枚保存されているか調べる
                string[] files = System.IO.Directory.GetFiles(epubDirectory, searchPageName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

                int i = 0;
                foreach (string f in files)
                {
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

                //webBrowser1を保存
                string savePath = pageContent[currentPageSingle].Replace("\\OEBPS", "") + "_" + k + ".png";
                CaptureScreen(savePath);
            }
        }

        //スクリーンショットの処理
        public void CaptureScreen(string saveFileName)
        {
            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size canvasSize = new System.Drawing.Size();

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
                bmp.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        //デジタルノートに送る
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureWindow dialog = new CaptureWindow();
            dialog.Owner = this;
            dialog.setepubInfo(epubPath, epubFileName, pageContent[currentPageSingle], subjectName, unitName[currentPageSingle], true);
            dialog.Show();
        }

        //ブラウザをズームインする
        private void Browser1ZoomIn()
        {
            webBrowser1.PageZoom *= (float)1.05;
        }

        //ブラウザをズームアウトする
        private void Browser1ZoomOut()
        {
            webBrowser1.PageZoom /= (float)1.05;
        }

        //ブラウザ倍率の調整
        private void adjustZoom()
        {
            double zoom;
            zoom = webBrowser1.GetScriptManager.CallFunction("adjustZoomOwner", new Object[] { }).ToNumber();
            if (zoom == 0)
            {
                zoom = webBrowser1.GetScriptManager.CallFunction("adjustZoomChild", new Object[] { }).ToNumber();
                if (zoom == 0)
                {
                    zoom = 1.0;
                }
            }
            webBrowser1.PageZoom = (float)zoom;
        }

        //デジタルノート起動
        private void RaunchDinoButton_Click(object sender, RoutedEventArgs e)
        {
        //    notePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ContentsData\\Note";

        //    note4.MainWindow dinoDalog = new note4.MainWindow();
        //    dinoDalog.Show();
        //    dinoDalog.isfShow(subjectName, "1.isf", notePath + "\\" + subjectName + "\\" + unitName[currentPageLeft]);

        //    if (dinoNowOpening.Contains("none"))
        //    {
        //        if (drawingFlag)
        //        {
        //            BrowserCapture();
        //        }

        //        DinoPageWindow dinoPageWin = new DinoPageWindow();
        //        dinoPageWin.Show();
        //        dinoPageWin.pageShow(subjectName, unitName[currentPageSingle], epubFileName, currentPageSingle);

        //        this.Close();
        //    }
        //    else
        //    {
        //        if (drawingFlag)
        //        {
        //            BrowserCapture();
        //        }

        //        DinoMainWindow dinoMainWin = new DinoMainWindow();
        //        dinoMainWin.Show();
        //        dinoMainWin.isfShow(subjectName, dinoNowOpening, notePath + "\\" + subjectName + "\\" + unitName[currentPageSingle], unitName[currentPageSingle], epubFileName, currentPageSingle);

        //        this.Close();
        //    }

        }

        //ブラウザのピンチの処理
        private void windowsFormsHost1_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            double scale = Math.Max(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.Y);
            if (scale != 0)
            {
                webBrowser1.PageZoom *= (float)scale;
            }
        }

        //ツールバーを移動させる
        private void MoveMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (menuIsLeft) //左から右へ移動
            {
                Grid.SetColumn(windowsFormsHost1, 0);

                Grid.SetColumn(Button2, 3);
                Grid.SetColumn(Button3, 3);
                Grid.SetColumn(indexButton, 3);

                Grid.SetColumn(AnnotationButton, 3);
                Grid.SetColumn(SaveAnnotationButton, 3);
                Grid.SetColumn(ShowAnnotationButton, 3);
                Grid.SetColumn(SendButton, 3);
                Grid.SetColumn(RaunchDenoButton, 3);

                Grid.SetColumn(OpenContaintsAddinButton, 3);

                Grid.SetColumn(MoveMenuButton, 3);
                MoveMenuButton.Content = "👈";
                Grid.SetColumn(EasyModeButton, 3);
                Grid.SetColumn(PopupButton, 3);
                Grid.SetColumn(SpacingButton, 3);
                Grid.SetColumn(CloseButton, 3);

                menuIsLeft = false;
            }

            else //右から左へ移動
            {
                Grid.SetColumn(windowsFormsHost1, 2);

                Grid.SetColumn(Button2, 0);
                Grid.SetColumn(Button3, 0);
                Grid.SetColumn(indexButton, 0);

                Grid.SetColumn(AnnotationButton, 0);
                Grid.SetColumn(SaveAnnotationButton, 0);
                Grid.SetColumn(ShowAnnotationButton, 0);
                Grid.SetColumn(SendButton, 0);
                Grid.SetColumn(RaunchDenoButton, 0);

                Grid.SetColumn(OpenContaintsAddinButton, 0);

                Grid.SetColumn(MoveMenuButton, 0);
                MoveMenuButton.Content = "👉";
                Grid.SetColumn(EasyModeButton, 0);
                Grid.SetColumn(PopupButton, 0);
                Grid.SetColumn(SpacingButton, 0);
                Grid.SetColumn(CloseButton, 0);

                menuIsLeft = true;
            }
        }

        //ツールバーの表示。非表示切り替え
        private void EasyModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEasyMode) //イージーモードにする
            {
                Grid.SetColumn(windowsFormsHost1, 0);
                Grid.SetColumnSpan(windowsFormsHost1, 4);

                if (menuIsLeft)
                {
                    Grid.SetColumn(Button2, 1);
                    Grid.SetColumnSpan(Button2, 1);
                    Grid.SetRow(Button2, 15);

                    Grid.SetColumn(Button3, 4);
                    Grid.SetColumnSpan(Button3, 1);
                    Grid.SetRow(Button3, 15);

                    Grid.SetColumn(slider1, 2);
                    Grid.SetColumnSpan(slider1, 2);

                    EasyModeButton.Content = "Menu";
                    Grid.SetColumn(EasyModeButton, 0);
                    Grid.SetColumnSpan(EasyModeButton, 1);
                    Grid.SetRow(EasyModeButton, 15);
                }
                else
                {
                    Grid.SetColumn(Button2, 0);
                    Grid.SetColumnSpan(Button2, 1);
                    Grid.SetRow(Button2, 15);

                    Grid.SetColumn(Button3, 3);
                    Grid.SetColumnSpan(Button3, 1);
                    Grid.SetRow(Button3, 15);

                    Grid.SetColumn(slider1, 1);
                    Grid.SetColumnSpan(slider1, 2);

                    EasyModeButton.Content = "Menu";
                    Grid.SetColumn(EasyModeButton, 4);
                    Grid.SetColumnSpan(EasyModeButton, 1);
                    Grid.SetRow(EasyModeButton, 15);
                }

                indexButton.Visibility = System.Windows.Visibility.Collapsed;
                AnnotationButton.Visibility = System.Windows.Visibility.Collapsed;
                SaveAnnotationButton.Visibility = System.Windows.Visibility.Collapsed;
                ShowAnnotationButton.Visibility = System.Windows.Visibility.Collapsed;
                SendButton.Visibility = System.Windows.Visibility.Collapsed;
                RaunchDenoButton.Visibility = System.Windows.Visibility.Collapsed;

                OpenContaintsAddinButton.Visibility = System.Windows.Visibility.Collapsed;

                MoveMenuButton.Visibility = System.Windows.Visibility.Collapsed;
                PopupButton.Visibility = System.Windows.Visibility.Collapsed;
                SpacingButton.Visibility = System.Windows.Visibility.Collapsed;
                CloseButton.Visibility = System.Windows.Visibility.Collapsed;

                isEasyMode = true;
            }

            else //普通のメニューバーを表示させる
            {
                if (menuIsLeft)
                {
                    Grid.SetColumn(windowsFormsHost1, 2);
                    Grid.SetColumnSpan(windowsFormsHost1, 3);

                    Grid.SetColumn(Button2, 0);
                    Grid.SetColumnSpan(Button2, 2);
                    Grid.SetRow(Button2, 0);

                    Grid.SetColumn(Button3, 0);
                    Grid.SetColumnSpan(Button3, 2);
                    Grid.SetRow(Button3, 1);

                    EasyModeButton.Content = "EasyMode";
                    Grid.SetColumn(EasyModeButton, 0);
                    Grid.SetColumnSpan(EasyModeButton, 2);
                    Grid.SetRow(EasyModeButton, 10);

                    Grid.SetColumn(slider1, 0);
                    Grid.SetColumnSpan(slider1, 10);

                }
                else
                {
                    Grid.SetColumn(windowsFormsHost1, 0);
                    Grid.SetColumnSpan(windowsFormsHost1, 3);

                    Grid.SetColumn(Button2, 3);
                    Grid.SetColumnSpan(Button2, 2);
                    Grid.SetRow(Button2, 0);

                    Grid.SetColumn(Button3, 3);
                    Grid.SetColumnSpan(Button3, 2);
                    Grid.SetRow(Button3, 1);

                    EasyModeButton.Content = "EasyMode";
                    Grid.SetColumn(EasyModeButton, 3);
                    Grid.SetColumnSpan(EasyModeButton, 2);
                    Grid.SetRow(EasyModeButton, 10);

                    Grid.SetColumn(slider1, 0);
                    Grid.SetColumnSpan(slider1, 5);
                }

                indexButton.Visibility = System.Windows.Visibility.Visible;
                AnnotationButton.Visibility = System.Windows.Visibility.Visible;
                SaveAnnotationButton.Visibility = System.Windows.Visibility.Visible;
                ShowAnnotationButton.Visibility = System.Windows.Visibility.Visible;
                SendButton.Visibility = System.Windows.Visibility.Visible;
                RaunchDenoButton.Visibility = System.Windows.Visibility.Visible;

                OpenContaintsAddinButton.Visibility = System.Windows.Visibility.Visible;

                MoveMenuButton.Visibility = System.Windows.Visibility.Visible;
                PopupButton.Visibility = System.Windows.Visibility.Visible;
                SpacingButton.Visibility = System.Windows.Visibility.Visible;
                CloseButton.Visibility = System.Windows.Visibility.Visible;

                isEasyMode = false;
            }
        }

        //追加教材があったら開く
        private void OpenContentsAddinButton_Click(object sender, RoutedEventArgs e)
        {
            if (addinFilePath.Equals("0"))
            {
                return;
            }

            ContaintsAddInWindow caw = new ContaintsAddInWindow();
            caw.Owner = this;
            caw.Top = 10;
            caw.Left = 75;
            caw.Show();
            caw.init( addinFilePath, addinFilesDirectory );
        }

    } //MainWindow
} //epubReader4.0