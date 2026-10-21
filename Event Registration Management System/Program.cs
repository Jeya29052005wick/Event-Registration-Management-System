using Event_Registration_Management_System.Data;
using Event_Registration_Management_System.Services;

public class Execute
{
    public static void Main(string[] args)
    {
        EventService eventService = new EventService(DataStore.Events);
        ParticipantService participantService = new ParticipantService(DataStore.Participants);
        bool running = true;

        while (running)
        {
            Console.WriteLine("1. Display All Events");
            Console.WriteLine("2. Search Event");
            Console.WriteLine("3. Display All Participants");
            Console.WriteLine("4. Search Participant");
            Console.WriteLine("5. Update Event");
            Console.WriteLine("6. Delete Event");
            Console.WriteLine("7. Update Participant");
            Console.WriteLine("8. Delete Participant");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Enter a valid Choice.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    eventService.DisplayAllEvent();
                    break;

                case 2:
                    eventService.SearchEvent();
                    break;

                case 3:
                    participantService.DisplayAllParticipant();
                    break;

                case 4:
                    participantService.SearchParticipant();
                    break;

                case 5:
                    eventService.UpdateEvent();
                    break;

                case 6:
                    eventService.DeleteEvent();
                    break;

                case 7:
                    participantService.UpdateParticipant();
                    break;

                case 8:
                    participantService.DeleteParticipant();
                    break;

                case 0:
                    running = false;
                    Console.WriteLine("Application closed.");
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}