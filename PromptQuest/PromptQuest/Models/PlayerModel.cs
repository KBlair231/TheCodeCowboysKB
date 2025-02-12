using System.ComponentModel.DataAnnotations;
using System.Drawing;   // Required for bitmaps

namespace PromptQuest.Models
{
    public class PlayerModel
    {
        [Required] // Data annotation to specify that the Name property is Required
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must not contain numbers or special characters.")]
        public string Name { get; set; }
    }
}