using System;

namespace RAMSocketAsync {
    public class UserAuthenticator
    {
        public int CurrentUserID { get; set; }
        public UserAuthenticator()
        {
             // instead of creating a list of user credentials, we need save the credentials in a file
             // include a userId in the file 
             // userid      username    password
            //temp code to populate user list
        }

        public bool AuthenticateUser(string userName, string userPassword)
        {
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)\userCredentials.txt");
                string fileLine = file.ReadLine();
                string[] userData = null;
                
                
                while (fileLine != null)
                {
                    userData = fileLine.Split(' ');
                    CurrentUserID = Convert.ToInt32(userData[0]);
                    if (userData[1] == userName)
                    {
                        if (userData[2] == userPassword)
                        {
                            Console.WriteLine("Username and Password are correct.\n" +
                                "User ID: " + CurrentUserID);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect password submitted.");
                            return false;
                        }
                    }
                    fileLine = file.ReadLine();
                }
                Console.WriteLine("User does not exist.");
                return false;
            }
            catch (Exception exp )
            {
                Console.WriteLine("Exception : {0}", exp.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("Authentication process finished");


            }
        }
    }
}
