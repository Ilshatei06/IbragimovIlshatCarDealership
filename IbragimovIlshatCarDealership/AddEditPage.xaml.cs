using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
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
using System.Xaml;

namespace IbragimovIlshatCarDealership
{

    public partial class AddEditPage : Page
    {
        private Car _currentCar = new Car();
        public AddEditPage(Car SelectedCar)
        {
            InitializeComponent();

            if (SelectedCar != null)
                _currentCar = SelectedCar;

            DataContext = _currentCar;



            var marks = IbragimovCarDealershipDBEntities.GetContext().Mark.ToList();
            ComboMark.ItemsSource = marks;

            if (SelectedCar != null)
            {
                ComboMark.SelectedValue = _currentCar.MarkID;
            }
            
            var classes = IbragimovCarDealershipDBEntities.GetContext().Class.ToList();
            ComboClass.ItemsSource = classes;

            if (SelectedCar != null)
            {
                ComboClass.SelectedValue = _currentCar.ClassID;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboMark.SelectedItem == null)
                errors.AppendLine("Выберите Марку!");

            if (string.IsNullOrWhiteSpace(_currentCar.Model))
                errors.AppendLine("Укажите Модель!");

            if(string.IsNullOrWhiteSpace(_currentCar.Price.ToString()))
                errors.AppendLine("Укажите Цену!");
            else if (_currentCar.Price <= 0)
                errors.AppendLine("Укажите положительную Цену!");

            if (_currentCar.YearProduct < 1900 || _currentCar.YearProduct >= DateTime.Now.Year + 1)
                errors.AppendLine("Укажите Год верно (1900-текущий)!");

            if (string.IsNullOrWhiteSpace(_currentCar.Color))
                errors.AppendLine("Укажите Цвет!");

            if (ComboClass.SelectedItem == null)
                errors.AppendLine("Выберите Кузов!");

            if (string.IsNullOrWhiteSpace(_currentCar.Count.ToString()) || _currentCar.Count < 0)
                errors.AppendLine("Укажите положительное Наличие!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var allCars = IbragimovCarDealershipDBEntities.GetContext().Car.Where(p => p.MarkID == _currentCar.MarkID &&
                                                                                       p.Model == _currentCar.Model &&
                                                                                       p.YearProduct == _currentCar.YearProduct &&
                                                                                       p.Color == _currentCar.Color &&
                                                                                       p.ClassID == _currentCar.ClassID &&
                                                                                       p.CarID != _currentCar.CarID).ToList();


            if (allCars.Count == 0)
            {
                if (_currentCar.CarID == 0)
                    IbragimovCarDealershipDBEntities.GetContext().Car.Add(_currentCar);
                try
                {
                    IbragimovCarDealershipDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена!");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("Уже существует похожая конфигурация!");
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageFolderPath = System.IO.Path.Combine(appDirectory, "image");
                myOpenFileDialog.InitialDirectory = imageFolderPath;

                string path = myOpenFileDialog.FileName;
                int index = path.LastIndexOf("image");
                path = "\\" + path.Substring(index);

                _currentCar.Image = path;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }
    }
}
