using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLibraryApp.Models
{
    public class Movie
    {
        public string? MovieId { get; set; }
        public string? Title { get; set; }
        public string? Director { get; set; }
        public string? Genre { get; set; }
        public int? ReleaseYear { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
