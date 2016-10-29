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
    /// PNGAnnotationToolsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGAnnotationToolsWindow : Window
    {
        public PNGAnnotationToolsWindow()
        {
            InitializeComponent();
        }

        //直線モードか、自由線モードか
        bool isStraightMode = false; //falseが自由線、trueが直線

        //ツールボックスの移動
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //色変更
        private void BlackButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 0, 0, 0);
        }

        private void WhiteButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 255, 255, 255);
        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 255, 0, 0);
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 0, 0, 255);
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 0, 255, 0);
        }

        private void YellowButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeColor(255, 0, 255, 255);
        }

        //直線・自由線切り替え
        private void ModeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isStraightMode) //直線モードにする
            {
                ((PNGWindow)this.Owner).ChangeStrokeMode("straight");
                ModeSwitchButton.Content = "自由線";
                isStraightMode = true;
            }
            else //自由線にする
            {
                ((PNGWindow)this.Owner).ChangeStrokeMode("free");
                ModeSwitchButton.Content = "直線";
                isStraightMode = false;

            }
        }

        //太さ変更
        private void Width1Button_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeWidth(1);
        }

        private void Width3Button_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeWidth(3);
        }

        private void Width20Button_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ChangeWidth(20);
        }

        //一つ戻る
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).Undo();
        }

        //全消去
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).ClearStrokes();
        }

        //閉じる
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((PNGWindow)this.Owner).inkCanvas1.EditingMode = InkCanvasEditingMode.None;
            ((PNGWindow)this.Owner).inkCanvas1.Visibility = System.Windows.Visibility.Hidden;
            this.Close();
        }
    }
}
