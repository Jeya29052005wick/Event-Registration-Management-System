
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventRegistrationManagementSystem.Models;
using EventRegistrationManagementSystem.Services;
using EventRegistrationManagementSystem.Utilities;

namespace EventRegistrationManagementSystem
{
    internal class Program
    {
        private static readonly JsonFileHelper json = new();

        private static List<Event> events = new();
        private static List<Participant> participants = new();
        private static List<Registration> registrations = new();

        private static EventService eventService = null!;
        private static ParticipantService participantService = null!;
        private static RegistrationService registrationService = null!;
        private static ReportService reportService = null!;

        static void Main()
        {
            Console.Title = "Event Registration Management System";

            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to load data: {ex.Message}");
                Console.WriteLine("Fix the saved files before restarting.");
                return;
            }

            Action save = SaveData;

            eventService = new EventService(events, registrations, save);

            participantService = new ParticipantService(
                participants, registrations, save);

            registrationService = new RegistrationService(
                registrations, events, participants, save);

            reportService = new ReportService(
                events, participants, registrations);

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n======================================");
                Console.WriteLine(" EVENT REGISTRATION MANAGEMENT SYSTEM");
                Console.WriteLine("======================================");
                Console.WriteLine("1. Manage Events");
                Console.WriteLine("2. Manage Participants");
                Console.WriteLine("3. Manage Registrations");
                Console.WriteLine("4. Search / View Records");
                Console.WriteLine("5. Reports");
                Console.WriteLine("6. Save Data");
                Console.WriteLine("0. Exit");

                int choice = InputHelper.ReadInt("Enter choice: ");

                try
                {
                    switch (choice)
                    {
                        case 1:
                            EventMenu();
                            break;

                        case 2:
                            ParticipantMenu();
                            break;

                        case 3:
                            RegistrationMenu();
                            break;

                        case 4:
                            SearchMenu();
                            break;

                        case 5:
                            ReportMenu();
                            break;

                        case 6:
                            SaveData();
                            break;

                        case 0:
                            SaveData();
                            running = false;
                            Console.WriteLine("Application closed.");
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (BusinessRuleException ex)
                {
                    Console.WriteLine($"Validation error: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"File error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Operation failed: {ex.Message}");
                }
            }
        }


        static void LoadData()
        {
            events = json.Load<Event>("events.json");
            participants = json.Load<Participant>("participants.json");
            registrations = json.Load<Registration>("registrations.json");

            bool duplicateIds =
                events.GroupBy(e => e.EventId).Any(g => g.Count() > 1) ||
                participants.GroupBy(p => p.ParticipantId).Any(g => g.Count() > 1) ||
                registrations.GroupBy(r => r.RegistrationId).Any(g => g.Count() > 1);

            if (duplicateIds)
                throw new InvalidDataException("Duplicate IDs found in saved data.");

            bool invalidReferences = registrations.Any(r =>
                !events.Any(e => e.EventId == r.EventId) ||
                !participants.Any(p => p.ParticipantId == r.ParticipantId));

            if (invalidReferences)
                throw new InvalidDataException(
                    "A registration references a missing event or participant.");

            if (events.Any(e =>
                    e.EventId <= 0 ||
                    string.IsNullOrWhiteSpace(e.EventName) ||
                    string.IsNullOrWhiteSpace(e.Venue) ||
                    e.Capacity <= 0 ||
                    e.RegistrationFee < 0) ||
                participants.Any(p =>
                    p.ParticipantId <= 0 ||
                    string.IsNullOrWhiteSpace(p.FullName) ||
                    string.IsNullOrWhiteSpace(p.Email) ||
                    string.IsNullOrWhiteSpace(p.PhoneNumber)) ||
                registrations.Any(r =>
                    r.RegistrationId <= 0 ||
                    r.AmountPaid < 0 ||
                    !Enum.IsDefined(typeof(RegistrationStatus), r.Status)))
            {
                throw new InvalidDataException(
                    "Saved records contain invalid field values.");
            }

            if (participants.GroupBy(
                p => p.Email, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1))
            {
                throw new InvalidDataException(
                    "Duplicate participant email addresses found.");
            }

            if (registrations.Any(r =>
                r.Status != RegistrationStatus.Cancelled &&
                registrations.Any(other =>
                    other.RegistrationId != r.RegistrationId &&
                    other.EventId == r.EventId &&
                    other.ParticipantId == r.ParticipantId &&
                    other.Status != RegistrationStatus.Cancelled)))
            {
                throw new InvalidDataException(
                    "Duplicate active registrations found.");
            }

            foreach (Event ev in events)
            {
                int active = registrations.Count(r =>
                    r.EventId == ev.EventId &&
                    r.Status != RegistrationStatus.Cancelled);

                if (active > ev.Capacity)
                    throw new InvalidDataException(
                        $"Event {ev.EventId} exceeds its capacity.");
            }

            Console.WriteLine(
                $"Loaded {events.Count} events, " +
                $"{participants.Count} participants and " +
                $"{registrations.Count} registrations.");
        }

        static void SaveData()
        {
            json.Save("events.json", events);
            json.Save("participants.json", participants);
            json.Save("registrations.json", registrations);

            Console.WriteLine("Data saved successfully.");
        }


        static void EventMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- EVENT MANAGEMENT ---");
                Console.WriteLine("1. Create Event");
                Console.WriteLine("2. Display All Events");
                Console.WriteLine("3. Find Event by ID");
                Console.WriteLine("4. Search by Name / Venue");
                Console.WriteLine("5. Update Event");
                Console.WriteLine("6. Delete Event");
                Console.WriteLine("0. Back");

                try
                {
                    switch (InputHelper.ReadInt("Choice: "))
                    {
                        case 1:
                            Event ev = new Event(
                                InputHelper.ReadInt("Event ID: "),
                                InputHelper.ReadRequired("Event name: "),
                                InputHelper.ReadRequired("Venue: "),
                                InputHelper.ReadDate("Event date (dd-MM-yyyy): "),
                                InputHelper.ReadInt("Capacity: "),
                                InputHelper.ReadDecimal("Registration fee: "));

                            eventService.Add(ev);
                            Console.WriteLine("Event created.");
                            break;

                        case 2:
                            DisplayEvents(eventService.GetAll());
                            break;

                        case 3:
                            ShowEvent(InputHelper.ReadInt("Event ID: "));
                            break;

                        case 4:
                            DisplayEvents(eventService.Search(
                                InputHelper.ReadRequired("Search term: ")));
                            break;

                        case 5:
                            int eventId = InputHelper.ReadInt("Event ID: ");

                            eventService.Update(
                                eventId,
                                InputHelper.ReadRequired("New venue: "),
                                InputHelper.ReadDate("New date (dd-MM-yyyy): "),
                                InputHelper.ReadInt("New capacity: "),
                                InputHelper.ReadDecimal("New fee: "));

                            Console.WriteLine("Event updated.");
                            break;

                        case 6:
                            eventService.Delete(
                                InputHelper.ReadInt("Event ID to delete: "));
                            Console.WriteLine("Event deleted.");
                            break;

                        case 0:
                            back = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (BusinessRuleException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Save failed: {ex.Message}");
                }
            }
        }

        static void DisplayEvents(IEnumerable<Event> list)
        {
            List<Event> result = list.ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("No events found.");
                return;
            }

            foreach (Event ev in result)
            {
                Console.WriteLine(ev.GetDetails());
                Console.WriteLine(
                    $"Active registrations: {eventService.ActiveCount(ev.EventId)} | " +
                    $"Remaining capacity: {eventService.RemainingCapacity(ev)}");
            }
        }

        static void ShowEvent(int id)
        {
            Event? ev = eventService.GetById(id);

            if (ev == null)
            {
                Console.WriteLine("Event not found.");
                return;
            }

            DisplayEvents(new List<Event> { ev });
        }


        static void ParticipantMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- PARTICIPANT MANAGEMENT ---");
                Console.WriteLine("1. Register Participant");
                Console.WriteLine("2. Display All Participants");
                Console.WriteLine("3. Find Participant by ID");
                Console.WriteLine("4. Search by Name / Email");
                Console.WriteLine("5. Update Participant");
                Console.WriteLine("6. Delete Participant");
                Console.WriteLine("0. Back");

                try
                {
                    switch (InputHelper.ReadInt("Choice: "))
                    {
                        case 1:
                            Participant p = new Participant(
                                InputHelper.ReadInt("Participant ID: "),
                                InputHelper.ReadRequired("Full name: "),
                                InputHelper.ReadEmail("Email: "),
                                InputHelper.ReadPhone("Phone: "));

                            participantService.Add(p);
                            Console.WriteLine("Participant registered.");
                            break;

                        case 2:
                            DisplayParticipants(participantService.GetAll());
                            break;

                        case 3:
                            ShowParticipant(
                                InputHelper.ReadInt("Participant ID: "));
                            break;

                        case 4:
                            DisplayParticipants(participantService.Search(
                                InputHelper.ReadRequired("Search term: ")));
                            break;

                        case 5:
                            int participantId =
                                InputHelper.ReadInt("Participant ID: ");

                            participantService.Update(
                                participantId,
                                InputHelper.ReadRequired("New full name: "),
                                InputHelper.ReadEmail("New email: "),
                                InputHelper.ReadPhone("New phone: "));

                            Console.WriteLine("Participant updated.");
                            break;

                        case 6:
                            participantService.Delete(
                                InputHelper.ReadInt("Participant ID to delete: "));
                            Console.WriteLine("Participant deleted.");
                            break;

                        case 0:
                            back = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (BusinessRuleException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Save failed: {ex.Message}");
                }
            }
        }

        static void DisplayParticipants(IEnumerable<Participant> list)
        {
            List<Participant> result = list.ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("No participants found.");
                return;
            }

            foreach (Participant p in result)
                Console.WriteLine(p.GetDetails());
        }

        static void ShowParticipant(int id)
        {
            Participant? p = participantService.GetById(id);

            if (p == null)
                Console.WriteLine("Participant not found.");
            else
                Console.WriteLine(p.GetDetails());
        }


        static void RegistrationMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- REGISTRATION MANAGEMENT ---");
                Console.WriteLine("1. Register for Event");
                Console.WriteLine("2. Display All Registrations");
                Console.WriteLine("3. Find Registration by ID");
                Console.WriteLine("4. Cancel Registration");
                Console.WriteLine("5. Mark Attendance");
                Console.WriteLine("0. Back");

                try
                {
                    switch (InputHelper.ReadInt("Choice: "))
                    {
                        case 1:
                            registrationService.Register(
                                InputHelper.ReadInt("Registration ID: "),
                                InputHelper.ReadInt("Event ID: "),
                                InputHelper.ReadInt("Participant ID: "));

                            Console.WriteLine("Registration completed.");
                            break;

                        case 2:
                            DisplayRegistrations(registrationService.GetAll());
                            break;

                        case 3:
                            ShowRegistration(
                                InputHelper.ReadInt("Registration ID: "));
                            break;

                        case 4:
                            registrationService.Cancel(
                                InputHelper.ReadInt("Registration ID: "));

                            Console.WriteLine(
                                "Registration cancelled; capacity released.");
                            break;

                        case 5:
                            registrationService.MarkAttended(
                                InputHelper.ReadInt("Registration ID: "));

                            Console.WriteLine("Attendance marked.");
                            break;

                        case 0:
                            back = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (BusinessRuleException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Save failed: {ex.Message}");
                }
            }
        }

        static void DisplayRegistrations(IEnumerable<Registration> list)
        {
            List<Registration> result = list.ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("No registrations found.");
                return;
            }

            foreach (Registration r in result)
                ShowRegistration(r.RegistrationId);
        }

        static void ShowRegistration(int id)
        {
            Registration? r = registrationService.GetById(id);

            if (r == null)
            {
                Console.WriteLine("Registration not found.");
                return;
            }

            Event? ev = eventService.GetById(r.EventId);
            Participant? p = participantService.GetById(r.ParticipantId);

            Console.WriteLine(r.GetDetails());
            Console.WriteLine($"Event: {ev?.EventName ?? "Missing event"}");
            Console.WriteLine($"Participant: {p?.FullName ?? "Missing participant"}");
            Console.WriteLine($"Email: {p?.Email ?? "N/A"}");
        }


        static void SearchMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- SEARCH / VIEW ---");
                Console.WriteLine("1. Participants for an Event");
                Console.WriteLine("2. Events for a Participant");
                Console.WriteLine("3. Remaining Capacity");
                Console.WriteLine("4. Full Events");
                Console.WriteLine("0. Back");

                switch (InputHelper.ReadInt("Choice: "))
                {
                    case 1:
                        ListParticipantsForEvent(
                            InputHelper.ReadInt("Event ID: "));
                        break;

                    case 2:
                        reportService.ParticipantEvents(
                            InputHelper.ReadInt("Participant ID: "));
                        break;

                    case 3:
                        reportService.CapacityReport();
                        break;

                    case 4:
                        DisplayEvents(events.Where(e =>
                            eventService.ActiveCount(e.EventId) >= e.Capacity));
                        break;

                    case 0:
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void ListParticipantsForEvent(int eventId)
        {
            Event? ev = eventService.GetById(eventId);

            if (ev == null)
            {
                Console.WriteLine("Event not found.");
                return;
            }

            var result = registrationService.GetByEvent(eventId)
                .Where(r => r.Status != RegistrationStatus.Cancelled)
                .Join(
                    participants,
                    r => r.ParticipantId,
                    p => p.ParticipantId,
                    (r, p) => new
                    {
                        p.ParticipantId,
                        p.FullName,
                        p.Email,
                        p.PhoneNumber,
                        r.Status,
                        r.AmountPaid
                    })
                .OrderBy(p => p.FullName)
                .ToList();

            Console.WriteLine($"\nParticipants for {ev.EventName}");

            if (result.Count == 0)
            {
                Console.WriteLine("No active registrations.");
                return;
            }

            foreach (var p in result)
            {
                Console.WriteLine(
                    $"{p.ParticipantId} | {p.FullName} | {p.Email} | " +
                    $"{p.PhoneNumber} | {p.Status} | ₹{p.AmountPaid:F2}");
            }
        }


        static void ReportMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- REPORTS ---");
                Console.WriteLine("1. Participants per Event");
                Console.WriteLine("2. Capacity and Full Events");
                Console.WriteLine("3. Registrations Grouped by Event");
                Console.WriteLine("4. Payments by Event and Overall");
                Console.WriteLine("5. Attendance Count and Percentage");
                Console.WriteLine("6. Event Fee Summary");
                Console.WriteLine("0. Back");

                switch (InputHelper.ReadInt("Choice: "))
                {
                    case 1:
                        reportService.ParticipantsPerEvent();
                        break;

                    case 2:
                        reportService.CapacityReport();
                        break;

                    case 3:
                        reportService.RegistrationGroups();
                        break;

                    case 4:
                        reportService.PaymentReport();
                        break;

                    case 5:
                        reportService.AttendanceReport();
                        break;

                    case 6:
                        reportService.FeeSummary();
                        break;

                    case 0:
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
