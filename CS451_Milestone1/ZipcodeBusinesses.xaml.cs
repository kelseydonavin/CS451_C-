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

namespace CS451_Milestone1
{
    /// <summary>
    /// Interaction logic for ZipcodeBusinesses.xaml
    /// </summary>
    public partial class ZipcodeBusinesses : Window
    {
        public ZipcodeBusinesses()
        {
            InitializeComponent();
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = Project51; password = Luckyme324";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(object category in categoriesListBox.SelectedItems)
            {

            }
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "Business Name";
            col1.Width = 255;
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("tips");
            col2.Header = "Business Tips";
            col2.Width = 150;

        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
