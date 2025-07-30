using UnityEngine;

namespace ROUNDSCheat.GUI
{
    public class DeckStackGUI : MonoBehaviour
    {
        public static bool IsVisible { get; set; } = false;

        private Rect deckWindowRect;
        private const int DeckWindowId = 54322;
        private Vector2 deckScrollPosition = Vector2.zero;
        private bool initialized = false;

        // Resizing functionality
        private bool isResizing = false;
        private Vector2 resizeStartMouse;
        private Vector2 resizeStartSize;
        private const float MinHeight = 200f;
        private const float ResizeBorderSize = 18f;

        void OnGUI()
        {
            if (!IsVisible || !DeckManager.IsDeckBuilderModeEnabled)
                return;

            // Initialize deckWindowRect if not already done
            if (!initialized)
            {
                deckWindowRect = new Rect(CardSelectionGUI.WindowMaxXPos + 10, CardSelectionGUI.InitialYPos, 300, 400); // Position it to the right of the main window
                initialized = true;
            }

            deckWindowRect = GUILayout.Window(DeckWindowId, deckWindowRect, DrawDeckWindow, "Deck Stack");
        }

        void DrawDeckWindow(int windowId)
        {
            GUILayout.BeginVertical();

            if (DeckManager.Deck.Count == 0)
            {
                GUILayout.Label("Deck is empty. Add cards from the selector.");
            }
            else
            {
                GUILayout.Label($"Card Count: {DeckManager.Deck.Count}");

                if (GUILayout.Button("Clear Deck"))
                {
                    DeckManager.ClearDeck();
                }
            }

            deckScrollPosition = GUILayout.BeginScrollView(deckScrollPosition, GUILayout.ExpandHeight(true));

            for (int i = 0; i < DeckManager.Deck.Count; i++)
            {
                var card = DeckManager.Deck[i];
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(card.cardName, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Up", GUILayout.Width(40)))
                {
                    DeckManager.MoveCardUp(i);
                    break;
                }
                if (GUILayout.Button("Down", GUILayout.Width(50)))
                {
                    DeckManager.MoveCardDown(i);
                    break;
                }
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    DeckManager.RemoveCardAt(i);
                    break;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            // Handle resizing
            HandleResizing();

            UnityEngine.GUI.DragWindow();
            GUILayout.EndVertical();
        }

        public void SetPosition(float x, float y)
        {
            deckWindowRect.x = x;
            deckWindowRect.y = y;
        }

        private void HandleResizing()
        {
            Event e = Event.current;

            // Resize handle at the bottom of the window
            Rect resizeRect = new Rect(
                0,
                deckWindowRect.height - ResizeBorderSize,
                deckWindowRect.width,
                ResizeBorderSize
            );
            UnityEngine.GUI.Box(resizeRect, "");

            // Start resizing if mouse down on resize handle
            if (e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition))
            {
                isResizing = true;
                // Convert GUI mouse position to screen coordinates for global tracking
                Vector2 screenMousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                resizeStartMouse = screenMousePos;
                resizeStartSize = new Vector2(deckWindowRect.width, deckWindowRect.height);
                e.Use();
            }

            // Handle resizing using global mouse coordinates
            if (isResizing)
            {
                // Use Input.mousePosition for global mouse tracking (note: Input.mousePosition is in screen coordinates)
                Vector2 currentScreenMouse = Input.mousePosition;
                // Unity's Input.mousePosition has Y=0 at bottom, but GUI has Y=0 at top, so flip Y
                currentScreenMouse.y = Screen.height - currentScreenMouse.y;

                // Calculate new height based on mouse movement from start position (only Y for vertical resize)
                float deltaY = currentScreenMouse.y - resizeStartMouse.y;
                float newHeight = Mathf.Max(MinHeight, resizeStartSize.y + deltaY);

                deckWindowRect.height = newHeight;

                // Stop resizing when mouse button is released
                if (!Input.GetMouseButton(0))
                {
                    isResizing = false;
                }
            }
        }
    }
}
