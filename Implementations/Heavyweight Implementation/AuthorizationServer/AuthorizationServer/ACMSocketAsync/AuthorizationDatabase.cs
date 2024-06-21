using System.Data;
using System.Data.SqlClient;

namespace ACMSocketAsync {
    public class AuthorizationDatabase {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Programming Dump\\Heavyweight Implementation\\DatabaseFile\\DatabaseFile\\PolicyDBCopy.mdf;Integrated Security = True";

        public DataTable getA_Hierarchy() {
            DataTable a_Hierarchy = new DataTable("A_Hierarchy");
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM A_Hierarchy", connection);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3}",
                    //    reader[0], reader[1], reader[2], reader[3]));
                    //}
                    a_Hierarchy.Load(reader);
                }
                connection.Close();
            }

            return a_Hierarchy;
        }

        public DataTable getSTZone() {
            DataTable sTZone = new DataTable("STZone");
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM STZone", connection);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3}",
                    //    reader[0], reader[1], reader[2], reader[3]));
                    //}
                    sTZone.Load(reader);
                }
                connection.Close();
            }

            return sTZone;
        }

        public DataTable getLocations() {
            DataTable locations = new DataTable("Locations");
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Locations", connection);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3}",
                    //    reader[0], reader[1], reader[2], reader[3]));
                    //}
                    locations.Load(reader);
                }
                connection.Close();
            }

            return locations;
        }

        public DataTable getTimeIntervals() {
            DataTable timeIntervals = new DataTable("TimeIntervals");
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM TimeIntervals", connection);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3}",
                    //    reader[0], reader[1], reader[2], reader[3]));
                    //}
                    timeIntervals.Load(reader);
                }
                connection.Close();
            }

            return timeIntervals;
        }

        // fields of all tables 
        public AuthorizationDatabase() {
            //foreach (DataRow row in A_HierarchyGet().Rows)
            //{
            //    //A_HierarchyList.Add(new A_Hierarchy() { Id = 1, SeniorRoleId = 1, JuniorRoleId = 1, STZoneId = 1 });

            //    Console.Write(row.ItemArray[0]);

            //}

            //Console.WriteLine();

            //make connection to DataBase and initialize objects with Database data.

            // write sql quertiy to get all the table and initialize them 
        }
    }
}
