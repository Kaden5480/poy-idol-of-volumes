#if BEPINEX
using BepInEx.Configuration;

#elif MELONLOADER
using MelonLoader;

#endif

namespace IdolOfVolumes {
    public class Cfg {
        public static bool idolInUse = false;

#if BEPINEX
        public static ConfigEntry<bool> _idolInUse;

#elif MELONLOADER
        public static MelonPreferences_Entry<bool> _idolInUse;

#endif

        /**
         * <summary>
         * Initializes injected fields.
         * </summary>
         */
        public static void Init() {
            idolInUse = _idolInUse.Value;
        }

        /**
         * <summary>
         * Updates the config based upon the injected fields
         * every frame.
         * </summary>
         */
        public static void Update() {
            _idolInUse.Value = idolInUse;
        }
    }
}
