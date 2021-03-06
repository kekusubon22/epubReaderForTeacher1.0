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

using System.IO;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Ink;

using System.Runtime.InteropServices;
using System.Management;


namespace epubReaderForTeacher1._0
{
    /// <summary>
    /// PNGWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGWindow : Window
    {
        public PNGWindow()
        {
            InitializeComponent();
        }

        //epub解析のための変数
        string thawPath;
        string epubFileName;
        string epubDirectory;
        string[] opfHref = new string[256];
        string[] pageContent = new string[256];
        string[] xhtmlPage = new string[256];
        int pageLim = 0;
        int currentPageNum;

        //スライダーに関する変数
        double slider;
        double x = 0;

        //メニューバーに関する変数
        bool menuIsLeft = true;
        bool isEasyMode = false;

        //デジタルノートシステムと連携させるための変数
        string subjectName;
        string[] unitName = new string[32];
        string notePath;
        string dinoNowOpening = "none";

        //教材追加機能に関する変数
        string addinDirectory;
        string myAddinDirectory;
        string cameraRollDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Camera Roll";

        //表示している画像の元の大きさを表す変数
        int picWidth;
        int picHeight;

        //ウインドウの実際の大きさを示す変数
        double image1w;
        double image1h;

        //実際の倍率からどれくらい拡大(縮小)しているか
        double resizeRate = 1;

        //imageの左右に余白があった場合、その数値
        double imageSpace = 0;

        //アノテーションに必要な変数
        List<StrokeLine> strokeLines = new List<StrokeLine>();
        List<System.Windows.Point> points;
        System.Windows.Media.Color color = new System.Windows.Media.Color();
        List<DrawingAttributes> inkDAs = new List<DrawingAttributes>();
        bool isFreeLine = true;
        bool canvasDisplay = true;
        int inkWidth;
        int strokeId = 0;
        bool drawFlag = false;

        //マウスイベントに必要な変数
        System.Windows.Point startP = new System.Windows.Point();
        System.Windows.Point prevP = new System.Windows.Point();
        System.Windows.Point finP = new System.Windows.Point();
        bool dragging = false;
        int counter = 0;
        bool elementSelected = false;
        int selectedElementNum = -1;

        //ポップアップ・スペーシングのために必要な変数
        string popupDirectory;
        List<Element> elementList = new List<Element>();
        bool spacingNow = false;
        int spaceHeight = 200;
        string position = "none"; //スペーシングが必要な位置
        double spaceY = 0; //スペーシングが始まるy座標

        //動作のログ
        List<LearningLog> learningLogs = new List<LearningLog>();

        //ユーザ情報を表すオブジェクト
        User user = new User();

        //初期処理
        public void init(string epubDirectory, string epubFileName, int pageNum)
        {
            this.epubFileName = epubFileName;
            this.epubDirectory = epubDirectory;

            //解凍先とcontent.opfのパス
            thawPath = epubDirectory + "\\" + epubFileName.Replace(".epub", "");
            string opfPath = thawPath + "\\OEBPS\\content.opf";

            //Popup画像の置場
            popupDirectory = thawPath + "\\PopupImage";
            System.IO.Directory.CreateDirectory(popupDirectory);

            //ここからcontent.opfを解析する
            int i = 0;
            int k = 0;
            int l = 0;

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

                    string strItemHref = "start.";
                    while (true)
                    {
                        try
                        {
                            // 取得した<item>要素はXmlNodeListなのでXmlElementにキャストする
                            XmlElement xmlName = (XmlElement)xmlNode.Item(i);

                            // <item>要素のhrefの属性値を取得します
                            strItemHref = xmlName.GetAttribute("href");
                            opfHref[i] = strItemHref;

                            //content.opfからxhtmlのパスを取得する
                            if (opfHref[i].Contains(".xhtml") || opfHref[i].Contains(".html"))
                            {
                                xhtmlPage[k] = thawPath + "\\OEBPS\\" + opfHref[i];
                                pageContent[k] = thawPath + "\\OEBPS\\image\\" + opfHref[i].Replace("xhtml", "png");
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
                                k++;
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

            //1ページ目から読み込ませる
            currentPageNum = 0;
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(pageContent[currentPageNum]);
            bmp.EndInit();
            image1.Source = bmp;

            //画像の幅と高さを取得
            Bitmap bmpBase = new Bitmap(pageContent[currentPageNum]);
            picWidth = bmpBase.Width;
            picHeight = bmpBase.Height;
            bmpBase.Dispose();

            //さいしょはかけない
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;

            //色の初期値として黒を指定
            DrawingAttributes inkDA = new DrawingAttributes();
            color = System.Windows.Media.Color.FromArgb(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
            inkDA.Color = color;

            //太さの初期値は3
            inkWidth = 3;
            inkDA.Width = 3;
            inkDA.Height = 3;
            inkCanvas1.DefaultDrawingAttributes = inkDA;
            inkCanvas1.AllowDrop = true;

            //inkCanvas1にマウスイベントを設定
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseDown), true);
            inkCanvas1.AddHandler(InkCanvas.MouseMoveEvent, new MouseEventHandler(inkCanvas1_MouseMove), true);
            inkCanvas1.AddHandler(InkCanvas.MouseUpEvent, new MouseButtonEventHandler(inkCanvas1_MouseUp), true);

            //image1にマウスイベントを設定
            //image1.AddHandler(System.Windows.Controls.Image.MouseDownEvent, new MouseButtonEventHandler(image1_MouseDown), true);
            //image1.AddHandler(System.Windows.Controls.Image.MouseMoveEvent, new MouseEventHandler(image1_MouseMove), true);
            //image1.AddHandler(System.Windows.Controls.Image.MouseUpEvent, new MouseButtonEventHandler(image1_MouseUp), true);

            //要素の情報をxhtmlから取得する
            setElementInfo();

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

            //ユーザ情報
            user.SetId("Administrator");
            user.SetType("administrator");
        }

        //imageコンポーネントと表示倍率の取得
        public void setImageInfo()
        {
            //Imageコンポーネントの実際の大きさ
            image1w = image1.ActualWidth;
            image1h = image1.ActualHeight;

            //表示倍率の取得
            resizeRate = image1w / (double)picWidth;
            resizeRate = image1h / (double)picHeight;

            //imageの左右の余白の大きさ
            imageSpace = ((double)this.Width - Button2.ActualWidth - image1w) / (double)2;
        }

        //現在のページの要素の情報をセットするメソッド
        public void setElementInfo()
        {
            elementList = new List<Element>();

            // XmlDocumentオブジェクトを作成
            XmlDocument xhtmlDoc = new XmlDocument();
            xhtmlDoc.Load(xhtmlPage[currentPageNum]);
            try
            {
                // ルートの要素を取得
                XmlElement xhtmlRoot = xhtmlDoc.DocumentElement;

                // <element>要素をセット
                XmlNodeList xhtmlNode = xhtmlRoot.GetElementsByTagName("element");

                int i = 0;
                while (true)
                {
                    try
                    {
                        // 取得した<element>要素はXmlNodeListなのでXmlElementにキャストする
                        XmlElement xhtmlName = (XmlElement)xhtmlNode.Item(i);

                        // <element>要素の各属性値をリストに格納
                        Element element = new Element();
                        element.SetId(xhtmlName.GetAttribute("id"));
                        element.SetX(Int32.Parse(xhtmlName.GetAttribute("x")));
                        element.SetY(Int32.Parse(xhtmlName.GetAttribute("y")));
                        element.SetWidth(Int32.Parse(xhtmlName.GetAttribute("width")));
                        element.SetHeight(Int32.Parse(xhtmlName.GetAttribute("height")));
                        elementList.Add(element);
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

        //もどるボタン
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ReleaseElementSelected();

            if (currentPageNum == 0)
            {
                MessageBox.Show("最初のページです。");
                return;
            }

            SaveAnnotateRecord();
            currentPageNum--;

            //読み込ませる
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(pageContent[currentPageNum]);
            bmp.EndInit();
            image1.Source = bmp;

            //画像の幅と高さを取得
            Bitmap bmpBase = new Bitmap(pageContent[currentPageNum]);
            picWidth = bmpBase.Width;
            picHeight = bmpBase.Height;
            bmpBase.Dispose();

            //描画情報をクリア
            strokeLines.Clear();
            learningLogs.Clear();
            inkCanvas1.Strokes.Clear();
            strokeId = 0;
            drawFlag = false;

            //要素の情報をxhtmlから取得する
            setElementInfo();

            //前に描画した情報があれば読み込む
            try
            {
                RoadAnnotateRecord();
                drawAll();
                strokeId = strokeLines.Count;
            }
            catch
            {
                //MessageBox.Show("再読み込み失敗");
            }
        }

        //すすむボタン
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ReleaseElementSelected();

            try
            {
                SaveAnnotateRecord();
                currentPageNum++;

                //読み込ませる
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(pageContent[currentPageNum]);
                bmp.EndInit();
                image1.Source = bmp;

                //画像の幅と高さを取得
                Bitmap bmpBase = new Bitmap(pageContent[currentPageNum]);
                picWidth = bmpBase.Width;
                picHeight = bmpBase.Height;
                bmpBase.Dispose();

                //描画情報をクリア
                strokeLines.Clear();
                learningLogs.Clear();
                inkCanvas1.Strokes.Clear();
                strokeId = 0;
                drawFlag = false;

                //要素の情報をxhtmlから取得する
                setElementInfo();

                //前に描画した情報があれば読み込む
                try
                {
                    RoadAnnotateRecord();
                    drawAll();
                    strokeId = strokeLines.Count;
                }
                catch
                {
                    //MessageBox.Show("再読み込み失敗");
                }

            }

            catch
            {
                MessageBox.Show("最後のページです。");
            }
        }

        //スライダーの値を変えたとき
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        //本一覧へ戻る
        private void indexButton_Click(object sender, RoutedEventArgs e)
        {
            SelectEpubWindow dialog = new SelectEpubWindow();
            dialog.Show();
        }

        //アノテーション機能にする
        private void AnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            ReleaseElementSelected();

            inkCanvas1.Visibility = System.Windows.Visibility.Visible;
            PNGAnnotationToolsWindow paw = new PNGAnnotationToolsWindow();
            paw.Owner = this;
            paw.Show();
        }

        //部分キャプチャ
        private void SaveAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureWindow cw = new CaptureWindow();
            cw.Owner = this;
            cw.setepubInfo(thawPath, epubFileName, pageContent[currentPageNum], subjectName, unitName[currentPageNum], false);
            cw.Show();
        }

        //学習記録（キャプチャ）の閲覧
        private void ShowAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            PNGSelectHowToShowWindow pshsw = new PNGSelectHowToShowWindow();
            pshsw.Owner = this;
            pshsw.Show();

            if (elementSelected)
            {
                pshsw.init(elementList[selectedElementNum].GetId() + ".png", epubDirectory, epubFileName, user);
            }
            else
            {
                pshsw.init(pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", ""), epubDirectory, epubFileName, user);
            }
        }

        //教材追加ボタン
        private void AddContaintsButton_Click(object sender, RoutedEventArgs e)
        {
            //この教師用アプリケーションを起動しているPCが親機となり、教材置き場になる
            //要素が選択されていたらそれと関連付ける
            if (elementSelected)
            {
                myAddinDirectory =
                    epubDirectory.Replace("epub", "Addin") + "\\Administrator\\" + epubFileName.Replace(".epub", "") + "\\" + elementList[selectedElementNum].GetId();
            }
            //選択されていなければ単元と関連付ける
            else
            {
                myAddinDirectory =
                    epubDirectory.Replace("epub", "Addin") + "\\Administrator\\" + epubFileName.Replace(".epub", "") + "\\" + unitName[currentPageNum];

            }

            //管理者用のアドインファイル置き場がなければつくる
            if (!Directory.Exists(myAddinDirectory))
            {
                Directory.CreateDirectory(myAddinDirectory);
            }

            //管理者のアドイン置き場とカメラロールのパスを渡して一覧表示
            SelectMyAddinWindow smaw = new SelectMyAddinWindow();
            smaw.Owner = this;
            smaw.Show();
            smaw.init(cameraRollDirectory, myAddinDirectory, user);
        }

        //追加教材閲覧機能ボタン
        private void OpenContaintsAddinButton_Click(object sender, RoutedEventArgs e)
        {
            //この教師用アプリケーションを起動しているPCが親機となり、教材置き場になる
            //アドインファイル置き場のパス
            addinDirectory = epubDirectory.Replace("epub", "Addin");

            //誰の教材を表示するか選択する画面へ
            SelectWhoseAddinWindow swaw = new SelectWhoseAddinWindow();
            swaw.Owner = this;
            swaw.Show();

            //要素が選択されていればその要素についての、選択されていなければ単元についての教材を表示
            if (elementSelected)
            {
                swaw.init(addinDirectory, epubFileName, elementList[selectedElementNum].GetId(), user);
            }
            else
            {
                swaw.init(addinDirectory, epubFileName, unitName[currentPageNum], user);
            }
        }

        //ポップアップボタン
        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            if (elementSelected && selectedElementNum >= 0)
            {
                string popupFileName = popupDirectory + "\\" + elementList[selectedElementNum].GetId() + ".png";

                //ポップアップ画像がなければ作成
                if (!File.Exists(popupFileName))
                {
                    Bitmap bmpBase = new Bitmap(pageContent[currentPageNum]);
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
                        elementList[selectedElementNum].GetX(),
                        elementList[selectedElementNum].GetY(),
                        elementList[selectedElementNum].GetWidth(),
                        elementList[selectedElementNum].GetHeight()
                    );
                    Bitmap bmpNew = bmpBase.Clone(rect, bmpBase.PixelFormat);
                    bmpNew.Save(popupFileName, ImageFormat.Png);
                }

                //PopupWindowにパスを渡す
                PNGPopupWindow ppw = new PNGPopupWindow();
                ppw.Owner = this;
                ppw.Show();
                ppw.init(popupFileName, thawPath);
            }
        }

        //スペーシングボタン
        private void SpacingButton_Click(object sender, RoutedEventArgs e)
        {
            //スペーシングの処理
            if (!spacingNow)
            {
                if (!elementSelected)
                {
                    return;
                }

                if (!Directory.Exists(thawPath + "\\SpacingImages"))
                {
                    Directory.CreateDirectory(thawPath + "\\SpacingImages");
                }

                //対象の要素を境に紙面を分割
                FileStream fs = new FileStream(pageContent[currentPageNum], FileMode.Open, FileAccess.Read);
                Bitmap originImage = (Bitmap)System.Drawing.Bitmap.FromStream(fs);
                fs.Close();
                string spacingFileName = thawPath + "\\SpacingImages\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "");

                spaceY = elementList[selectedElementNum].GetX() + elementList[selectedElementNum].GetHeight();
                System.Drawing.Rectangle upperRect;
                System.Drawing.Rectangle downerRect;
                System.Drawing.Rectangle anotherRect;

                //対象要素が左側にあるとき
                if (picWidth / 2 > elementList[selectedElementNum].GetX())
                {
                    position = "left";

                    //対象の要素より上側の領域を指定
                    upperRect = new System.Drawing.Rectangle(
                        0,
                        0,
                        picWidth / 2,
                        elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight());

                    //Grid位置の指定
                    Grid.SetColumn(upperSideImage, 0);
                    rowDefinition1.Height = new GridLength(elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight(), GridUnitType.Pixel);

                    //対象の要素より下側の領域を指定
                    downerRect = new System.Drawing.Rectangle(
                        0,
                        elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight(),
                        picWidth / 2,
                        picHeight - (elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight()));

                    //Grid位置の指定
                    Grid.SetColumn(downerSideImage, 0);
                    rowDefinition3.Height = new GridLength(picHeight - (elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight()), GridUnitType.Pixel);

                    //対象の要素と反対側の領域を指定
                    anotherRect = new System.Drawing.Rectangle(
                        picWidth / 2,
                        0,
                        picWidth / 2,
                        picHeight);

                    //Grid位置の指定
                    Grid.SetColumn(anotherSideImage, 1);
                    Grid.SetColumn(emptyImage, 0);
                    rowDefinition2.Height = new GridLength(200, GridUnitType.Pixel);
                }

                //対象要素が右側にあるとき
                else
                {
                    position = "right";

                    //対象の要素より上側の領域指定
                    upperRect = new System.Drawing.Rectangle(
                        picWidth / 2,
                        0,
                        picWidth / 2,
                        elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight());

                    //Grid位置の指定
                    Grid.SetColumn(upperSideImage, 1);
                    rowDefinition1.Height = new GridLength(elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight(), GridUnitType.Pixel);

                    //対象の要素より下側の領域を指定
                    downerRect = new System.Drawing.Rectangle(
                        picWidth / 2,
                        elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight(),
                        picWidth / 2,
                        picHeight - (elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight()));

                    //Grid位置の指定
                    Grid.SetColumn(downerSideImage, 1);
                    rowDefinition3.Height = new GridLength(picHeight - (elementList[selectedElementNum].GetY() + elementList[selectedElementNum].GetHeight()), GridUnitType.Pixel);

                    //対象の要素と反対側の領域を指定
                    anotherRect = new System.Drawing.Rectangle(
                        0,
                        0,
                        picWidth / 2,
                        picHeight);

                    //Grid位置の指定
                    Grid.SetColumn(anotherSideImage, 0);
                    Grid.SetColumn(emptyImage, 1);
                    rowDefinition2.Height = new GridLength(200, GridUnitType.Pixel);
                }

                //トリミングと保存の処理
                using (Bitmap upperImage = originImage.Clone(upperRect, originImage.PixelFormat))
                {
                    FileStream ufs = new FileStream(spacingFileName + "_upper.png", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    upperImage.Save(ufs, ImageFormat.Png);
                    ufs.Close();
                }
                using (Bitmap downerImage = originImage.Clone(downerRect, originImage.PixelFormat))
                {
                    FileStream dfs = new FileStream(spacingFileName + "_downer.png", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    downerImage.Save(dfs, ImageFormat.Png);
                    dfs.Close();
                }
                using (Bitmap anotherImage = originImage.Clone(anotherRect, originImage.PixelFormat))
                {
                    FileStream afs = new FileStream(spacingFileName + "_another.png", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    anotherImage.Save(afs, ImageFormat.Png);
                    afs.Close();
                }

                //元の画像を非表示にして分割した３つのimageを表示する
                image1.Visibility = System.Windows.Visibility.Collapsed;

                //上側
                BitmapImage ubi = new BitmapImage();
                using (FileStream uifs = new FileStream(spacingFileName + "_upper.png", FileMode.Open, FileAccess.Read))
                {
                    ubi.BeginInit();
                    ubi.CacheOption = BitmapCacheOption.OnLoad;
                    ubi.StreamSource = uifs;
                    ubi.EndInit();
                }
                upperSideImage.Source = ubi;
                upperSideImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                upperSideImage.Visibility = System.Windows.Visibility.Visible;

                //下側
                BitmapImage dbi = new BitmapImage();
                using (FileStream difs = new FileStream(spacingFileName + "_downer.png", FileMode.Open, FileAccess.Read))
                {
                    dbi.BeginInit();
                    dbi.CacheOption = BitmapCacheOption.OnLoad;
                    dbi.StreamSource = difs;
                    dbi.EndInit();
                }
                downerSideImage.Source = dbi;
                downerSideImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                downerSideImage.Visibility = System.Windows.Visibility.Visible;


                //反対側
                BitmapImage abi = new BitmapImage();
                using (FileStream aifs = new FileStream(spacingFileName + "_another.png", FileMode.Open, FileAccess.Read))
                {
                    abi.BeginInit();
                    abi.CacheOption = BitmapCacheOption.OnLoad;
                    abi.StreamSource = aifs;
                    abi.EndInit();
                }
                anotherSideImage.Source = abi;
                anotherSideImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                anotherSideImage.Visibility = System.Windows.Visibility.Visible;

                //空白の画像
                emptyImage.Stretch = Stretch.Fill;
                emptyImage.StretchDirection = StretchDirection.Both;
                BitmapImage wbi = new BitmapImage();
                using (FileStream wifs = new FileStream(Directory.GetCurrentDirectory() + "\\..\\..\\..\\images\\white.jpg", FileMode.Open, FileAccess.Read))
                {
                    wbi.BeginInit();
                    wbi.CacheOption = BitmapCacheOption.OnLoad;
                    wbi.StreamSource = wifs;
                    wbi.EndInit();
                }
                emptyImage.Source = wbi;
                emptyImage.Visibility = System.Windows.Visibility.Visible;

                //要素の選択を解除する
                rect1.Visibility = System.Windows.Visibility.Hidden;
                elementSelected = false;
                PopupButton.Visibility = System.Windows.Visibility.Hidden;
                //SpacingButton.Visibility = System.Windows.Visibility.Hidden;
                selectedElementNum = -1;

                spacingNow = true;
                SpacingButton.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\..\\icon\\Spacing2.png", UriKind.Relative)));
            }

            //スペーシングの解除    
            else
            {
                //
                upperSideImage.Source = null;
                downerSideImage.Source = null;
                anotherSideImage.Source = null;

                //
                upperSideImage.Visibility = System.Windows.Visibility.Hidden;
                downerSideImage.Visibility = System.Windows.Visibility.Hidden;
                anotherSideImage.Visibility = System.Windows.Visibility.Hidden;
                emptyImage.Visibility = System.Windows.Visibility.Hidden;
                image1.Visibility = System.Windows.Visibility.Visible;
                image1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                rect1.Visibility = System.Windows.Visibility.Visible;

                //行グリッドの高さを戻す
                rowDefinition1.Height = new GridLength(picHeight / 3, GridUnitType.Pixel);
                rowDefinition2.Height = new GridLength(picHeight / 3, GridUnitType.Pixel);
                rowDefinition3.Height = new GridLength(picHeight / 3, GridUnitType.Pixel);

                spacingNow = false;
                position = "none";
                SpacingButton.Visibility = System.Windows.Visibility.Hidden;
                SpacingButton.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\..\\icon\\Spacing1.png", UriKind.Relative)));
            }
        }

        //ストロークをを下げる
        private void moveDownStrokes(int targetY)
        {
            bool needToRedraw = false;

            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //始点がスペースを空けた部分より下にあるストロークをとりだし、すべてのy座標を変える
                //上下の判定
                if (sl.GetPoints()[0].Y > targetY)
                {
                    //左右の判定
                    if ((position.Equals("left") && sl.GetPoints()[0].X < image1w / 2) || (position.Equals("right") && sl.GetPoints()[0].X > image1w / 2))
                    {
                        List<System.Windows.Point> newPoints = new List<System.Windows.Point>();
                        for (int j = 0; j < sl.GetPoints().Count; j++)
                        {
                            double x = sl.GetPoints()[j].X;
                            double y = sl.GetPoints()[j].Y;
                            y += spaceHeight;
                            newPoints.Add(new System.Windows.Point(x, y));
                        }
                        strokeLines[i].SetPoints(newPoints);
                        strokeLines[i].SetDownNow(true);
                        needToRedraw = true;
                    }
                }
            }

            if (needToRedraw)
            {
                ClearStrokes();
                drawAll();
            }

        }

        //ストロークをを上げる
        private void moveUpStrokes()
        {
            bool needToRedraw = false;

            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //下がっているストロークを取り出し、y座標をもとにもどす
                if (sl.GetDownNow())
                {
                    List<System.Windows.Point> newPoints = new List<System.Windows.Point>();
                    for (int j = 0; j < sl.GetPoints().Count; j++)
                    {
                        double x = sl.GetPoints()[j].X;
                        double y = sl.GetPoints()[j].Y;
                        y -= spaceHeight;
                        newPoints.Add(new System.Windows.Point(x, y));

                    }
                    strokeLines[i].SetPoints(newPoints);
                    strokeLines[i].SetDownNow(false);
                    needToRedraw = true;
                }
            }

            if (needToRedraw)
            {
                ClearStrokes();
                drawAll();
            }
        }

        //スペース内にあるストロークを隠す
        private void hideStrokesInSpace()
        {
            bool needToRedraw = false;

            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //スペースの範囲にあるストロークを取り出し、y座標をもとにもどす
                //上下の判定
                if (sl.GetPoints()[0].Y > spaceY && spaceY + spaceHeight > sl.GetPoints()[0].Y)
                {
                    //左右の判定
                    if ((position.Equals("left") && sl.GetPoints()[0].X < image1w / 2) || (position.Equals("right") && sl.GetPoints()[0].X > image1w / 2))
                    {
                        strokeLines[i].SetInSpace(true);
                        needToRedraw = true;
                    }
                }
            }

            if (needToRedraw)
            {
                ClearStrokes();
                drawAll();
            }
        }

        //動作の履歴を別画面で再生
        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            ReviewAnnotationWindow raw = new ReviewAnnotationWindow();
            raw.Owner = this;
            raw.Show();
            raw.init(pageContent[currentPageNum], strokeLines, learningLogs);
        }

        //閉じるボタン
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("epubReaderを終了しますか？", "かくにん", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                SaveAnnotateRecord();
                this.Close();
            }
        }

        //描画処理（マウス用）
        private void inkCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //UIElement el = sender as UIElement;
            ////Console.WriteLine("まうすがおされたよ");

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

        //描画処理（マウス用）
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
            //    Stroke stroke = new Stroke(spc, inkDAs.Last());
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

        //描画処理（マウス用）
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
            //    Stroke stroke = new Stroke(spc, inkDAs.Last());
            //    inkCanvas1.Strokes.Add(stroke);

            //    //配列strokeLinesに追加
            //    StrokeLine strokeLine = new StrokeLine();
            //    strokeLine.SetId(strokeId);
            //    strokeLine.SetPoints(points);
            //    strokeLine.SetColor(color);
            //    strokeLine.SetWidth(inkWidth);
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
            //    strokeLine.SetWidth(inkWidth);
            //    strokeLine.SetDownNow(false);
            //    strokeLine.SetInSpace(false);
            //    strokeLine.SetEreased(false);

            //    strokeLines.Add(strokeLine);

            //    dragging = false;
            //    counter = 0;
            //}
        }

        //描画処理（ペン用）
        private void inkCanvas1_StylusDown(object sender, StylusDownEventArgs e)
        {
            UIElement el = sender as UIElement;
            //Console.WriteLine("まうすがおされたよ");

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

        //描画処理（ペン用）
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
                Stroke stroke = new Stroke(spc, inkDAs.Last());
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
                Stroke stroke = new Stroke(spc, inkDAs.Last());
                inkCanvas1.Strokes.Add(stroke);

                counter++;
            }
        }

        //描画処理（ペン用）
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
                Stroke stroke = new Stroke(spc, inkDAs.Last());
                inkCanvas1.Strokes.Add(stroke);

                //配列strokeLinesに追加
                StrokeLine strokeLine = new StrokeLine();
                strokeLine.SetId(strokeId);
                strokeLine.SetPoints(points);
                strokeLine.SetColor(color);
                strokeLine.SetWidth(inkWidth);
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
                Stroke stroke = new Stroke(spc, inkDAs.Last());
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
                strokeLine.SetWidth(inkWidth);
                strokeLine.SetDownNow(false);
                strokeLine.SetInSpace(false);
                strokeLine.SetEreased(false);

                strokeLines.Add(strokeLine);

                dragging = false;
                counter = 0;
            }
        }

        //要素選択処理 mousedown
        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setImageInfo();

            //要素を選択する
            if (!elementSelected)
            {
                //現在のポインタの位置の取得
                System.Drawing.Point dp = System.Windows.Forms.Cursor.Position;
                System.Windows.Point wp = new System.Windows.Point(dp.X, dp.Y);

                //マウス座標から論理座標に変換
                PresentationSource src = PresentationSource.FromVisual(this);
                CompositionTarget ct = src.CompositionTarget;
                System.Windows.Point p = ct.TransformFromDevice.Transform(wp);

                double nowX = p.X - Button2.ActualWidth - imageSpace;
                double nowY = p.Y;

                //MessageBox.Show("x=" + elementList[0].GetX());
                //MessageBox.Show("y=" + elementList[0].GetY());
                //MessageBox.Show("width=" + elementList[0].GetWidth());
                //MessageBox.Show("height=" + elementList[0].GetHeight());

                int i = 0;
                while (true)
                {
                    try
                    {
                        bool xOK = (double)elementList[i].GetX() * resizeRate < nowX && nowX < (double)elementList[i].GetX() * resizeRate + (double)elementList[i].GetWidth() * resizeRate;
                        bool yOK = (double)elementList[i].GetY() * resizeRate < nowY && nowY < (double)elementList[i].GetY() * resizeRate + (double)elementList[i].GetHeight() * resizeRate;
                        if (xOK && yOK)
                        {
                            double marginLeft = elementList[i].GetX() * resizeRate + imageSpace;
                            double marginTop = elementList[i].GetY() * resizeRate;
                            double marginRight = image1w - (elementList[i].GetX() * resizeRate) - (elementList[i].GetWidth() * resizeRate) + imageSpace;
                            double marginBottom = image1h - (elementList[i].GetY() * resizeRate) - (elementList[i].GetHeight() * resizeRate);

                            rect1.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
                            rect1.Visibility = System.Windows.Visibility.Visible;

                            elementSelected = true;
                            PopupButton.Visibility = System.Windows.Visibility.Visible;
                            //SpacingButton.Visibility = System.Windows.Visibility.Visible;
                            selectedElementNum = i;
                            break;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            //要素の選択を解除する
            else
            {
                rect1.Visibility = System.Windows.Visibility.Hidden;
                elementSelected = false;
                PopupButton.Visibility = System.Windows.Visibility.Hidden;
                SpacingButton.Visibility = System.Windows.Visibility.Hidden;
                selectedElementNum = -1;
            }
        }

        //要素選択処理 mousemove
        private void image1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        //要素選択処理 mouseup
        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        //要素の選択を解除する
        private void ReleaseElementSelected()
        {
            rect1.Visibility = System.Windows.Visibility.Hidden;
            elementSelected = false;
            PopupButton.Visibility = System.Windows.Visibility.Hidden;
            SpacingButton.Visibility = System.Windows.Visibility.Hidden;
            selectedElementNum = -1;
        }

        //ピンチイン / ピンチアウトの処理
        private void image1_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {

        }

        //色変更
        public void ChangeColor(int a, int r, int g, int b)
        {
            DrawingAttributes inkDA = new DrawingAttributes();
            color = System.Windows.Media.Color.FromArgb(Convert.ToByte(a), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
            inkDA.Color = color;
            inkDA.Width = inkWidth;
            inkDA.Height = inkWidth;
            inkDAs.Add(inkDA);
            inkCanvas1.DefaultDrawingAttributes = inkDAs.Last();
        }

        //直線・自由線切り替え
        public void ChangeStrokeMode(string s)
        {
            //直線モードにする
            if (s.Equals("straight"))
            {
                isFreeLine = false;
                inkCanvas1.EditingMode = InkCanvasEditingMode.None;
            }

            //自由線モードにする
            else if (s.Equals("free"))
            {
                isFreeLine = true;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //太さ変更
        public void ChangeWidth(int value)
        {
            DrawingAttributes inkDA = new DrawingAttributes();
            inkWidth = value;
            inkDA.Width = value;
            inkDA.Height = value;
            inkDA.Color = color;
            inkDAs.Add(inkDA);
            inkCanvas1.DefaultDrawingAttributes = inkDAs.Last();
        }

        //すべての線を再描画
        private void drawAll()
        {
            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //消去済みでなく、隠れたスペースに書いていないストローク以外を再描画
                if (!sl.GetInSpace() && !sl.GetEreased())
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
            inkCanvas1.DefaultDrawingAttributes = inkDAs.Last();
        }

        //ひとつ戻る
        public void Undo()
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

        //すべての線を消去
        public void ClearStrokes()
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

        //ストローク情報の保存
        private void SaveAnnotateRecord()
        {
            if (!drawFlag)
            {
                return;
            }

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
                thawPath + "\\Strokes\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));
            StreamWriter lsw = new StreamWriter(
                thawPath + "\\LearningLog\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));

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
            string strokeXmlPath = thawPath + "\\Strokes\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "") + ".xml";
            string logXmlPath = thawPath + "\\LearningLog\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "") + ".xml";

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

        //ストローク情報と動作履歴を削除する
        private void DeleteLearningLog()
        {
            //動作のログを削除する
            if (Directory.Exists(thawPath + "\\Strokes"))
            {
                Directory.Delete(thawPath + "\\Strokes", true);
            }
            if (Directory.Exists(thawPath + "\\LearningLog"))
            {
                Directory.Delete(thawPath + "\\LearningLog", true);
            }
        }

        //紙面全体をキャプチャ
        public void ImageCaptureAll()
        {
            string k = null;
            string searchPageName = pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\", "");
            //MessageBox.Show(searchPageName);

            //保存先にページ.pngが何枚保存されているか調べる
            string[] files = System.IO.Directory.GetFiles(thawPath + "\\LearningRecord", searchPageName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

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
            string savePath = pageContent[currentPageNum].Replace("\\OEBPS\\image", "\\LearningRecord") + "_" + k + ".png";
            CaptureScreen(savePath);
        }

        //スクリーンショットの処理
        public void CaptureScreen(string saveFileName)
        {
            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size canvasSize = new System.Drawing.Size();

            browserLeftTop = image1.PointToScreen(new System.Windows.Point(0.0, 0.0));
            canvasSize.Height = (int)image1.RenderSize.Height;
            canvasSize.Width = (int)image1.RenderSize.Width;


            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
            rc.Size = canvasSize;

            ImageBrush ib = new ImageBrush();

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);

                    ib.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

                //スクリーンショットの保存
                bmp.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        //以下、ファイル共有関係のコード（おれもよくわかんない笑）
        /* 
        * WNetGetUniversalNameをインポートする
        */
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int
            WNetGetUniversalName(
            string lpLocalPath,                                 // ネットワーク資源のパス 
            [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,      // 情報のレベル
            IntPtr lpBuffer,                                    // 名前バッファ
            [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize  // バッファのサイズ
        );


        /*
         * dwInfoLevelに指定するパラメータ
         *  lpBuffer パラメータが指すバッファで受け取る構造体の種類を次のいずれかで指定
         */
        const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        const int REMOTE_NAME_INFO_LEVEL = 0x00000002; //こちらは、テストしていない


        /*
         * lpBufferで受け取る構造体
         */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct UNIVERSAL_NAME_INFO
        {
            public string lpUniversalName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct _REMOTE_NAME_INFO  //こちらは、テストしていない
        {
            string lpUniversalName;
            string lpConnectionName;
            string lpRemainingPath;
        }

        /* エラーコード一覧
        * WNetGetUniversalName固有のエラーコード
        *   http://msdn.microsoft.com/ja-jp/library/cc447067.aspx
        * System Error Codes (0-499)
        *   http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx
        */
        const int NO_ERROR = 0;
        const int ERROR_NOT_SUPPORTED = 50;
        const int ERROR_MORE_DATA = 234;
        const int ERROR_BAD_DEVICE = 1200;
        const int ERROR_CONNECTION_UNAVAIL = 1201;
        const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        const int ERROR_EXTENDED_ERROR = 1208;
        const int ERROR_NO_NETWORK = 1222;
        const int ERROR_NOT_CONNECTED = 2250;

        /*
        * UNC変換ロジック本体
        */
        public static string GetUniversalName(string path_src)
        {
            string unc_path_dest = path_src; //解決できないエラーが発生した場合は、入力されたパスをそのまま戻す
            int size = 1;

            /*
             * 前処理
             *   意図的に、ERROR_MORE_DATAを発生させて、必要なバッファ・サイズ(size)を取得する。
             */
            //1バイトならば、確実にERROR_MORE_DATAが発生するだろうという期待。
            IntPtr lp_dummy = Marshal.AllocCoTaskMem(size);

            //サイズ取得をトライ
            int apiRetVal = WNetGetUniversalName(path_src, UNIVERSAL_NAME_INFO_LEVEL, lp_dummy, ref size);

            //ダミーを解放
            Marshal.FreeCoTaskMem(lp_dummy);
            /*
                        * UNC変換処理
                        */
            switch (apiRetVal)
            {
                case ERROR_MORE_DATA:
                    //受け取ったバッファ・サイズ(size)で再度メモリ確保
                    IntPtr lpBufUniversalNameInfo = Marshal.AllocCoTaskMem(size);

                    //UNCパスへの変換を実施する。
                    apiRetVal = WNetGetUniversalName(path_src, UNIVERSAL_NAME_INFO_LEVEL, lpBufUniversalNameInfo, ref size);

                    //UNIVERSAL_NAME_INFOを取り出す。
                    UNIVERSAL_NAME_INFO a = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(lpBufUniversalNameInfo, typeof(UNIVERSAL_NAME_INFO));

                    //バッファを解放する
                    Marshal.FreeCoTaskMem(lpBufUniversalNameInfo);

                    if (apiRetVal == NO_ERROR)
                    {
                        //UNCに変換したパスを返す
                        unc_path_dest = a.lpUniversalName;
                    }
                    else
                    {
                        //MessageBox.Show(path_src +"ErrorCode:" + apiRetVal.ToString());
                    }
                    break;

                case ERROR_BAD_DEVICE: //すでにUNC名(\\servername\test)
                case ERROR_NOT_CONNECTED: //ローカル・ドライブのパス(C:\test)
                    //MessageBox.Show(path_src +"\nErrorCode:" + apiRetVal.ToString());
                    break;
                default:
                    //MessageBox.Show(path_src + "\nErrorCode:" + apiRetVal.ToString());
                    break;

            }

            return unc_path_dest;
        }

        
    }
}