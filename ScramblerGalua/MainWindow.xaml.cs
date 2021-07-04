using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;

using ScramblerGalua.Model;
using ScramblerGalua.Interface;
using Vector = ScramblerGalua.Model.Vector;

namespace ScramblerGalua
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int Format = 2;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void ButtonPolinomGenerate_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarProgress.IsIndeterminate = true;
            (sender as Button).IsEnabled = false;

            IPolinom polinom = new Polinom();
            BigInteger r = await polinom.Generate(int.Parse(ComboBoxPower.Text));
            TextBoxPolinom.Text = BigIntegerHelper.ToString(r, int.Parse(ComboBoxFormat.Text));

            ProgressBarProgress.IsIndeterminate = false;
            (sender as Button).IsEnabled = true;
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidForm() == false)
                return;
            DateTime timeStart = DateTime.Now;

            TypeMatrix Matrix = (TypeMatrix) Convert.ToInt16(GridMatrix.Children.OfType<RadioButton>().FirstOrDefault(x => x.IsChecked == true).Tag);
            BigInteger Polinom = BigIntegerHelper.ToBigInteger(TextBoxPolinom.Text, Format);
            BigInteger Omega = BigIntegerHelper.ToBigInteger(TextBoxOmega.Text, Format);
            BigInteger Vector = BigIntegerHelper.ToBigInteger(TextBoxVector.Text, Format);
            Ikey key = new CryptoKey(Matrix, Polinom, Omega, Vector);

            string input = TextBoxInputFile.Text;
            string output = TextBoxOutputFile.Text;
            IEncrypting encrypting = new Encrypting(key);
            encrypting.Progres += (val) => {
                Dispatcher.Invoke(() => 
                {
                    ProgressBarProgress.Value = val;
                    Label_progres.Content = val + "%";
                });
            };
            await Task.Run(() =>
            {
                encrypting.Encrypt(input, output);
            });
            ProgressBarProgress.Value = 0;
            Label_progres.Content = "";
            LabelResult.Content = "Процес завершен!";
            Label_time.Content = (DateTime.Now - timeStart).ToString(@"hh\:mm\:ss\:fff");
            LabelResult.Foreground = Brushes.Green;
        }
        private void ButtonGenerateVector_Click(object sender, RoutedEventArgs e)
        {
            BigInteger Vector = new Vector().Generate(int.Parse(ComboBoxPower.Text));
            TextBoxVector.Text = BigIntegerHelper.ToString(Vector, int.Parse(ComboBoxFormat.Text));
        }
        private async void ButtonGeneratOmega_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPolinom.Text.Length <= 2)
            {
                MessageBox.Show("Спочатку вкажіть поліном!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }   
            ProgressBarProgress.IsIndeterminate = true;
            (sender as Button).IsEnabled = false;
            IOmega key = new Omega(int.Parse(ComboBoxPower.Text));
            BigInteger Omega = await key.Generate(BigIntegerHelper.ToBigInteger(TextBoxPolinom.Text,int.Parse(ComboBoxFormat.Text)));
            TextBoxOmega.Text = BigIntegerHelper.ToString(Omega, int.Parse(ComboBoxFormat.Text));
            ProgressBarProgress.IsIndeterminate = false;
            (sender as Button).IsEnabled = true;
        }
        private void ComboBoxFormat_DropDownClosed(object sender, EventArgs e)
        {
            BigInteger Polinom = BigIntegerHelper.ToBigInteger(TextBoxPolinom.Text ,Format);
            BigInteger Omega = BigIntegerHelper.ToBigInteger(TextBoxOmega.Text, Format);
            BigInteger Vector = BigIntegerHelper.ToBigInteger(TextBoxVector.Text, Format);

            Format = int.Parse(ComboBoxFormat.Text);
            TextBoxPolinom.Text = BigIntegerHelper.ToString(Polinom, int.Parse(ComboBoxFormat.Text)); 
            TextBoxOmega.Text = BigIntegerHelper.ToString(Omega, int.Parse(ComboBoxFormat.Text)); 
            TextBoxVector.Text = BigIntegerHelper.ToString(Vector, int.Parse(ComboBoxFormat.Text));
        }
        private void ButtonInputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "";
            dlg.Filter = "(*.txt)|*.txt|(*.docx)|*.docx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBoxInputFile.Text = dlg.FileName;
            }
        }
        private void ButtonOutputFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.Filter = "(*.txt)|*.txt|(*.docx)|*.docx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBoxOutputFile.Text = dlg.FileName;
            }
        }
        private void TextBoxInputFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox texBox = sender as TextBox;
            if (IsValidPath(texBox.Text) == false)
            {
                texBox.BorderBrush = Brushes.Red;
                LabelResult.Content = "Невірний шлях до файлу!";
                LabelResult.Foreground = Brushes.Red;
            }
            else
            {
                LabelResult.Content = "";
                LabelResult.Foreground = Brushes.Green;
                texBox.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0078D7");
            }
        }
        private bool IsValidForm()
        {
            if (IsValidPath(TextBoxInputFile.Text) == false || IsValidPath(TextBoxOutputFile.Text) == false)
            {
                MessageBox.Show("Невірно вказаний шлях файлу!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            if (TextBoxPolinom.Text.Length <= 2)
            {
                MessageBox.Show("Невірно вказаний поліном!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            if (TextBoxOmega.Text.Length < 2)
            {
                MessageBox.Show("Невірно вказаний утворюючий елемент!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            if (TextBoxVector.Text.Length < 1)
            {
                MessageBox.Show("Невірно вказаний вектор ініціалізації!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }
        private bool IsValidPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            bool isValidPath = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            bool isValidDirecroty = Directory.Exists(Path.GetDirectoryName(filePath));
            return isValidPath & isValidDirecroty;
        }
        private void UpdateForm(Action action)
        {
            Dispatcher.Invoke(action);
        }
        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LabelResult.Content = "";
        }
    }
}
