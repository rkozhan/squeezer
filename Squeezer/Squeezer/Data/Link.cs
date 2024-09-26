using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squeezer.Data
{
    public class Link
    {
        [Key]
        public long Id { get; set; }

        [Required, Length(10, 250)]
        public string LongUrl { get; set; }

        [Required, Length(6, 10)]
        public string ShortCode { get; set; }

        [Required]
        public string ShortUrl { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }

        [InverseProperty(nameof(LinkAnalytic.OriginalLink))]
        public virtual ICollection<LinkAnalytic> linkAnalytics { get; set; }
    }
}