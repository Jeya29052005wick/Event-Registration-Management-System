
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using EventRegistrationManagementSystem.Models;

namespace EventRegistrationManagementSystem.Services
{
    public class ParticipantService
    {
        private readonly List<Participant> _participants;
        private readonly List<Registration> _registrations;
        private readonly Action _save;

        public ParticipantService(
            List<Participant> participants,
            List<Registration> registrations,
            Action save)
        {
            _participants = participants;
            _registrations = registrations;
            _save = save;
        }

        public List<Participant> GetAll()
        {
            return _participants
                .OrderBy(p => p.FullName)
                .ToList();
        }

        public Participant? GetById(int id)
        {
            return _participants.FirstOrDefault(
                p => p.ParticipantId == id);
        }

        public List<Participant> Search(string term)
        {
            return _participants
                .Where(p =>
                    p.FullName.Contains(term,
                        StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(term,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.FullName)
                .ToList();
        }

        private void Validate(
            string name,
            string email,
            string phone,
            int excludedId = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException(
                    "Full name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleException(
                    "Email is required.");

            try
            {
                MailAddress address = new MailAddress(email);

                if (!address.Address.Equals(
                    email, StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                throw new BusinessRuleException(
                    "Enter a valid email address.");
            }

            if (_participants.Any(p =>
                p.ParticipantId != excludedId &&
                p.Email.Equals(
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessRuleException(
                    "Email is already registered.");
            }

            if (string.IsNullOrWhiteSpace(phone))
                throw new BusinessRuleException(
                    "Phone number is required.");

            string digits = new string(
                phone.Where(char.IsDigit).ToArray());

            bool validCharacters = phone.All(c =>
                char.IsDigit(c) ||
                c == '+' || c == '-' ||
                c == ' ' || c == '(' || c == ')');

            if (!validCharacters ||
                digits.Length < 7 ||
                digits.Length > 15)
            {
                throw new BusinessRuleException(
                    "Enter a valid phone number.");
            }
        }

        public void Add(Participant participant)
        {
            if (participant.ParticipantId <= 0)
                throw new BusinessRuleException(
                    "Participant ID must be positive.");

            if (_participants.Any(p =>
                p.ParticipantId == participant.ParticipantId))
            {
                throw new BusinessRuleException(
                    "Participant ID already exists.");
            }

            Validate(
                participant.FullName,
                participant.Email,
                participant.PhoneNumber);

            participant.FullName = participant.FullName.Trim();
            participant.Email = participant.Email.Trim();
            participant.PhoneNumber = participant.PhoneNumber.Trim();

            _participants.Add(participant);
            _save();
        }

        public void Update(
            int id,
            string name,
            string email,
            string phone)
        {
            Participant? participant = GetById(id);

            if (participant == null)
                throw new BusinessRuleException(
                    "Participant not found.");

            Validate(name, email, phone, id);

            participant.FullName = name.Trim();
            participant.Email = email.Trim();
            participant.PhoneNumber = phone.Trim();

            _save();
        }

        public void Delete(int id)
        {
            Participant? participant = GetById(id);

            if (participant == null)
                throw new BusinessRuleException(
                    "Participant not found.");

            if (_registrations.Any(r =>
                r.ParticipantId == id))
            {
                throw new BusinessRuleException(
                    "Cannot delete a participant with registration history.");
            }

            _participants.Remove(participant);
            _save();
        }
    }
}
