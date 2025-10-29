using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class BookModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(256)]
    [Display(Name = "Title", Description = "Title of the book")]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(256)]
    [Display(Name = "Author", Description = "Author of the book")]
    public string Author { get; set; }
    
    [Required]
    [Display(Name = "Year", Description = "Publication year of the book")]
    public int Year { get; set; }
}