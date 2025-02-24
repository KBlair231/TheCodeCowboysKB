using System.ComponentModel.DataAnnotations;
using System.Drawing;   // Required for bitmaps

namespace PromptQuest.Models {
	public class Player {
		[Required] // Data annotation to specify that the Name property is Required
		[RegularExpression(@"^[a-zA-Z\s]+$",ErrorMessage = "Name must not contain numbers or special characters.")]
		public string Name { get; set; }
		public int HealthPotions { get; set; }
		public int MaxHealth { get; set; }
		public int CurrentHealth { get; set; }
		public int Defense { get; set; } = 1;
		public int Attack { get; set; }

	}
}