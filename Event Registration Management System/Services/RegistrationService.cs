
using System;
using System.Collections.Generic;
using System.Linq;
using EventRegistrationManagementSystem.Models;

namespace EventRegistrationManagementSystem.Services
{
    public class RegistrationService
    {
        private readonly List<Registration> _registrations;
        private readonly List<Event> _events;
        private readonly List<Participant> _participants;
        private readonly Action _save;

        public RegistrationService(
            List<Registration> registrations,
            List<Event> events,
            List<Participant> participants,
            Action save)
        {
            _registrations = registrations;
            _events = events;
            _participants = participants;
            _save = save;
        }

        public List<Registration> GetAll()
        {
            return _registrations
                .OrderBy(r => r.RegistrationId)
                .ToList();
        }

        public Registration? GetById(int id)
        {
            return _registrations.FirstOrDefault(
                r => r.RegistrationId == id);
        }

        public List<Registration> GetByEvent(int eventId)
        {
            return _registrations
                .Where(r => r.EventId == eventId)
                .ToList();
        }

        public List<Registration> GetByParticipant(int participantId)
        {
            return _registrations
                .Where(r => r.ParticipantId == participantId)
                .ToList();
        }

        public void Register(
            int registrationId,
            int eventId,
            int participantId)
        {
            if (registrationId <= 0)
                throw new BusinessRuleException(
                    "Registration ID must be positive.");

            if (_registrations.Any(r =>
                r.RegistrationId == registrationId))
            {
                throw new BusinessRuleException(
                    "Registration ID already exists.");
            }

            Event? ev = _events.FirstOrDefault(
                e => e.EventId == eventId);

            if (ev == null)
                throw new BusinessRuleException(
                    "Event not found.");

            Participant? participant = _participants.FirstOrDefault(
                p => p.ParticipantId == participantId);

            if (participant == null)
                throw new BusinessRuleException(
                    "Participant not found.");

            // Registration closes at the start of the event date.
            if (ev.EventDate.Date <= DateTime.Today)
                throw new BusinessRuleException(
                    "Registration is closed on the event date and afterward.");

            bool duplicate = _registrations.Any(r =>
                r.EventId == eventId &&
                r.ParticipantId == participantId &&
                r.Status != RegistrationStatus.Cancelled);

            if (duplicate)
                throw new BusinessRuleException(
                    "Participant already has an active registration.");

            int active = _registrations.Count(r =>
                r.EventId == eventId &&
                r.Status != RegistrationStatus.Cancelled);

            if (active >= ev.Capacity)
                throw new BusinessRuleException(
                    "Event is full.");

            Registration registration = new Registration(
                registrationId,
                eventId,
                participantId,
                DateTime.Now,
                RegistrationStatus.Registered,
                ev.RegistrationFee);

            _registrations.Add(registration);
            _save();
        }

        public void Cancel(int id)
        {
            Registration? registration = GetById(id);

            if (registration == null)
                throw new BusinessRuleException(
                    "Registration not found.");

            if (registration.Status != RegistrationStatus.Registered)
                throw new BusinessRuleException(
                    "Only Registered registrations can be cancelled.");

            registration.Status = RegistrationStatus.Cancelled;
            _save();
        }

        public void MarkAttended(int id)
        {
            Registration? registration = GetById(id);

            if (registration == null)
                throw new BusinessRuleException(
                    "Registration not found.");

            if (registration.Status != RegistrationStatus.Registered)
                throw new BusinessRuleException(
                    "Only Registered registrations can be marked as Attended.");

            registration.Status = RegistrationStatus.Attended;
            _save();
        }
    }
}
