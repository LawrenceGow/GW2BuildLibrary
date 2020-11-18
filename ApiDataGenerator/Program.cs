using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDataGenerator
{
    internal class Program
    {
        private const string apiURL = @"https://api.guildwars2.com/v2";
        private static readonly string apiOutputDir = Path.Combine(@"D:\Temp\GW2BuildLibrary");
        private static readonly string specsFilePath = Path.Combine(apiOutputDir, "Specialization.txt");
        private static readonly string specIconsDir = Path.Combine(apiOutputDir, @"Icons\Specialisations");
        private static readonly string professionIconsDir = Path.Combine(apiOutputDir, @"Icons\Professions");

        private static async Task Main(string[] args)
        {
            Directory.CreateDirectory(apiOutputDir);
            Directory.CreateDirectory(specIconsDir);
            Directory.CreateDirectory(professionIconsDir);

            using (HttpClient client = new HttpClient())
            {
                // Get the professions
                JArray professions = JArray.Parse(await client.GetStringAsync($"{apiURL}/professions?ids=all"));
                await SaveProfessionIcons(professions);

                // Get the specialisations
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids=all"));

                File.Delete(specsFilePath);
                using (StreamWriter specFile = new StreamWriter(specsFilePath, false))
                {
                    foreach (JToken profession in professions)
                        await WriteSpecialisationsForProfession(profession["name"].ToString(), specs, specFile);
                }
            }


            Console.WriteLine("DONE! Press any key to exit.");
            Console.ReadKey();
            // Open the output folder
            Process.Start(apiOutputDir);
        }

        private static async Task SaveProfessionIcons(JArray professions)
        {
            foreach (JToken profession in professions)
            {

                using (HttpClient client = new HttpClient())
                {
                    string iconURL = profession["icon"].ToString();
                    WriteStreamToFile(await client.GetStreamAsync(iconURL),
                        Path.Combine(professionIconsDir, $"{profession["name"]}.png"));
                }
            }
        }

        /// <summary>
        /// Writes the specialisation enum values with the given <see cref="StreamWriter"/>
        /// for the specified profession.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialisation JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        private static async Task WriteSpecialisationsForProfession(string professionName, JArray specs, StreamWriter specFile)
        {
            specFile.WriteLine($"// {professionName}");
            foreach (JToken spec in specs.Where(s => s["profession"].ToString() == professionName))
            {
                string specName = spec["name"].ToString().Replace(" ", "");
                specFile.WriteLine($"{specName} = {spec["id"]},");
                await SaveSpecialisationImage(specName, spec);
            }
            specFile.WriteLine();
        }

        /// <summary>
        /// Saves the specialisation icon.
        /// </summary>
        /// <param name="specName">The name of the specialisation.</param>
        /// <param name="spec">The specialisation JSON object.</param>
        private static async Task SaveSpecialisationImage(string specName, JToken spec)
        {
            using (HttpClient client = new HttpClient())
            {
                string iconURL = spec["icon"].ToString();
                WriteStreamToFile(await client.GetStreamAsync(iconURL),
                    Path.Combine(specIconsDir, $"{specName}.png"));

                JToken profIcon = spec["profession_icon"];
                if (profIcon != null)
                {
                    WriteStreamToFile(await client.GetStreamAsync(profIcon.ToString()),
                        Path.Combine(professionIconsDir, $"{specName}.png"));
                }
            }
        }

        private static void WriteStreamToFile(Stream source, string path)
        {
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                int b;
                while ((b = source.ReadByte()) != -1)
                    file.WriteByte((byte)b);
            }
        }
    }
}
