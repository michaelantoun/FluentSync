using System;
using System.Diagnostics.CodeAnalysis;

namespace FluentSync.Tests.Models
{
    internal class Event : IComparable<Event>
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int CompareTo([AllowNull] Event other)
        {
            return ValueTypeExtensions.CompareTo(Id, other?.Id);
        }
    }
}
