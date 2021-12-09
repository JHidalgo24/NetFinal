using System;
using ConsoleTables;
using NetFinal.DataManagers.Movie;
using NetFinal.DataManagers.Users;
using NetFinal.Misc;
using NLog;

namespace NetFinal
{
    class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            Menu menu = new Menu();
            IMovieManager movieManager = new DBMovieManager();
            IUserManager userManager = new DBUserManager();
            int option = 0;
            while (option != 9)
            {
                menu.DisplayOptions();
                option = menu.IntValueGetter();
                switch (option)
                {
                    case 1:
                        movieManager.AddMovie();
                        break;
                    case 2:
                        var tableTemp = new ConsoleTable("Option","Choice");
                        tableTemp.Options.EnableCount = false;
                        tableTemp.AddRow(1, "Edit Movie").AddRow(2, "Delete Movie");
                        tableTemp.Write();
                        var decision = menu.IntValueGetter();
                        if (decision == 1)
                        {
                            logger.Debug("User wants to alter/Edit movie");
                            movieManager.EditMovie();
                        }
                        else if (decision == 2)
                        {
                            logger.Debug("User wants to Delete movie");
                            movieManager.DeleteMovie();
                        }
                        else
                        {
                            logger.Debug("User chose an invalid option for editing or deleting"); 
                            Console.WriteLine("Invalid input");
                        }
                        break;
                    case 3:
                        logger.Debug("User Chose option 3");
                        var table = new ConsoleTable("Option","");
                        table.Options.EnableCount = false;
                        table.AddRow(1, "Search").AddRow(2,"Display all");
                        table.Write();
                        int choice = menu.IntValueGetter();
                        if (choice == 1)
                            movieManager.SearchMovie();
                        else if (choice == 2){
                            logger.Debug("User chose Display All Movies");
                            movieManager.DisplayAll();
                        }
                        else
                            Console.WriteLine("Sorry not an option");
                        break;
                    case 4:
                        logger.Debug("User chose Add User");
                        userManager.AddUser();
                        break;
                    case 5:
                        logger.Debug("User chose Add User Rating");
                        userManager.AddUserRating();
                        break;
                    case 6:
                        logger.Debug("User chose Show User Ratings");
                        userManager.ShowUserRatings();
                        break;
                    case 7:
                        logger.Debug("User chose Search User Ratings");
                        userManager.UserRatingSearch();
                        break;
                    case 8:
                        movieManager.DisplayMoviesWithGenre();
                        logger.Debug("User chose Display movies by genres");
                        break;
                    case 9:
                        Console.WriteLine("Thank you for using the Application!");
                        break;
                    default:
                        logger.Debug("User exited Program");
                        Console.WriteLine("Sorry that isn't a choice!");
                        break;
                }
                if (option != 9)
                {
                    Console.WriteLine("Press enter to Continue...");
                    Console.ReadLine();
                }
            }

        }
    }
}