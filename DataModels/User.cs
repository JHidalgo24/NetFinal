using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetFinal.DataModels
{
    public class User
    {
        public long Id { get; set; }
        public long Age { get; set; }
        public string Gender { get; set; }
        public string ZipCode { get; set; }
        
        [ForeignKey("OccupationID")]
        public virtual Occupation Occupation { get; set; }
        public virtual ICollection<UserMovie> UserMovies {get;set;}
    }
}
