using System;
using System.Collections.Generic;
using System.Reflection;
using AssetsAdvancedEditor.Assets;
using UnityTools;

namespace AssetsAdvancedEditor.Plugins
{
    public class PluginManager
    {
        private AssetsManager Am { get; }
        private List<PluginInfo> LoadedPlugins { get; }

        public PluginManager(AssetsManager am)
        {
            LoadedPlugins = new List<PluginInfo>();
            Am = am;
        }

        public bool LoadPluginsLibrary(string path)
        {
            try
            {
                var asm = Assembly.LoadFrom(path);
                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(IPlugin).IsAssignableFrom(type)) continue;
                    var typeInst = Activator.CreateInstance(type);
                    if (typeInst == null)
                        return false;

                    var plugInst = (IPlugin)typeInst;
                    var plugInf = plugInst.Init();
                    LoadedPlugins.Add(plugInf);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<PluginMenuInfo> GetSupportedPlugins(List<AssetItem> selectedItems)
        {
            var menuInfos = new List<PluginMenuInfo>();
            foreach (var pluginInf in LoadedPlugins)
            {
                foreach (var option in pluginInf.Options)
                {
                    var supported = option.IsValidForPlugin(Am, selectedItems);
                    if (!supported) continue;
                    var menuInf = new PluginMenuInfo(pluginInf, option);
                    menuInfos.Add(menuInf);
                }
            }
            return menuInfos;
        }
    }
}
