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

namespace IbragimovIlshatCarDealership
{
    public partial class ClientWindow : Window
    {
        private Client _currentClient = new Client();
        public ClientWindow()
        {
            InitializeComponent();

            DataContext = _currentClient;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.Surname))
                errors.AppendLine("Укажите Фамилия!");

            if (string.IsNullOrWhiteSpace(_currentClient.Name))
                errors.AppendLine("Укажите Имя!");

            if (string.IsNullOrWhiteSpace(_currentClient.Passport))
                errors.AppendLine("Укажите номер пасспорта!");
            else if (_currentClient.Passport.Length != 10)
                errors.AppendLine("Укажите номер пасспорта верно!");

            if (string.IsNullOrWhiteSpace(_currentClient.Adress))
                errors.AppendLine("Укажите Адрес!");

            if (_currentClient.Age == 0)
                errors.AppendLine("Укажите Возраст!");
            else if (_currentClient.Age < 18 || _currentClient.Age > 100)
                errors.AppendLine("Клиенту должно быть от 18 до 100 лет!");

            if (ComboGender.SelectedItem == null)
                errors.AppendLine("Выберите Пол!");


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var allClient = IbragimovCarDealershipDBEntities.GetContext().Client.Where(p => p.Surname == _currentClient.Surname &&
                                                                                       p.Name == _currentClient.Name &&
                                                                                       p.Patronymic == _currentClient.Patronymic &&
                                                                                       p.Passport == _currentClient.Passport).ToList();


            if (allClient.Count == 0)
            {
                try
                {
                    if (ComboGender.SelectedIndex == 0)
                        _currentClient.Gender = "м";
                    else
                        _currentClient.Gender = "ж";

                    IbragimovCarDealershipDBEntities.GetContext().Client.Add(_currentClient);
                    IbragimovCarDealershipDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Клиент успешно добавлен!");
                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("Уже существует клиент с такими данными!");
        }
    }
}
