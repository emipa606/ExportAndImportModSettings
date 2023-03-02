using System.IO;
using System.Linq;
using Mlie;
using RimWorld;
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

        listing_Standard.Gap();
        if (listing_Standard.ButtonText("EIMS.ExportAllActive".Translate()))
        {
            var settingsExported = 0;
            foreach (var mod in LoadedModManager.ModHandles)
            {
                var modFolderName = mod.Content.FolderName;
                var modTypeName = mod.GetType().Name;
                var settingsFilename = LoadedModManager.GetSettingsFilename(modFolderName, modTypeName);
                if (!File.Exists(settingsFilename))
                {
                    continue;
                }

                var exportFilename = Path.Combine(instance.Settings.SaveLocation,
                    GenText.SanitizeFilename($"Mod_{modFolderName}_{modTypeName}.xml"));
                File.Copy(settingsFilename, exportFilename, true);
                settingsExported++;
            }

            Messages.Message("EIMS.ExportedAllSettings".Translate(settingsExported, instance.Settings.SaveLocation),
                MessageTypeDefOf.TaskCompletion,
                false);
        }

        listing_Standard.Gap();
        if (listing_Standard.ButtonText("EIMS.ImportAllActive".Translate()))
        {
            Find.WindowStack.Add(new Dialog_MessageBox(
                "EIMS.ImportAllVerify".Translate(instance.Settings.SaveLocation),
                "No".Translate(), null, "Yes".Translate(),
                delegate
                {
                    var settingsImported = 0;
                    var settingsSkipped = 0;
                    var modHandles = LoadedModManager.ModHandles.ToList();
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < modHandles.Count; i++)
                    {
                        var mod = modHandles[i];
                        var modFolderName = mod.Content.FolderName;
                        var modTypeName = mod.GetType().Name;
                        var importFileName = Path.Combine(instance.Settings.SaveLocation,
                            GenText.SanitizeFilename($"Mod_{modFolderName}_{modTypeName}.xml"));
                        if (!File.Exists(importFileName))
                        {
                            continue;
                        }

                        var settingsFilename = LoadedModManager.GetSettingsFilename(modFolderName, modTypeName);

                        if (File.Exists(settingsFilename) && AreFilesTheSame(importFileName, settingsFilename))
                        {
                            settingsSkipped++;
                            continue;
                        }

                        File.Copy(importFileName, settingsFilename, true);

                        var modType = mod.GetType();
                        LoadedModManager.runningModClasses.Remove(LoadedModManager.runningModClasses
                            .First(pair => modType == pair.Key).Key);
                        settingsImported++;
                    }

                    if (settingsImported > 0)
                    {
                        LoadedModManager.CreateModClasses();
                    }

                    Messages.Message(
                        "EIMS.ImportedAllSettings".Translate(settingsImported, settingsSkipped,
                            instance.Settings.SaveLocation),
                        MessageTypeDefOf.TaskCompletion,
                        false);
                }));
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

    private bool AreFilesTheSame(string firstFilePath, string secondFilePath)
    {
        if (!File.Exists(firstFilePath) || !File.Exists(secondFilePath))
        {
            return false;
        }

        return File.ReadLines(firstFilePath).SequenceEqual(File.ReadLines(secondFilePath));
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