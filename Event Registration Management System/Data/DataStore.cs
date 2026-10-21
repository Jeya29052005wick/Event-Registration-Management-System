using Event_Registration_Management_System.Models;
using System.Collections.Generic;

namespace Event_Registration_Management_System.Data
{
    public static class DataStore
    {
        public static List<Event> Events = new List<Event>
        {
            new Event(1, "Dancing Floor", "SRL Auditorium","21-10-2026", 100, 1000),
            new Event(2, "Singing Floor", "CM Auditorium","22-10-2026", 50, 2000)
        };

        public static List<Participant> Participants = new List<Participant>
        {
            new Participant(1, "Ragul", 564789303,"jeyamav@gmail.com"),
            new Participant(2, "Kishore", 789789303,"jeyac@gmail.com")
        };
    }
}