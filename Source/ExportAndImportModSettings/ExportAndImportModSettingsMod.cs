using System.IO;
using Mlie;
using UnityEngine;
using Verse;

namespace ExportAndImportModSettings;

[StaticConstructorOnStartup]
internal class ExportAndImportModSettingsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static ExportAndImportModSettingsMod instance;

    private static string currentVersion;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public ExportAndImportModSettingsMod(ModContentPack content) : base(content)
    {
        instance = this;
        Settings = GetSettings<ExportAndImportModSettingsSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal ExportAndImportModSettingsSettings Settings { get; }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Export And Import Mod Settings";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);

        listing_Standard.Label("EIMS.SaveLocation".Translate(), -1,
            "EIMS.SaveLocationTT".Translate());
        Settings.SaveLocation = listing_Standard.TextEntry(Settings.SaveLocation);

        if (!Directory.Exists(Settings.SaveLocation))
        {
            listing_Standard.Label("EIMS.SaveLocationMissing".Translate());
        }

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("EIMS.modVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public override void WriteSettings()
    {
        if (!Directory.Exists(Settings.SaveLocation))
        {
            Settings.SaveLocation = ExportAndImportModSettings.GetDefaultSaveLocation();
        }

        base.WriteSettings();
    }
}