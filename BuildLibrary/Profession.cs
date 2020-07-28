namespace GW2BuildLibrary
{
    /// <summary>
    /// The available professions.
    /// </summary>
    public enum Profession : byte
    {
        None = 0,

        Guardian = 0b00000001,
        Dragonhunter = 0b00010001,
        Firebrand = 0b00100001,

        Warrior = 0b00000010,
        Berserker = 0b00010010,
        Spellbreaker = 0b00100010,

        Engineer = 0b00000011,
        Scrapper = 0b00010011,
        Holosmith = 0b00100011,

        Ranger = 0b00000100,
        Druid = 0b00010100,
        Soulbeast = 0b00100100,

        Thief = 0b00000101,
        Daredevil = 0b00010101,
        Deadeye = 0b00100101,

        Elementalist = 0b00000110,
        Tempest = 0b00010110,
        Weaver = 0b00100110,

        Mesmer = 0b00000111,
        Chronomancer = 0b00010111,
        Mirage = 0b00100111,

        Necromancer = 0b00001000,
        Reaper = 0b00011000,
        Scourge = 0b00101000,

        Revenant = 0b00001001,
        Herald = 0b00011001,
        Renegade = 0b00101001,
    }
}
