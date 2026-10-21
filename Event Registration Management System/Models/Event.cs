using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Event_Registration_Management_System.Models
{
    public class Event
    {
        public int EventId {  get; set; }
        public string EventName { get; set; }
        public string Venue { get; set; }
        public string EventDate { get; set; }
        public int Capacity { get; set; }
        public int RegistraionFee { get; set; }

        public Event(int eventId, string eventName, string venue,string  eventDate, int capacity, int registraionFee)
        {
            EventId = eventId;
            EventName = eventName;
            Venue = venue;
            EventDate = eventDate;
            Capacity = capacity;
            RegistraionFee = registraionFee;
        }

        public string GetDetails()
        {
            return $"ID : {EventId} - NAME : {EventName}- VENUE : {Venue}+" +
                $" Date : {EventDate:g} + Registration Fee : {RegistraionFee:c}";
        }

    }
}
