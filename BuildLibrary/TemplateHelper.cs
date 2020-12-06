using System.Collections.Generic;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Helpful methods for <see cref="Profession"/> and <see cref="Specialization"/> bit-twiddling.
    /// </summary>
    public static class TemplateHelper
    {
        #region Fields

        private static readonly HashSet<Specialization> EndOfDragonsSpecs = new HashSet<Specialization>();

        private static readonly HashSet<Specialization> HeartOfThornsSpecs = new HashSet<Specialization>()
        { Specialization.Dragonhunter, Specialization.Berserker, Specialization.Scrapper, Specialization.Druid,
            Specialization.Daredevil, Specialization.Tempest, Specialization.Chronomancer, Specialization.Reaper,
            Specialization.Herald };

        private static readonly HashSet<Specialization> PathOfFireSpecs = new HashSet<Specialization>()
        { Specialization.Firebrand, Specialization.Spellbreaker, Specialization.Holosmith, Specialization.Soulbeast,
            Specialization.Deadeye, Specialization.Weaver, Specialization.Mirage, Specialization.Scourge,
            Specialization.Renegade };

        public const byte Core = 0b0000;
        public const byte EndOfDragons = 0b0011;
        public const byte HeartOfThorns = 0b0001;
        public const byte PathOfFire = 0b0010;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Generates a profession enum based on the provided profession byte and third specialization.
        /// </summary>
        /// <param name="professionByte">The profession byte.</param>
        /// <param name="specialization">The third specialization.</param>
        /// <returns></returns>
        public static Profession GetProfessionFromBytes(in byte professionByte, in Specialization specialization) =>
            GetProfessionFromBytes(professionByte, GetSetByte(specialization));

        /// <summary>
        /// Generates a profession enum based on the provided profession and set bytes.
        /// </summary>
        /// <param name="professionByte">The profession byte.</param>
        /// <param name="setByte">The set byte.</param>
        /// <returns></returns>
        public static Profession GetProfessionFromBytes(in byte professionByte, in byte setByte)
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
        /// Gets the byte that represents the set of elite specializations required for the specializations provided.
        /// </summary>
        /// <param name="specialization">The third specialization.</param>
        /// <returns></returns>
        public static byte GetSetByte(in Specialization specialization)
        {
            if (HeartOfThornsSpecs.Contains(specialization))
                return HeartOfThorns;

            if (PathOfFireSpecs.Contains(specialization))
                return PathOfFire;

            if (EndOfDragonsSpecs.Contains(specialization))
                return EndOfDragons;

            return Core;
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

        #endregion Methods
    }
}