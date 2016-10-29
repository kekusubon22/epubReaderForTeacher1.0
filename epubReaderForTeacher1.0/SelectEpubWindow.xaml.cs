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

using Microsoft.Win32;
using Ionic.Zip;
using Ionic.Zlib;
using System.Xml;
using System.IO;


//using System.Delegate;
//using System.Reflection.EventInfo;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// SelectEpubWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectEpubWindow : Window
    {
        public string epubDirectory;
        public string opfPath;
        public string[] epubCover = new string[32];
        public string[] epubName = new string[32];
        public string[] opfHref = new string[1024];
        public string[] opfId = new string[1024];

        public SelectEpubWindow()
        {
            InitializeComponent();

            int i = 0;
            for (i = 0; i < 32; i++)
            {
                epubName[i] = null;
                epubCover[i] = null;
            }

            epubDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ContentsData\\epub";

            //epubファイルを、大文字小文字を区別して探す
            System.Collections.ObjectModel.ReadOnlyCollection<string> files =
                Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                    epubDirectory,
                    "",
                    false,
                    Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories,
                    new string[] { "*.epub" });

           
            i = 0;
            foreach (string f in files)
            {
                //MessageBox.Show(f);
                epubName[i] = f;
                epubName[i] = epubName[i].Replace(epubDirectory + "\\", "");
                //MessageBox.Show("epubName[" + i + "] = " + epubName[i]);
                i++;
            }

            //ここから解凍処理
            for (int a = 0; epubName[a] != null; a++)
            {
                ZipFile zip = new ZipFile(epubDirectory + "\\" + epubName[a], Encoding.GetEncoding("shift_jis"));

                //zipファイルのパスから解凍先を指定
                //epubのファイル名と同名のフォルダを作成する．ここが解凍先になる
                string epubFolderName = epubName[a].Replace(".epub", "");
                string thawPath = epubDirectory + "\\" + epubFolderName;

                //すでに同名のディレクトリが存在しているときは、解凍しない
                if (!Directory.Exists(thawPath))
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@thawPath);
                    di.Create();

                    //解凍処理
                    zip.ExtractAll(@thawPath, ExtractExistingFileAction.OverwriteSilently);

                    //後始末。面倒ならusingしておいてもおｋ
                    zip.Dispose();
                }

            }

            //ここから、content.opfを解析し、カバー画像を探す
            for (int x = 0; epubName[x] != null; x++)
            {
                opfPath = epubDirectory + "\\" + epubName[x].Replace(".epub", "") + "\\OEBPS\\content.opf";
                //MessageBox.Show(opfPath);

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
                        String strItemId = "start.";
                        int y = 0;
                        while (true)
                        {
                            try
                            {
                                // 取得した<item>要素はXmlNodeListなのでXmlElementにキャストする
                                XmlElement xmlName = (XmlElement)xmlNode.Item(y);

                                // <item>要素のhrefの属性値を取得します
                                strItemHref = xmlName.GetAttribute("href");
                                opfHref[y] = strItemHref;

                                // <item>要素のhrefの属性値を取得します
                                strItemId = xmlName.GetAttribute("id");
                                opfId[y] = strItemId;

                                //取得したitem要素のリストから、idがcover-imageのものを探す
                                if (opfId[y].Contains("cover-image") || opfId[y].Contains("cover_img"))
                                {
                                    epubCover[x] = epubDirectory + "\\" + epubName[x].Replace(".epub", "") + "\\OEBPS\\" + opfHref[y];
                                    //MessageBox.Show(epubCover[x]);
                                    break;
                                }

                                //MessageBox.Show("<item>の属性href=" + strItemHref);
                            }
                            catch
                            {
                                break;
                            }
                            y++;
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
            }

            //ボタンを生成
            int num = 0;
            for (num = 0; epubName[num] != null; num++) { }
            //MessageBox.Show("epubファイルは" + epubDirectory + "内に" + num + "個あります．");

            Button[] btn = new Button[num];

            int j=0; //グリッドの列要素の位置
            int k=0; //グリッドの行要素の位置

            for (i = 0; i < num; i++)
            {
                btn[i] = new Button() { Content = epubName[i] };
                if(System.IO.File.Exists(epubCover[i]))
                {
                    btn[i].Background = new ImageBrush(new BitmapImage(new Uri(epubCover[i], UriKind.Relative)));
                }
                else
                {
                    btn[i].Background = new SolidColorBrush( Color.FromArgb(255, 200, 200, 255) );
                }
                if (j < 5)
                {
                    ColumnDefinition cd1 = new ColumnDefinition() { Width = new GridLength(200) };
                    grid1.ColumnDefinitions.Add(cd1);
                    j++;
                }
                else
                {
                    RowDefinition rd1 = new RowDefinition() { Height = new GridLength(200) };
                    grid1.RowDefinitions.Add(rd1);
                    j=1;
                    k++;
                }
                btn[i].Content = string.Format("{0}." + epubName[i], i + 1);
                Grid.SetColumn(btn[i], j);
                Grid.SetRow(btn[i], k);
                grid1.Children.Add(btn[i]);
                btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                btn[i].Width = double.NaN;  //Autoという意味
                btn[i].Height = double.NaN; //Autoという意味

                btn[i].Click += new RoutedEventHandler(btn_Click);
            }
            //grid1.ShowGridLines = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        public void btn_Click(object sender, RoutedEventArgs e)
        {
            String epubName = sender.ToString();
            epubName = epubName.Replace("System.Windows.Controls.Button: ", "");
            int x = epubName.IndexOf(".");
            epubName = epubName.Remove(0, x+1);

            //ここでcontent.opfを解析して、フォーマットごとに飛ばすウインドウを分ける
            string thawPath = (epubDirectory  + "\\" + epubName).Replace(".epub", "");
            string opfPath = thawPath + "\\OEBPS\\content.opf";
            string format = "null";

            if (File.Exists(opfPath))
            {
                // XmlDocumentオブジェクトを作成
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(@opfPath);

                try
                {
                    // ルートの要素を取得
                    XmlElement xmlRoot = xmlDoc.DocumentElement;

                    // <meta>要素をセット
                    XmlNodeList xmlNode = xmlRoot.GetElementsByTagName("meta");

                    int a = 0;
                    while (true)
                    {
                        // 取得した<meta>要素はXmlNodeListなのでXmlElementにキャストする
                        XmlElement xmlName = (XmlElement)xmlNode.Item(a);

                        try
                        {
                            // <item>要素のformatの属性値を取得する
                            format = xmlName.GetAttribute("format");
                        }
                        catch
                        {
                            break;
                        }
                        a++;
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

            //epubファイルが一枚画像形式の場合
            if (format.Equals("png"))
            {
                PNGWindow pngw = new PNGWindow();
                pngw.Show();
                pngw.init(epubDirectory, epubName, 0);
                this.Owner = pngw;
                this.Close();
            }

            //epubファイルが通常の電子書籍、あるいは要素集合形式の場合
            else
            {
                MainWindow mainDialog = new MainWindow();
                mainDialog.Show();
                this.Owner = mainDialog;
                this.Close();
                mainDialog.ePubClick(epubName, 0, "none");
            }
            
        }
    }
}
