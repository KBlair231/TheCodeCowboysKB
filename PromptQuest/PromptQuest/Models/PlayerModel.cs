using System.ComponentModel.DataAnnotations;
using System.Drawing;   // Required for bitmaps

namespace PromptQuest.Models
{
    public class PlayerModel
    {
        [Required] // Data annotation to specify that the Name property is Required
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must be a single word with no spaces or special characters.")]
        public string Name { get; set; }
    }
}