using Npgsql;
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
    /// Interaction logic for BuisnessTips.xaml
    /// </summary>
    public partial class BuisnessTips : Window
    {
        public BuisnessTips()
        {
            InitializeComponent();
        }

        public void addTipResultsRow(NpgsqlDataReader R)
        {
            BusinessTipsTable.Items.Add(new
            {
                Date = R.GetString(0),
                Name = R.GetString(1),
                Likes = R.GetString(2),
                Text = R.GetString(3)
            });
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            
        }
    }
}
