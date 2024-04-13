using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.BookReading
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = String.Empty;

        [MaxLength(length: 100)]    
        public string Description { get; set; } = String.Empty;
        public string Language { get; set; } = String.Empty;

        [Required,
        MaxLength(length: 17)]
        public string ISBookNumber { get; set; } = String.Empty;

        [Required,
        DataType(DataType.Date),
        Display(Name = "Date Published")]
        public DateTime DatePublished { get; set; }

        [Required,
        DataType(DataType.Currency)]
        public int Price { get; set; }

        [Required]
        public string Author { get; set; } = String.Empty;

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = String.Empty;
    }
}
