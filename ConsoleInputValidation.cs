using System;
using System.Collections.Generic;
using System.Text;

namespace TreasureHunter
{
    static class ConsoleInputValidation
    {
        // Display a message and wait for player input
        public static String ReadString(String Message)
        {
            Console.WriteLine(Message);
            return Console.ReadLine();
        }

        // Display a message and wait for player input, checking against allowed strings until the player enters a valid one
        public static String ReadString(String Message, String[] AllowedInputs)
        {
            String input;

            do
            {
                Console.WriteLine(Message);
                input = Console.ReadLine();
                foreach (String sentence in AllowedInputs)
                {
                    if (input.ToLower() == sentence.ToLower())
                    {
                        return sentence;
                    }
                }
            }
            while (true);
        }

        // Convert player input to an integer
        public static int ReadInteger()
        {
            int IntValue;

            while (!int.TryParse(Console.ReadLine(), out IntValue))
            {
                Console.WriteLine("Error: Not a valid number. Please try again.");
            }

            return IntValue;
        }

        // Display a message and wait for player input. Converts it to an integer
        public static int ReadInteger(String Message)
        {
            int IntValue;

            while (!int.TryParse(ReadString(Message), out IntValue))
            {
                Console.WriteLine("Error: Not a valid number. Please try again.");
            }

            return IntValue;
        }

        // Display a message and wait for player input. Converts it to an integer and makes sure its in the defined range
        public static int ReadInteger(String Message, int RangeMin, int RangeMax)
        {
            int IntValue = ReadInteger(Message);
            while (IntValue < RangeMin || IntValue > RangeMax)
            {
                Console.WriteLine("That number is out of range. Please try again");
                IntValue = ReadInteger();
            }

            return IntValue;
        }
    }
}
