using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Channels;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetFinal.Context;
using NetFinal.DataManagers.Movie;
using NetFinal.DataModels;
using NetFinal.Misc;

namespace NetFinal.DataManagers.Users
{
    public class DBUserManager : IUserManager
    {
        public override User AddUser()
        {
            Menu menu = new Menu();
            try
            {
                var user = new User();
                using (var db = new MovieContext())
                {
                    Console.WriteLine("What is your age?");
                    var age = menu.IntValueGetter();
                    while (age is > 118 or < 0)
                    {
                        Console.WriteLine("That is not a valid age enter a new one");
                        age = menu.IntValueGetter();
                    }
                    Console.WriteLine("What is your gender?(M/F)");
                    var gender = Console.ReadLine().ToUpper().Substring(0,1);
                    while (gender is not ("F" or "M"))
                    {
                        Console.WriteLine("Sorry only F and M are accepted. Enter again!");
                        gender = Console.ReadLine().ToUpper().Substring(0,1);
                    }

                    var occupationTables = new ConsoleTable("ID", "Occupation");
                    occupationTables.Options.EnableCount = false;
                    foreach (var x in db.Occupations)
                    {
                        occupationTables.AddRow(x.Id, x.Name);
                    }
                    occupationTables.Write();
                    Console.WriteLine("Is your occupation in the table? (Y/N)");
                    var answer = Console.ReadLine();
                    if (answer.ToLower().Substring(0,1) == "n")
                    {
                        AddOccupation();
                    }
                    Console.WriteLine("What is your occupation (Enter the one you added if any)?");
                    var occupation = Console.ReadLine();
                    var occupationForUser = db.Occupations.FirstOrDefault(c => c.Name.ToLower() == occupation.ToLower());
                    if (occupationForUser == null)
                    {
                        Console.WriteLine("Sorry that isn't an occupation (Check Spelling)");
                        occupationForUser = db.Occupations.FirstOrDefault(c => c.Name.ToLower() == occupation.ToLower());
                    }
                    Console.WriteLine("What is your zip code?");
                    var zipcode = Console.ReadLine();
                    if (zipcode.Length != 5)
                    {
                        Console.WriteLine("Sorry ZipCode must be 5 Characters long");
                        zipcode = Console.ReadLine();
                    }
                    User temp = new User();
                    temp.Age = age;
                    temp.Gender = gender;
                    temp.ZipCode = zipcode;
                    temp.Occupation = occupationForUser;
                    db.Users.Add(temp);
                    db.SaveChanges();
                    user = db.Users.FirstOrDefault(x => x == temp);
                }
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override User GetUser()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    Menu menu = new Menu();
                    Console.WriteLine("What is your age?");
                    var age = menu.IntValueGetter();
                    if (age < 0 || age > 118)
                    {
                        Console.WriteLine("Sorry that isn't a valid age\nEnter a new age");
                        age = menu.IntValueGetter();
                    }
                    Console.WriteLine("What is your gender?(M/F)");
                    var gender = Console.ReadLine().ToUpper().Substring(0,1);
                    while (gender is not ("F" or "M"))
                    {
                        Console.WriteLine("Sorry only F and M are accepted. Enter again!");
                        gender = Console.ReadLine().ToUpper().Substring(0,1);
                    }
                    Console.WriteLine("What is your zip code?");
                    var zipcode = Console.ReadLine();
                    if (zipcode.Length != 5)
                    {
                        Console.WriteLine("Sorry ZipCode must be 5 Characters long");
                        zipcode = Console.ReadLine();
                    }
                    var occupationTables = new ConsoleTable("ID", "Occupation");
                    occupationTables.Options.EnableCount = false;
                    foreach (var x in db.Occupations)
                    {
                        occupationTables.AddRow(x.Id, x.Name);
                    }
                    occupationTables.Write();
                    Console.WriteLine("What is your occupation?");
                    var occupation = Console.ReadLine();
                    if (!db.Occupations.Any(oc => oc.Name.ToLower() == occupation.ToLower()))
                    {
                        Console.WriteLine("Sorry that occupation doesn't match any in the table\nEnter a new one");
                        occupation = Console.ReadLine();
                    }
                    var chosenOc = db.Occupations.FirstOrDefault(oc => oc.Name.ToLower() == occupation.ToLower());
                    var user = db.Users.FirstOrDefault(c => c.Age == age && c.Gender == gender && c.Occupation == chosenOc && c.ZipCode == zipcode);
                    if (user == null)
                    {
                        Console.WriteLine("Sorry unable to find user!");
                        return null;
                    }
                    else
                        return user;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        public override void AddUserRating()
        {
            try
            {
                List<DataModels.Movie> movies = new List<DataModels.Movie>();
                List<UserMovie> userMovies = new List<UserMovie>();
                List<User> users = new List<User>();
                List<Occupation> occupations = new List<Occupation>();
                Menu menu = new Menu();
                var tempUser = new User();
                var movieChoices = new ConsoleTable("Option","Movie");
                int currentChoice = 1;
                movieChoices.Options.EnableCount = false;
                using (var db = new MovieContext())
                {
                    movies = db.Movies.ToList();
                    userMovies = db.UserMovies.ToList();
                    users = db.Users.ToList();
                    occupations = db.Occupations.ToList();
                }
                Console.WriteLine("Are you already a user?(Y/N)");
                var alreadyUser = Console.ReadLine().Substring(0, 1).ToUpper();
                if (alreadyUser == "Y")
                {
                    tempUser = GetUser();
                }
                if (tempUser == null || alreadyUser == "N")
                {
                    tempUser = AddUser();
                }
                Console.WriteLine("What movie would you like to rate?");
                var search = Console.ReadLine();
                var fiteredMovies = movies.Where(t => t.Title.ToLower().Contains(search.ToLower()));
                while (fiteredMovies.Count() == 0)
                {
                    Console.WriteLine("There isn't a movie with that title enter a new search");
                    search = Console.ReadLine();
                    fiteredMovies = movies.Where(t => t.Title.ToLower().Contains(search.ToLower()));
                }
                foreach (var x in fiteredMovies)
                {
                    movieChoices.AddRow(currentChoice, x.Title);
                    currentChoice++;
                }
                movieChoices.Write();
                Console.WriteLine("Which option would you like to rate?");
                int decision = menu.IntValueGetter();
                while (decision > fiteredMovies.Count() || decision < 0)
                {
                    Console.WriteLine("Sorry that isn't a choice (Enter a new one)");
                    decision = menu.IntValueGetter();
                }
                var moviePicked = fiteredMovies.ToList()[decision - 1];
                Console.WriteLine($"What rating would you like to give {moviePicked.Title} (1-5)? ");
                var rating = menu.IntValueGetter();
                while (rating > 5 || rating <= 0)
                {
                    if (rating > 5)
                    {
                        Console.WriteLine("If you really liked the movie just put 5");
                    }
                    else
                    {
                        Console.WriteLine("If you hated the movie just put 1");
                    }
                    Console.WriteLine("Enter again");
                    rating = menu.IntValueGetter();
                }
                var ratedAt = DateTime.Now;
                UserMovie tempUserMovies = new UserMovie();
                tempUserMovies.Rating = rating;
                tempUserMovies.RatedAt = ratedAt;
                tempUserMovies.User = tempUser;
                tempUserMovies.Movie = moviePicked;
                using (var db = new MovieContext())
                {
                    db.UserMovies.Update(tempUserMovies);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry couldn't access Movie Database");
                throw;
            }
        } 

        public override void ShowUserRatings()
        {

            try
            {
                List<Occupation> occupations;
                List<UserMovie> userMovies = new List<UserMovie>();
                using (var db = new MovieContext())
                {
                    var userRatings = db.UserMovies.Include(c => c.Movie).Include(c => c.User).OrderByDescending(c=>c.Rating);
                    var userRatingsTable = new ConsoleTable("Movie","Rating","Occupation");
                    userRatingsTable.Options.EnableCount = false;
                    occupations = db.Occupations.ToList();
                    foreach (var x in occupations)
                    {
                        var movie = userRatings.FirstOrDefault(c => c.User.Occupation == x);
                        userRatingsTable.AddRow(movie?.Movie.Title == null ? "No Movie Info": movie.Movie.Title, movie?.Rating == null ? "N/A": movie.Rating, x.Name);
                    }
                    userRatingsTable.Write();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public override void AddOccupation()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    Console.WriteLine("What is your Occupation? EX.(Student, Teacher, etc.)");
                    var occupationName = Console.ReadLine();
                    if (db.Occupations.Any(x=> x.Name.ToLower() == occupationName))
                    {
                        Console.WriteLine("Sorry that is already in the list\nEnter a new Occupation");
                        occupationName = Console.ReadLine();
                    }
                    Occupation temp = new Occupation();
                    temp.Name = occupationName;
                    db.Occupations.Add(temp);
                    db.SaveChanges();
                    var table = new ConsoleTable("ID","Occupation Name");
                    table.Options.EnableCount = false;
                    foreach (var x in db.Occupations)
                    {
                        table.AddRow(x.Id, x.Name);
                    }
                    table.Write();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void DisplayUsers()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    var userTable = new ConsoleTable("ID","Age","Gender","ZipCode","OccupationID");
                    userTable.Options.EnableCount = false;
                    foreach (var x in db.Users.Include(t => t.Occupation))
                    {
                        userTable.AddRow(x.Id, x.Age, x.Gender, x.ZipCode, x.Occupation.Name);
                    }
                    userTable.Write();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void UserRatingSearch()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    var user = GetUser();
                    var table = new ConsoleTable("User","Title","Rating");
                    table.Options.EnableCount = false;
                    if (user == null)
                    {
                        Console.WriteLine("Sorry that user wasn't found");
                    }
                    else
                    {
                        var userMovies = db.UserMovies.Where(t => t.User == user).Include(t => t.Movie)
                            .Include(t => t.User);
                        foreach (var x in userMovies)
                        {
                            table.AddRow($"UserID:{x.User.Id}", x.Movie.Title, x.Rating);
                        }
                        table.Write();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to access Movie Databse");
                throw;
            }
        }
    }
}