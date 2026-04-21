using DAL.Repositories.Interfaces;
using Models;
using MongoDB.Bson;
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
            (e.TargetType == "public") ||
            (e.TargetUserNames != null && e.TargetUserNames.Contains(userName))
        //(e.OwnerName == userName)
        )
        .ToList();
        }


        public bool DeleteEvent(string id)
        {
            // Vi skapar ett filter som MongoDB förstår oavsett om Id är sträng eller ObjectId
            var filter = Builders<CalendarEvent>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = _eventCollection.DeleteOne(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public void UpdateEvent(CalendarEvent calendarEvent)
        {
            _eventCollection.ReplaceOne(e => e.Id == calendarEvent.Id, calendarEvent);
        }
    }
}


