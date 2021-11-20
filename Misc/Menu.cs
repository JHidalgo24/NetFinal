using ConsoleTables;
using System;
namespace NetFinal.Misc
{
    public class Menu
    {
        //display options
        public void DisplayOptions()
        {
            var table = new ConsoleTable("Choice","Option");
            table.AddRow(1, "Add Movie").AddRow(2, "Remove Movie").AddRow(3, "Update Movie")
                .AddRow(4, "Search Movie/Display All").AddRow(5,"Add User and Occupation").AddRow(6,"Add Rating to Movie").AddRow(7,"List Top Movie");
            table.Options.EnableCount = false;
            table.Write();
            
        }
        
        //make a value getter to avoid exception handling
        public int ValueGetter()
        {
            string option = Console.ReadLine();
            int number;
            bool success = Int32.TryParse(option, out number);

            while (!success)
            {
                Console.WriteLine("That isn't a number sorry!");
                option = Console.ReadLine();
                success = Int32.TryParse(option, out number);
            }

            return number;
        }
        //
    }
}