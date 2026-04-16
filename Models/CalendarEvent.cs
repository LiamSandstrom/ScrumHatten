using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CalendarEvent
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Color { get; set; }   
    }
}
