using DAL.Repositories.Interfaces;
using Models;
using MongoDB.Driver;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly IMongoCollection<CalendarEvent> _eventCollection;

        public CalendarRepository(MongoConnector mc)
        {
            if (mc == null)
                throw new Exception("MongoConnector is NULL");

            if (mc._database == null)
                throw new Exception("Mongo database is NULL");

            _eventCollection = mc._database.GetCollection<CalendarEvent>("Calendar");
        }
        public void AddEvent(CalendarEvent calendarEvent)
        {
            _eventCollection.InsertOne(calendarEvent);
        }
        public List<CalendarEvent> GetEvents(string userName)
        {
            return _eventCollection
                .Find(e =>
                    (e.TargetType != null && e.TargetType == "public") ||
                    (e.TargetUserNames != null && e.TargetUserNames.Any(x=> x == userName)))
                .ToList();
        }
    }
}


