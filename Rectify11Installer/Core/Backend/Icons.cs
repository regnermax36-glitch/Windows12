using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using static System.Environment;

namespace Rectify11Installer.Core
{
    internal class Icons
    {
        #region Variables
        private enum PatchType
        {
            General = 0,
            Mui,
            Troubleshooter,
            x86
        }
        #endregion

        public static bool Install(FrmWizard frm)
        {
            try
            {
                Logger.WriteLine("Installing icons");
                Logger.WriteLine("────────────────");

                var patches = PatchesParser.GetAll();
                var patch = patches.Items;
                decimal progress = 0;
                List<string> fileList = new();
                List<string> x86List = new();
                for (var i = 0; i < patch.Length; i++)
                {
                    for (var j = 0; j < InstallOptions.iconsList.Count; j++)
                    {
                        if (patch[i].Mui.Contains(InstallOptions.iconsList[j]))
                        {
                            var number = Math.Round((progress / InstallOptions.iconsList.Count) * 100m);
                            frm.InstallerProgress = "Patching " + patch[i].Mui + " (" + number + "%)";
                            if (!MatchAndApplyRule(patch[i]))
                            {
                                Logger.Warn("MatchAndApplyRule() on " + patch[i].Mui + " failed");
                            }
                            else
                            {
                                fileList.Add(patch[i].HardlinkTarget);
                                if (!string.IsNullOrWhiteSpace(patch[i].x86))
                                {
                                    x86List.Add(patch[i].HardlinkTarget);
                                }
                            }
                            progress++;
                        }
                    }
                }
                Logger.WriteLine("MatchAndApplyRule() succeeded.");

                if (!WritePendingFiles(fileList, x86List))
                    return false;

                if (!Common.WriteFiles(true, false))
                    return false;

                frm.InstallerProgress = "Replacing files";

                if (InstallOptions.iconsList.Contains("SSText3D.scr"))
                {
                    Helper.ImportReg(Path.Combine(Variables.r11Files, "screensaver.reg"));
                }

                if (InstallOptions.iconsList.Contains("mmc.exe.mui"))
                {
                    if (!MMCHelper.PatchAll())
                        return false;
                }

                if (InstallOptions.iconsList.Contains("odbcad32.exe"))
                    FixOdbc();

                // Try Phase2 first; if missing or fails, fall back to direct/pending replace
                bool phase2Ok = TryRunPhase2("/install");
                if (!phase2Ok)
                {
                    Logger.Warn("Phase2 unavailable or failed — using fallback replacement");
                    FallbackReplacePatchedFiles();
                }

                Helper.ImportReg(Path.Combine(Variables.r11Files, "icons.reg"));

                ClearIconCache();

                Variables.RestartRequired = true;
                Logger.WriteLine("Icons.Install() succeeded.");
                Logger.WriteLine("══════════════════════════════════════════════");
                return true;
            }
            catch (Exception ex)
            {
                Helper.SafeDirectoryDeletion(Path.Combine(Variables.r11Folder, "Tmp"), false);
                Logger.WriteLine("Icons.Install() failed", ex);
                return false;
            }
        }

        private static bool TryRunPhase2(string args)
        {
            try
            {
                var phase2 = Path.Combine(Variables.r11Folder, "Rectify11.Phase2.exe");
                if (!File.Exists(phase2))
                {
                    Logger.Warn("Rectify11.Phase2.exe not found");
                    return false;
                }
                // Skip if it's our placeholder (tiny file)
                var fi = new FileInfo(phase2);
                if (fi.Length < 1024)
                {
                    Logger.Warn("Rectify11.Phase2.exe is a placeholder — skipping");
                    return false;
                }
                Helper.RunAsTI(phase2, args);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Phase2 run failed", ex);
                return false;
            }
        }

        /// <summary>
        /// Fallback: copy patched files from Tmp over the originals.
        /// If a file is locked, schedule replacement on reboot via MoveFileEx.
        /// </summary>
        private static void FallbackReplacePatchedFiles()
        {
            try
            {
                var tmp = Path.Combine(Variables.r11Folder, "Tmp");
                if (!Directory.Exists(tmp)) return;

                foreach (var patched in Directory.GetFiles(tmp, "*", SearchOption.AllDirectories))
                {
                    var rel = Path.GetRelativePath(tmp, patched);
                    // Best-effort: map back to system32 / system32\en-US etc. is complex;
                    // for the common case we place next to original via HardlinkTarget already stored.
                    // Here we simply try to replace any file of the same name under system32.
                    var name = Path.GetFileName(patched);
                    var dest = Path.Combine(Variables.sys32Folder, name);
                    if (!File.Exists(dest)) continue;

                    try
                    {
                        if (!Helper.SafeFileDeletion(dest))
                        {
                            // schedule pending replace
                            var pending = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            File.Copy(patched, pending, true);
                            NativeMethods.MoveFileEx(pending, dest, NativeMethods.MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT | NativeMethods.MoveFileFlags.MOVEFILE_REPLACE_EXISTING);
                            Logger.WriteLine("Scheduled reboot replace for " + name);
                        }
                        else
                        {
                            File.Copy(patched, dest, true);
                            Logger.WriteLine("Replaced " + name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Could not replace " + name, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("FallbackReplacePatchedFiles failed", ex);
            }
        }

        public static bool Uninstall()
        {
            try
            {
                Common.WriteFiles(true, false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true)
                    ?.CreateSubKey("Rectify11", true)
                    ?.SetValue("UninstallFiles", UninstallOptions.uninstIconsList.ToArray());

                if (!Variables.Phase2Skip)
                {
                    Logger.WriteLine("Executed Rectify11.Phase2.exe");
                    TryRunPhase2("/uninstall");
                }

                Helper.SafeFileDeletion(Path.Combine(Variables.r11Folder, "Rectify11.Phase2.exe"));
                Helper.SafeFileDeletion(Path.Combine(Variables.r11Folder, "NSudoL.exe"));

                Logger.WriteLine("Icons.Uninstall() succeeded.");
                Logger.WriteLine("══════════════════════════════════════════════");
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Icons.Uninstall() failed", ex);
                return false;
            }
        }

        private static bool FixOdbc()
        {
            try
            {
                var filename = string.Empty;
                var admintools = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "Administrative Tools");
                if (!Directory.Exists(admintools)) return false;

                var files = Directory.GetFiles(admintools);
                for (var i = 0; i < files.Length; i++)
                {
                    if (!Path.GetFileName(files[i]).Contains("ODBC") ||
                        !Path.GetFileName(files[i])!.Contains("32")) continue;
                    filename = Path.GetFileName(files[i]);
                    File.Delete(files[i]);
                }
                using ShellLink shortcut = new();
                shortcut.Target = Path.Combine(Variables.sysWOWFolder, "odbcad32.exe");
                shortcut.WorkingDirectory = @"%windir%\system32";
                shortcut.IconPath = Path.Combine(Variables.sys32Folder, "odbcint.dll");
                shortcut.IconIndex = 0;
                shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
                if (filename != null) shortcut.Save(Path.Combine(admintools, filename));

                Logger.WriteLine("FixOdbc() succeeded.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("FixOdbc() failed", ex);
                return false;
            }
        }

        private static bool WritePendingFiles(List<string> fileList, List<string> x86List)
        {
            using var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true)?.CreateSubKey("Rectify11", true);
            if (reg == null) return false;
            try
            {
                SafeWriteReg(reg, "PendingFiles", fileList.ToArray(), true);
                if (x86List.Count != 0)
                    SafeWriteReg(reg, "x86PendingFiles", x86List.ToArray(), true);

                SafeWriteReg(reg, "Language", CultureInfo.CurrentUICulture.Name, false);
                SafeWriteReg(reg, "Version", Assembly.GetEntryAssembly()?.GetName().Version, false);
                SafeWriteReg(reg, "WindowsUpdate", Variables.WindowsUpdate ? 1 : 0, false);
                using var ubrReg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
                string build = OSVersion.Version.Major + "." + OSVersion.Version.Minor + "." + OSVersion.Version.Build + "." + ubrReg.GetValue("UBR").ToString();
                SafeWriteReg(reg, "OSVersion", build, false);

                Logger.WriteLine("WritePendingFiles() succeeded.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("WritePendingFiles() failed", ex);
                return false;
            }
        }

        private static bool Patch(string file, PatchesPatch patch, PatchType type)
        {
            if (!File.Exists(file)) return false;
            string name;
            string backupfolder;
            string tempfolder;
            if (type == PatchType.Troubleshooter)
            {
                name = patch.Mui.Replace("Troubleshooter: ", "DiagPackage") + ".dll";
                backupfolder = Path.Combine(Variables.r11Folder, "backup", "Diag");
                tempfolder = Path.Combine(Variables.r11Folder, "Tmp", "Diag");
            }
            else if (type == PatchType.x86)
            {
                var ext = Path.GetExtension(patch.Mui);
                name = Path.GetFileNameWithoutExtension(patch.Mui) + "86" + ext;
                backupfolder = Path.Combine(Variables.r11Folder, "backup");
                tempfolder = Path.Combine(Variables.r11Folder, "Tmp");
            }
            else
            {
                name = patch.Mui;
                backupfolder = Path.Combine(Variables.r11Folder, "backup");
                tempfolder = Path.Combine(Variables.r11Folder, "Tmp");
            }

            if (string.IsNullOrWhiteSpace(name)) return false;

            if (type == PatchType.Troubleshooter)
            {
                Directory.CreateDirectory(backupfolder);
                Directory.CreateDirectory(tempfolder);
            }

            File.Copy(file, Path.Combine(tempfolder, name), true);

            var filename = name + ".res";
            var masks = patch.mask;
            string filepath;
            if (type == PatchType.Troubleshooter)
                filepath = Path.Combine(Variables.r11Files, "Diag");
            else
                filepath = Variables.r11Files;

            if (patch.mask.Contains("|"))
            {
                if (!string.IsNullOrWhiteSpace(patch.Ignore)
                    && ((!string.IsNullOrWhiteSpace(patch.MinVersion) && OSVersion.Version.Build <= int.Parse(patch.MinVersion)) || (!string.IsNullOrWhiteSpace(patch.MaxVersion) && OSVersion.Version.Build >= int.Parse(patch.MaxVersion))))
                {
                    masks = masks.Replace(patch.Ignore, "");
                }
                var str = masks.Split('|');
                for (var i = 0; i < str.Length; i++)
                {
                    PatchCmd(type, filename, name, tempfolder, filepath, str[i]);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(patch.Ignore) && ((!string.IsNullOrWhiteSpace(patch.MinVersion) && OSVersion.Version.Build <= int.Parse(patch.MinVersion)) || (!string.IsNullOrWhiteSpace(patch.MaxVersion) && OSVersion.Version.Build >= int.Parse(patch.MaxVersion))))
                {
                    masks = masks.Replace(patch.Ignore, "");
                }
                PatchCmd(type, filename, name, tempfolder, filepath, masks);
            }
            return true;
        }

        private static bool MatchAndApplyRule(PatchesPatch patch)
        {
            string newhardlink = Helper.FixString(patch.HardlinkTarget, false);
            if (newhardlink.Contains(".mui"))
            {
                if (!Patch(newhardlink, patch, PatchType.Mui)) return false;
            }
            else if (patch.HardlinkTarget.Contains("%diag%"))
            {
                if (!Patch(newhardlink, patch, PatchType.Troubleshooter)) return false;
            }
            else
            {
                if (!Patch(newhardlink, patch, PatchType.General)) return false;
            }

            if (!string.IsNullOrWhiteSpace(patch.x86))
            {
                newhardlink = Helper.FixString(patch.HardlinkTarget, true);
                if (!Patch(newhardlink, patch, PatchType.x86)) return false;
            }
            return true;
        }

        private static void ClearIconCache()
        {
            try
            {
                DirectoryInfo di = new(Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), "microsoft", "windows", "explorer"));
                var files = di.GetFiles("*.db");

                for (var i = 0; i < files.Length; i++)
                {
                    files[i].Attributes = FileAttributes.Normal;
                    if (File.Exists(files[i].FullName))
                    {
                        Helper.SafeFileDeletion(files[i].FullName);
                    }
                }
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", true);
                key?.SetValue("ResetIconCache", Path.Combine(Variables.sys32Folder, "ie4uinit.exe") + " -show", RegistryValueKind.String);
                key.Close();
            }
            catch (Exception ex)
            {
                Logger.Warn("Clearing icon cache failed", ex);
            }
        }

        private static void PatchCmd(PatchType type, string filename, string name, string tempfolder, string filepath, string mask)
        {
            if (type == PatchType.x86)
            {
                filename = Path.GetFileNameWithoutExtension(name).Remove(Path.GetFileNameWithoutExtension(name).Length - 2, 2) + Path.GetExtension(name) + ".res";
            }
            if (type != PatchType.Mui)
            {
                Interaction.Shell(Path.Combine(Variables.r11Folder, "ResourceHacker.exe") +
                " -open " + Path.Combine(tempfolder, name) +
                " -save " + Path.Combine(tempfolder, name) +
                " -action " + "delete" +
                " -mask " + mask, AppWinStyle.Hide, true);
            }
            Interaction.Shell(Path.Combine(Variables.r11Folder, "ResourceHacker.exe") +
            " -open " + Path.Combine(tempfolder, name) +
            " -save " + Path.Combine(tempfolder, name) +
            " -action " + "addskip" +
            " -resource " + Path.Combine(filepath, filename) +
            " -mask " + mask, AppWinStyle.Hide, true);
        }

        private static bool SafeWriteReg(RegistryKey key, string name, object value, bool hardError)
        {
            try
            {
                key?.SetValue(name, value);
                Logger.WriteLine("Wrote to " + name);
                return true;
            }
            catch (Exception ex)
            {
                if (hardError)
                {
                    Logger.WriteLine("Error writing to " + name, ex);
                    throw new Exception("error");
                }
                else
                {
                    Logger.Warn("Error writing to " + name, ex);
                    return true;
                }
            }
        }
    }
}
