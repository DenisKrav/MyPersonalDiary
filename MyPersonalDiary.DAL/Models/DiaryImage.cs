using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.DAL.Models
{
    public class DiaryImage
    {
        [Key]
        public Guid Id { get; set; }

        public long UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        public string ContentType { get; set; }

        public long Size { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

}
