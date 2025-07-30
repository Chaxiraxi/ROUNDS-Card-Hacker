using System.Collections.Generic;
using System.Linq;

namespace ROUNDSCheat
{
    public static class DeckManager
    {
        public static bool IsDeckBuilderModeEnabled { get; set; } = false;
        public static List<CardInfo> Deck { get; private set; } = new List<CardInfo>();

        public static void AddCardToDeck(CardInfo card)
        {
            if (card != null)
            {
                Deck.Add(card);
                ROUNDSCheatPlugin.Logger.LogInfo($"Added {card.cardName} to deck. Deck now has {Deck.Count} cards.");
            }
        }

        public static CardInfo GetNextCard()
        {
            if (Deck.Count > 0)
            {
                return Deck.First();
            }
            return null;
        }

        public static void RemoveNextCard()
        {
            if (Deck.Count > 0)
            {
                CardInfo removedCard = Deck.First();
                Deck.RemoveAt(0);
                ROUNDSCheatPlugin.Logger.LogInfo($"Removed {removedCard.cardName} from deck. Deck now has {Deck.Count} cards.");
            }
        }

        public static void ClearDeck()
        {
            Deck.Clear();
            ROUNDSCheatPlugin.Logger.LogInfo("Deck cleared.");
        }

        public static void MoveCardUp(int index)
        {
            if (index > 0 && index < Deck.Count)
            {
                CardInfo card = Deck[index];
                Deck.RemoveAt(index);
                Deck.Insert(index - 1, card);
            }
        }

        public static void MoveCardDown(int index)
        {
            if (index >= 0 && index < Deck.Count - 1)
            {
                CardInfo card = Deck[index];
                Deck.RemoveAt(index);
                Deck.Insert(index + 1, card);
            }
        }

        public static void RemoveCardAt(int index)
        {
            if (index >= 0 && index < Deck.Count)
            {
                ROUNDSCheatPlugin.Logger.LogInfo($"Removed {Deck[index].cardName} from deck.");
                Deck.RemoveAt(index);
            }
        }
    }
}
