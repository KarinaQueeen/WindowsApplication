using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;

namespace Progect
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string dbfile = "Data Source=";
        public MainWindow()
        {
            InitializeComponent();
            using (var f = new StreamReader(@"Config.txt"))
            { dbfile += f.ReadLine(); }; 
            ReadDB();
        }

        private void ReadDB()
        {
            ReadLeftTable();
            ReadAppTable();
        }

        private void ReadLeftTable()
        {
            List<LeftTable> ltList = new List<LeftTable>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand("SELECT FIO, INN, object_activity, (SELECT COUNT(job) FROM APPLICATION WHERE (APPLICATION.id_client = CLIENT.id_client)) AS count, (SELECT MAX(date) FROM APPLICATION WHERE (APPLICATION.id_client = CLIENT.id_client)) AS date FROM CLIENT JOIN ACTIVITY ON CLIENT.activity = ACTIVITY.id_activity GROUP BY FIO ORDER BY FIO, INN DESC", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ltList.Add(new LeftTable
                    {
                        nameClient = (string)reader["FIO"],
                        innClient = (string)reader["INN"],
                        activityClient = (string)reader["object_activity"],
                        countApplication = int.Parse($"{reader["count"]}"),
                        dateLastApplication = (string)reader["date"],
                    });
                    
                }
                connection.Close();
                leftGrid.ItemsSource = ltList;
                
            }
        }
        private void ReadAppTable()
        {
            List<AppTable> AppList = new List<AppTable>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand("SELECT date, job, FIO, description, object_status FROM APPLICATION JOIN CLIENT ON CLIENT.id_client = APPLICATION.id_client JOIN STATUS ON APPLICATION.status = STATUS.id_status ORDER BY date DESC", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    
                    AppList.Add(new AppTable
                    {
                        dateApplication = (string)reader["date"],
                        nameApplication = (string)reader["job"],
                        nameClient = (string)reader["FIO"],
                        commentApplication = (string)reader["description"],
                        statusApplication = (string)reader["object_status"],
                    });
                }
                connection.Close();
                AppGrid.ItemsSource = AppList;
            }
        }
        private void ReadRightTable(string name)
        {
            List<RightTable> RtList = new List<RightTable>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand($"SELECT date, job, description, object_status, comment FROM APPLICATION JOIN CLIENT ON CLIENT.id_client = APPLICATION.id_client JOIN STATUS ON APPLICATION.status = STATUS.id_status WHERE CLIENT.FIO = '{name}'", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                string comm = string.Empty; 
                while (reader.Read())
                {

                    RtList.Add(new RightTable
                    {
                        dateApplication = (string)reader["date"],
                        nameApplication = (string)reader["job"],
                        commentApplication = (string)reader["description"],
                        statusApplication = (string)reader["object_status"],
                    });
                    comm = (string)reader["comment"];
                    }
                connection.Close();
                RightGrid.ItemsSource = RtList;
                Comment.Text = comm; 
            }
        }

        private void leftGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            LeftTable lf = dg.SelectedItem as LeftTable;
            ReadRightTable(lf.nameClient);
        }
    }
}
