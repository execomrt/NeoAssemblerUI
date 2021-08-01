using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NeoGeo;
namespace NeoAssemblerUI
{
    /// <summary>
    /// Interaction logic for HeaderEdit.xaml
    /// </summary>
    public partial class HeaderEdit : Window
    {
        public Header Info;
        public HeaderEdit()
        {
            InitializeComponent();
            
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Label_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        public bool ShowDialogAndAccepted(Window owner)
        {
            this.Owner = owner;
            Nullable<bool> dialogResult = ShowDialog();

            return dialogResult.Value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mGrid.DataContext = Info;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            
            DialogResult = false;
            Close();
        }
    }
}
