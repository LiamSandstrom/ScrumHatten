using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface ICalendarRepository
    {
        void AddEvent(CalendarEvent calendarEvent);
        List<CalendarEvent> GetEvents(string userId, bool showAll);

        bool DeleteEvent(string id);
        void UpdateEvent(CalendarEvent calendarEvent);
    }
}
