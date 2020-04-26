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
        private List<string> distinctID = new List<string>();

        public Test()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.username = Username.Text;
            string sqlStr = "SELECT user_id FROM Users WHERE name = '" + username + "'";
            executeQuery(sqlStr, addUserIDList);
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
            string sqlStr2 = "SELECT Users.name, Users.total_likes, Users.average_stars, Users.yelping_since_time, Users.yelping_since_date " +
                "FROM Friend INNER JOIN Users ON Friend.friend_id = Users.user_id WHERE Friend.user_id = '" + userID + "'";
            executeQuery(sqlStr2, addFriendRow);
            string sqlStr3 =
                "SELECT Friend.friend_id, Users.name, business.name, business.city, tip.text, tip.date, tip.time " +
                "FROM Friend INNER JOIN Users ON Friend.friend_id = Users.user_id " +
                "INNER JOIN tip ON Friend.friend_id = tip.user_id " +
                "INNER JOIN business ON tip.business_id = business.business_id " +
                "WHERE Friend.user_id = '" + userID + "' " +
                "ORDER BY date desc";
            executeQuery(sqlStr3, addFriendTipsRow);
        }

        private void addUserIDList(NpgsqlDataReader R)
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

        private void addFriendRow(NpgsqlDataReader R)
        {
            string yelpingSinceDateTime = R.GetString(3) + " " + R.GetString(4);
            FriendsGrid.Items.Add(new
            {
                Name = R.GetString(0),
                TotalLikes = R.GetInt64(1),
                AvgStars = R.GetDouble(2),
                YelpingSince = yelpingSinceDateTime
            });
        }

        private void addFriendTipsRow(NpgsqlDataReader R)
        {
            string yelpingSinceDateTime = R.GetDate(5).ToString() + " " + R.GetTimeSpan(6).ToString();
            if (!distinctID.Contains(R.GetString(0)))
            {
                FriendsTips.Items.Add(new
                {
                    UserName = R.GetString(1),
                    Business = R.GetString(2),
                    City = R.GetString(3),
                    Text = R.GetString(4),
                    Date = yelpingSinceDateTime
                });
            }
            distinctID.Add(R.GetString(0));
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
    }
}
