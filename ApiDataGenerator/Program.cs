using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDataGenerator
{
    /// <summary>
    /// Main program class for the ApiDataGenerator. Makes API calls and stores data locally for use
    /// in other projects.
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
        private static readonly string specsCSharpFilePath = Path.Combine(apiOutputDir, "Specialization.cs");

        /// <summary>
        /// The file path for the generated specialization js file.
        /// </summary>
        private static readonly string specsJSFilePath = Path.Combine(apiOutputDir, "specializations.js");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Cleans the specified tokens name for use in a file.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A clean name.</returns>
        private static string GetCleanTokenName(in JToken token) => token["name"].ToString().Replace(" ", "");

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
                var profIcons = SaveProfessionIcons(professions);
                var skillIcons = SaveProfessionSkillIcons(professions);

                // Get the specializations
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids=all"));

                File.Delete(specsCSharpFilePath);
                using (StreamWriter specCSFile = new StreamWriter(specsCSharpFilePath, false))
                {
                    foreach (JToken profession in professions)
                        await WriteSpecializationsForProfession_CS(profession["name"].ToString(), specs, specCSFile);
                }

                File.Delete(specsJSFilePath);
                using (StreamWriter specJSFile = new StreamWriter(specsJSFilePath, false))
                {
                    await WriteSpecializations_JS(specs, specJSFile);
                }

                await profIcons;
                await skillIcons;
            }

            Console.WriteLine("Complete!");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
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
                        Path.Combine(iconsDir, $"{GetCleanTokenName(profession)}_Profession.png"));
                }
            }
        }

        /// <summary>
        /// Saves the skill icons.
        /// </summary>
        /// <param name="professions">The professions.</param>
        /// <returns></returns>
        private static async Task SaveProfessionSkillIcons(JArray professions)
        {
            foreach (JToken profession in professions)
            {
                // Build the skill id collection
                var skillIds = profession["skills"]
                    .Where(s => s["type"].ToString() == "Utility"
                        || s["type"].ToString() == "Heal"
                        || s["type"].ToString() == "Elite")
                    .Select(s => s["id"].Value<int>());

                using (HttpClient client = new HttpClient())
                {
                    string skillsUrl = $"{apiURL}/skills?ids={string.Join(",", skillIds)}";
                    JArray skills = JArray.Parse(await client.GetStringAsync(skillsUrl));
                    foreach (JToken skill in skills)
                    {
                        string iconURL = skill["icon"].ToString();
                        WriteStreamToFile(await client.GetStreamAsync(iconURL),
                            Path.Combine(iconsDir, $"{skill["id"]}.png"));
                    }
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
        /// Writes the specializations with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="profs">The collection of specialization JSON objects.</param>
        /// <param name="profFile">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private static Task WriteSpecializations_JS(JArray specs, StreamWriter specFile)
        {
            return Task.Run(() =>
            {
                specFile.WriteLine("/**\n * The available specializations\n */");
                specFile.WriteLine("exports.specializations = Object.freeze({");
                foreach (JToken spec in specs)
                {
                    string specName = spec["name"].ToString().Replace(" ", "");
                    specFile.WriteLine($"  {spec["id"]}: '{specName}',");
                    specFile.WriteLine($"  {specName}: {spec["id"]},");
                }
                specFile.WriteLine("});");
            });
        }

        /// <summary>
        /// Writes the specialization enum values with the given <see cref="StreamWriter"/> for the
        /// specified profession.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialization JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private static async Task WriteSpecializationsForProfession_CS(string professionName, JArray specs, StreamWriter specFile)
        {
            specFile.WriteLine($"// {professionName}");
            foreach (JToken spec in specs.Where(s => s["profession"].ToString() == professionName))
            {
                string specName = GetCleanTokenName(spec);
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