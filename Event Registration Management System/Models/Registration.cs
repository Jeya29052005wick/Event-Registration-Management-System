
using System;

namespace EventRegistrationManagementSystem.Models
{
    public enum RegistrationStatus
    {
        Registered,
        Cancelled,
        Attended
    }

    public class Registration : IDisplayable
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public RegistrationStatus Status { get; set; }
        public decimal AmountPaid { get; set; }

        public Registration()
        {
        }

        public Registration(int registrationId, int eventId,
                            int participantId, DateTime registrationDate,
                            RegistrationStatus status, decimal amountPaid)
        {
            RegistrationId = registrationId;
            EventId = eventId;
            ParticipantId = participantId;
            RegistrationDate = registrationDate;
            Status = status;
            AmountPaid = amountPaid;
        }

        public string GetDetails()
        {
            return $"Registration ID: {RegistrationId} | " +
                   $"Event ID: {EventId} | Participant ID: {ParticipantId} | " +
                   $"Date: {RegistrationDate:dd-MM-yyyy HH:mm} | " +
                   $"Status: {Status} | Amount Paid: ₹{AmountPaid:F2}";
        }
    }
}
