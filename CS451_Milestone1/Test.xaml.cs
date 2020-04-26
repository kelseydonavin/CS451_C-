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
        private string username = "";
        private string userID;

        public Test()
        {
            InitializeComponent();
            //addUserID();
            //addColumnsToFriendsGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.username = Username.Text;
            string sqlStr = "SELECT user_id FROM Users WHERE name = '" + username + "'";
            executeQuery(sqlStr, addUserIDRow);
        }

        private void userID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserID.SelectedIndex > -1)
            {
                this.userID = UserID.SelectedItem.ToString();
            }
            string sqlStr = "SELECT name, average_stars, fans, yelping_since_time, funny, useful, cool," +
                "tip_count, total_likes, latitude, longitude FROM Users WHERE user_id = '" + userID + "'";
            executeQuery(sqlStr, addUserInformation);
        }

        private void addUserIDRow(NpgsqlDataReader R)
        {
            UserID.Items.Add(R.GetString(0));
        }

        private void addUserInformation(NpgsqlDataReader R)
        {
            Name.Text = R.GetString(0);
            Stars.Text = R.GetDouble(1).ToString();
            Fans.Text = R.GetInt16(2).ToString();
            YelpingSince.Text = R.GetString(3);
            Funny.Text = R.GetInt16(4).ToString();
            Useful.Text = R.GetInt16(5).ToString();
            Cool.Text = R.GetInt16(6).ToString();
            TipCount.Text = R.GetInt16(7).ToString();
            TotalTipLikes.Text = R.GetInt16(8).ToString();
            Latitude.Text = R.GetDouble(9).ToString();
            Longitude.Text = R.GetDouble(10).ToString();
        }

        //private void addColumnsToFriendsGrid()
        //{
        //    DataGridTextColumn col1 = new DataGridTextColumn();
        //    col1.Binding = new Binding("Name");
        //    col1.Header = "Name";
        //    col1.Width = 100;
        //    friendsGrid.Columns.Add(col1);

        //    DataGridTextColumn col2 = new DataGridTextColumn();
        //    col2.Binding = new Binding("TotalLikes");
        //    col2.Header = "Total Likes";
        //    col2.Width = 80;
        //    friendsGrid.Columns.Add(col2);

        //    DataGridTextColumn col3 = new DataGridTextColumn();
        //    col3.Binding = new Binding("AvgStars");
        //    col3.Header = "Avg Stars";
        //    col3.Width = 80;
        //    friendsGrid.Columns.Add(col3);

        //    DataGridTextColumn col4 = new DataGridTextColumn();
        //    col4.Binding = new Binding("YelpingSince");
        //    col4.Header = "Yelping Since";
        //    col4.Width = 232;
        //    friendsGrid.Columns.Add(col4);
        //}

        //private void addUserID()
        //{
        //    using (var connection = new NpgsqlConnection(buildConnectionString()))
        //    {
        //        connection.Open();
        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = connection;
        //            cmd.CommandText = "SELECT user_id FROM Users WHERE name = '" + username + "'";
        //            try
        //            {
        //                var reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                    UserID.Text = reader.GetString(0);
        //                    UserID.AppendText(Environment.NewLine);
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                Console.WriteLine(ex.Message.ToString());
        //                System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}

        //private void addName()
        //{
        //    using (var connection = new NpgsqlConnection(buildConnectionString()))
        //    {
        //        connection.Open();
        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = connection;
        //            cmd.CommandText = "SELECT Users.name FROM Friend INNER JOIN Users ON Friend.friend_id = Users.user_id " +
        //                "WHERE Friend.user_id = '" + username + "'";
        //            try
        //            {
        //                var reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                    friendsGrid.Items.Add(reader.GetString(0));
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                Console.WriteLine(ex.Message.ToString());
        //                System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}

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
    }
}
