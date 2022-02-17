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
    /// Class to make API calls and store data locally for use in other projects.
    /// </summary>
    internal class ApiTrawler
    {
        #region Fields

        /// <summary>
        /// Base URL for all API calls.
        /// </summary>
        private const string apiURL = @"https://api.guildwars2.com/v2";

        /// <summary>
        /// The directory to place the gathered pet icons.
        /// </summary>
        private readonly string iconsDirPets;

        /// <summary>
        /// The directory to place the gathered profession icons.
        /// </summary>
        private readonly string iconsDirProfs;

        /// <summary>
        /// The directory to place the gathered skill icons.
        /// </summary>
        private readonly string iconsDirSkills;

        /// <summary>
        /// The directory to place the gathered specialization icons.
        /// </summary>
        private readonly string iconsDirSpecs;

        /// <summary>
        /// The file path for the generated pets js file.
        /// </summary>
        private readonly string petsJSFilePath;

        /// <summary>
        /// The file path for the generated skill palettes js file.
        /// </summary>
        private readonly string skillPaletteJSFilePath;

        /// <summary>
        /// The file path for the generated specialization enum file.
        /// </summary>
        private readonly string specsCSharpFilePath;

        /// <summary>
        /// The file path for the generated specialization js file.
        /// </summary>
        private readonly string specsJSFilePath;

        /// <summary>
        /// The version of the api to query
        /// </summary>
        internal const string apiVersion = "v=2022-01-01T00:00:00Z";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ApiTrawler"/> instance.
        /// </summary>
        /// <param name="apiOutputDir">The output directory.</param>
        public ApiTrawler(string apiOutputDir)
        {
            if (string.IsNullOrEmpty(apiOutputDir))
                throw new ArgumentException("Missing output directory.");

            iconsDirProfs = Path.Combine(apiOutputDir, "Icons", "Professions");
            iconsDirSkills = Path.Combine(apiOutputDir, "Icons", "Skills");
            iconsDirSpecs = Path.Combine(apiOutputDir, "Icons", "Specializations");
            iconsDirPets = Path.Combine(apiOutputDir, "Icons", "Pets");
            skillPaletteJSFilePath = Path.Combine(apiOutputDir, "skillPalettes.js");
            specsCSharpFilePath = Path.Combine(apiOutputDir, "Specialization.cs");
            specsJSFilePath = Path.Combine(apiOutputDir, "specializations.js");
            petsJSFilePath = Path.Combine(apiOutputDir, "pets.js");
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Resizes all the images in the specified path by the given scale factor.
        /// </summary>
        /// <param name="inputPath">The input path containing images to resize.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <returns></returns>
        private async Task CropImages(string inputPath, int newWidth, int newHeight)
        {
            string outputPath = inputPath + "_s";
            Directory.CreateDirectory(outputPath);
            foreach (FileInfo oldImage in new DirectoryInfo(inputPath).EnumerateFiles())
            {
                using (Image srcImage = Image.FromFile(oldImage.FullName))
                {
                    using (Bitmap newImage = new Bitmap(newWidth, newHeight))
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.Bicubic;
                        graphics.SetClip(new Rectangle(0, 0, newWidth, newHeight));
                        graphics.DrawImage(srcImage, new Rectangle(-(srcImage.Width - newWidth) / 2,
                            -(srcImage.Height - newHeight) / 2, srcImage.Width, srcImage.Height));
                        newImage.Save(Path.Combine(outputPath, oldImage.Name));
                    }
                }
            }
            Directory.Delete(inputPath, true);
            Directory.Move(outputPath, inputPath);
        }

        /// <summary>
        /// Cleans the specified tokens name for use in a file.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A clean name.</returns>
        private string GetCleanTokenName(in JToken token) => token["name"].ToString().Replace(" ", "");

        /// <summary>
        /// Resizes all the images in the specified path by the given scale factor.
        /// </summary>
        /// <param name="inputPath">The input path containing images to resize.</param>
        /// <param name="newSize">The size to set all the images to.</param>
        /// <returns></returns>
        private async Task ResizeImages(string inputPath, int newSize)
        {
            if (newSize < 0)
                return;

            string outputPath = inputPath + "_s";
            Directory.CreateDirectory(outputPath);
            foreach (FileInfo oldImage in new DirectoryInfo(inputPath).EnumerateFiles())
            {
                using (Image srcImage = Image.FromFile(oldImage.FullName))
                {
                    using (Bitmap newImage = new Bitmap(newSize, newSize))
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.Bicubic;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newSize, newSize));
                        newImage.Save(Path.Combine(outputPath, oldImage.Name));
                    }
                }
            }
            Directory.Delete(inputPath, true);
            Directory.Move(outputPath, inputPath);
        }

        /// <summary>
        /// Saves the pet icons
        /// </summary>
        /// <param name="pets">The pets.</param>
        /// <returns></returns>
        private async Task SavePetIcons(JArray pets)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (JToken pet in pets)
                {
                    string iconURL = pet["icon"].ToString();
                    WriteStreamToFile(await client.GetStreamAsync(iconURL),
                        Path.Combine(iconsDirPets, $"{pet["id"]}.png"));
                }
            }
        }

        /// <summary>
        /// Saves the profession icons.
        /// </summary>
        /// <param name="professions">The collection professions JSON objects.</param>
        /// <returns></returns>
        private async Task SaveProfessionIcons(JArray professions)
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
        private async Task SaveProfessionSkillIcons(JArray professions)
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
        private async Task SaveSpecializationImage(string specName, JToken spec)
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
        /// Writes the pets with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="pets">The collection of pet JSON objects.</param>
        /// <param name="file">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private async Task WritePets_JS(JArray pets, StreamWriter file)
        {
            file.WriteLine("/**\n * The available pets\n */");
            file.WriteLine("exports.pets = Object.freeze({");
            foreach (JToken pet in pets)
                file.WriteLine($"  {pet["id"]}: '{pet["name"].ToString()}',");
            file.WriteLine("});");
        }

        /// <summary>
        /// Writes the specializations with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="specs">The collection of specialization JSON objects.</param>
        /// <param name="file">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private async Task WriteSpecializations_JS(JArray specs, StreamWriter file)
        {
            file.WriteLine("/**\n * The available specializations\n */");
            file.WriteLine("exports.specializations = Object.freeze({");
            foreach (JToken spec in specs)
            {
                string specName = spec["name"].ToString().Replace(" ", "");
                file.WriteLine($"  {spec["id"]}: '{specName}',");
                file.WriteLine($"  {specName}: {spec["id"]},");
            }
            file.WriteLine("});");
        }

        /// <summary>
        /// Writes the specialization enum values with the given <see cref="StreamWriter"/> for the
        /// specified profession.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialization JSON objects.</param>
        /// <param name="specFile">The <see cref="StreamWriter"/> to write to.</param>
        /// <returns></returns>
        private async Task WriteSpecializationsForProfession_CS(string professionName, JArray specs, StreamWriter specFile)
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
        private void WriteStreamToFile(Stream source, string path)
        {
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                int b;
                while ((b = source.ReadByte()) != -1)
                    file.WriteByte((byte)b);
            }
        }

        /// <summary>
        /// Trawls the API for ids and icons and writes the result to the output for this trawler.
        /// </summary>
        /// <param name="specIconSize">The size to scale the specialization icons to.</param>
        /// <param name="skillIconSize">The size to scale the skill icons to.</param>
        /// <param name="petIconSize">The size to scale the pet icons to.</param>
        /// <returns></returns>
        public async Task Trawl(int specIconSize, int skillIconSize, int petIconSize)
        {
            // Create output directories
            Directory.CreateDirectory(iconsDirProfs);
            Directory.CreateDirectory(iconsDirSpecs);
            Directory.CreateDirectory(iconsDirSkills);
            Directory.CreateDirectory(iconsDirPets);

            // Build skill palette lookup
            File.Delete(skillPaletteJSFilePath);

            using (HttpClient client = new HttpClient())
            {
                // Get the professions
                JArray professions = JArray.Parse(await client.GetStringAsync($"{apiURL}/professions?ids=all&{apiVersion}"));
                Task profIcons = SaveProfessionIcons(professions);
                Task skillIcons = SaveProfessionSkillIcons(professions)
                    .ContinueWith((t) => ResizeImages(iconsDirSkills, skillIconSize));

                // Get the specializations
                JArray specs = JArray.Parse(await client.GetStringAsync($"{apiURL}/specializations?ids=all&{apiVersion}"));

                File.Delete(specsCSharpFilePath);
                using (StreamWriter file = new StreamWriter(specsCSharpFilePath, false))
                {
                    foreach (JToken profession in professions)
                        await WriteSpecializationsForProfession_CS(profession["name"].ToString(), specs, file).ConfigureAwait(false);
                }

                Task specIcons = ResizeImages(iconsDirSpecs, specIconSize);

                File.Delete(specsJSFilePath);
                using (StreamWriter file = new StreamWriter(specsJSFilePath, false))
                {
                    await WriteSpecializations_JS(specs, file).ConfigureAwait(false);
                }

                // Pets
                JArray pets = JArray.Parse(await client.GetStringAsync($"{apiURL}/pets?ids=all&{apiVersion}"));
                Task petIcons = SavePetIcons(pets)
                    .ContinueWith((t) => CropImages(iconsDirPets, 128, 128))
                    .ContinueWith((t) => ResizeImages(iconsDirPets, petIconSize));

                File.Delete(petsJSFilePath);
                using (StreamWriter file = new StreamWriter(petsJSFilePath, false))
                {
                    await WritePets_JS(pets, file).ConfigureAwait(false);
                }

                await profIcons.ConfigureAwait(false);
                await skillIcons.ConfigureAwait(false);
                await specIcons.ConfigureAwait(false);
                await petIcons.ConfigureAwait(false);
            }
        }

        #endregion Methods
    }
}