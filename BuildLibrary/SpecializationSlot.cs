namespace GW2BuildLibrary
{
    /// <summary>
    /// Represents a specialization slot. Containing information about the selected specialization and traits.
    /// </summary>
    public class SpecializationSlot
    {
        /// <summary>
        /// The specialization in this slot.
        /// </summary>
        public Specialization Specialization
        { get; set; }

        /// <summary>
        /// The first trait choice.
        /// </summary>
        public byte Trait1
        { get; set; }

        /// <summary>
        /// The second trait choice.
        /// </summary>
        public byte Trait2
        { get; set; }

        /// <summary>
        /// The third trait choice.
        /// </summary>
        public byte Trait3
        { get; set; }

        /// <summary>
        /// Loads this <see cref="SpecializationSlot"/> from the provided bytes.
        /// </summary>
        /// <param name="specializationByte">The byte for the specialization.</param>
        /// <param name="traitsByte">The byte for the trait choices.</param>
        /// <remarks>
        /// See: https://wiki.guildwars2.com/wiki/Chat_link_format#Build_templates_link
        /// for details on how the data is loaded.
        /// </remarks>
        public void LoadFromBytes(in byte specializationByte, in byte traitsByte)
        {
            Specialization = (Specialization)specializationByte;
            Trait1 = (byte)(traitsByte & 0b00000011);
            Trait2 = (byte)((traitsByte & 0b00001100) >> 2);
            Trait3 = (byte)((traitsByte & 0b00110000) >> 4);
        }

        /// <summary>
        /// Gets a <see cref="string"/> representation of this <see cref="SpecializationSlot"/> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this <see cref="SpecializationSlot"/> instance.
        /// </returns>
        public override string ToString() => $"{Specialization} - {Trait1}|{Trait2}|{Trait3}";
    }
}
