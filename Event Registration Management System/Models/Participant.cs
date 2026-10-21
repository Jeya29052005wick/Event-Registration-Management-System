using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Event_Registration_Management_System.Models
{
    public class Participant
    {
        public int ParticipantID { get; set; }
        public string ParticipantName { get; set; }
        public long ParticipantPhone { get; set; }
        public string ParticipantEmail { get; set; }

        public Participant(int participantID, string participantName, long participantPhone, string participantEmail)
        {
            ParticipantID = participantID;
            ParticipantName = participantName;
            ParticipantPhone = participantPhone;
            ParticipantEmail = participantEmail;
        }

        public string GetDetails()
        {
            return $"ID: {ParticipantID} | Name: {ParticipantName} | " +
                   $"Phone: {ParticipantPhone} | Email: {ParticipantEmail}";
        }

    }
}
