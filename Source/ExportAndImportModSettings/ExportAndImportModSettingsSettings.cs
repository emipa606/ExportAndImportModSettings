using Verse;

namespace ExportAndImportModSettings;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class ExportAndImportModSettingsSettings : ModSettings
{
    public string SaveLocation = ExportAndImportModSettings.GetDefaultSaveLocation();

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref SaveLocation, "SaveLocation", ExportAndImportModSettings.GetDefaultSaveLocation());
    }
}