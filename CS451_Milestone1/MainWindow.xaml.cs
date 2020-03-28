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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace CS451_Milestone1
{
    /// <summary>
    /// Milestone1 of Term Project
    /// Group Project-51
    /// Members: Trento Fales, Kelsey Donavan, Joshua Stallworth
    /// Major Reference: Shakire's Step by Step Demo for POSTSQL
    /// Minor Reference: Recommended Link in Video
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class Buisness
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set;  }
            //public string zipcode { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            addState();
            addZipCode();
            addColumnsToGrid();

        }


        private void addState()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            stateList.Items.Add(reader.GetString(0));
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            
        }

        private void addZipCode()
        {
            zipcodeList.Items.Add("88977");
            zipcodeList.Items.Add("78899");
            zipcodeList.Items.Add("17489");
            zipcodeList.Items.Add("78414");
            zipcodeList.Items.Add("78954");
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "Business Name";
            col1.Width = 255;
            buisnessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("state");
            col2.Header = "State";
            col2.Width = 60;
            buisnessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 150;
            buisnessGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("bid");
            col4.Header = "Bid";
            col4.Width = 0;
            buisnessGrid.Columns.Add(col4);
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password=#NicetryNathan01";
        }

        private void executeQuery(string sqlStr, Action<NpgsqlDataReader> myF)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlStr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            myF(reader);
                        Console.WriteLine("Connected successfully!");
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine("Could not connect to postgreSQL!");
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error -" + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void addStateRow(NpgsqlDataReader R)
        {
            buisnessGrid.Items.Add(new Buisness() { name = R.GetString(0), state = R.GetString(1), city = R.GetString(2), bid = R.GetString(3) });
            // buisnessGrid.Items.Add(new Buisness() { name = R.GetString(0), state = R.GetString(1), city = R.GetString(2) });
            // buisnessGrid.Items.Add(new Buisness() { name = "Test", state = "Test", city = "Test", bid = "Test" });

        }

        private void addCityRow(NpgsqlDataReader R)
        {
            cityList.Items.Add(R.GetString(0));
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear();
            if (stateList.SelectedIndex > -1)
            {
                string sqlStr = "SELECT distinct city FROM business WHERE state = '" + stateList.SelectedItem.ToString() + "' ORDER BY city;";
                executeQuery(sqlStr, addCityRow);
            }
        }

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buisnessGrid.Items.Clear();
            if (cityList.SelectedIndex > -1)
            {
                string sqlStr = "SELECT name, state, city, business_id FROM business WHERE state = '" + stateList.SelectedItem.ToString() + "' AND city = '" + cityList.SelectedItem.ToString() + "' ORDER BY name;";
                executeQuery(sqlStr, addStateRow);
            }
        }

        private void buisnessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buisnessGrid.SelectedIndex > -1)
            {
                Buisness B = buisnessGrid.Items[buisnessGrid.SelectedIndex] as Buisness;
                if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
                {
                    BusinessDetails businessWindow = new BusinessDetails(B.bid.ToString());
                    businessWindow.Show();
                }

            }
        }
    }
}
