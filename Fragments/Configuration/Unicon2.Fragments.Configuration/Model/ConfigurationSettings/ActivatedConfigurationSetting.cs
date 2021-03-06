﻿using System.Threading.Tasks;
using Newtonsoft.Json;
using Unicon2.Fragments.Configuration.Infrastructure.Keys;
using Unicon2.Fragments.Configuration.Infrastructure.StructItemsInterfaces;
using Unicon2.Infrastructure.Connection;
using Unicon2.Infrastructure.FragmentInterfaces.FagmentSettings;
using Unicon2.Infrastructure.FragmentInterfaces.FagmentSettings.QuickMemoryAccess;

namespace Unicon2.Fragments.Configuration.Model.ConfigurationSettings
{
    [JsonObject(MemberSerialization.OptIn)]

    public class ActivatedConfigurationSetting : IActivatedSetting
    {
        [JsonProperty] public ushort ActivationAddress { get; set; }

        [JsonProperty] public bool IsSettingEnabled { get; set; }

        public Task<bool> Write()
        {
            return Task.FromResult(false);
        }

        public string StrongName => ConfigurationKeys.Settings.ACTIVATION_CONFIGURATION_SETTING;


        public async Task<bool> ApplySetting(ISettingApplyingContext settingApplyingContext)
        {
            if (IsSettingEnabled &&
                settingApplyingContext is IActivatedSettingApplyingContext activatedSettingApplyingContext)
            {
                IQueryResult queryResult = await activatedSettingApplyingContext.DataProvider.WriteSingleCoilAsync(
                    ActivationAddress, true,
                    ConfigurationKeys.Settings.ACTIVATION_CONFIGURATION_SETTING);
                return queryResult.IsSuccessful;
            }

            return false;
        }

    }
}