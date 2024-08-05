using System;
namespace FRIWO.WorkerServices
{

    public static class ConsoleExtensions
	{

        /// <summary>
        ///	Extension of Console for making color
        /// </summary>
        /// <param name="content">Content</param>
        /// <param name="color">Console Color</param>
        public static void WriteLineColor(this string content,
                                          ConsoleColor color)
		{

            Console.ForegroundColor = color;
			Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
	}
}