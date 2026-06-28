# GitHub Copilot Instructions for RimWorld Mod: Export and Import Mod Settings

## Overview
### Mod Name: Export and Import Mod Settings
**Author:** Mlie  
**PackageId:** Mlie.ExportAndImportModSettings  

The "Export and Import Mod Settings" mod is designed to address requests from users who wish to create mod-packs with specific pre-configured settings. Instead of implementing this functionality within each individual mod, this mod provides a centralized solution to export and import mod settings universally for all mods (except HugsLib).

## Purpose
This mod facilitates the exporting and importing of mod settings for a streamlined user experience when configuring mods within RimWorld's mod-options window. It benefits users who create or use mod-packs by allowing them to easily save and load their desired mod configurations.

## Key Features and Systems
- **Export/Import Buttons:** Adds two buttons at the top of the mod-options window for exporting and importing current mod settings. These settings are saved to a user-specified directory (default: Desktop).
- **Batch Operations:** Users have the capability to export or import settings for all active mods from within the mod's settings window.
- **Persistence and Reload:** Importing settings overwrites current settings and reloads the mod to apply updates. Users may need to restart the game to ensure settings are applied correctly, particularly if warnings/errors are encountered.

## Coding Patterns and Conventions
- **C# Overview:** The project includes 4 C# files, defining 5 types and 10 members. 
- **File Organization:** Each functionality is encapsulated within its respective class, following SOLID principles where possible.
    - `ExportAndImportModSettings.cs`
    - `Dialog_ModSettings_DoWindowContents.cs`
    - `ExportAndImportModSettingsSettings.cs`
    - `ExportAndImportModSettingsMod.cs`
- **Naming Conventions:** Follows C# camelCase for methods and PascalCase for types.
- **Modular Design:** The use of individual classes for handling specific tasks (e.g., settings management) ensures a modular and maintainable codebase.

## XML Integration
- **Absence of XML Files:** This project currently does not utilize XML files for definitions or configurations, focusing instead on C# scripts for programmatic configuration and operation.

## Harmony Patching
- **Harmony Usage:** The mod uses Harmony patches to extend and modify the behavior of RimWorld's existing classes and methods.
- **Example Patch:** The `Dialog_ModSettings_DoWindowContents` class includes a Postfix patch to inject custom logic after the original method execution.

## Suggestions for Copilot
- **Button Placement Logic:** Suggest code snippets that modify or extend the UI elements in RimWorld to better position or style the export/import buttons.
- **Directory Selection:** Assist with improving or altering the directory selection mechanism for exports, utilizing system path dialogues or settings.
- **Error Handling:** Provide suggestions for more robust error handling to catch common or user-specific errors during the import/export process.
- **Performance Optimization:** Copilot can contribute by suggesting runtime optimizations or refactoring opportunities that enhance performance when processing settings.

By adhering to these guidelines and leveraging the existing structure, contributors can effectively enhance the mod while maintaining consistency in coding standards and functionality.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

