using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.DAL.Models
{
    public class DiaryRecord
    {
        [Key]
        public Guid Id { get; set; }

        public long UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        [MaxLength(500)]
        public string EncryptedContent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public ICollection<DiaryImage> Images { get; set; }
    }
}
