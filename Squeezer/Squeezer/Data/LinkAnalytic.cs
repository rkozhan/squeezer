using System.ComponentModel.DataAnnotations;
namespace Squeezer.Data

{
    public class LinkAnalytic
    {
        [Key]
        public long Id { get; set; }
        public long LinkId { get; set; }
        public DateTime ClicedAt { get; set; }

        public virtual Link OriginalLink { get; set; }

    }
}

