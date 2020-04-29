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
    /// Interaction logic for CheckIn.xaml
    /// </summary>
    public partial class CheckIn : Window
    {
        public CheckIn()
        {
            InitializeComponent();
        }

        public void addCheckInResultsRow(NpgsqlDataReader R)
        {
            Check_In.Items.Add(new
            {
                Name = R.GetString(0),
                Month = R.GetString(1),
                Date = R.GetString(2),
                Year = R.GetString(3)
            });
        }
    }
}
