using Event_Registration_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Event_Registration_Management_System.Services
{
    public class ParticipantService
    {
        private List<Participant> participants;

        public ParticipantService(List<Participant> participants)
        {
            this.participants = participants;
        }

        public void DisplayAllParticipant()
        {
            var result = from item in participants
                         select item;

            foreach (var item in result)
            {
                Console.WriteLine(item.GetDetails());
            }

            if (!participants.Any())
            {
                Console.WriteLine("No participants available.");
            }
        }

        public void SearchParticipant()
        {
            Console.Write("Enter Participant Name: ");
            string? searchName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchName))
            {
                Console.WriteLine("Give a valid participant name.");
                return;
            }

            var result = participants.Where(p =>p.ParticipantName.Contains(
                    searchName,
                    StringComparison.OrdinalIgnoreCase));

            if (!result.Any())
            {
                Console.WriteLine("Participant not found.");
                return;
            }

            foreach (var item in result)
            {
                Console.WriteLine(item.GetDetails());
            }
        }

        
        public void UpdateParticipant()
        {
            Console.Write("Enter Participant ID to update: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Participant ID.");
                return;
            }

            Participant? item = participants.FirstOrDefault(
                p => p.ParticipantID == id);

            if (item == null)
            {
                Console.WriteLine("Participant not found.");
                return;
            }

            Console.Write("Enter new Participant Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter new Email: ");
            string email = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name) ||string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Enter a valid name and email.");
                return;
            }

            item.ParticipantName = name;
            item.ParticipantEmail = email;

            Console.WriteLine("Participant updated successfully.");
        }

        public void DeleteParticipant()
        {
            Console.Write("Enter Participant ID to delete: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Participant ID.");
                return;
            }

            Participant? item = participants.FirstOrDefault(
                p => p.ParticipantID == id);

            if (item == null)
            {
                Console.WriteLine("Participant not found.");
                return;
            }

            participants.Remove(item);

            Console.WriteLine("Participant deleted successfully.");
        }
    }
}