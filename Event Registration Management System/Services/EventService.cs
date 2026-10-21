
using System;
using System.Collections.Generic;
using System.Linq;
using EventRegistrationManagementSystem.Models;

namespace EventRegistrationManagementSystem.Services
{
    public class EventService
    {
        private readonly List<Event> _events;
        private readonly List<Registration> _registrations;
        private readonly Action _save;

        public EventService(
            List<Event> events,
            List<Registration> registrations,
            Action save)
        {
            _events = events;
            _registrations = registrations;
            _save = save;
        }

        public List<Event> GetAll()
        {
            return _events.OrderBy(e => e.EventDate).ToList();
        }

        public Event? GetById(int id)
        {
            return _events.FirstOrDefault(e => e.EventId == id);
        }

        public List<Event> Search(string term)
        {
            return _events
                .Where(e =>
                    e.EventName.Contains(term,
                        StringComparison.OrdinalIgnoreCase) ||
                    e.Venue.Contains(term,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.EventName)
                .ToList();
        }

        public int ActiveCount(int eventId)
        {
            return _registrations.Count(r =>
                r.EventId == eventId &&
                r.Status != RegistrationStatus.Cancelled);
        }

        public int RemainingCapacity(Event ev)
        {
            return ev.Capacity - ActiveCount(ev.EventId);
        }

        public void Add(Event ev)
        {
            if (ev.EventId <= 0)
                throw new BusinessRuleException(
                    "Event ID must be positive.");

            if (_events.Any(e => e.EventId == ev.EventId))
                throw new BusinessRuleException(
                    "Event ID already exists.");

            if (string.IsNullOrWhiteSpace(ev.EventName))
                throw new BusinessRuleException(
                    "Event name is required.");

            if (string.IsNullOrWhiteSpace(ev.Venue))
                throw new BusinessRuleException(
                    "Venue is required.");

            if (ev.EventDate <= DateTime.Now)
                throw new BusinessRuleException(
                    "Event date must be in the future.");

            if (ev.Capacity <= 0)
                throw new BusinessRuleException(
                    "Capacity must be positive.");

            if (ev.RegistrationFee < 0)
                throw new BusinessRuleException(
                    "Registration fee cannot be negative.");

            _events.Add(ev);
            _save();
        }

        public void Update(
            int id,
            string venue,
            DateTime date,
            int capacity,
            decimal fee)
        {
            Event? ev = GetById(id);

            if (ev == null)
                throw new BusinessRuleException(
                    "Event not found.");

            if (string.IsNullOrWhiteSpace(venue))
                throw new BusinessRuleException(
                    "Venue is required.");

            if (date <= DateTime.Now)
                throw new BusinessRuleException(
                    "Event date must be in the future.");

            if (capacity <= 0)
                throw new BusinessRuleException(
                    "Capacity must be positive.");

            int active = ActiveCount(id);

            if (capacity < active)
                throw new BusinessRuleException(
                    $"Capacity cannot be less than {active} active registrations.");

            if (fee < 0)
                throw new BusinessRuleException(
                    "Fee cannot be negative.");

            ev.Venue = venue.Trim();
            ev.EventDate = date;
            ev.Capacity = capacity;
            ev.RegistrationFee = fee;

            _save();
        }

        public void Delete(int id)
        {
            Event? ev = GetById(id);

            if (ev == null)
                throw new BusinessRuleException(
                    "Event not found.");

            if (_registrations.Any(r => r.EventId == id))
                throw new BusinessRuleException(
                    "Cannot delete an event with registration history.");

            _events.Remove(ev);
            _save();
        }
    }
}
