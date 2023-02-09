using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
using System.Xml.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Progect
{
    /// <summary>
    /// Логика взаимодействия для ClientEdit.xaml
    /// </summary>
    public partial class ClientEdit : Window
    {           
        string dbfile = "Data Source=";

        Dictionary<string, int> cbDic1 = new Dictionary<string, int>();
        Dictionary<string, int> cbDic2 = new Dictionary<string, int>();
        public ClientEdit()
        {
            InitializeComponent();
            using (var f = new StreamReader(@"Config.txt")) { dbfile += f.ReadLine(); };
            ReadDataBase();
            
        }
        private void ReadDataBase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand("SELECT id_client, FIO, INN, id_activity, object_activity, comment FROM CLIENT JOIN ACTIVITY ON CLIENT.activity = ACTIVITY.id_activity", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxItem item1 = new ComboBoxItem();
                    item1.Content = (string)reader["FIO"];
                    ComboFio.Items.Add(item1);
                    cbDic1[(string)reader["FIO"]] = int.Parse($"{reader["id_client"]}");   
                } 

                command = new SQLiteCommand("SELECT id_client, FIO, INN, id_activity, object_activity, comment FROM CLIENT JOIN ACTIVITY ON CLIENT.activity = ACTIVITY.id_activity GROUP BY object_activity", connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxItem item2 = new ComboBoxItem();
                    item2.Content = (string)reader["object_activity"];
                    ComboActivity.Items.Add(item2);
                    cbDic2[(string)reader["object_activity"]] = int.Parse($"{reader["id_activity"]}");
                }

                connection.Close();
            }
        }

        private void PrintClient(object sender, SelectionChangedEventArgs e)
        {
            if (ComboFio.Text != "")

            {
                int id_client = cbDic1[ComboFio.Text];

                using (SQLiteConnection connection = new SQLiteConnection(dbfile))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand($"SELECT INN, object_activity, comment FROM CLIENT JOIN ACTIVITY ON CLIENT.activity = ACTIVITY.id_activity WHERE id_client = '{id_client}'", connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    TBInn.Text = (string)reader["INN"];
                    ComboActivity.Text = (string)reader["object_activity"];
                    TBComment.Text = (string)reader["comment"];

                    connection.Close();

                }
            }
        }

        private void Save_Click_1(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(ComboFio.Text))
            {
                errors.Append("Выберите ФИО!\n");
            }
            if (string.IsNullOrEmpty(TBInn.Text))
            {
                errors.Append("Поле 'ИНН' не содержит данных!\n");
            }
            if (string.IsNullOrEmpty(TBComment.Text))
            {
                errors.Append("Заполните поле 'Примечание'!\n");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            else
            {
                string sMessageBoxText = "Подтвердить изменения?";
                string sCaption = ComboFio.Text;
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                MessageBoxImage icnMessageBox = MessageBoxImage.Question;
                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                if (rsltMessageBox == MessageBoxResult.Yes)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(dbfile))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand ($"UPDATE CLIENT SET INN = '{TBInn.Text}', activity = {cbDic2[ComboActivity.Text]}, comment = '{TBComment.Text}' WHERE id_client = {cbDic1[ComboFio.Text]}", connection);
                        SQLiteDataReader reader = command.ExecuteReader();
                        connection.Close();
                    }
                    MessageBox.Show($"Клиент с именем: '{ComboFio.Text}' изменен!", Title = "L-PAK"); ;
                   
                }
            }
        }
    }
}
