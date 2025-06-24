using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExportAndImportModSettings;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Dialog_ModSettings), nameof(Dialog_ModSettings.DoWindowContents))]
public static class Dialog_ModSettings_DoWindowContents
{
    public static readonly Texture2D ExportSettings = ContentFinder<Texture2D>.Get("UI/ExportSettings");
    public static readonly Texture2D ImportSettings = ContentFinder<Texture2D>.Get("UI/ImportSettings");
    public static readonly Texture2D ImportSettingsInactive = ContentFinder<Texture2D>.Get("UI/ImportSettingsInactive");

    public static void Postfix(Rect inRect, ref Mod ___mod, ref Dialog_ModSettings __instance)
    {
        var buttonSize = new Vector2(24f, 24f);
        Vector2 basePosition;
        switch (ExportAndImportModSettingsMod.Instance.Settings.ButtonLocation)
        {
            case 1:
                basePosition = new Vector2(inRect.width - 5f - (buttonSize.x * 2), 0f);
                break;
            case 2:
                basePosition = new Vector2(0, inRect.height - buttonSize.y);
                break;
            case 3:
                basePosition = new Vector2(inRect.width - 5f - (buttonSize.x * 2), inRect.height - buttonSize.y);
                break;
            default:
                basePosition = new Vector2(0, 0);
                // add the measured length of the settings category 
                Text.Font = GameFont.Medium;
                basePosition.x += Text.CalcSize(___mod.SettingsCategory()).x + 5f;
                Text.Font = GameFont.Small;
                break;
        }

        var importButtonRect = new Rect(basePosition, buttonSize);
        var exportButtonRect = new Rect(basePosition + new Vector2(buttonSize.x, 0), buttonSize);
        var modFolderName = ___mod.Content.FolderName;
        var modTypeName = ___mod.GetType().Name;
        var fileNameOfSettings = Path.Combine(ExportAndImportModSettingsMod.Instance.Settings.SaveLocation,
            GenText.SanitizeFilename($"Mod_{modFolderName}_{modTypeName}.xml"));

        TooltipHandler.TipRegion(exportButtonRect, "EIMS.ExportTooltip".Translate(fileNameOfSettings));

        if (File.Exists(fileNameOfSettings))
        {
            TooltipHandler.TipRegion(importButtonRect, "EIMS.ImportTooltip".Translate(fileNameOfSettings));
            if (Widgets.ButtonImage(importButtonRect, ImportSettings))
            {
                if (File.Exists(fileNameOfSettings))
                {
                    var instance = __instance;
                    var mod = ___mod;
                    Find.WindowStack.Add(new Dialog_MessageBox(
                        "EIMS.ImportVerify".Translate(fileNameOfSettings),
                        "No".Translate(), null, "Yes".Translate(),
                        delegate
                        {
                            instance.Close();
                            File.Copy(fileNameOfSettings,
                                LoadedModManager.GetSettingsFilename(modFolderName, modTypeName),
                                true);
                            Messages.Message(
                                "EIMS.ImportedSettings".Translate(mod.SettingsCategory(), fileNameOfSettings),
                                MessageTypeDefOf.TaskCompletion, false);
                            var modType = mod.GetType();
                            LoadedModManager.runningModClasses.Remove(LoadedModManager.runningModClasses
                                .First(pair => modType == pair.Key).Key);
                            LoadedModManager.CreateModClasses();
                        }));
                }
                else
                {
                    Messages.Message("EIMS.FailedToFindFile".Translate(___mod.SettingsCategory(), fileNameOfSettings),
                        MessageTypeDefOf.NegativeEvent, false);
                }
            }
        }
        else
        {
            TooltipHandler.TipRegion(importButtonRect, "EIMS.ImportTooltipInactive".Translate(fileNameOfSettings));
            Widgets.DrawAtlas(importButtonRect, ImportSettingsInactive);
        }

        if (!Widgets.ButtonImage(exportButtonRect, ExportSettings))
        {
            return;
        }

        Scribe.saver.InitSaving(fileNameOfSettings, "SettingsBlock");
        try
        {
            Scribe_Deep.Look(ref ___mod.modSettings, "ModSettings");
            Messages.Message("EIMS.ExportedSettings".Translate(___mod.SettingsCategory(), fileNameOfSettings),
                MessageTypeDefOf.TaskCompletion, false);
        }
        catch (Exception exception)
        {
            Messages.Message("EIMS.FailedToExport".Translate(___mod.SettingsCategory(), fileNameOfSettings),
                MessageTypeDefOf.NegativeEvent, false);
            Log.Error(exception.ToString());
        }
        finally
        {
            Scribe.saver.FinalizeSaving();
        }
    }
}