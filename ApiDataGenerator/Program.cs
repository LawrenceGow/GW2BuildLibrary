using Mono.Options;
using System;
using System.Threading.Tasks;

namespace ApiDataGenerator
{
    /// <summary>
    /// Main program class for the ApiDataGenerator.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        private static async Task Main(string[] args)
        {
            string apiOutputDir = "";

            int specIconSize = -1,
                skillIconSize = -1,
                petIconSize = -1;

            OptionSet options = new OptionSet()
            {
                // Output Directory
                {
                    "o|output=",
                    "Output Directory",
                    (v) => apiOutputDir = v
                },

                // Specialization Icon Size
                {
                    "sps=",
                    "Specialization Icon Size",
                    (int v) => specIconSize = v
                },

                // Skill Icon Size
                {
                    "sks=",
                    "Skill Icon Size",
                    (int v) => skillIconSize = v
                },

                // Pet Icon Size
                {
                    "ps=",
                    "Pet Icon Size",
                    (int v) => petIconSize = v
                },
            };

            try
            {
                options.Parse(args);
                await new ApiTrawler(apiOutputDir).Trawl(specIconSize, skillIconSize, petIconSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Complete!");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        #endregion Methods
    }
}