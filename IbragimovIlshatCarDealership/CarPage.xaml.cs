using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// <summary>
    /// Логика взаимодействия для CarPage.xaml
    /// </summary>
    public partial class CarPage : Page
    {
        int allCountRecord;
        public CarPage()
        {
            InitializeComponent();

            var currentCar = IbragimovCarDealershipDBEntities.GetContext().Car.ToList();

            CarListView.ItemsSource = currentCar;
           

            ComboSort.SelectedIndex = 0;
            ComboType.SelectedIndex = 0;


            UpdateServices();
        }

        private void UpdateServices()
        {
            var currentCar = IbragimovCarDealershipDBEntities.GetContext().Car.ToList();
            allCountRecord = currentCar.Count;

            if (ComboType.SelectedIndex == 0)
                currentCar = currentCar.ToList();

            if (ComboType.SelectedIndex == 1)
                currentCar = currentCar.Where(p => p.Price < 500000).ToList();

            if (ComboType.SelectedIndex == 2)
                currentCar = currentCar.Where(p => p.Price >= 500000 && p.Price < 1000000).ToList();

            if (ComboType.SelectedIndex == 3)
                currentCar = currentCar.Where(p => p.Price >= 1000000 && p.Price < 5000000).ToList();

            if (ComboType.SelectedIndex == 4)
                currentCar = currentCar.Where(p => p.Price >= 5000000 && p.Price < 10000000).ToList();

            if (ComboType.SelectedIndex == 5)
                currentCar = currentCar.Where(p => p.Price >= 10000000).ToList();


            //currentCar = currentCar.Where(p => p.MarkString.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
            //                                   p.Model.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
            //                                   p.YearProduct.ToString().Contains(TBoxSearch.Text.ToLower()) ||
            //                                   p.Color.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
            //                                   p.ClassString.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            var searchText = TBoxSearch.Text.ToLower();
            string[] searchWords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (searchWords.Length == 0)
                CarListView.ItemsSource = currentCar.ToList();
            else
            {
                currentCar = currentCar.Where(p => searchWords.All(word =>
                                                        p.MarkString.ToLower().Contains(word) ||
                                                        p.Model.ToLower().Contains(word) ||
                                                        p.Price.ToString().ToLower().Contains(word) ||
                                                        p.YearProduct.ToString().Contains(word) ||
                                                        p.Color.ToLower().Contains(word) ||
                                                        p.ClassString.ToLower().Contains(word) ||
                                                        p.Count.ToString().ToLower().Contains(word) ||
                                                        (p.Description ?? "").ToLower().Contains(word))).ToList();

                CarListView.ItemsSource = currentCar.ToList();
            }

            CarListView.ItemsSource = currentCar.ToList();

            if (ComboSort.SelectedIndex == 0)
                currentCar = currentCar.ToList();
            if (ComboSort.SelectedIndex == 1)
                currentCar = currentCar.OrderBy(p => p.Price).ToList();
            if (ComboSort.SelectedIndex == 2)
                currentCar = currentCar.OrderByDescending(p => p.Price).ToList();

            CarListView.ItemsSource = currentCar;

            tbCount.Text = "Количество записей: " + currentCar.Count.ToString() + " из " + allCountRecord;
        }


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                IbragimovCarDealershipDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                CarListView.ItemsSource = IbragimovCarDealershipDBEntities.GetContext().Car.ToList();

                UpdateServices();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Car));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentCar = (sender as Button).DataContext as Car;

            var Sales = IbragimovCarDealershipDBEntities.GetContext().Sale.ToList();
            Sales = Sales.Where(p => p.CarID == currentCar.CarID).ToList();

            if (Sales.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существуют продажа");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        IbragimovCarDealershipDBEntities.GetContext().Car.Remove(currentCar);
                        IbragimovCarDealershipDBEntities.GetContext().SaveChanges();

                        CarListView.ItemsSource = IbragimovCarDealershipDBEntities.GetContext().Car.ToList();

                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

      
    }
}
