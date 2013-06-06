using System;

namespace AsteroidRebuttal
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AsteroidRebuttal game = new AsteroidRebuttal())
            {
                game.Run();
            }
        }
    }
}

