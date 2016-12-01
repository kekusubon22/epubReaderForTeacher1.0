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

using System.IO;

using System.Runtime.InteropServices;
using System.Management;

namespace epubReaderForTeacher1._0
{
    /// <summary>
    /// PNGSelectHowToShowWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGSelectHowToShowWindow : Window
    {
        public PNGSelectHowToShowWindow()
        {
            InitializeComponent();
        }

        //要素ごとならtrue、ユーザごとならfalse
        bool isElement = false;

        string imageFileName;
        string epubDirectory;
        string epubFileName;
        User user;

        //初期処理
        public void init(string imageFileName, string epubDirectory, string epubFileName, User user)
        {
            this.imageFileName = imageFileName;
            this.epubDirectory = epubDirectory;
            this.epubFileName = epubFileName;
            this.user = user;
        }

        //要素（あるいはページ）ごとにすべてのユーザのキャプチャを表示させる
        public void elementButton_Click(object sender, RoutedEventArgs e)
        {
            //キャプチャのリスト（files）のそれぞれの書き主のID
            string[] captureOwnerId = new string[1];

            isElement = true;
            string[] directories;
            string[] files = new string[1];
            int i = 0;


            //ファイル共有するならこっち
            if (Directory.Exists(GetUniversalName(@"\\MCDYNA20\ContentsData")))
            {
                string searchDirectory = @"\\MCDYNA20\ContentsData\Annotation";
                string unc_path = GetUniversalName(searchDirectory);

                try
                {
                    //Annotationフォルダ内に内にあるユーザのディレクトリを検索
                    directories = System.IO.Directory.GetDirectories(unc_path, "*", SearchOption.TopDirectoryOnly);

                    //それぞれのユーザのディレクトリから、対象の要素（あるいはページ）に対するキャプチャを探す
                    foreach (string d in directories)
                    {
                        //現在の要素（あるいはページ）に対する全ユーザのキャプチャを検索
                        string[] captures = System.IO.Directory.GetFiles(d + "\\" + epubFileName.Replace(".epub", ""), imageFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
                        foreach (string c in captures)
                        {
                            // 配列を拡張
                            Array.Resize(ref files, i + 1);
                            files[i] = c;

                            // 配列を拡張
                            Array.Resize(ref captureOwnerId, i + 1);
                            captureOwnerId[i] = d.Replace(unc_path + "\\", "");

                            i++;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("ファイルがありません。");
                    return;
                }

            }

            //しないならこっち
            else
            {
                string searchDirectory = epubDirectory.Replace("epub", "Annotation");

                try
                {
                    //Annotationフォルダ内に内にあるユーザのディレクトリを検索
                    directories = System.IO.Directory.GetDirectories(searchDirectory, "*", SearchOption.TopDirectoryOnly);

                    //それぞれのユーザのディレクトリから、対象の要素（あるいはページ）に対するキャプチャを探す
                    foreach (string d in directories)
                    {
                        //現在の要素（あるいはページ）に対する全ユーザのキャプチャを検索
                        string[] captures = System.IO.Directory.GetFiles(d + "\\" + epubFileName.Replace(".epub", ""), imageFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
                        foreach (string c in captures)
                        {
                            // 配列を拡張
                            Array.Resize(ref files, i + 1);
                            files[i] = c;

                            // 配列を拡張
                            Array.Resize(ref captureOwnerId, i + 1);
                            captureOwnerId[i] = d.Replace(searchDirectory + "\\", "");

                            i++;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("ファイルがありません");
                    return;
                }
            }

            PNGSelectAnnotationWindow pslaw = new PNGSelectAnnotationWindow();
            pslaw.Owner = this;
            pslaw.Show();
            pslaw.init(files, captureOwnerId, isElement);
        }

        //ユーザごとにすべての要素のキャプチャを表示させる
        public void userButton_Click(object sender, RoutedEventArgs e)
        {
            //Annottionフォルダ内にあるユーザ名のリスト
            string[] users = new string[1];

            isElement = false;
            string[] directories;
            int i = 0;

            //ファイル共有するならこっち
            if (Directory.Exists(GetUniversalName(@"\\MCDYNA20\ContentsData")))
            {
                string searchDirectory = @"\\MCDYNA20\ContentsData\Annotation";
                string unc_path = GetUniversalName(searchDirectory);

                //Annotationフォルダ内に内にあるユーザのディレクトリを検索
                directories = System.IO.Directory.GetDirectories(unc_path, "*", SearchOption.TopDirectoryOnly);

                foreach(string d in directories)
                {
                    // 配列を拡張
                    Array.Resize(ref users, i + 1);
                    users[i] = d.Replace(unc_path + "\\", "");
                    i++;
                }
            }

            //しないならこっち
            else
            {
                string searchDirectory = epubDirectory.Replace("epub", "Annotation");

                //Annotationフォルダ内に内にあるユーザのディレクトリを検索
                directories = System.IO.Directory.GetDirectories(searchDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string d in directories)
                {
                    // 配列を拡張
                    Array.Resize(ref users, i + 1);
                    users[i] = d.Replace(searchDirectory + "\\", "");
                    i++;
                }
            }

            PNGSelectUserWindow psuw = new PNGSelectUserWindow();
            psuw.Owner = this;
            psuw.Show();
            psuw.init(users, epubDirectory, epubFileName);


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