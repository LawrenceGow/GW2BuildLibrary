using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDataGenerator
{
    /// <summary>
    /// Main program class for the ApiDataGenerator.
    /// Makes API calls and stores data locally for use in other projects.
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// Base URL for all API calls.
        /// </summary>
        private const string apiURL = @"https://api.guildwars2.com/v2";

        /// <summary>
        /// The output directory.
        /// </summary>
        private static readonly string apiOutputDir = Path.Combine(@"D:\Temp\GW2BuildLibrary");

        /// <summary>
        /// The directory to place all the gathered icons.
        /// </summary>
        private static readonly string iconsDir = Path.Combine(apiOutputDir, "Icons");

        /// <summary>
        /// The file path for the generated specialization enum file.
        /// </summary>
        private static readonly string specsFilePath = Path.Combine(apiOutputDir, "Specialization.txt");

        #endregion Fields

        #region Methods

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        private static async Task Main(string[] args)
        {
            Directory.CreateDirectory(apiOutputDir);
            Directory.CreateDirectory(iconsDir);

            using (HttpClient client = new HttpClient())
            {
                // Get the professions
                JArray professions = JArray.Parse(await client.GetStringAsync($"{apiURL}/professions?ids=all"));
                await SaveProfessionIcons(professions);

                // Get the specializations
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids=all"));

                File.Delete(specsFilePath);
                using (StreamWriter specFile = new StreamWriter(specsFilePath, false))
                {
                    foreach (JToken profession in professions)
                        await WriteSpecializationsForProfession(profession["name"].ToString(), specs, specFile);
                }
            }

            Console.WriteLine("DONE! Press any key to exit.");
            Console.ReadKey();
            // Open the output folder
            Process.Start(apiOutputDir);
        }

        /// <summary>
        /// Saves the profession icons.
        /// </summary>
        /// <param name="professions">The collection professions JSON objects.</param>
        /// <returns></returns>
        private static async Task SaveProfessionIcons(JArray professions)
        {
            foreach (JToken profession in professions)
            {
                using (HttpClient client = new HttpClient())
                {
                    string iconURL = profession["icon"].ToString();
                    WriteStreamToFile(await client.GetStreamAsync(iconURL),
                        Path.Combine(iconsDir, $"{profession["name"]}_Profession.png"));
                }
            }
        }

        /// <summary>
        /// Saves the specialization icon.
        /// </summary>
        /// <param name="specName">The name of the specialization.</param>
        /// <param name="spec">The specialization.</param>
        /// <returns></returns>
        private static async Task SaveSpecializationImage(string specName, JToken spec)
        {
            using (HttpClient client = new HttpClient())
            {
                string iconURL = spec["icon"].ToString();
                WriteStreamToFile(await client.GetStreamAsync(iconURL),
                    Path.Combine(iconsDir, $"{specName}_Specialization.png"));

                JToken profIcon = spec["profession_icon"];
                if (profIcon != null)
                {
                    WriteStreamToFile(await client.GetStreamAsync(profIcon.ToString()),
                        Path.Combine(iconsDir, $"{specName}_Profession.png"));
                }
            }
        }

        /// <summary>
        /// Writes the specialization enum values with the given <see cref="StreamWriter"/> for the specified profession.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialization JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private static async Task WriteSpecializationsForProfession(string professionName, JArray specs, StreamWriter specFile)
        {
            specFile.WriteLine($"// {professionName}");
            foreach (JToken spec in specs.Where(s => s["profession"].ToString() == professionName))
            {
                string specName = spec["name"].ToString().Replace(" ", "");
                specFile.WriteLine($"{specName} = {spec["id"]},");
                await SaveSpecializationImage(specName, spec);
            }
            specFile.WriteLine();
        }

        /// <summary>
        /// Writes the given <see cref="Stream"/> to a file at the specified path.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to read from.</param>
        /// <param name="path">The target file path.</param>
        private static void WriteStreamToFile(Stream source, string path)
        {
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                int b;
                while ((b = source.ReadByte()) != -1)
                    file.WriteByte((byte)b);
            }
        }

        #endregion Methods
    }
}