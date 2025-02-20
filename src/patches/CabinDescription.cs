using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

namespace IdolOfVolumes.Patches {
    static class InjectCustom {
        private static FieldInfo volumesInUse = AccessTools.Field(typeof(Cfg), nameof(Cfg.idolInUse));
        private static FieldInfo control = AccessTools.Field(
            typeof(GameManager), nameof(GameManager.control)
        );
        private static FieldInfo seedsInUse = AccessTools.Field(
            typeof(GameManager), nameof(GameManager.alps_statue_seeds_InUse)
        );

        /**
         * <summary>
         * If the provided instruction contains text relating to the idol
         * of seeds, modify it.
         * </summary>
         * <param name="inst">The instruction to modify</param>
         */
        private static void ModifyText(CodeInstruction inst) {
            if (inst.opcode != OpCodes.Ldstr) {
                return;
            }

            if ("IDOL OF SEEDS".Equals(inst.operand) == true) {
                inst.operand = "IDOL OF VOLUMES";
            }
            else if ("'INTERACT' to activate infinite bird seed uses.".Equals(inst.operand) == true) {
                inst.operand = "'INTERACT' to turn holds into volumes.";
            }
        }

        /**
         * <summary>
         * Inject the custom field for checking if the idol
         * of volumes is enabled.
         * </summary>
         * <param name="insts">The instructions to inject into</param>
         * <return>The instructions after injection</return>
         */
        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> insts
        ) {
            // Replace reads
            IEnumerable<CodeInstruction> replaced = Helper.Replace(insts,
                new[] {
                    new CodeInstruction(OpCodes.Ldsfld, control),
                    new CodeInstruction(OpCodes.Ldfld, seedsInUse),
                },
                new[] {
                    new CodeInstruction(OpCodes.Ldsfld, volumesInUse),
                }
            );

            // Replace writes
            replaced = Helper.Replace(replaced,
                new[] {
                    new CodeInstruction(OpCodes.Ldsfld, control),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stfld, seedsInUse),
                },
                new[] {
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stsfld, volumesInUse),
                }
            );

            replaced = Helper.Replace(replaced,
                new[] {
                    new CodeInstruction(OpCodes.Ldsfld, control),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld, seedsInUse),
                },
                new[] {
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stsfld, volumesInUse),
                }
            );

            foreach (CodeInstruction inst in replaced) {
                ModifyText(inst);
                yield return inst;
            }
        }
    }

    /**
     * <summary>
     * Injects custom text and keeps the state of injected fields up to date.
     * </summary>
     */
    [HarmonyPatch(typeof(RopeCabinDescription), "Update")]
    static class CabinDescriptionUpdate {
        static void Postfix() {
            Cfg.Update();
        }

        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> insts
        ) {
            return InjectCustom.Inject(insts);
        }
    }

    /**
     * <summary>
     * Ensures the particle effect is enabled when loading the cabin
     * if the idol is in use.
     * </summary>
     */
    [HarmonyPatch(typeof(RopeCabinDescription), "CheckCabinItems")]
    static class CabinDescriptionCabin {
        static void Postfix(RopeCabinDescription __instance) {
            if (__instance.alpStatueParticle9 == null) {
                return;
            }

            __instance.alpStatueParticle9.SetActive(Cfg.idolInUse);
        }
    }

    /**
     * <summary>
     * Injects custom text.
     * </summary>
     */
    [HarmonyPatch(typeof(RopeCabinDescription), "CheckCabin4DLCStuff")]
    static class CabinDescriptionCabin4 {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> insts
        ) {
            return InjectCustom.Inject(insts);
        }
    }
}
