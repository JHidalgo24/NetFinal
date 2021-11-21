using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks.Dataflow;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using NetFinal.Context;
using NetFinal.DataManagers.Users;
using NetFinal.DataModels;
using NetFinal.Migrations;
using NetFinal.Misc;

namespace NetFinal.DataManagers.Movie
{
    public class DBMovieManager: IMovieManager
    {
        public void AddMovie()
        {
            Menu menu = new Menu();
            try
            {
                
                using (var db = new MovieContext())
                {
                    Console.WriteLine("What is the title of the film?");
                    string title = Console.ReadLine();
                    Console.WriteLine("What year was the movie made? ex.(1979)");
                    int year = menu.ValueGetter();
                    DateTime yearFormatted = DateTime.Parse($"01-01-{year}");
                    title = title + " (" + year + ")";
                    var movies = db.Movies.ToList();
                    if (movies.Any(c=> c.Title.ToLower() == title.ToLower()))
                    {
                        Console.WriteLine($"Sorry your Title of: {title} is already in the database");
                    }
                    else
                    {
                        DataModels.Movie temp = new DataModels.Movie();
                        temp.Title = title;
                        temp.ReleaseDate = yearFormatted;
                        db.Movies.Add(temp);
                        db.SaveChanges();
                        var genres = db.Genres.ToList();
                        var tableGenres = new ConsoleTable("ID", "Genre");
                        tableGenres.Options.EnableCount = false;
                        foreach (var x in genres)
                        {
                            tableGenres.AddRow(x.GenreId, x.Name);
                        }
                        tableGenres.Write();
                        Console.WriteLine("Do you want to add a genre that is not in the list? (Y/N)");
                        string z = Console.ReadLine();
                        while (z.ToLower().Substring(0,1) == "y")
                        {
                            AddGenre();
                            Console.WriteLine("Is there another genre you want to add to the list?(Y/N)");
                            z = Console.ReadLine();
                        }

                        var tableGenresUpdated = new ConsoleTable("ID", "Genre");
                        tableGenresUpdated.Options.EnableCount = false;
                        foreach (var x in db.Genres)
                        {
                            tableGenresUpdated.AddRow(x.GenreId, x.Name);
                        }
                        tableGenresUpdated.Write();
                        var movieInputted = db.Movies.ToList().FirstOrDefault(c => c.Title == title);
                        Console.WriteLine("How many of the genres in here are in the film?");
                        var genreCount = menu.ValueGetter();
                        for (int i = 0; i < genreCount; i++)
                        {
                            Console.WriteLine("Which genres are in the film?\n" +
                                              "Include the ones you added to the list if any");
                            string genrePicked = Console.ReadLine();
                            while (!db.Genres.Any(c=> c.Name.ToLower() == genrePicked.ToLower()))
                            {
                                Console.WriteLine("Sorry that is not a genre in the List Select a new one");
                                genrePicked = Console.ReadLine();
                            }
                            var tempGenre = db.Genres.FirstOrDefault(c => c.Name.ToLower() == genrePicked.ToLower());
                            MovieGenre tempMG = new MovieGenre();
                            tempMG.Genre = tempGenre;
                            tempMG.Movie = movieInputted;
                            db.MovieGenres.Add(tempMG);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception )
            {
                Console.WriteLine("Couldn't connect to Movie Database");
                throw;
            }
        }

        public void EditMovie()
        {
            Menu menu = new Menu();
            try
            {
                using (var db = new MovieContext())
                {
                    Console.WriteLine("Do you want to see all the movies (Y/N)");
                    var previousTitle = "";
                    var choice = Console.ReadLine();
                    if (choice.ToLower().Substring(0,1) == "y")
                    {
                        DisplayAll();
                    }

                    Console.WriteLine("Which movie do you want to edit?");
                    var titleOfMovie = Console.ReadLine();
                    var movies = db.Movies.Where(c => c.Title.ToLower().Contains(titleOfMovie));
                    if (movies == null)
                    {
                        Console.WriteLine("Sorry that title didn't match any movies");
                    }
                    else
                    {
                        var table = new ConsoleTable("Option","ID","Title");
                        table.Options.EnableCount = false;
                        var num = 1;
                        foreach (var x in movies)
                        {
                            table.AddRow(num,x.MovieId, x.Title);
                            num++;
                        }
                        table.Write();
                        Console.WriteLine($"Out of the {movies.Count()} Movies in the table which Movie do you want(Select Option)?");
                        var option = menu.ValueGetter();
                        DataModels.Movie selectedFilm = new DataModels.Movie();
                        if (movies.Count() < option)
                        {
                            Console.WriteLine("Sorry that is not an option");
                        }
                        else
                        {
                            selectedFilm = movies.ToList()[option - 1];
                            Console.WriteLine($"You have selected {movies.ToList()[option - 1].Title} to edit");
                            var movieGenres = db.MovieGenres.Where(x => x.Movie == selectedFilm);
                            previousTitle = movies.ToList()[option - 1].Title;
                            foreach (var x in movieGenres)
                            {
                                db.Remove(x);
                            }
                        }
                        Console.WriteLine("Do you want to edit title?");
                        choice = Console.ReadLine();
                        if (choice.ToLower().Substring(0,1) == "y")
                        {
                            Console.WriteLine("What is the title of the film?");
                            var title = Console.ReadLine();
                            Console.WriteLine("What year was the movie made? ex.(1979)");
                            int year = menu.ValueGetter();
                            DateTime yearFormatted = DateTime.Parse($"01-01-{year}");
                            title = title + " (" + year + ")";
                            if (db.Movies.Any(c=> c.Title.ToLower() == title.ToLower()))
                            {
                                Console.WriteLine("You can't add that, that film already exists sorry");
                                Console.WriteLine("Enter a new title");
                                title = Console.ReadLine();
                                Console.WriteLine("What year was the movie made? ex.(1979)");
                                year = menu.ValueGetter();
                                yearFormatted = DateTime.Parse($"01-01-{year}");
                                title = title + " (" + year + ")";
                            }
                            else
                            {
                                selectedFilm.Title = title;
                                selectedFilm.ReleaseDate = yearFormatted;
                                db.Movies.Update(selectedFilm);
                                db.SaveChanges();
                                var tableGenresUpdated = new ConsoleTable("ID", "Genre");
                                tableGenresUpdated.Options.EnableCount = false;
                                foreach (var x in db.Genres)
                                {
                                    tableGenresUpdated.AddRow(x.GenreId, x.Name);
                                }
                                tableGenresUpdated.Write();
                                Console.WriteLine("How many genres in the list do you want to add to updated field?(All previous genres have been cleared)");
                                var genreAmount = menu.ValueGetter();
                                for (int i = 0; i < genreAmount; i++)
                                {
                                    Console.WriteLine("Which genres are in the film?");
                                    string genrePicked = Console.ReadLine();
                                    while (!db.Genres.Any(c=> c.Name.ToLower() == genrePicked.ToLower()))
                                    {
                                        Console.WriteLine("Sorry that is not a genre in the List Select a new one");
                                        genrePicked = Console.ReadLine();
                                    }
                                    var tempGenre = db.Genres.FirstOrDefault(c => c.Name.ToLower() == genrePicked.ToLower());
                                    MovieGenre tempMG = new MovieGenre();
                                    tempMG.Genre = tempGenre;
                                    tempMG.Movie = selectedFilm;
                                    db.MovieGenres.Add(tempMG);
                                    db.SaveChanges();
                                }
                                Console.WriteLine($"You have now changed the film to {selectedFilm.Title} from {previousTitle}");


                            }
                            
                        }

                    }
                    
                        
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't access Movie Database");
                throw;
            }
            
        }

        public void DisplayAll()
        {
            List<DataModels.Movie> movie = new List<DataModels.Movie>();
            List<MovieGenre> movieGenres = new List<MovieGenre>();
            List<Genre> genres = new List<Genre>();
            try
            {
                using (var db = new MovieContext())
                {
                    movie = db.Movies.ToList();
                    movieGenres = db.MovieGenres.ToList();
                    genres = db.Genres.ToList();
                }
                List<string> genresTogether = new List<string>();
                var movieTable = new ConsoleTable("Id","Title","Year","Genres");
                movieTable.Options.EnableCount = false;
                List<string?> listOfGenres = new List<string?>();
                foreach (var x in movie)
                {
                    List<string?> genresAppened = new List<string?>();
                    var tempGenres = movieGenres.Where(c => c.Movie == x);
                    foreach (var y in tempGenres)
                    {
                        genresTogether.Add(y.Genre.Name);
                    }
                    movieTable.AddRow(x.MovieId, x.Title, x.ReleaseDate,string.Join("|",genresTogether)); 
                    genresTogether.Clear();
                }
                movieTable.Write();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to connect to Movie Database");
                throw;
            }
        }
        

        public void SearchMovie()
        {
            using (var db = new MovieContext())
            {
                Console.WriteLine("What is the title of the movie you want to look for?");
                var title = Console.ReadLine();
                var moviesFound = db.Movies.Where(c => c.Title.ToLower().Contains(title.ToLower()));
                var table = new ConsoleTable("ID", "Title", "Year Released","");
                table.Options.EnableCount = false;
                foreach (var x in moviesFound)
                {
                    table.AddRow(x.MovieId, x.Title, x.ReleaseDate, x.MovieGenres);
                }
                table.Write();
            }
            
        }

        public void AddGenre()
        {
            try
            {
                using (var db = new MovieContext())
                {
                    var genresList = db.Genres.ToList();
                    Console.WriteLine("What is the Genre you want to Add?");
                    string genre = Console.ReadLine();
                    if (genre != null && (genresList.Any(c => c.Name.ToLower() == genre.ToLower()) || genre.Length < 2))
                    {
                        Console.WriteLine($"Sorry your input of {genre} already matches a genre in DB or is not long enough");
                    }
                    else
                    {
                        Genre temp = new Genre();
                        temp.Name = genre;
                        db.Genres.Add(temp);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't load Genre Database");
            }
        }
        
    }
}