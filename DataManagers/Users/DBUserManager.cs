using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public override User? AddUser()
        {
            Menu menu = new Menu();
            try
            {
                using (var db = new MovieContext())
                {
                    Console.WriteLine("What is your age?");
                    var age = menu.ValueGetter();
                    while (age is > 118 or < 0)
                    {
                        Console.WriteLine("That is not a valid age enter a new one");
                        age = menu.ValueGetter();
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
                    var user = db.Users.FirstOrDefault(x => x == temp);
                    return user;
                }
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
                    var age = menu.ValueGetter();
                    if (age < 0 || age > 118)
                    {
                        Console.WriteLine("Sorry that isn't a valid age\nEnter a new age");
                        age = menu.ValueGetter();
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
                using (var db = new MovieContext())
                {
                    Menu menu = new Menu();
                    var movieTable = new ConsoleTable("Option","Movie Title");
                    var option = 1;
                    User user = new User();
                    var userValid = "";
                    Console.WriteLine("Are you already a user? (Y/N)");
                    var alreadyUser = Console.ReadLine().ToLower().Substring(0,1);
                    if (alreadyUser == "y")
                    {
                        user = GetUser();
                    }
                    if (alreadyUser =="n" || user == null)
                    {
                        if (user == null)
                        {
                            Console.WriteLine("Sorry your user wasn't found\nYou will need to make a new one"); 
                        }
                        user = AddUser();
                        
                    }
                    Console.WriteLine("What film do you want to add a rating for?");
                    var title = Console.ReadLine();
                    var selectedMovies = db.Movies.Where(c => c.Title.ToLower().Contains(title.ToLower()));
                    while (selectedMovies.Count() == 0)
                    {
                        Console.WriteLine("Sorry nothing came up for that search\nEnter a new Film Title");
                        title = Console.ReadLine();
                        selectedMovies = db.Movies.Where(c => c.Title.ToLower().Contains(title.ToLower()));
                    }
                    foreach (var x in selectedMovies)
                    {
                        movieTable.AddRow(option, x.Title);
                        option++;
                    }
                    movieTable.Write();
                    Console.WriteLine("Which option do you want to add a rating for?");
                    option = menu.ValueGetter();
                    while (selectedMovies.ToList().Count < option || option <= 0)
                    {
                        Console.WriteLine("Sorry that isn't an option\nSelect a new Option");
                        option = menu.ValueGetter();
                    }
                    var selectedFilm = selectedMovies.ToList()[option - 1];
                    Console.WriteLine($"You have selected {selectedFilm.Title}");
                    Console.WriteLine("What is the rating you want to give this film? 1-5");
                    var rating = menu.ValueGetter();
                    while (rating > 5 || rating <= 0)
                    {
                        Console.WriteLine("Sorry only 1-5 is allowed\nEnter a new rating");
                        rating = menu.ValueGetter();
                    }
                    DateTime ratedAt = DateTime.Now;
                    UserMovie temp = new UserMovie();
                    temp.Movie = selectedFilm;
                    temp.Rating = rating;
                    temp.RatedAt = ratedAt;
                    temp.User = user;
                    db.UserMovies.Add(temp);
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
            /*still need to figure out how to make this work
            says can't manually add Key for occupations although 
            occupations isn't being accessed here (Last thing to add before piecing main together)*/
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
    }
}