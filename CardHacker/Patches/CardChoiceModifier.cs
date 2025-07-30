using HarmonyLib;
using UnityEngine;
using ROUNDSCheat.GUI;
using System.Linq;

namespace ROUNDSCheat.Patches
{
    [HarmonyPatch(typeof(CardChoice))]
    public class CardChoiceModifier
    {
        private static CardInfo[] cards = null;
        private static int spawnCallCount = 0;
        private static System.Random random = new System.Random();
        private static int selectedCardPosition = random.Next(0, 5);

        [HarmonyPrefix]
        [HarmonyPatch("Spawn")]
        public static void ForceSpawnCard(ref GameObject objToSpawn, Vector3 pos, Quaternion rot, CardChoice __instance)
        {
            if (__instance.cards != null && cards == null)
            {
                cards = __instance.cards;
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Registered {cards.Length} cards");
            }

            CardInfo selectedCard = null;
            // Check if a card is selected in the GUI
            if (DeckManager.IsDeckBuilderModeEnabled)
            {
                selectedCard = DeckManager.GetNextCard();
            }
            else
            {
                selectedCard = CardSelectionGUI.GetSelectedCard();
            }

            // Reset batch and randomize position when starting a new batch of 5 calls
            if (spawnCallCount % 5 == 0)
            {
                selectedCardPosition = random.Next(0, 5);
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: New batch started, selected position: {selectedCardPosition + 1}");
            }

            if (selectedCard != null && objToSpawn != null && spawnCallCount % 5 == selectedCardPosition)
            {
                // Force spawn the selected card at the randomized position
                objToSpawn = selectedCard.gameObject;
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Force spawned selected card: {selectedCard.cardName} at position {selectedCardPosition + 1}");
            }
            else if (objToSpawn != null && cards != null && cards.Length > 0)
            {
                // Use original behavior for other positions
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Spawned default card: {objToSpawn.GetComponent<CardInfo>().cardName} at position {(spawnCallCount % 5) + 1}");
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

            spawnCallCount++;
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpawnUniqueCard")]
        public static void FixSourceCard(ref GameObject __result, CardChoice __instance)
        {
            // Check if a card is selected in the GUI
            CardInfo selectedCard;
            if (DeckManager.IsDeckBuilderModeEnabled)
            {
                selectedCard = DeckManager.GetNextCard();
            }
            else
            {
                selectedCard = CardSelectionGUI.GetSelectedCard();
            }

            // Only fix the source card if we're at the position where ForceSpawnCard would have applied the selected card
            if (__result != null && selectedCard != null &&
            (spawnCallCount - 1) % 5 == selectedCardPosition)
            {
                // Fix the source card and clear the selection
                __result.GetComponent<CardInfo>().sourceCard = selectedCard;
                if (CardSelectionGUI.ShouldAutoclearSelectedCard && !DeckManager.IsDeckBuilderModeEnabled) CardSelectionGUI.ClearSelectedCard();
                if (DeckManager.IsDeckBuilderModeEnabled) DeckManager.RemoveNextCard();
                ROUNDSCheatPlugin.Logger.LogInfo($"CardChoiceTestPatch: Fixed source card to {__result.GetComponent<CardInfo>().sourceCard.cardName}");
            }
        }

        // Public method to provide card list to GUI
        public static CardInfo[] GetAvailableCards() => cards;
    }
}