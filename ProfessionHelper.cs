using System.Collections.Generic;

namespace GW2BuildLibrary
{
    public static class ProfessionHelper
    {
        public const byte HeartOfThorns = 0b0001;
        public const byte PathOfFire = 0b0010;
        public const byte IcebroodSaga = 0b0011;

        private static readonly HashSet<byte> HeartOfThornsSpecs = new HashSet<byte>()
        { 5, 7, 18, 27, 34, 40, 43, 48, 52 };

        private static readonly HashSet<byte> PathOfFireSpecs = new HashSet<byte>()
        { 55, 56, 57, 58, 59, 60, 61, 62, 63 };

        private static readonly HashSet<byte> IcebroodSagaSpecs = new HashSet<byte>()
        { };

        /// <summary>
        /// Gets the byte that represents the set of elite specs required
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

            if (IcebroodSagaSpecs.Contains(eliteSlotByte))
                return IcebroodSaga;

            return 0; // 0 is core
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
    }
}
