
using System;
using System.Collections.Generic;
using System.Linq;
using EventRegistrationManagementSystem.Models;

namespace EventRegistrationManagementSystem.Services
{
    public class ReportService
    {
        private readonly List<Event> _events;
        private readonly List<Participant> _participants;
        private readonly List<Registration> _registrations;

        public ReportService(
            List<Event> events,
            List<Participant> participants,
            List<Registration> registrations)
        {
            _events = events;
            _participants = participants;
            _registrations = registrations;
        }

        public void ParticipantsPerEvent()
        {
            Console.WriteLine("\n--- PARTICIPANTS PER EVENT ---");

            foreach (Event ev in _events.OrderBy(e => e.EventName))
            {
                var result = from r in _registrations
                             join p in _participants
                                 on r.ParticipantId equals p.ParticipantId
                             where r.EventId == ev.EventId &&
                                   r.Status != RegistrationStatus.Cancelled
                             orderby p.FullName
                             select new
                             {
                                 p.ParticipantId,
                                 p.FullName,
                                 p.Email,
                                 r.Status,
                                 r.AmountPaid
                             };

                var list = result.ToList();

                Console.WriteLine($"\nEvent: {ev.EventName} (ID: {ev.EventId})");

                if (list.Count == 0)
                {
                    Console.WriteLine("No active registrations.");
                    continue;
                }

                foreach (var p in list)
                {
                    Console.WriteLine(
                        $"{p.ParticipantId} | {p.FullName} | {p.Email} | " +
                        $"{p.Status} | Paid: ₹{p.AmountPaid:F2}");
                }

                Console.WriteLine($"Active registrations: {list.Count}");
            }
        }

        public void CapacityReport()
        {
            Console.WriteLine("\n--- EVENT CAPACITY REPORT ---");

            var result = _events
                .Select(e => new
                {
                    e.EventId,
                    e.EventName,
                    e.Capacity,
                    Active = _registrations.Count(r =>
                        r.EventId == e.EventId &&
                        r.Status != RegistrationStatus.Cancelled)
                })
                .Select(x => new
                {
                    x.EventId,
                    x.EventName,
                    x.Capacity,
                    x.Active,
                    Remaining = x.Capacity - x.Active,
                    IsFull = x.Active >= x.Capacity
                })
                .OrderBy(x => x.EventName)
                .ToList();

            if (result.Count == 0)
                Console.WriteLine("No events available.");

            foreach (var item in result)
            {
                Console.WriteLine(
                    $"{item.EventId} | {item.EventName} | " +
                    $"Capacity: {item.Capacity} | Active: {item.Active} | " +
                    $"Remaining: {item.Remaining} | Full: {item.IsFull}");
            }

            Console.WriteLine("\n--- FULL EVENTS ---");

            var fullEvents = result.Where(e => e.IsFull).ToList();

            if (fullEvents.Count == 0)
                Console.WriteLine("No full events.");

            foreach (var ev in fullEvents)
                Console.WriteLine($"{ev.EventId} | {ev.EventName}");
        }

        public void RegistrationGroups()
        {
            Console.WriteLine("\n--- REGISTRATIONS GROUPED BY EVENT ---");

            var groups = _registrations
                .Where(r => r.Status != RegistrationStatus.Cancelled)
                .GroupBy(r => r.EventId)
                .OrderBy(g => g.Key)
                .ToList();

            if (groups.Count == 0)
            {
                Console.WriteLine("No active registrations.");
                return;
            }

            foreach (var group in groups)
            {
                Event? ev = _events.FirstOrDefault(
                    e => e.EventId == group.Key);

                Console.WriteLine(
                    $"{ev?.EventName ?? "Unknown Event"} | " +
                    $"Active registrations: {group.Count()}");
            }
        }

        public void PaymentReport()
        {
            Console.WriteLine("\n--- PAYMENTS COLLECTED BY EVENT ---");

            foreach (Event ev in _events.OrderBy(e => e.EventName))
            {
                // Cancelled registrations remain in payment totals.
                // Refunds are not implemented in this application.
                decimal total = _registrations
                    .Where(r => r.EventId == ev.EventId)
                    .Sum(r => r.AmountPaid);

                Console.WriteLine($"{ev.EventName}: ₹{total:F2}");
            }

            decimal overall = _registrations.Sum(r => r.AmountPaid);

            Console.WriteLine("----------------------------------");
            Console.WriteLine($"OVERALL PAYMENTS: ₹{overall:F2}");
        }

        public void AttendanceReport()
        {
            Console.WriteLine("\n--- ATTENDANCE REPORT ---");

            foreach (Event ev in _events.OrderBy(e => e.EventName))
            {
                int eligible = _registrations.Count(r =>
                    r.EventId == ev.EventId &&
                    r.Status != RegistrationStatus.Cancelled);

                int attended = _registrations.Count(r =>
                    r.EventId == ev.EventId &&
                    r.Status == RegistrationStatus.Attended);

                double percentage = eligible == 0
                    ? 0
                    : attended * 100.0 / eligible;

                Console.WriteLine(
                    $"{ev.EventName} | Registered: {eligible} | " +
                    $"Attended: {attended} | Attendance: {percentage:F2}%");
            }
        }

        public void ParticipantEvents(int participantId)
        {
            Participant? participant = _participants.FirstOrDefault(
                p => p.ParticipantId == participantId);

            if (participant == null)
            {
                Console.WriteLine("Participant not found.");
                return;
            }

            var result = from r in _registrations
                         join e in _events
                             on r.EventId equals e.EventId
                         where r.ParticipantId == participantId &&
                               r.Status != RegistrationStatus.Cancelled
                         orderby e.EventDate
                         select new
                         {
                             e.EventId,
                             e.EventName,
                             e.Venue,
                             e.EventDate,
                             r.Status,
                             r.AmountPaid
                         };

            var list = result.ToList();

            Console.WriteLine($"\nEvents for {participant.FullName}");

            if (list.Count == 0)
            {
                Console.WriteLine("No active event registrations.");
                return;
            }

            foreach (var item in list)
            {
                Console.WriteLine(
                    $"{item.EventId} | {item.EventName} | {item.Venue} | " +
                    $"{item.EventDate:dd-MM-yyyy} | {item.Status} | " +
                    $"Paid: ₹{item.AmountPaid:F2}");
            }
        }

        public void FeeSummary()
        {
            Console.WriteLine("\n--- EVENT FEE SUMMARY ---");

            if (_events.Count == 0)
            {
                Console.WriteLine("No events available.");
                return;
            }

            var fees = _events.Select(e => e.RegistrationFee).ToList();

            Console.WriteLine($"Average fee: ₹{fees.Average():F2}");
            Console.WriteLine($"Minimum fee: ₹{fees.Min():F2}");
            Console.WriteLine($"Maximum fee: ₹{fees.Max():F2}");
        }
    }
}
