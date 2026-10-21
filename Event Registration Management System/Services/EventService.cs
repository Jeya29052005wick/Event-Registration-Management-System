using Event_Registration_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Event_Registration_Management_System.Services
{
    public class EventService
    {
        private List<Event> events;

        public EventService(List<Event> events)
        {
            this.events = events;
        }

        public void DisplayAllEvent()
        {
            var result = from item in events
                         select item;

            foreach (var item in result)
            {
                Console.WriteLine(item.GetDetails());
            }

            if (!events.Any())
            {
                Console.WriteLine("No events available.");
            }
        }

        public void SearchEvent()
        {
            Console.Write("Enter Event Name: ");
            string? searchName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchName))
            {
                Console.WriteLine("Give a valid event name.");
                return;
            }

            var result = events.Where(e => e.EventName.Contains(
                    searchName,
                    StringComparison.OrdinalIgnoreCase));

            if (!result.Any())
            {
                Console.WriteLine("Event not found.");
                return;
            }

            foreach (var item in result)
            {
                Console.WriteLine(item.GetDetails());
            }
        }

        public void UpdateEvent()
        {
            Console.Write("Enter Event ID to update: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Event ID.");
                return;
            }

            Event? item = events.FirstOrDefault(e => e.EventId == id);

            if (item == null)
            {
                Console.WriteLine("Event not found.");
                return;
            }

            Console.Write("Enter new Event Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter new Venue: ");
            string venue = Console.ReadLine() ?? "";

            Console.Write("Enter new Event Date: ");
            string date = Console.ReadLine() ?? "";


            if (string.IsNullOrWhiteSpace(name) ||string.IsNullOrWhiteSpace(venue) ||string.IsNullOrWhiteSpace(date))
            {
                Console.WriteLine("Fields cannot be empty.");
                return;
            }

            item.EventName = name;
            item.Venue = venue;
            item.EventDate = date;

            Console.WriteLine("Event updated successfully.");
        }

        public void DeleteEvent()
        {
            Console.Write("Enter Event ID to delete: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Event ID.");
                return;
            }

            Event? item = events.FirstOrDefault(e => e.EventId == id);

            if (item == null)
            {
                Console.WriteLine("Event not found.");
                return;
            }

            events.Remove(item);

            Console.WriteLine("Event deleted successfully.");
        }

    }
}