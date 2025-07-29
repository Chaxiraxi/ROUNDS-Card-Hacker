using UnityEngine;

namespace ROUNDSCheat
{
    internal static class Settings
    {
        // Aimbot settings
        public static bool AimbotEnabled { get; set; } = Defaults.AimbotEnabled;
        public static KeyCode AimbotToggleKey { get; set; } = Defaults.AimbotToggleKey;
        public static float AimbotPredictionMultiplier { get; set; } = Defaults.AimbotPredictionMultiplier;

        // Autoblock settings
        public static bool AutoblockEnabled { get; set; } = Defaults.AutoblockEnabled;
        public static KeyCode AutoblockToggleKey { get; set; } = Defaults.AutoblockToggleKey;
        public static float AutoblockDistance { get; set; } = Defaults.AutoblockDistance;

        // Stat modifications
        public static bool HealthModEnabled { get; set; } = Defaults.HealthModEnabled;
        public static KeyCode HealthIncreaseKey { get; set; } = Defaults.HealthIncreaseKey;
        public static KeyCode RespawnIncreaseKey { get; set; } = Defaults.RespawnIncreaseKey;
        public static float HealthIncrement { get; set; } = Defaults.HealthIncrement;

        // GUI settings
        public static bool ShowGUI { get; set; } = Defaults.ShowGUI;

        internal static void Reset()
        {
            AimbotEnabled = Defaults.AimbotEnabled;
            AimbotToggleKey = Defaults.AimbotToggleKey;
            AimbotPredictionMultiplier = Defaults.AimbotPredictionMultiplier;
            AutoblockEnabled = Defaults.AutoblockEnabled;
            AutoblockToggleKey = Defaults.AutoblockToggleKey;
            AutoblockDistance = Defaults.AutoblockDistance;
            HealthModEnabled = Defaults.HealthModEnabled;
            HealthIncreaseKey = Defaults.HealthIncreaseKey;
            RespawnIncreaseKey = Defaults.RespawnIncreaseKey;
            HealthIncrement = Defaults.HealthIncrement;
            ShowGUI = Defaults.ShowGUI;
        }

        internal static class Defaults
        {
            public const bool AimbotEnabled = false;
            public const KeyCode AimbotToggleKey = KeyCode.E;
            public const float AimbotPredictionMultiplier = 1.0f;

            public const bool AutoblockEnabled = false;
            public const KeyCode AutoblockToggleKey = KeyCode.Q;
            public const float AutoblockDistance = 1.4f;

            public const bool HealthModEnabled = false;
            public const KeyCode HealthIncreaseKey = KeyCode.C;
            public const KeyCode RespawnIncreaseKey = KeyCode.V;
            public const float HealthIncrement = 50f;

            public const bool ShowGUI = true;
        }
    }
}
