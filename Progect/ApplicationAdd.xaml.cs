using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Permissions;
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
    /// Логика взаимодействия для ApplicationAdd.xaml
    /// </summary>
    public partial class ApplicationAdd : Window
    {
        private ClassApplication newApplication = new ClassApplication();
        public ApplicationAdd()
        {
            InitializeComponent();
            DataContext = newApplication;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(newApplication.nameApplication))
            {
                errors.Append("Вы не указали в заявке наименование работы!\n");
            }
            if (string.IsNullOrEmpty(newApplication.commentApplication))
            {
                errors.Append("Добавьте описание работы в заявке!");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            newApplication.dateApplication = DateTime.Now.ToString("yyyy-MM-dd");
            newApplication.statusApplication = "новая";
            
            //сохранить в БД

            MessageBox.Show("Данные сохранены!");
        }
    }
}

        
