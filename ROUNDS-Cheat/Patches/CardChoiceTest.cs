using HarmonyLib;
using UnityEngine;
using ROUNDSCheat.GUI;
using System.Linq;

namespace ROUNDSCheat.Patches
{
    [HarmonyPatch(typeof(CardChoice))]  // STFU Copilot, __instance is a typo in the original code
    public class CardChoiceTestPatch
    {
        private static CardInfo[] cards = null;

        [HarmonyPrefix]
        [HarmonyPatch("Spawn")]
        public static void ForceSpawnCard(ref GameObject objToSpawn, Vector3 pos, Quaternion rot, CardChoice __instance)
        {
            if (__instance.cards != null && cards == null)
            {
                cards = __instance.cards;
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Registered {cards.Length} cards");
            }

            // Check if a card is selected in the GUI
            CardInfo selectedCard = CardSelectionGUI.GetSelectedCard();

            if (selectedCard != null && objToSpawn != null)
            {
                // Force spawn the selected card
                objToSpawn = selectedCard.gameObject;
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Force spawned selected card: {selectedCard.cardName}");

                // Optionally clear the selection after use (uncomment if you want single-use selection)
                // CardSelectionGUI.ClearSelectedCard();
            }
            else if (objToSpawn != null && cards != null && cards.Length > 0)
            {
                // Fallback to first card if no GUI selection (original behavior)
                objToSpawn = cards[0].gameObject;
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Spawned default card: {cards[0].cardName}");
            }
            else
            {
                ROUNDSCheatPlugin.Logger.LogWarning("CardChoiceTestPatch: Attempted to spawn a null card or no cards available.");
                ROUNDSCheatPlugin.Logger.LogInfo("SpawnState: objToSpawn: " + (objToSpawn != null ? objToSpawn.name : "null"));
                ROUNDSCheatPlugin.Logger.LogInfo("SpawnState: selectedCard: " + (selectedCard != null ? selectedCard.cardName : "null"));
                ROUNDSCheatPlugin.Logger.LogInfo("SpawnState: cards.Length: " + (cards != null ? cards.Length : "null"));
                if (cards != null && cards.Length > 0)
                {
                    ROUNDSCheatPlugin.Logger.LogInfo($"Available cards: {string.Join(", ", cards.Select(c => c.cardName))}");
                }
                else
                {
                    ROUNDSCheatPlugin.Logger.LogWarning("No cards available to spawn.");
                }
            }
        }

        // Public method to provide card list to GUI
        public static CardInfo[] GetAvailableCards()
        {
            return cards;
        }
    }
}