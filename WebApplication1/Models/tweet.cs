using System;

namespace WebApplication1.Models
{
    public class tweet
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MediaUrl { get; set; }
        public string Text { get; set; }
        public int Faces { get; set; }
        public int MaleFace { get; set; }
        public int FemailFace { get; set; }
        public string MatchingTrack { get; set; }
    }
}