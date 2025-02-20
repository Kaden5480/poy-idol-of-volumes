using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX
using BepInEx;

namespace IdolOfVolumes {
    [BepInPlugin("com.github.Kaden5480.poy-idol-of-volumes", "IdolOfVolumes", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        /**
         * <summary>
         * Executes when this plugin is initialized.
         * </summary>
         */
        public void Awake() {
            Cfg._idolInUse = Config.Bind(
                "General", "idolInUse", false,
                "Whether the idol is in use"
            );
            Cfg.Init();

            SceneManager.sceneLoaded += OnSceneLoaded;

            Harmony.CreateAndPatchAll(typeof(Patches.CabinDescriptionUpdate));
            Harmony.CreateAndPatchAll(typeof(Patches.CabinDescriptionCabin));
            Harmony.CreateAndPatchAll(typeof(Patches.CabinDescriptionCabin4));
            Harmony.CreateAndPatchAll(typeof(Patches.CheckIdols));
            Harmony.CreateAndPatchAll(typeof(Patches.CheckYFYD));
        }

        /**
         * <summary>
         * Executes when this plugin is destroyed.
         * </summary>
         */
        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /**
         * <summary>
         * Executes when a scene is loaded.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         * <param name="mode">The mode the scene was loaded with</param>
         */
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            CommonSceneLoad();
        }

#elif MELONLOADER
using MelonLoader;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(IdolOfVolumes.Plugin), "IdolOfVolumes", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace IdolOfVolumes {
    public class Plugin: MelonMod {
        /**
         * <summary>
         * Executes when this mod is initialized.
         * </summary>
         */
        public override void OnInitializeMelon() {
            string filePath = $"{MelonEnvironment.UserDataDirectory}/com.github.Kaden5480.poy-idol-of-volumes.cfg";
            MelonPreferences_Category general = MelonPreferences.CreateCategory("IdolOfVolumes_General");
            general.SetFilePath(filePath);

            Cfg._idolInUse = general.CreateEntry<bool>("idolInUse", false);
            Cfg.Init();
        }

        /**
         * <summary>
         * Executes when a scene is loaded.
         * </summary>
         * <param name="buildIndex">The build index of the scene</param>
         * <param name="sceneName">The name of the scene</param>
         */
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
            CommonSceneLoad();
        }

#endif

        /**
         * <summary>
         * Executes when a scene is loaded.
         * </summary>
         */
        private void CommonSceneLoad() {
            if (Cfg.idolInUse == false) {
                return;
            }

            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
                if (obj.name.StartsWith("ClimbableSloper") == true) {
                    continue;
                }

                if ("Climbable".Equals(obj.tag) == true) {
                    obj.tag = "Volume";
                }
                else if ("ClimbableMicroHold".Equals(obj.tag)) {
                    obj.name = "Volume";
                    obj.tag = "Volume";
                }
            }
        }

    }
}
