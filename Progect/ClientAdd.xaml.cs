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

namespace Progect
{
    /// <summary>
    /// Логика взаимодействия для ClientAdd.xaml
    /// </summary>
    public partial class ClientAdd : Window
    {
        private ClassClient newClient = new ClassClient();
        public ClientAdd()
        {
            InitializeComponent();
            DataContext = newClient;
            //выпадающий список
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(newClient.nameClient))
            {
                errors.Append("Укажите ФИО!\n");
            }
            if (string.IsNullOrEmpty(newClient.innClient))
            { 
                errors.Append("Введите ИНН!\n");
            }
            if (string.IsNullOrEmpty(newClient.activityClient))
            {
                errors.Append("Выберите сферу деятельности!\n");
            }
            if (string.IsNullOrEmpty(newClient.commentClient))
            {
                errors.Append("Укажите дополнительную информацию о клиенте!\n");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            MessageBox.Show("Данные сохранены!");
        }

        private void ComboActivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }
    }
}
