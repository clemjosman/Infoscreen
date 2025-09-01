using System;
using System.Collections.Generic;
using System.Text;

namespace Infoscreens.Common.Models.CachedData
{
    public class IdeaCached
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public string OriginalLanguageCode { get; set; }
        public bool MyIdea { get; set; }
        public float AverageRating { get; set; }
        public int RatingCount { get; set; }
        public string Created { get; set; }
        public IdeaBusinessUnit BusinessUnit { get; set; }
        public IdeaCategory Category { get; set; }
        public IdeaStatus CurrentStatus { get; set; }
    }

    public class IdeaCategory
    {
        public int Id { get; set; }
        public string TextCode { get; set; }
        public string Color { get; set; }
    }

    public class IdeaBusinessUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string SourceId { get; set; }
        public string SourceSystemTextCode { get; set; }
        //public object Manager { get; set; } // Always null and might not be needed yet
        public IEnumerable<IdeaCategory> Categories { get; set; }
    }

    public class IdeaStatus
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public string TextCode { get; set; }
        public string Color { get; set; }
    }
}
