using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        /// The version of the api to query
        /// </summary>
        private static readonly string apiVersion = "v=2022-01-01T00:00:00Z";

        /// <summary>
        /// The directory to place the gathered profession icons.
        /// </summary>
        private static readonly string iconsDirProfs = Path.Combine(apiOutputDir, "Icons", "Professions");

        /// <summary>
        /// The directory to place the gathered skill icons.
        /// </summary>
        private static readonly string iconsDirSkills = Path.Combine(apiOutputDir, "Icons", "Skills");

        /// <summary>
        /// The directory to place the gathered specialization icons.
        /// </summary>
        private static readonly string iconsDirSpecs = Path.Combine(apiOutputDir, "Icons", "Specializations");

        /// <summary>
        /// The file path for the generated skill palettes js file.
        /// </summary>
        private static readonly string skillPaletteJSFilePath = Path.Combine(apiOutputDir, "skillPalettes.js");

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
            // Create output directories
            Directory.CreateDirectory(apiOutputDir);
            Directory.CreateDirectory(iconsDirProfs);
            Directory.CreateDirectory(iconsDirSpecs);
            Directory.CreateDirectory(iconsDirSkills);

            // Build skill palette lookup
            File.Delete(skillPaletteJSFilePath);

            using (HttpClient client = new HttpClient())
            {
                // Get the professions
                JArray professions = JArray.Parse(await client.GetStringAsync($"{apiURL}/professions?ids=all&{apiVersion}"));
                Task profIcons = SaveProfessionIcons(professions);
                Task skillIcons = SaveProfessionSkillIcons(professions)
                    .ContinueWith((t) => ResizeImages(iconsDirSkills, 0.25));

                // Get the specializations
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids=all&{apiVersion}"));

                File.Delete(specsCSharpFilePath);
                using (StreamWriter specCSFile = new StreamWriter(specsCSharpFilePath, false))
                {
                    foreach (JToken profession in professions)
                        await WriteSpecializationsForProfession_CS(profession["name"].ToString(), specs, specCSFile).ConfigureAwait(false);
                }

                Task specIcons = ResizeImages(iconsDirSpecs, 1.0);

                File.Delete(specsJSFilePath);
                using (StreamWriter specJSFile = new StreamWriter(specsJSFilePath, false))
                {
                    await WriteSpecializations_JS(specs, specJSFile).ConfigureAwait(false);
                }

                await profIcons.ConfigureAwait(false);
                await skillIcons.ConfigureAwait(false);
                await specIcons.ConfigureAwait(false);
            }

            Console.WriteLine("Complete!");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Resizes all the images in the specified path by the given scale factor.
        /// </summary>
        /// <param name="inputPath">The input path containing images to resize.</param>
        /// <param name="scaleFactor">The scale factor to apply to all the images.</param>
        /// <returns></returns>
        private static async Task ResizeImages(string inputPath, double scaleFactor)
        {
            if (scaleFactor == 1.0)
                return;

            string outputPath = inputPath + "_s";
            Directory.CreateDirectory(outputPath);
            foreach (FileInfo oldImage in new DirectoryInfo(inputPath).EnumerateFiles())
            {
                using (Image srcImage = Image.FromFile(oldImage.FullName))
                {
                    int newWidth = (int)(srcImage.Width * scaleFactor);
                    int newHeight = (int)(srcImage.Height * scaleFactor);
                    using (Bitmap newImage = new Bitmap(newWidth, newHeight))
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.Bicubic;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(Path.Combine(outputPath, oldImage.Name));
                    }
                }
            }
            Directory.Delete(inputPath, true);
            Directory.Move(outputPath, inputPath);
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
                        Path.Combine(iconsDirProfs, $"{GetCleanTokenName(profession)}.png"));
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
            Dictionary<int, string> skillPaletteFileNameMap = new Dictionary<int, string>();
            foreach (JToken profession in professions)
            {
                // We have revs by themselves
                if (profession["code"].Value<int>() == 9)
                    continue;

                // Build the skill id collection
                foreach (JArray pair in profession["skills_by_palette"]
                    .Select(v => JArray.Parse(v.ToString())))
                {
                    int skillId = pair[1].Value<int>();
                    short paletteId = pair[0].Value<short>();
                    skillPaletteFileNameMap[skillId] = $"0_{paletteId}.png";
                }
            }

            using (HttpClient client = new HttpClient())
            {
                // Revenants are a unique beast in that they use the same skill palettes for every
                // legend: 4572 for heal, 4614/4651/4564 for utilities, 4554 for elite.

                // We'll get around this by having the filenames be '<LegendCode>_<PaletteId>.png'
                JArray legends = JArray.Parse(await client.GetStringAsync($"{apiURL}/Legends?ids=all&{apiVersion}"));

                IEnumerable<(int, int, int)> heals = legends
                    .Select(l => (l["code"].Value<int>(), l["heal"].Value<int>(), 4572));
                IEnumerable<(int, int, int)> elites = legends
                    .Select(l => (l["code"].Value<int>(), l["elite"].Value<int>(), 4554));
                IEnumerable<(int, int, int)> util1s = legends
                    .Select(l => (l["code"].Value<int>(), JArray.Parse(l["utilities"].ToString())[0].Value<int>(), 4614));
                IEnumerable<(int, int, int)> util2s = legends
                    .Select(l => (l["code"].Value<int>(), JArray.Parse(l["utilities"].ToString())[1].Value<int>(), 4651));
                IEnumerable<(int, int, int)> util3s = legends
                    .Select(l => (l["code"].Value<int>(), JArray.Parse(l["utilities"].ToString())[2].Value<int>(), 4564));

                // Add rev skills into the map
                foreach ((int, int, int) item in heals.Concat(util1s).Concat(util2s).Concat(util3s).Concat(elites))
                    skillPaletteFileNameMap[item.Item2] = $"{item.Item1}_{item.Item3}.png";

                for (int i = 0; i < skillPaletteFileNameMap.Count; i += 100)
                {
                    string ids = string.Join(",", skillPaletteFileNameMap.Keys.Skip(i).Take(100));
                    string skillsUrl = $"{apiURL}/skills?ids={ids}";
                    JArray skills = JArray.Parse(await client.GetStringAsync(skillsUrl));
                    foreach (JToken skill in skills)
                    {
                        string type = skill["type"].ToString();
                        if (type != "Utility" && type != "Heal" && type != "Elite")
                            continue;

                        string iconURL = skill["icon"].ToString();
                        int skillId = skill["id"].Value<int>();
                        if (skillId != 30800)
                        {
                            WriteStreamToFile(await client.GetStreamAsync(iconURL),
                                Path.Combine(iconsDirSkills, skillPaletteFileNameMap[skillId]));
                        }
                        // Mortar kit has two palettes depending on the source you ask
                        else
                        {
                            // Save the palette from skills_by_palette
                            WriteStreamToFile(await client.GetStreamAsync(iconURL),
                                Path.Combine(iconsDirSkills, "0_408.png"));
                            // Save the palette the GW2 client actually uses in its link codes
                            WriteStreamToFile(await client.GetStreamAsync(iconURL),
                                Path.Combine(iconsDirSkills, "0_4857.png"));
                        }
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
                    Path.Combine(iconsDirSpecs, $"{specName}.png"));

                JToken profIcon = spec["profession_icon"];
                if (profIcon != null)
                {
                    WriteStreamToFile(await client.GetStreamAsync(profIcon.ToString()),
                        Path.Combine(iconsDirProfs, $"{specName}.png"));
                }
            }
        }

        /// <summary>
        /// Writes the specializations with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="profs">The collection of specialization JSON objects.</param>
        /// <param name="profFile">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private static async Task WriteSpecializations_JS(JArray specs, StreamWriter specFile)
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