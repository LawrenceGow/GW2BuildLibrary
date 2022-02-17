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

            double specIconScale = 1.0,
                skillIconScale = 1.0,
                petIconScale = 1.0;

            OptionSet options = new OptionSet()
            {
                // Output Directory
                {
                    "o|output=",
                    "Output Directory",
                    (v) => apiOutputDir = v
                },

                // Specialization Icon Scale
                {
                    "sps=",
                    "Specialization Icon Scale",
                    (double v) => specIconScale = v
                },

                // Skill Icon Scale
                {
                    "sks=",
                    "Skill Icon Scale",
                    (double v) => skillIconScale = v
                },

                // Pet Icon Scale
                {
                    "ps=",
                    "Pet Icon Scale",
                    (double v) => petIconScale = v
                },
            };

            try
            {
                options.Parse(args);
                await new ApiTrawler(apiOutputDir).Trawl(specIconScale, skillIconScale, petIconScale);
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