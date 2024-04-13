using AuthAi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Models.BookReading
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>());
            if (context.Books.Any()) // Check if databese contains any books
            {
                return; // Database conteins books already
            }

            context.Books.AddRange(
                new Book
                {
                    Title = "Broderna lejonhjorta",
                    Language = "Swedish",
                    ISBookNumber = "9789129688313",
                    DatePublished = DateTime.Parse(s: "2013-9-26"),
                    Price = 139,
                    Author = "Astrid Lindgren",
                    ImageUrl = "/images/lojonhjorta.jpg"
                },

                new Book
                {
                    Title = "The Fellowship of the Ring",
                    Language = "English",
                    ISBookNumber = "9780261102354",
                    DatePublished = DateTime.Parse(s: "1991-7-4"),
                    Price = 100,
                    Author = "J. R. R. Tolkien",
                    ImageUrl = "/images/lotr.jpg"
                },

                new Book
                {
                    Title = "Mystic River",
                    Language = "English",
                    ISBookNumber = "9780062068408",
                    DatePublished = DateTime.Parse(s: "2011-6-1"),
                    Price = 91,
                    Author = "Dennis Lehane",
                    ImageUrl = "/images/mystic-river.jpg"
                },

                new Book
                {
                    Title = "Of Mice and Men",
                    Language = "",
                    ISBookNumber = "9780062068408",
                    DatePublished = DateTime.Parse(s: "1994-1-2"),
                    Price = 166,
                    Author = "John Steinbeck",
                    ImageUrl = "/images/of-mice-and-men.jpg"
                },

                new Book
                {
                    Title = "The Old Man and the Sea",
                    Language = "English",
                    ISBookNumber = "9780062068408",
                    DatePublished = DateTime.Parse(s: "1994-8-18"),
                    Price = 84,
                    Author = "Ernest Hemingway",
                    ImageUrl = "/images/old-man-and-the-sea.jpg"
                },

                new Book
                {
                    Title = "The Road",
                    Language = "English",
                    ISBookNumber = "9780307386458",
                    DatePublished = DateTime.Parse(s: "2007-5-1"),
                    Price = 95,
                    Author = "Cormac McCarthy",
                    ImageUrl = "/images/the-road.jpg"
                }
              );

            context.SaveChanges();
        }
    }
}
