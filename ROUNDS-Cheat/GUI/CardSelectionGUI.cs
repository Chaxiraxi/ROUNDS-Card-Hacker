using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ROUNDSCheat.GUI
{
    public class CardSelectionGUI : MonoBehaviour
    {
        public static bool IsVisible { get; set; } = false;
        public static CardInfo SelectedCard { get; set; } = null;

        private Rect windowRect;
        private const int WindowId = 54321; // Unique ID for the window
        private Vector2 scrollPosition = Vector2.zero;

        // Resizing functionality
        private bool isResizing = false;
        private Vector2 resizeStartMouse;
        private Vector2 resizeStartSize;
        private const float MinWidth = 300f;
        private const float MinHeight = 400f;
        private const float ResizeBorderSize = 10f;

        // Search functionality
        private string searchFilter = "";
        private bool showSearch = true;

        private bool initialized = false;
        private static CardSelectionGUI Instance;
        private const float WindowProportion = 0.4f; // 40% of screen size

        // Card list cache
        private CardInfo[] availableCards = null;
        private List<CardInfo> filteredCards = new List<CardInfo>();

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            // Toggle GUI with F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                IsVisible = !IsVisible;
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
                windowRect = new Rect(50, 50, width, height);
                initialized = true;
            }

            windowRect = GUILayout.Window(WindowId, windowRect, DrawCardSelectionWindow, "ROUNDS Card Selector");
        }

        void DrawCardSelectionWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // Title and status
            var boldStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontStyle = FontStyle.Bold };
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
                foreach (var card in filteredCards)
                {
                    if (card == null) continue;

                    GUILayout.BeginHorizontal("box");

                    // Highlight selected card
                    if (SelectedCard == card)
                    {
                        UnityEngine.GUI.color = Color.cyan;
                    }

                    // Card selection button
                    if (GUILayout.Button($"{card.cardName}", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    {
                        SelectedCard = card;
                        ROUNDSCheatPlugin.Logger.LogInfo($"Selected card: {card.cardName}");
                    }

                    UnityEngine.GUI.color = Color.white;

                    // Show some card info if available
                    // GUILayout.BeginVertical(GUILayout.Width(200));
                    // var miniStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontSize = 10 };
                    // GUILayout.Label("Info", miniStyle);
                    // if (!string.IsNullOrEmpty(card.cardDestription))
                    // {
                    //     GUILayout.Label(card.cardDestription, miniStyle);
                    // }
                    // if (card.cardStats != null && card.cardStats.Length > 0)
                    // {
                    //     foreach (var stat in card.cardStats)
                    //     {
                    //         GUILayout.Label(stat.GetSimpleAmount(), miniStyle);
                    //     }
                    // }
                    // GUILayout.Label($"Rarity: {card.rarity}", miniStyle);
                    // GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
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

            if (e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition))
            {
                isResizing = true;
                resizeStartMouse = e.mousePosition;
                resizeStartSize = new Vector2(windowRect.width, windowRect.height);
                e.Use();
            }

            if (isResizing)
            {
                if (e.type == EventType.MouseDrag)
                {
                    float w = Mathf.Max(MinWidth, resizeStartSize.x + (e.mousePosition.x - resizeStartMouse.x));
                    float h = Mathf.Max(MinHeight, resizeStartSize.y + (e.mousePosition.y - resizeStartMouse.y));
                    windowRect.width = w;
                    windowRect.height = h;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp)
                {
                    isResizing = false;
                    e.Use();
                }
            }
        }

        public void RefreshCardList()
        {
            // Get cards from the CardChoiceTestPatch
            availableCards = ROUNDSCheat.Patches.CardChoiceModifier.GetAvailableCards();
            FilterCards();

            if (availableCards != null)
            {
                ROUNDSCheatPlugin.Logger.LogInfo($"Refreshed card list: {availableCards.Length} cards found");
            }
            else
            {
                ROUNDSCheatPlugin.Logger.LogWarning("No cards available to refresh");
            }
        }

        private void FilterCards()
        {
            filteredCards.Clear();

            if (availableCards == null) return;

            if (string.IsNullOrEmpty(searchFilter))
            {
                filteredCards.AddRange(availableCards);
            }
            else
            {
                string filter = searchFilter.ToLower();
                filteredCards.AddRange(availableCards.Where(card =>
                    card != null &&
                    card.cardName != null &&
                    card.cardName.ToLower().Contains(filter)
                ));
            }

            // Sort cards alphabetically
            filteredCards.Sort((a, b) =>
            {
                if (a?.cardName == null) return 1;
                if (b?.cardName == null) return -1;
                return a.cardName.CompareTo(b.cardName);
            });
        }

        public static CardInfo GetSelectedCard()
        {
            return SelectedCard;
        }

        public static void ClearSelectedCard()
        {
            SelectedCard = null;
        }
    }
}
