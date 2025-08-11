using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.ViewModels.Note.Request
{
    public class DeleteNoteRequestViewModel
    {
        [Required(ErrorMessage = "Note type is required.")]
        [RegularExpression("^(Text|Image)$", ErrorMessage = "Note type must be either 'Text' or 'Image'.")]
        public string NoteType { get; set; }

        [Required(ErrorMessage = "Note ID is required.")]
        [NotEqualToEmptyGuid(ErrorMessage = "Note ID cannot be empty.")]
        public Guid NoteId { get; set; }

        /// <summary>
        /// Custom validation attribute to ensure GUID is not empty.
        /// </summary>
        public class NotEqualToEmptyGuidAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value is Guid guidValue)
                {
                    return guidValue != Guid.Empty;
                }
                return false;
            }
        }
    }
}

