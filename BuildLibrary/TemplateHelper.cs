using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Helpful methods for <see cref="Profession"/> and <see cref="Specialization"/> bit-twiddling.
    /// </summary>
    public static class TemplateHelper
    {
        public const byte Core = 0b0000;
        public const byte HeartOfThorns = 0b0001;
        public const byte PathOfFire = 0b0010;
        public const byte EndOfDragons = 0b0011;

        private static readonly HashSet<byte> HeartOfThornsSpecs = new HashSet<byte>()
        { 5, 7, 18, 27, 34, 40, 43, 48, 52 };

        private static readonly HashSet<byte> PathOfFireSpecs = new HashSet<byte>()
        { 55, 56, 57, 58, 59, 60, 61, 62, 63 };

        private static readonly HashSet<byte> EndOfDragonsSpecs = new HashSet<byte>()
        { };

        /// <summary>
        /// Gets the byte that represents the set of elite specialisations required
        /// for the specializations provided.
        /// </summary>
        /// <param name="eliteSlotByte">The elite specialization slot byte.</param>
        /// <returns></returns>
        public static byte GetSetByte(in byte eliteSlotByte)
        {
            if (HeartOfThornsSpecs.Contains(eliteSlotByte))
                return HeartOfThorns;

            if (PathOfFireSpecs.Contains(eliteSlotByte))
                return PathOfFire;

            if (EndOfDragonsSpecs.Contains(eliteSlotByte))
                return EndOfDragons;

            return Core;
        }

        /// <summary>
        /// Generates a profession enum based on the provided profession and set bytes.
        /// </summary>
        /// <param name="professionByte">The profession byte.</param>
        /// <param name="setByte">The set byte.</param>
        /// <returns></returns>
        public static Profession FromBytes(in byte professionByte, in byte setByte)
        {
            try
            {
                return (Profession)((setByte << 4) | professionByte);
            }
            catch
            {
                return Profession.None;
            }
        }

        /// <summary>
        /// Gets whether the profession is based off of the check provided.
        /// </summary>
        /// <param name="profession">The profession.</param>
        /// <param name="check">The base profession to check with.</param>
        /// <returns><c>True</c> if the profession is based off of the one provided, otherwise <c>false</c>.</returns>
        public static bool IsBasedOn(this Profession profession, in Profession check)
        {
            return ((byte)profession & 0b00001111) == (byte)check;
        }

        /// <summary>
        /// Writes a file contain the currently available specialisations for all classes.
        /// </summary>
        public static async void WriteSpecialisationsFile()
        {
            using (HttpClient client = new HttpClient())
            {
                // Get the list of specialisation ids
                string specIds = string.Join(",",
                    JArray.Parse(await client.GetStringAsync("https://api.guildwars2.com/v2/specializations")));

                // Get the specialisation info
                JArray specs = JArray.Parse(await client.GetStringAsync($"https://api.guildwars2.com/v2/specializations?ids={specIds}"));

                using (StreamWriter file = new StreamWriter(Path.Combine(App.BaseDirectory, "Specialization.txt"), false))
                {
                    WriteSpecialisations("Guardian", specs, file);
                    WriteSpecialisations("Warrior", specs, file);
                    WriteSpecialisations("Engineer", specs, file);
                    WriteSpecialisations("Ranger", specs, file);
                    WriteSpecialisations("Thief", specs, file);
                    WriteSpecialisations("Elementalist", specs, file);
                    WriteSpecialisations("Mesmer", specs, file);
                    WriteSpecialisations("Necromancer", specs, file);
                    WriteSpecialisations("Revenant", specs, file);
                }
            }
        }

        /// <summary>
        /// Writes the specialisation enum values with the given <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="professionName">The name of the profession to write about.</param>
        /// <param name="specs">The collection of specialisation JSON objects.</param>
        /// <param name="file">The <see cref="StreamWriter"/> to write to.</param>
        private static void WriteSpecialisations(string professionName, JArray specs, StreamWriter file)
        {
            file.WriteLine($"// {professionName}");
            foreach (JToken spec in specs.Where(s => s["profession"].ToString() == professionName))
            {
                string name = spec["name"].ToString().Replace(" ", "");
                file.WriteLine($"{name} = {spec["id"]},");
            }
            file.WriteLine();
        }
    }
}
