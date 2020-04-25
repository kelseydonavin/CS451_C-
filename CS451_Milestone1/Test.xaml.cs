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
using System.Configuration;
using Npgsql;

namespace CS451_Milestone1
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private string username;

        public Test()
        {
            InitializeComponent();
            addUserID();
            addColumnsToFriendsGrid();
        }

        private void Username_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            username = textBox.Text;
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = Project51; password=Luckyme324";
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

        private void addColumnsToFriendsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("Name");
            col1.Header = "Name";
            col1.Width = 100;
            friendsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("TotalLikes");
            col2.Header = "Total Likes";
            col2.Width = 80;
            friendsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("AvgStars");
            col3.Header = "Avg Stars";
            col3.Width = 80;
            friendsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("YelpingSince");
            col4.Header = "Yelping Since";
            col4.Width = 232;
            friendsGrid.Columns.Add(col4);
        }

        private void addUserID()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT user_id FROM Users WHERE name = '" + username + "'";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            UserID.Text = reader.GetString(0);
                            UserID.AppendText(Environment.NewLine);
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

        private void addName()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT Users.name FROM Friend INNER JOIN Users ON Friend.friend_id = Users.user_id " +
                        "WHERE Friend.user_id = '" + username + "'";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            friendsGrid.Items.Add(reader.GetString(0));
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
    }
}
