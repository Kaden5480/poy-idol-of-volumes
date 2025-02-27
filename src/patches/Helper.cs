using System.Reflection.Emit;
using System.Collections.Generic;

using HarmonyLib;

namespace IdolOfVolumes.Patches {
    public class Helper {
        /**
         * <summary>
         * Compare two instructions for equivalence.
         * </summary>
         * <param name="a">The first instruction to compare</param>
         * <param name="b">The second instruction to compare</param>
         */
        public static bool InstsEqual(CodeInstruction a, CodeInstruction b) {
            // If either are null, always match
            if (a == null || b == null) {
                return true;
            }

            // Check opcodes
            if (a.opcode != b.opcode) {
                return false;
            }

            // Check null operands
            if (a.operand == null || b.operand == null) {
                return true;
            }

            // Check operand equivalence
            return a.operand.Equals(b.operand);
        }

        /**
         * <summary>
         * Given a sequence of instructions, find a pattern and replace
         * it with the provided sequence.
         * </summary>
         * <param name="instructions">The instructions to search in</param>
         * <param name="pattern">The pattern to search for</param>
         * <param name="replacement">What to replace the pattern with</param>
         * <returns>The patched instructions</returns>
         */
        public static IEnumerable<CodeInstruction> Replace(
            IEnumerable<CodeInstruction> instructions,
            CodeInstruction[] pattern,
            CodeInstruction[] replacement
        ) {
            List<CodeInstruction> buffer = new List<CodeInstruction>();
            int patternIndex = 0;

            // If empty pattern, return normally
            if (pattern.Length < 1) {
                foreach (CodeInstruction instruction in instructions) {
                    yield return instruction;
                }

                yield break;
            }

            foreach (var instruction in instructions) {
                // If pattern matched, return the replacement
                if (patternIndex >= pattern.Length) {
                    // Move all labels to first instruction
                    foreach (CodeInstruction buffered in buffer) {
                        buffered.MoveLabelsTo(replacement[0]);
                    }

                    foreach (var replace in replacement) {
                        yield return replace;
                    }

                    yield return instruction;

                    patternIndex = 0;
                    buffer.Clear();

                    continue;
                }

                // If the pattern isn't fully matched, return
                // all buffered instructions normally
                if (InstsEqual(instruction, pattern[patternIndex]) == false) {
                    foreach (var buffered in buffer) {
                        yield return buffered;
                    }

                    yield return instruction;

                    patternIndex = 0;
                    buffer.Clear();

                    continue;
                }

                // Otherwise, store matching instructions
                buffer.Add(instruction);
                patternIndex++;
            }
        }
    }
}
