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
            DBUserManager temp = new DBUserManager();
            temp.AddUser();
            temp.DisplayUsers();
        }
    }
}