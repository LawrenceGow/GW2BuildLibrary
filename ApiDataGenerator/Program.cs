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
        private static readonly string apiOutputPath = Path.Combine(@"D:\Temp\GW2BuildLibrary");
        private static readonly string specsFilePath = Path.Combine(apiOutputPath, "Specialization.txt");

        private static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                // Get the list of professions ids
                string professionIds = string.Join(",",
                    JArray.Parse(await client.GetStringAsync($"{apiURL}/professions")));

                // Get the profession info
                JArray professions = JArray.Parse(await client.GetStringAsync($"{apiURL}/professions?ids={professionIds}"));

                // Get the list of specialisation ids
                string specIds = string.Join(",",
                    JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations")));

                // Get the specialisation info
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids={specIds}"));

                // Ensure the output directory exists
                Directory.CreateDirectory(apiOutputPath);

                File.Delete(specsFilePath);
                using (StreamWriter specFile = new StreamWriter(specsFilePath, false))
                {
                    foreach (var profession in professions)
                        WriteSpecialisationsForProfession(profession["name"].ToString(), specs, specFile);
                }
            }


            Console.WriteLine("DONE! Press any key to exit.");
            Console.ReadKey();
            // Open the output folder
            Process.Start(apiOutputPath);
        }

        /// <summary>
        /// Writes the specialisation enum values with the given <see cref="StreamWriter"/>
        /// for the specified profession.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialisation JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        private static void WriteSpecialisationsForProfession(string professionName, JArray specs, StreamWriter specFile)
        {
            specFile.WriteLine($"// {professionName}");
            foreach (JToken spec in specs.Where(s => s["profession"].ToString() == professionName))
            {
                string specName = spec["name"].ToString().Replace(" ", "");
                specFile.WriteLine($"{specName} = {spec["id"]},");
                WriteSpecialisationImgFile(specName, spec);
            }
            specFile.WriteLine();
        }

        /// <summary>
        /// Writes the specialisation icon to a file.
        /// </summary>
        /// <param name="specName">The name of the specialisation.</param>
        /// <param name="spec">The specialisation JSON object.</param>
        private static async void WriteSpecialisationImgFile(string specName, JToken spec)
        {
            string dir = Path.Combine(apiOutputPath, @"Icons\Specialisations");
            Directory.CreateDirectory(dir);

            using (FileStream specImgFile = new FileStream(Path.Combine(dir, $"{specName}.png"),
                FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (HttpClient client = new HttpClient())
            {
                string iconURL = spec["icon"].ToString();
                Stream data = await client.GetStreamAsync(iconURL);
                int b;
                while ((b = data.ReadByte()) != -1)
                    specImgFile.WriteByte((byte)b);
            }
        }
    }
}
