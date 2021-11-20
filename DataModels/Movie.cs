using System;
using System.Collections.Generic;

namespace NetFinal.DataModels
{
    public class Movie
    {
        public long MovieId { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }


        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<UserMovie> UserMovies { get; set; }
    }
}