﻿using System;
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
using static System.Net.Mime.MediaTypeNames;

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
            { 
                dbfile += f.ReadLine(); 
            }; 
            ReadDB();
        }

        private void ReadDB()
        {
            ReadLeftTable();
            ReadAppTable();
        }

        private void ReadLeftTable()
        {
            List<ClassClient> ltList = new List<ClassClient>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand("SELECT id_client, FIO, INN, object_activity, (SELECT COUNT(job) FROM APPLICATION WHERE (APPLICATION.id_client = CLIENT.id_client)) AS count, (SELECT MAX(date) FROM APPLICATION WHERE (APPLICATION.id_client = CLIENT.id_client)) AS date FROM CLIENT JOIN ACTIVITY ON CLIENT.activity = ACTIVITY.id_activity GROUP BY FIO ORDER BY FIO, INN DESC", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                // иф нет данных
                while (reader.Read())
                {
                    ltList.Add(new ClassClient
                    {
                        idClient = int.Parse($"{reader["id_client"]}"),
                        nameClient = (string)reader["FIO"],
                        innClient = (string)reader["INN"],
                        activityClient = (string)reader["object_activity"],
                        countApplication = int.Parse($"{reader["count"]}"),
                        dateLastApplication = (string)reader["date"],
                    });
                }

                connection.Close();

                LeftGrid.ItemsSource = ltList;
            }
        }

        private void ReadAppTable()
        {
            List<ClassApplication> AppList = new List<ClassApplication>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand("SELECT id_application, date, job, FIO, description, object_status FROM APPLICATION JOIN CLIENT ON CLIENT.id_client = APPLICATION.id_client JOIN STATUS ON APPLICATION.status = STATUS.id_status ORDER BY date DESC", connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    AppList.Add(new ClassApplication
                    {
                        idApplication = int.Parse($"{reader["id_application"]}"),
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
            List<ClassApplication> RtList = new List<ClassApplication>();
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                string comm = string.Empty; 

                connection.Open();

                SQLiteCommand command = new SQLiteCommand($"SELECT id_application, date, job, description, object_status, comment FROM APPLICATION JOIN CLIENT ON CLIENT.id_client = APPLICATION.id_client JOIN STATUS ON APPLICATION.status = STATUS.id_status WHERE CLIENT.FIO = '{name}' ORDER BY date DESC", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                                
                while (reader.Read())
                {
                    RtList.Add(new ClassApplication
                    {
                        idApplication = int.Parse($"{reader["id_application"]}"),
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
            ClassClient name = dg.SelectedItem as ClassClient;
            if (name != null)
            ReadRightTable(name.nameClient);
        }

        private void MenuItem_Click_Add_Client (object sender, RoutedEventArgs e)
        {
            ClientAdd ca = new ClientAdd();
            ca.ShowDialog();
        }
                       
        private void MenuItem_Click_Edit_Client (object sender, RoutedEventArgs e)
        {
            ClientEdit ce = new ClientEdit();
            ce.ShowDialog();
        }
                  
        private void MenuItem_Click_Add_Application(object sender, RoutedEventArgs e)
        {
            ApplicationAdd aa = new ApplicationAdd();
            aa.ShowDialog();
        }

        private void MenuItem_Click_Delete_Application(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if (AppGrid.SelectedItem != null)
            {
                ClassApplication item = (ClassApplication)AppGrid.SelectedItem;
                id = item.idApplication;
            }
            if (RightGrid.SelectedItem != null)
            {
                ClassApplication item = (ClassApplication)RightGrid.SelectedItem;
                id = item.idApplication;
            }

            string sMessageBoxText = "Удалить заявку?";
            string sCaption = "L-PAK";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;
            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            if (rsltMessageBox == MessageBoxResult.Yes)
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbfile))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand($"DELETE FROM APPLICATION WHERE id_application='{id}'", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
                MessageBox.Show($"Заявка удалена!", Title = "L-PAK");
                ReadDB();
            }
        }

        private void MenuItem_Click_Delete_Client(object sender, RoutedEventArgs e)
        {
            string name = string.Empty;
            int id = 0;
            if (LeftGrid.SelectedItem != null)
            {
                ClassClient item = (ClassClient)LeftGrid.SelectedItem;
                name = item.nameClient;
                id = item.idClient;
            }

            string sMessageBoxText = "Удалить клиента?";
            string sCaption = name;
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;
            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            
            if (rsltMessageBox == MessageBoxResult.Yes)
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbfile))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand($"DELETE FROM CLIENT WHERE id_client='{id}'", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
                MessageBox.Show($"Клиент с именем: '{name}' удален!", Title = "L-PAK"); ;
                ReadDB();
            }
        }
    }
}

