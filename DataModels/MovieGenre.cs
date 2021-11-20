using System.ComponentModel.DataAnnotations.Schema;

namespace NetFinal.DataModels
{
    public class MovieGenre
    {
    public int Id {get;set;}
    [ForeignKey("MovieId")]
    public virtual Movie Movie { get; set; }
    [ForeignKey("GenreId")]
    public virtual Genre Genre { get; set; }
    }
}
