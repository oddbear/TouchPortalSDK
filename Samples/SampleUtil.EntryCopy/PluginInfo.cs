using System;
using System.IO;
using System.Reflection;

namespace SampleUtils
{
    public static class PluginInfo
    {
        public static string AppDataRoaming { get; }
        public static string TpPluginsDirectory { get; }
        public static string AssemblyName { get; }
        public static string PluginDirectory { get; }
        public static string EntryTpPath { get; }

        static PluginInfo()
        {
            AppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TpPluginsDirectory = Path.Combine(AppDataRoaming, "TouchPortal\\plugins");

            AssemblyName = Assembly.GetEntryAssembly()?.GetName().Name!;
            PluginDirectory = Path.Combine(TpPluginsDirectory, AssemblyName);
            EntryTpPath = Path.Combine(PluginDirectory, "entry.tp");
        }
    }
}