namespace AppSettingsToAzureWebAppsConfig
{
    public class ApplicationConfigModel
    {
        public string name { get; set; }

        public string value { get; set; }

        public bool slotSetting { get; set; } = false;
    }
}