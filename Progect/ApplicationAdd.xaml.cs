using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Progect
{
    /// <summary>
    /// Логика взаимодействия для ApplicationAdd.xaml
    /// </summary>
    public partial class ApplicationAdd : Window
    {
        string dbfile = "Data Source=";
        Dictionary<string, int> cbDic = new Dictionary<string, int>();
        public ApplicationAdd()
        {
            InitializeComponent();
            using (var f = new StreamReader(@"Config.txt")) {dbfile += f.ReadLine();};
            ReadFio();
        }

        private void ReadFio()
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand("SELECT id_client, FIO FROM CLIENT", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = (string)reader["FIO"];
                    ComboFio.Items.Add(item);

                    cbDic[(string)reader["FIO"]] = int.Parse($"{reader["id_client"]}");
                }
                connection.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(TBJob.Text))
            {
                errors.Append("Вы не указали в заявке наименование работы!\n");
            }
            if (string.IsNullOrEmpty(TBComment.Text))
            {
                errors.Append("Добавьте описание работы в заявке!");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbfile))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand($"INSERT INTO APPLICATION (id_client, date, job, description, status) VALUES ({cbDic[ComboFio.Text]}, '{DateTime.Now.ToString("yyyy-MM-dd")}','{TBJob.Text}', '{TBComment.Text}', 1)", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    connection.Close();
                }

                MessageBox.Show("Данные сохранены!");
            } 
        }
    }
}

        
