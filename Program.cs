using System;

namespace NLBE_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Bot TheBot = new Bot();
                TheBot.RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reden waarom het niet werkt:\n" + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
