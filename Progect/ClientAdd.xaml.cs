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


namespace Progect
{
    /// <summary>
    /// Логика взаимодействия для ClientAdd.xaml
    /// </summary>
    /// 
           
    public partial class ClientAdd : Window
    {  
        string dbfile = "Data Source=";
        
        Dictionary<string, int> cbDic = new Dictionary<string, int>();
        public ClientAdd()
        {
            InitializeComponent();
            using (var f = new StreamReader(@"Config.txt")) {dbfile += f.ReadLine();};
            ReadActivity();
        }

        private void ReadActivity()
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand("SELECT id_activity, object_activity FROM ACTIVITY", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = (string)reader["object_activity"];
                    ComboActivity.Items.Add(item);
                    
                    cbDic[(string)reader["object_activity"]] = int.Parse($"{reader["id_activity"]}");
                }
                connection.Close();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(TBName.Text))
            {
                errors.Append("Укажите ФИО!\n");
            }
            if (string.IsNullOrEmpty(TBInn.Text))
            {
                errors.Append("Введите ИНН!\n");
            }
            if (string.IsNullOrEmpty(ComboActivity.Text))
            {
                errors.Append("Выберите сферу деятельности!\n");
            }
            if (string.IsNullOrEmpty(TBComment.Text))
            {
                errors.Append("Укажите дополнительную информацию о клиенте!\n");
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
                    SQLiteCommand command = new SQLiteCommand($"INSERT INTO CLIENT (FIO, INN, activity, comment) VALUES ('{TBName.Text}', '{TBInn.Text}', {cbDic[ComboActivity.Text]} ,'{TBComment.Text}')", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
                MessageBox.Show("Данные сохранены!");
            }
        }
    }
}
