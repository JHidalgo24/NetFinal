using System;
using System.Linq;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetFinal.Context;
using NetFinal.DataModels;
using NetFinal.Misc;

namespace NetFinal.DataManagers.Users
{
    public class DBUserManager : IUserManager
    {
        public override void AddUser()
        {
            Menu menu = new Menu();
            try
            {
                using (var db = new MovieContext())
                {
                    Console.WriteLine("What is your age?");
                    var age = menu.ValueGetter();
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
                    Console.WriteLine("What is your occupation?");
                    var occupation = Console.ReadLine();
                    if (!db.Occupations.Any(c => c.Name.ToLower() == occupation.ToLower()))
                    {
                        Console.WriteLine("Sorry that is not an occupation");
                        occupation = Console.ReadLine();
                    }

                    var occupationForUser = db.Occupations.FirstOrDefault(c => c.Name.ToLower() == occupation.ToLower());
                    Console.WriteLine("What is your zip code?");
                    int zipcode = menu.ValueGetter();
                    if (zipcode.ToString().Length != 5)
                    {
                        Console.WriteLine("Sorry ZipCode must be 5 Characters long");
                        zipcode = menu.ValueGetter();
                    }

                    User temp = new User();
                    temp.Age = age;
                    temp.Gender = gender;
                    temp.ZipCode = zipcode.ToString();
                    temp.Occupation = occupationForUser;
                    db.Users.Add(temp);
                    db.SaveChanges();
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
            throw new System.NotImplementedException();
        }

        public override void ShowUserRatings()
        {
            throw new System.NotImplementedException();
        }

        public override void AddOccupation()
        {
        }

        public override void DisplayUsers()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    var userTable = new ConsoleTable("ID","Age","Gender","ZipCode","OccupationID");
                    userTable.Options.EnableCount = false;
                    foreach (var x in db.Users)
                    {
                        userTable.AddRow(x.Id, x.Age, x.Gender, x.ZipCode, x.Occupation);
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