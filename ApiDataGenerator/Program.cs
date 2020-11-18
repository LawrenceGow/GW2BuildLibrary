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
        private static async Task Main(string[] args)
        {
            await WriteAPIDataToFiles();

            // Open the output folder
            Process.Start(apiOutputPath);
        }

        private static readonly string apiOutputPath = Path.Combine(@"D:\Temp\GW2BuildLibrary");
        private static readonly string specsFilePath = Path.Combine(apiOutputPath, "Specialization.txt");

        /// <summary>
        /// Writes a file contain the currently available specialisations for all classes.
        /// </summary>
        public static async Task WriteAPIDataToFiles()
        {
            using (HttpClient client = new HttpClient())
            {
                // Get the list of specialisation ids
                string specIds = string.Join(",",
                    JArray.Parse(await client.GetStringAsync("https://api.guildwars2.com/v2/specializations")));

                // Get the specialisation info
                JArray specs = JArray.Parse(await client.GetStringAsync($"https://api.guildwars2.com/v2/specializations?ids={specIds}"));

                // Ensure the output directory exists
                Directory.CreateDirectory(apiOutputPath);

                File.Delete(specsFilePath);
                using (StreamWriter specFile = new StreamWriter(specsFilePath, false))
                {
                    WriteSpecialisations("Guardian", specs, specFile);
                    WriteSpecialisations("Warrior", specs, specFile);
                    WriteSpecialisations("Engineer", specs, specFile);
                    WriteSpecialisations("Ranger", specs, specFile);
                    WriteSpecialisations("Thief", specs, specFile);
                    WriteSpecialisations("Elementalist", specs, specFile);
                    WriteSpecialisations("Mesmer", specs, specFile);
                    WriteSpecialisations("Necromancer", specs, specFile);
                    WriteSpecialisations("Revenant", specs, specFile);
                }
            }
        }

        /// <summary>
        /// Writes the specialisation enum values with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialisation JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        private static void WriteSpecialisations(string professionName, JArray specs, StreamWriter specFile)
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
