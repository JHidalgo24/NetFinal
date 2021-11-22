using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetFinal.DataModels
{
    public class Genre
    {
        public long GenreId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres {get;set;}
    }
}
