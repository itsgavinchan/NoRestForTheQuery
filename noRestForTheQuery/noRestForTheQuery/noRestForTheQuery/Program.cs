using System;

namespace noRestForTheQuery
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FinalGame game = new FinalGame())
            {
                game.Run();
            }
        }
    }
#endif
}

