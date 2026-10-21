
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace EventRegistrationManagementSystem.Utilities
{
    public static class InputHelper
    {
        public static string ReadRequired(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.WriteLine("This field is required.");
            }
        }

        public static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;

                Console.WriteLine("Enter a valid whole number.");
            }
        }

        public static decimal ReadDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (decimal.TryParse(input, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out decimal value))
                {
                    return value;
                }

                Console.WriteLine("Enter a valid amount, such as 250.50.");
            }
        }

        public static DateTime ReadDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (DateTime.TryParseExact(input, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }

                Console.WriteLine("Use dd-MM-yyyy format, e.g. 25-12-2026.");
            }
        }

        public static string ReadEmail(string message)
        {
            while (true)
            {
                string email = ReadRequired(message);

                try
                {
                    MailAddress address = new MailAddress(email);

                    if (address.Address.Equals(
                        email, StringComparison.OrdinalIgnoreCase))
                    {
                        return email;
                    }
                }
                catch (FormatException)
                {
                    // Ask for the email again.
                }

                Console.WriteLine("Enter a valid email address.");
            }
        }

        public static string ReadPhone(string message)
        {
            while (true)
            {
                string phone = ReadRequired(message);

                string digits = new string(
                    phone.Where(char.IsDigit).ToArray());

                bool validCharacters = phone.All(c =>
                    char.IsDigit(c) || c == '+' || c == '-' ||
                    c == ' ' || c == '(' || c == ')');

                if (validCharacters &&
                    digits.Length >= 7 &&
                    digits.Length <= 15)
                {
                    return phone;
                }

                Console.WriteLine("Enter a valid phone number.");
            }
        }
    }
}
