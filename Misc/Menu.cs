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
            table.AddRow(1, "Add Movie").AddRow(2, "Update Movie")
                .AddRow(3, "Search Movie/Display All").AddRow(4,"Add User and Occupation").AddRow(5,"Add Rating to Movie").AddRow(6,"List Top Movie").AddRow(7,"Look for User Ratings").AddRow(8,"Exit");
            table.Options.EnableCount = false;
            table.Write();

        }
        
        //make a value getter to avoid exception handling
        public int IntValueGetter()
        {
            string option = Console.ReadLine();
            int number;
            bool success = Int32.TryParse(option, out number);

            while (!success)
            {
                Console.WriteLine("Only whole number values are accepted sorry!");
                option = Console.ReadLine();
                success = Int32.TryParse(option, out number);
            }

            return number;
        }
        
    }
}