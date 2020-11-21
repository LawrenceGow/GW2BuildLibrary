using System.Collections.Generic;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Helpful methods for <see cref="Profession"/> and <see cref="Specialization"/> bit-twiddling.
    /// </summary>
    public static class TemplateHelper
    {
        #region Profession Methods

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
        /// Gets whether the profession is based off of the check provided.
        /// </summary>
        /// <param name="profession">The profession.</param>
        /// <param name="check">The base profession to check with.</param>
        /// <returns><c>True</c> if the profession is based off of the one provided, otherwise <c>false</c>.</returns>
        public static bool IsBasedOn(this Profession profession, in Profession check)
        {
            return ((byte)profession & 0b00001111) == (byte)check;
        }

        #endregion

        #region Specialisation Methods

        #endregion
    }
}
