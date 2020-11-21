namespace GW2BuildLibrary
{
    /// <summary>
    /// The available professions.
    /// </summary>
    public enum Profession : byte
    {
        None = 0,

        Guardian = 1,
        Dragonhunter = (TemplateHelper.HeartOfThorns << 4) | Guardian,
        Firebrand = (TemplateHelper.PathOfFire << 4) | Guardian,
        Guardian3 = (TemplateHelper.EndOfDragons << 4) | Guardian,

        Warrior = 2,
        Berserker = (TemplateHelper.HeartOfThorns << 4) | Warrior,
        Spellbreaker = (TemplateHelper.PathOfFire << 4) | Warrior,
        Warrior3 = (TemplateHelper.EndOfDragons << 4) | Warrior,

        Engineer = 3,
        Scrapper = (TemplateHelper.HeartOfThorns << 4) | Engineer,
        Holosmith = (TemplateHelper.PathOfFire << 4) | Engineer,
        Engineer3 = (TemplateHelper.EndOfDragons << 4) | Engineer,

        Ranger = 4,
        Druid = (TemplateHelper.HeartOfThorns << 4) | Ranger,
        Soulbeast = (TemplateHelper.PathOfFire << 4) | Ranger,
        Ranger3 = (TemplateHelper.EndOfDragons << 4) | Ranger,

        Thief = 5,
        Daredevil = (TemplateHelper.HeartOfThorns << 4) | Thief,
        Deadeye = (TemplateHelper.PathOfFire << 4) | Thief,
        Thief3 = (TemplateHelper.EndOfDragons << 4) | Thief,

        Elementalist = 6,
        Tempest = (TemplateHelper.HeartOfThorns << 4) | Elementalist,
        Weaver = (TemplateHelper.PathOfFire << 4) | Elementalist,
        Elementalist3 = (TemplateHelper.EndOfDragons << 4) | Elementalist,

        Mesmer = 7,
        Chronomancer = (TemplateHelper.HeartOfThorns << 4) | Mesmer,
        Mirage = (TemplateHelper.PathOfFire << 4) | Mesmer,
        Mesmer3 = (TemplateHelper.EndOfDragons << 4) | Mesmer,

        Necromancer = 8,
        Reaper = (TemplateHelper.HeartOfThorns << 4) | Necromancer,
        Scourge = (TemplateHelper.PathOfFire << 4) | Necromancer,
        Necromancer3 = (TemplateHelper.EndOfDragons << 4) | Necromancer,

        Revenant = 9,
        Herald = (TemplateHelper.HeartOfThorns << 4) | Revenant,
        Renegade = (TemplateHelper.PathOfFire << 4) | Revenant,
        Revenant3 = (TemplateHelper.EndOfDragons << 4) | Revenant
    }
}
