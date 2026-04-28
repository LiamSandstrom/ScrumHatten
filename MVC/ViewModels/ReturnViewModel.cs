using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class ReturnViewModel
    {
        [Required]
        public string OrderId { get; set; } = null!;
        [Required, MinLength(1, ErrorMessage = "Välj minst en produkt att returnera/reklamera")]
        public List<string> HatIds { get; set; } = new();
        [Required, MinLength(0, ErrorMessage = "Beskrivning måste vara minst 10 tecken lång"), MaxLength(255, ErrorMessage = "Beskrivning kan inte vara längre än 255 tecken")]
        public string Description { get; set; } = null!;
        [Required]
        public string CustomerId { get; set; } = null!;
    }
}
