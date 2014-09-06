using System;
using UnityEngine;

namespace UbioWeldingLtd
{
    public static class FileManager
    {
        private static KSP.IO.PluginConfiguration configFile;


        /// <summary>
        /// reads the config from the configfile and forwards them to the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PluginConfiguration loadConfig(PluginConfiguration configuration)
        {
            if (configFile == null)
            {
                configFile = KSP.IO.PluginConfiguration.CreateForType<PluginConfiguration>();
            }
            configFile.load();
            configuration.editorButtonX = configFile.GetValue<int>(Constants.settingEdiButX);
            configuration.editorButtonY = configFile.GetValue<int>(Constants.settingEdiButY);
            configuration.dataBaseAutoReload = configFile.GetValue<bool>(Constants.settingDbAutoReload);
            configuration.includeAllNodes = configFile.GetValue<bool>(Constants.settingAllNodes);
            configuration.allowCareerMode = configFile.GetValue<bool>(Constants.settingAllowCareer);
            configuration.dontProcessMasslessParts = configFile.GetValue<bool>(Constants.settingDontProcessMasslessParts);
            configuration.runInTestMode = configFile.GetValue<bool>(Constants.settingRunInTestMode);
            configuration.useStockToolbar = configFile.GetValue<bool>(Constants.settingUseStockToolbar);
            return configuration;
        }


        /// <summary>
        /// writes the configfile based on the values from the input
        /// </summary>
        /// <param name="input"></param>
        public static void saveConfig(PluginConfiguration configuration)
        {
            if (configFile == null)
            {
                configFile = KSP.IO.PluginConfiguration.CreateForType<PluginConfiguration>();
            }
            configFile.SetValue(Constants.settingEdiButX, configuration.editorButtonX);
            configFile.SetValue(Constants.settingEdiButY, configuration.editorButtonX);
            configFile.SetValue(Constants.settingDbAutoReload, configuration.dataBaseAutoReload);
            configFile.SetValue(Constants.settingAllNodes, configuration.includeAllNodes);
            configFile.SetValue(Constants.settingAllowCareer, configuration.allowCareerMode);
            configFile.SetValue(Constants.settingDontProcessMasslessParts, configuration.dontProcessMasslessParts);
            configFile.SetValue(Constants.settingRunInTestMode, configuration.runInTestMode);
            configFile.SetValue(Constants.settingUseStockToolbar, configuration.useStockToolbar);
            configFile.save();
        }
    }
}
