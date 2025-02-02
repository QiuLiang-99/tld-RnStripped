using System.Reflection;
using ModSettings;

namespace ttrRnStripped
{
    internal class RnSettings : JsonModSettings
    {
        [Section(" ")]

        [Name("Carcass Moving")]
        [Description("Add ability to haul medium sized kills such as deer and wolves around the world map, including indoors.")]
        public bool carcassMovingEnabled = true;

        [Name("Electric Torch Lighting")]
        [Description("Add ability to light torches using live wires and household outlets during auroras.\n\nUntested - let me know if this still works.")]
        public bool electricTorchLightingEnabled = true;
    }
    internal static class Settings
    {
        public static RnSettings options;
        public static void OnLoad()
        {
            options = new RnSettings();
            options.AddToModSettings("RnStriped Settings");
        }
    }
    
}