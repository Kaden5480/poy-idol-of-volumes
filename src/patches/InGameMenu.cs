using HarmonyLib;
using UnityEngine;

namespace IdolOfVolumes.Patches {
    /**
     * <summary>
     * Ensures the idol in the UI is enabled if it's in use.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "CheckIdols")]
    static class CheckIdols {
        static void Postfix(InGameMenu __instance) {
            if (PlayerPrefs.HasKey("HUDSetting") && PlayerPrefs.GetInt("HUDSetting") == 0) {
                return;
            }

            __instance.ui_idol_seeds.SetActive(Cfg.idolInUse);
        }
    }

    /**
     * <summary>
     * Ensures the idol in the UI is enabled if it's in use.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "CheckYFYD")]
    static class CheckYFYD {
        static void Postfix(InGameMenu __instance) {
            if (PlayerPrefs.HasKey("HUDSetting") && PlayerPrefs.GetInt("HUDSetting") == 0) {
                return;
            }

            __instance.ui_idol_seeds.SetActive(Cfg.idolInUse);
        }
    }
}
