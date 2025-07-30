using UnityEngine;
using System.Collections.Generic;
using System;

namespace ROUNDSCheat.GUI
{
    public class CardSelectionGUI : MonoBehaviour
    {
        public static bool IsVisible { get; set; } = false;
        public static CardInfo SelectedCard { get; set; } = null;
        public static bool ShouldAutoclearSelectedCard { get; private set; } = true;
        public static float WindowMaxXPos { get; private set; } = 0f;
        public const float InitialXPos = 50f; // Initial X position for the window
        public const float InitialYPos = 50f; // Initial Y position for the window

        private Rect windowRect;
        private const int WindowId = 54321; // Unique ID for the window
        private Vector2 scrollPosition = Vector2.zero;

        // Resizing functionality
        private bool isResizing = false;
        private Vector2 resizeStartMouse;
        private Vector2 resizeStartSize;
        private const float MinWidth = 300f;
        private const float MinHeight = 400f;
        private const float ResizeBorderSize = 15f;

        // Search functionality
        private string searchFilter = "";
        private bool initialized = false;
        private const float WindowProportion = 0.4f; // 40% of screen size

        // Card list cache
        private CardInfo[] availableCards = null;
        private List<CardInfo> filteredCards = new List<CardInfo>();

        // Reference to deck stack GUI
        private DeckStackGUI deckStackGUI;

        void Update()
        {
            // Initialize deck stack GUI if not already done
            if (deckStackGUI == null)
            {
                deckStackGUI = gameObject.AddComponent<DeckStackGUI>();
            }

            // Toggle GUI with F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                IsVisible = !IsVisible;
                DeckStackGUI.IsVisible = IsVisible && DeckManager.IsDeckBuilderModeEnabled;
                if (IsVisible)
                {
                    RefreshCardList();
                }
            }
        }

        void OnGUI()
        {
            if (!IsVisible)
                return;

            // Initialize windowRect to 40% of screen size, only once
            if (!initialized)
            {
                float width = Mathf.Max(MinWidth, Screen.width * WindowProportion);
                float height = Mathf.Max(MinHeight, Screen.height * WindowProportion);
                windowRect = new Rect(InitialXPos, InitialYPos, width, height);
                initialized = true;
            }

            windowRect = GUILayout.Window(WindowId, windowRect, DrawCardSelectionWindow, "ROUNDS Card Selector");
            WindowMaxXPos = windowRect.xMax;
        }

        void DrawCardSelectionWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // Title and status
            var boldStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontStyle = FontStyle.Bold };
            if (DeckManager.IsDeckBuilderModeEnabled)
            {
                GUILayout.Label("Click a card to add it to the deck", boldStyle);
            }
            else
            {
                GUILayout.Label("Select a card to force spawn on next card choice:", boldStyle);

                if (SelectedCard != null)
                {
                    UnityEngine.GUI.color = Color.green;
                    GUILayout.Label($"Selected: {SelectedCard.cardName}", boldStyle);
                    UnityEngine.GUI.color = Color.white;

                    if (GUILayout.Button("Clear Selection"))
                    {
                        SelectedCard = null;
                    }
                }
                else
                {
                    UnityEngine.GUI.color = Color.yellow;
                    GUILayout.Label("No card selected", boldStyle);
                    UnityEngine.GUI.color = Color.white;
                }
            }


            GUILayout.Space(10);

            // Refresh button and search
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Card List", GUILayout.Width(120)))
            {
                RefreshCardList();
            }

            GUILayout.Label("Search:", GUILayout.Width(50));
            string newSearchFilter = GUILayout.TextField(searchFilter);
            if (newSearchFilter != searchFilter)
            {
                searchFilter = newSearchFilter;
                FilterCards();
            }

            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                searchFilter = "";
                FilterCards();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Auto-clear checkbox
            if (!DeckManager.IsDeckBuilderModeEnabled)
            {
                GUILayout.BeginHorizontal();
                bool newAutoClearValue = GUILayout.Toggle(ShouldAutoclearSelectedCard, "Auto-clear selection after card choice");
                if (newAutoClearValue != ShouldAutoclearSelectedCard)
                {
                    ShouldAutoclearSelectedCard = newAutoClearValue;
                }
                GUILayout.EndHorizontal();
            }

            // Deck builder mode checkbox
            GUILayout.BeginHorizontal();
            bool newDeckBuilderMode = GUILayout.Toggle(DeckManager.IsDeckBuilderModeEnabled, "Deck Builder Mode");
            if (newDeckBuilderMode != DeckManager.IsDeckBuilderModeEnabled)
            {
                DeckManager.IsDeckBuilderModeEnabled = newDeckBuilderMode;
                // Update deck stack GUI visibility when mode changes
                if (deckStackGUI != null)
                {
                    DeckStackGUI.IsVisible = IsVisible && DeckManager.IsDeckBuilderModeEnabled;
                }
            }
            GUILayout.EndHorizontal();


            // Card count info
            if (availableCards != null)
            {
                GUILayout.Label($"Showing {filteredCards.Count} of {availableCards.Length} cards");
            }
            else
            {
                UnityEngine.GUI.color = Color.red;
                GUILayout.Label("No cards available. Start a game or wait for cards to load.");
                UnityEngine.GUI.color = Color.white;
            }

            GUILayout.Space(5);

            // Scrollable card list
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            if (filteredCards != null && filteredCards.Count > 0)
            {
                // Virtualization: only render visible cards
                int cardHeight = 60; // Approximate height per card row
                int visibleCount = Mathf.FloorToInt(windowRect.height / cardHeight);
                int startIndex = Mathf.FloorToInt(scrollPosition.y / cardHeight);
                int endIndex = Mathf.Min(filteredCards.Count, startIndex + visibleCount + 2); // +2 for buffer

                GUILayout.Space(startIndex * cardHeight); // Spacer for skipped cards

                for (int i = startIndex; i < endIndex; i++)
                {
                    var card = filteredCards[i];
                    if (card == null) continue;

                    GUILayout.BeginHorizontal("box");

                    if (!DeckManager.IsDeckBuilderModeEnabled && SelectedCard == card)
                    {
                        UnityEngine.GUI.color = Color.cyan;
                    }

                    var buttonStyle = new GUIStyle(UnityEngine.GUI.skin.button) { fontSize = 20 };
                    if (GUILayout.Button($"{card.cardName}", buttonStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        if (DeckManager.IsDeckBuilderModeEnabled)
                        {
                            DeckManager.AddCardToDeck(card);
                        }
                        else
                        {
                            SelectedCard = card;
                            ROUNDSCheatPlugin.Logger.LogInfo($"Selected card: {card.cardName}");
                        }
                    }

                    UnityEngine.GUI.color = Color.white;

                    GUILayout.BeginVertical(GUILayout.Width(200));
                    var miniStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontSize = 12, wordWrap = true };
                    miniStyle.margin = new RectOffset(0, 0, 0, 0);
                    if (!string.IsNullOrEmpty(card.cardDestription))
                    {
                        string plainDescription = System.Text.RegularExpressions.Regex.Replace(card.cardDestription, "<.*?>", string.Empty);
                        GUILayout.Label(plainDescription, miniStyle);
                    }
                    if (card.cardStats != null && card.cardStats.Length > 0)
                    {
                        foreach (var stat in card.cardStats)
                        {
                            string amount = stat.amount;
                            string statName = stat.stat;
                            GUILayout.Label($"{statName}: {amount}", miniStyle);
                        }
                    }
                    GUILayout.Label($"Rarity: {card.rarity}", miniStyle);
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }

                GUILayout.Space((filteredCards.Count - endIndex) * cardHeight); // Spacer for remaining cards
            }
            else if (availableCards != null)
            {
                GUILayout.Label("No cards match the search filter.");
            }

            GUILayout.EndScrollView();

            GUILayout.Space(5);

            // Footer info
            GUILayout.BeginHorizontal();
            var footerStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontSize = 10 };
            GUILayout.Label("Press F1 to toggle this window", footerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                IsVisible = false;
                DeckStackGUI.IsVisible = false; // Hide deck stack GUI when main window is closed
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            // Handle resizing
            HandleResizing();

            UnityEngine.GUI.DragWindow();
        }

        private void HandleResizing()
        {
            Event e = Event.current;

            // Resize handle
            Rect resizeRect = new Rect(
                windowRect.width - ResizeBorderSize,
                windowRect.height - ResizeBorderSize,
                ResizeBorderSize,
                ResizeBorderSize
            );
            UnityEngine.GUI.Box(resizeRect, "");

            // Start resizing if mouse down on resize handle (support both left and right click)
            if (e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition) && (e.button == 0 || e.button == 1))
            {
                isResizing = true;
                // Convert GUI mouse position to screen coordinates for global tracking
                Vector2 screenMousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                resizeStartMouse = screenMousePos;
                resizeStartSize = new Vector2(windowRect.width, windowRect.height);
                e.Use();
            }

            // Handle resizing using global mouse coordinates
            if (isResizing)
            {
                // Use Input.mousePosition for global mouse tracking (note: Input.mousePosition is in screen coordinates)
                Vector2 currentScreenMouse = Input.mousePosition;
                // Unity's Input.mousePosition has Y=0 at bottom, but GUI has Y=0 at top, so flip Y
                currentScreenMouse.y = Screen.height - currentScreenMouse.y;

                // Calculate new size based on mouse movement from start position
                float deltaX = currentScreenMouse.x - resizeStartMouse.x;
                float deltaY = currentScreenMouse.y - resizeStartMouse.y;

                float newWidth = Mathf.Max(MinWidth, resizeStartSize.x + deltaX);
                float newHeight = Mathf.Max(MinHeight, resizeStartSize.y + deltaY);

                windowRect.width = newWidth;
                windowRect.height = newHeight;

                // Stop resizing when either mouse button is released
                if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                {
                    isResizing = false;
                }
            }
        }

        public void RefreshCardList()
        {
            // Get cards from the CardChoiceTestPatch
            availableCards = Patches.CardChoiceModifier.GetAvailableCards();
            FilterCards();
            if (availableCards == null) ROUNDSCheatPlugin.Logger.LogWarning("No cards available to refresh");
        }

        private void FilterCards()
        {
            filteredCards.Clear();

            if (availableCards == null) return;

            if (string.IsNullOrEmpty(searchFilter))
            {
                // When search is empty, just show all cards in alphabetical order
                filteredCards.AddRange(availableCards);

                // Sort alphabetically
                filteredCards.Sort((a, b) =>
                {
                    if (a?.cardName == null) return 1;
                    if (b?.cardName == null) return -1;
                    return a.cardName.CompareTo(b.cardName);
                });
            }
            else
            {
                // Split search into individual words
                string[] searchWords = searchFilter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Dictionary to store relevance score for each card
                Dictionary<CardInfo, int> cardScores = new Dictionary<CardInfo, int>();
                // Dictionary to track which words matched for each card and at which position
                Dictionary<CardInfo, Dictionary<int, bool>> wordMatchPositions = new Dictionary<CardInfo, Dictionary<int, bool>>();

                foreach (var card in availableCards)
                {
                    if (card == null) continue;

                    int score = 0;
                    var matchedPositions = new Dictionary<int, bool>();

                    for (int wordIndex = 0; wordIndex < searchWords.Length; wordIndex++)
                    {
                        string word = searchWords[wordIndex];
                        bool wordMatched = false;

                        // Card name matches are worth 2 points
                        if (card.cardName != null && card.cardName.ToLower().Contains(word))
                        {
                            score += 2;
                            wordMatched = true;
                        }

                        // Description matches are worth 1 point
                        if (card.cardDestription != null && card.cardDestription.ToLower().Contains(word))
                        {
                            score += 1;
                            wordMatched = true;
                        }

                        // Stat matches are worth 1 point
                        if (card.cardStats != null)
                        {
                            foreach (var stat in card.cardStats)
                            {
                                if ((stat.stat != null && stat.stat.ToLower().Contains(word)) ||
                                    (stat.amount != null && stat.amount.ToLower().Contains(word)))
                                {
                                    score += 1;
                                    wordMatched = true;
                                    break; // Only count once per stat section per word
                                }
                            }
                        }

                        // Rarity matches are worth 1 point
                        if (card.rarity.ToString().ToLower().Contains(word))
                        {
                            score += 1;
                            wordMatched = true;
                        }

                        // Track which position this word matched at
                        matchedPositions[wordIndex] = wordMatched;
                    }

                    // Only include cards that matched at least one search term
                    if (score > 0)
                    {
                        cardScores[card] = score;
                        wordMatchPositions[card] = matchedPositions;
                    }
                }

                // Add cards to filtered list
                filteredCards.AddRange(cardScores.Keys);

                // Sort by score (descending), then by word order, then alphabetically as final tiebreaker
                filteredCards.Sort((a, b) =>
                {
                    int scoreA = cardScores[a];
                    int scoreB = cardScores[b];

                    // Primary sort by score (descending)
                    int scoreComparison = scoreB.CompareTo(scoreA);

                    // If scores are equal, use word order for tie-breaking
                    if (scoreComparison == 0)
                    {
                        // Compare by the order of matched words
                        var matchesA = wordMatchPositions[a];
                        var matchesB = wordMatchPositions[b];

                        // Check each word position in order
                        for (int i = 0; i < searchWords.Length; i++)
                        {
                            bool matchA = matchesA.ContainsKey(i) && matchesA[i];
                            bool matchB = matchesB.ContainsKey(i) && matchesB[i];

                            // If only one card matches this position, it wins
                            if (matchA && !matchB) return -1;
                            if (!matchA && matchB) return 1;
                        }

                        // If identical in terms of word matching, use alphabetical order as final tiebreaker
                        if (a?.cardName == null) return 1;
                        if (b?.cardName == null) return -1;
                        return a.cardName.CompareTo(b.cardName);
                    }

                    return scoreComparison;
                });
            }
        }

        public static CardInfo GetSelectedCard() => SelectedCard;
        public static void ClearSelectedCard() => SelectedCard = null;
    }
}
