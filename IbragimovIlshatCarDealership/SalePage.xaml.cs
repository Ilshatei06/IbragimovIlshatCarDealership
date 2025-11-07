using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IbragimovIlshatCarDealership
{
    public partial class SalePage : Page
    {
        private Car _currentCar = new Car();
        private Sale _currentSale = new Sale();
        public SalePage(Car SelectedCar)
        {
            InitializeComponent();

            _currentCar = SelectedCar;
            UpdateSaleListView();
            DataContext = _currentCar;

            var workers = IbragimovCarDealershipDBEntities.GetContext().Worker.ToList();
            ComboSerchWorker.ItemsSource = workers;
            ComboSerchWorker.SelectedValuePath = "WorkerID";

            var clients = IbragimovCarDealershipDBEntities.GetContext().Client.ToList();
            ComboSerchClient.ItemsSource = clients;
            ComboSerchClient.SelectedValuePath = "ClientID";

            saleDate.SelectedDate = DateTime.Now;
        }

        public void UpdateSaleListView()
        {
            var _currentSale = IbragimovCarDealershipDBEntities.GetContext().Sale.Where(p => p.CarID == _currentCar.CarID).ToList();
            CarSaleListView.ItemsSource = _currentSale.ToList();
        }

        private void SaleButton_Click(object sender, RoutedEventArgs e)
        {
            
            StringBuilder errors = new StringBuilder();


            if (ComboSerchWorker.SelectedItem == null)
                errors.AppendLine("Укажите Сотрудника!");
            if (ComboSerchClient.SelectedItem == null)
                errors.AppendLine("Укажите Клиента!");
            if (saleDate == null)
                errors.AppendLine("Укажите дату!");
            if (string.IsNullOrWhiteSpace(tbVIN.Text))
                errors.AppendLine("Укажите VIN!");
            else if (tbVIN.Text.Length != 17)
                errors.AppendLine("Укажите VIN верно! Он может содержать латинские буквы и цифры (кроме I,O,Q,0,1)");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentSale.WorkerID = (int)ComboSerchWorker.SelectedValue;
            _currentSale.ClientID = (int)ComboSerchClient.SelectedValue;
            _currentSale.CarID = _currentCar.CarID;
            _currentSale.CarVIN = tbVIN.Text;
            _currentSale.Date = saleDate.SelectedDate ?? DateTime.Now;

            var allSales = IbragimovCarDealershipDBEntities.GetContext().Sale.Where(p => p.CarVIN == _currentSale.CarVIN);

            if (allSales.Count() != 0)
                MessageBox.Show("Автомобиль с таким VIN уже продан!");
            else if (_currentCar.Count == 0)
                MessageBox.Show("Автомобиля нет в наличии!");
            else
            {
                try
                {
                    IbragimovCarDealershipDBEntities.GetContext().Sale.Add(_currentSale);

                    _currentCar.Count -= 1;

                    IbragimovCarDealershipDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Продажа добавлена!");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }


        private void tbVIN_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!IsValidVINChar(char.ToUpper(c)))
                {
                    e.Handled = true; 
                    return;
                }
            }
        }

        private bool IsValidVINChar(char c)
        {
            if (c == ' ') return false;
            if (c >= '2' && c <= '9') return true;
            if (c >= 'A' && c <= 'Z') return c != 'I' && c != 'O' && c != 'Q';
          
            return false;
        }


        private void addClient_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Owner = Window.GetWindow(this);
            clientWindow.ShowDialog();

            var clients = IbragimovCarDealershipDBEntities.GetContext().Client.ToList();
            ComboSerchClient.ItemsSource = clients;
        }

        private void btnDeleteSale_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            int saleId = (int)button.CommandParameter;

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Sale sale = IbragimovCarDealershipDBEntities.GetContext().Sale.FirstOrDefault(s => s.SaleID == saleId);


                    if (sale != null)
                    {
                        IbragimovCarDealershipDBEntities.GetContext().Sale.Remove(sale);
                        _currentCar.Count += 1;
                        Car car = IbragimovCarDealershipDBEntities.GetContext().Car.FirstOrDefault(p => p.CarID == sale.CarID);
                        IbragimovCarDealershipDBEntities.GetContext().SaveChanges();
                        UpdateSaleListView();
                        MessageBox.Show("Продажа удалена!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
