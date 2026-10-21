
using System;

namespace EventRegistrationManagementSystem.Models
{
    public class Event : IDisplayable
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int Capacity { get; set; }
        public decimal RegistrationFee { get; set; }

        public Event()
        {
        }

        public Event(int eventId, string eventName, string venue,
                     DateTime eventDate, int capacity, decimal registrationFee)
        {
            EventId = eventId;
            EventName = eventName;
            Venue = venue;
            EventDate = eventDate;
            Capacity = capacity;
            RegistrationFee = registrationFee;
        }

        public string GetDetails()
        {
            return $"Event ID: {EventId} | Name: {EventName} | " +
                   $"Venue: {Venue} | Date: {EventDate:dd-MM-yyyy} | " +
                   $"Capacity: {Capacity} | Fee: ₹{RegistrationFee:F2}";
        }
    }
}
