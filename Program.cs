using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetFinal.Context;
using NetFinal.DataManagers;
using NetFinal.DataManagers.Movie;
using NetFinal.DataManagers.Users;
using NetFinal.DataModels;
using NetFinal.Misc;

namespace NetFinal
{
    class Program
    {
        public static void Main(string[] args)
        {
            Menu menu = new Menu();
            IMovieManager movieManager = new DBMovieManager();
            IUserManager userManager = new DBUserManager();
            int option = 0;
            while (option != 7)
            {
                menu.DisplayOptions();
                option = menu.IntValueGetter();
                switch (option)
                {
                    case 1:
                        movieManager.AddMovie();
                        break;
                    case 2:
                        movieManager.EditMovie();
                        break;
                    case 3:
                        var table = new ConsoleTable("Option","");
                        table.Options.EnableCount = false;
                        table.AddRow(1, "Search").AddRow(2,"Display all");
                        table.Write();
                        int choice = menu.IntValueGetter();
                        if (choice == 1)
                            movieManager.SearchMovie();
                        else if (choice == 2)
                            movieManager.DisplayAll();
                        else
                            Console.WriteLine("Sorry not an option");
                        break;
                    case 4:
                        userManager.AddUser();
                        break;
                    case 5:
                        userManager.AddUserRating();
                        break;
                    case 6:
                        userManager.ShowUserRatings();
                        break;
                    case 7:
                        userManager.UserRatingSearch();
                        break;
                    case 8:
                        Console.WriteLine("Thank you for using the Application!");
                        break;
                    default:
                        Console.WriteLine("Sorry that isn't a choice!");
                        break;
                }

                if (option != 8)
                {
                    Console.WriteLine("Press enter to Continue...");
                    Console.ReadLine();
                }
            }

        }
    }
}