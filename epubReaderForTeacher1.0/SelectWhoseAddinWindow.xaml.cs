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

namespace epubReader4._0_Dino
{
    /// <summary>
    /// SelectWhoseAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectWhoseAddinWindow : Window
    {
        public SelectWhoseAddinWindow()
        {
            InitializeComponent();
        }

        //誰が追加した教材を見るかを選択するWindow
        string addinDirectory;
        User user;

        //初期処理
        public void init(string addinDirectory, User user)
        {
            this.addinDirectory = addinDirectory;
            this.user = user;
        }

        //管理者が追加した教材
        private void administratorButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAdministratorAddinWindow saaw = new ShowAdministratorAddinWindow();
            saaw.Owner = this;
            saaw.Show();
            saaw.init(addinDirectory, user);
        }

        //自分以外の児童が追加した教材
        private void studentButton_Click(object sender, RoutedEventArgs e)
        {
            ShowStudentAddinWindow ssaw = new ShowStudentAddinWindow();
            ssaw.Owner = this;
            ssaw.Show();
            ssaw.init(addinDirectory, user);
        }

        //自分が追加した教材
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMyAddinWindow smaw = new ShowMyAddinWindow();
            smaw.Owner = this;
            smaw.Show();
            smaw.init(addinDirectory, user);
        }
    }
}
