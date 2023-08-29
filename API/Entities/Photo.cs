using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int? id { get; set; }
        public string? url { get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }
        public AppUsers? AppUsers { get; set; }
        public int? AppUsersId { get; set; }
    }
}