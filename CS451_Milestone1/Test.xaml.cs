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
using System.ComponentModel;

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
        private TreeViewItem category = new TreeViewItem();
        private TreeViewItem attributes = new TreeViewItem();

        public Test()
        {
            InitializeComponent();
            addState();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserID.Items.Clear();
            this.username = Username.Text;
            string sqlStr = "SELECT user_id FROM Users WHERE name = '" + username + "'";
            executeQuery(sqlStr, addUserIDList);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Latitude.IsReadOnly = false;
            Longitude.IsReadOnly = false;
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            string sqlStr = "UPDATE Users SET latitude = " + Latitude.Text + ", longitude = " + Longitude.Text + " WHERE user_id = '" + userID + "'";
            executeQuery(sqlStr, addUserInformation);
            Latitude.IsReadOnly = true;
            Longitude.IsReadOnly = true;
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            BusinessSelectedCategories.Items.Add(BusinessCategories.SelectedItem);
        }

        private void Button_Click5(object sender, RoutedEventArgs e)
        {
            BusinessSelectedCategories.Items.Remove(BusinessSelectedCategories.SelectedItem);
            BusinessCategories.UnselectAll();
        }

        private void Button_Click6(object sender, RoutedEventArgs e)
        {
            List<bool?> attributes = new List<bool?>();
            List<string> attributeStrings = new List<string>()
            { "BusinessAcceptsCreditCards", "RestaurantsReservations", "WheelchairAccessible", "OutdoorSeating", "GoodForKids",
              "RestaurantsGoodForGroups", "RestaurantsDelivery", "RestaurantsTakeOut", "WiFi", "BikeParking", "RestaurantsPriceRange2",
              "RestaurantsPriceRange2", "RestaurantsPriceRange2", "RestaurantsPriceRange2", };
            List<string> attributeValueStrings = new List<string>() { "True", "True", "True", "True", "True", "True", "True", "True", "free", "True", "1", "2", "3", "4" };

            attributes.Add(AcceptsCreditCards.IsChecked);
            attributes.Add(TakesReservations.IsChecked);
            attributes.Add(WheelchairAccess.IsChecked);
            attributes.Add(OutdoorSeating.IsChecked);
            attributes.Add(GoodForKids.IsChecked);
            attributes.Add(GoodForGroups.IsChecked);
            attributes.Add(Delivery.IsChecked);
            attributes.Add(Takeout.IsChecked);
            attributes.Add(FreeWiFi.IsChecked);
            attributes.Add(BikeParking.IsChecked);
            attributes.Add(Price1.IsChecked);
            attributes.Add(Price2.IsChecked);
            attributes.Add(Price3.IsChecked);
            attributes.Add(Price4.IsChecked);

            // Run SQL query for search results
            searchResults.Items.Clear();

            string categoryString = "";
            if (BusinessCategories.SelectedItems.Count != 0)
            {
                categoryString += "AND c.category IN (";
                foreach (var item in BusinessSelectedCategories.Items)
                {
                    categoryString += "'" + item + "',";
                }
            }
            if (categoryString.Length > 1)
            {
                categoryString = categoryString.Substring(0, categoryString.Length - 1);
                categoryString += ") GROUP BY name HAVING Count(name) = " + BusinessSelectedCategories.Items.Count;
            }

            string attributeString = "";
            int i = 0;
            int count = 0;
            if (attributes.Contains(true))
            {
                attributeString += "AND a.attribute IN (";
            }
            foreach (var item in attributes)
            {
                if (item == true)
                {
                    attributeString += "'" + attributeStrings[i] + "',";
                    count++;
                }
                i++;
            }
            if (attributeString.Length > 1)
            {
                attributeString = attributeString.Substring(0, attributeString.Length - 1);
                attributeString += ") ";
            }

            int i2 = 0;
            string attributeValueString = "";
            if (attributes.Contains(true))
            {
                attributeValueString += "AND a.attribute_value IN (";
            }
            foreach (var item in attributes)
            {
                if (item == true)
                {
                    attributeValueString += "'" + attributeValueStrings[i2] + "',";
                }
                i2++;
            }
            if (attributeValueString.Length > 1)
            {
                attributeValueString = attributeValueString.Substring(0, attributeValueString.Length - 1);
                attributeValueString += ") GROUP BY name HAVING Count(name) = " + count;
            }

            string sqlStr =
                "SELECT distinct name, city, state, latitude, longitude, stars, tip_count, check_in_count FROM business " +
                "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " + 
                "AND name IN " +
                    "(SELECT name FROM business as b INNER JOIN category as c ON b.business_id = c.business_id " +
                    "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " +
                    categoryString + ") " +
                "AND name IN " +
                    "(SELECT name FROM business as b INNER JOIN attribute as a ON b.business_id = a.business_id " +
                    "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " +
                    attributeString + attributeValueString + ") " +
                "ORDER BY name";
            executeQuery(sqlStr, addSearchResultsRow);
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
            FriendsGrid.Items.Clear();
            string sqlStr2 = "SELECT Users.name, Users.total_likes, Users.average_stars, Users.yelping_since_time, Users.yelping_since_date " +
                "FROM Friend INNER JOIN Users ON Friend.friend_id = Users.user_id WHERE Friend.user_id = '" + userID + "'";
            executeQuery(sqlStr2, addFriendRow);
            FriendsTips.Items.Clear();
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
            Fans.Text = R.GetInt64(2).ToString();
            YelpingSince.Text = R.GetString(3);
            Funny.Text = R.GetInt64(4).ToString();
            Useful.Text = R.GetInt64(5).ToString();
            Cool.Text = R.GetInt64(6).ToString();
            TipCount.Text = R.GetInt64(7).ToString();
            TotalTipLikes.Text = R.GetInt64(8).ToString();
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

        private void addZipcodeRow(NpgsqlDataReader R)
        {
            zipcodeList.Items.Add(R.GetInt64(0).ToString());
        }

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipcodeList.Items.Clear();
            if (cityList.SelectedIndex > -1)
            {
                string sqlStr = "SELECT distinct postal_code FROM business WHERE state = '" + stateList.SelectedItem.ToString() + "' AND city = '" + cityList.SelectedItem.ToString() + "' ORDER BY postal_code";
                executeQuery(sqlStr, addZipcodeRow);
            }
        }

        private void addSearchResultsRow(NpgsqlDataReader R)
        {
            // Calculate distance
            double lat1 = (R.GetDouble(3) * Math.PI) / 180;
            double long1 = (R.GetDouble(4) * Math.PI) / 180;
            double lat2, long2;
            if (String.IsNullOrEmpty(Latitude.Text)) { lat2 = 0; } else { lat2 = (Convert.ToDouble(Latitude.Text) * Math.PI) / 180; }
            if (String.IsNullOrEmpty(Longitude.Text)) { long2 = 0; } else { long2 = (Convert.ToDouble(Longitude.Text) * Math.PI) / 180; }
            double dlat = lat2 - lat1;
            double dlong = long2 - long1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Pow(Math.Sin(dlong / 2), 2);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            double r = 3956;
            double distance = c * r;

            searchResults.Items.Add(new
            {
                BusinessName = R.GetString(0),
                Address = "",
                City = R.GetString(1),
                State = R.GetString(2),
                Distance = Math.Round(distance, 2),
                Stars = R.GetDouble(5),
                Tips = R.GetInt64(6),
                Checkins = R.GetInt64(7)
            });
        }

        private void addBusinessCategories(NpgsqlDataReader R)
        {
            BusinessCategories.Items.Add(R.GetString(0));
        }

        private void zipcodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchResults.Items.Clear();
            if (zipcodeList.SelectedIndex > -1)
            {
                string sqlStr =
                    "SELECT name, city, state, latitude, longitude, stars, tip_count, check_in_count FROM Business " +
                    "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + 
                    " ORDER BY name";
                executeQuery(sqlStr, addSearchResultsRow);
                BusinessCategories.Items.Clear();
                string sqlStr2 = "SELECT DISTINCT category FROM Category INNER JOIN Business ON Category.business_id = Business.business_id " +
                            "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem +
                            " ORDER BY category asc";
                executeQuery(sqlStr2, addBusinessCategories);
            }
        }

        private void addSelectedBusiness(NpgsqlDataReader R)
        {
            DayOfWeek day = DateTime.Today.DayOfWeek;
            string open = R.GetString(0);
            int openHour = Convert.ToInt32(open.Substring(0, open.IndexOf(":")));
            int openMinute = Convert.ToInt32(open.Substring(open.IndexOf(":") + 1));
            DateTime dt1 = new DateTime(10, 10, 10, openHour, openMinute, 10);
            string close = R.GetString(1);
            int closeHour = Convert.ToInt32(close.Substring(0, close.IndexOf(":")));
            int closeMinute = Convert.ToInt32(close.Substring(close.IndexOf(":") + 1));
            DateTime dt2 = new DateTime(10, 10, 10, closeHour, closeMinute, 10);
            string newOpen = String.Format("{0:hh:mm tt}", dt1);
            string newClose = String.Format("{0:hh:mm tt}", dt2);
            HoursTextBox.Text = "Today (" + day.ToString() + "):   Opens: " + newOpen + "   Closes: " + newClose;
        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchResults.SelectedIndex > -1)
            {
                object item = searchResults.SelectedItem;
                string name = (searchResults.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                string sqlStr = "SELECT open, close FROM Hours INNER JOIN Business ON Hours.business_id = Business.business_id" +
                                " WHERE name = '" + name + "' AND day = '" + DateTime.Today.DayOfWeek + "'";
                executeQuery(sqlStr, addSelectedBusiness);

                BusinessNameTextBox.Text = name;
                AddressTextBox.Text = cityList.SelectedItem + ", " + stateList.SelectedItem;

                // Add business category/attribute Tree View items
                addTreeView();
            }
        }

        private void addCategories(NpgsqlDataReader R)
        {
            category.Items.Add(new TreeViewItem() { Header=R.GetString(0)});
        }

        private void addAttributes(NpgsqlDataReader R)
        {
            if (R.GetString(1) == "True")
            {
                attributes.Items.Add(new TreeViewItem() { Header = R.GetString(0) });
            }
            else
            {
                attributes.Items.Add(new TreeViewItem() { Header = (R.GetString(0) + " (" + R.GetString(1) + ")") });
            }
        }

        private void addTreeView()
        {
            TreeView.Items.Clear();
            this.category.Header = "Categories";
            TreeView.Items.Add(category);
            this.attributes.Header = "Attributes";
            TreeView.Items.Add(attributes);

            object item = searchResults.SelectedItem;
            string name = (searchResults.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

            string sqlStr = "SELECT distinct category FROM business as b INNER JOIN category as c ON b.business_id = c.business_id WHERE name = '" + name + "' ORDER BY category";
            executeQuery(sqlStr, addCategories);

            string sqlStr2 = "SELECT distinct attribute, attribute_value FROM business as b INNER JOIN attribute as a ON b.business_id = a.business_id " +
                             "WHERE name = '" + name + "' AND attribute_value<> 'False' ORDER BY attribute";
            executeQuery(sqlStr2, addAttributes);
        }

        private void sortList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> column = new List<string>() { "name", "stars", "tip_count", "check_in_count", "name" };
            List<string> direction = new List<string>() { "asc", "desc", "desc", "desc", "asc" };

            List<bool?> attributes = new List<bool?>();
            List<string> attributeStrings = new List<string>()
            { "BusinessAcceptsCreditCards", "RestaurantsReservations", "WheelchairAccessible", "OutdoorSeating", "GoodForKids",
              "RestaurantsGoodForGroups", "RestaurantsDelivery", "RestaurantsTakeOut", "WiFi", "BikeParking", "RestaurantsPriceRange2",
              "RestaurantsPriceRange2", "RestaurantsPriceRange2", "RestaurantsPriceRange2", };
            List<string> attributeValueStrings = new List<string>() { "True", "True", "True", "True", "True", "True", "True", "True", "free", "True", "1", "2", "3", "4" };

            attributes.Add(AcceptsCreditCards.IsChecked);
            attributes.Add(TakesReservations.IsChecked);
            attributes.Add(WheelchairAccess.IsChecked);
            attributes.Add(OutdoorSeating.IsChecked);
            attributes.Add(GoodForKids.IsChecked);
            attributes.Add(GoodForGroups.IsChecked);
            attributes.Add(Delivery.IsChecked);
            attributes.Add(Takeout.IsChecked);
            attributes.Add(FreeWiFi.IsChecked);
            attributes.Add(BikeParking.IsChecked);
            attributes.Add(Price1.IsChecked);
            attributes.Add(Price2.IsChecked);
            attributes.Add(Price3.IsChecked);
            attributes.Add(Price4.IsChecked);

            // Run SQL query for search results
            searchResults.Items.Clear();

            string categoryString = "";
            if (BusinessCategories.SelectedItems.Count != 0)
            {
                categoryString += "AND c.category IN (";
                foreach (var item in BusinessSelectedCategories.Items)
                {
                    categoryString += "'" + item + "',";
                }
            }
            if (categoryString.Length > 1)
            {
                categoryString = categoryString.Substring(0, categoryString.Length - 1);
                categoryString += ") GROUP BY name HAVING Count(name) = " + BusinessSelectedCategories.Items.Count;
            }

            string attributeString = "";
            int i = 0;
            int count = 0;
            if (attributes.Contains(true))
            {
                attributeString += "AND a.attribute IN (";
            }
            foreach (var item in attributes)
            {
                if (item == true)
                {
                    attributeString += "'" + attributeStrings[i] + "',";
                    count++;
                }
                i++;
            }
            if (attributeString.Length > 1)
            {
                attributeString = attributeString.Substring(0, attributeString.Length - 1);
                attributeString += ") ";
            }

            int i2 = 0;
            string attributeValueString = "";
            if (attributes.Contains(true))
            {
                attributeValueString += "AND a.attribute_value IN (";
            }
            foreach (var item in attributes)
            {
                if (item == true)
                {
                    attributeValueString += "'" + attributeValueStrings[i2] + "',";
                }
                i2++;
            }
            if (attributeValueString.Length > 1)
            {
                attributeValueString = attributeValueString.Substring(0, attributeValueString.Length - 1);
                attributeValueString += ") GROUP BY name HAVING Count(name) = " + count;
            }

            searchResults.Items.Clear();
            string sqlStr =
                "SELECT distinct name, city, state, latitude, longitude, stars, tip_count, check_in_count FROM business " +
                "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " +
                "AND name IN " +
                    "(SELECT name FROM business as b INNER JOIN category as c ON b.business_id = c.business_id " +
                    "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " +
                    categoryString + ") " +
                "AND name IN " +
                    "(SELECT name FROM business as b INNER JOIN attribute as a ON b.business_id = a.business_id " +
                    "WHERE state = '" + stateList.SelectedItem + "' AND city = '" + cityList.SelectedItem + "' AND postal_code = " + zipcodeList.SelectedItem + " " +
                    attributeString + attributeValueString + ") " +
                "ORDER BY " + column[Sort.SelectedIndex] + " " + direction[Sort.SelectedIndex];
            executeQuery(sqlStr, addSearchResultsRow);
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



        private void ShowCheckins_Click(object sender, RoutedEventArgs e)
        {
            string sqlStr = "SELECT ";

            executeQuery(sqlStr, addSearchResultsRow);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            object item = searchResults.SelectedItem;
            string name = (searchResults.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
            string sqlStr = "SELECT tip.date, users.name, tip.likes, tip.text " +
                "FROM tip, users, business" +
                "WHERE business.name = '" + name + "'" +
                "AND business.business_id = tip.business_id" +
                "AND tip.user_id = users.user_id";

            BuisnessTips businessWindow = new BuisnessTips();

            executeQuery(sqlStr, businessWindow.addTipResultsRow);

            businessWindow.Show();
        }
    }
}
