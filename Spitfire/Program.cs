using System;

namespace Spitfire
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SpitfireGame game = new SpitfireGame())
            {
                game.Run();
            }
        }
    }
}

