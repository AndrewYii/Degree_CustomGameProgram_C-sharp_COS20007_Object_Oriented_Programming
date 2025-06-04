using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is game class that manages the game state, player, units, maps, and various game mechanics.
    /// </summary>
    public class Game
    {
        public const int WINDOW_WIDTH = 1600;
        public const int WINDOW_HEIGHT = 1200;
        public const int GRID_SIZE = 60;
        private const double DOUBLE_CLICK_TIME = 0.5;
        private GameState _currentState;
        private Difficulty _currentDifficulty;
        private List<Map> _map;
        private Player? _player;
        private List<Unit> _units;
        private int _currentLevel;
        private int _selectedCharacterIndex = 0;
        private string[] _characterRoles = new string[] { "Knight", "Archer", "Axeman" };
        private Player[] _characterSelectionPlayers;
        private string _playerName = "";
        private Item? _selectedItem = null;
        private Equipment? _draggingEquipment = null;
        private bool _isDragging = false;
        private Vector2 _dragStartPosition = new Vector2(0, 0);
        private Vector2 _currentDragPosition = new Vector2(0, 0);
        private int _inventoryScrollPosition = 0;
        private int _battleItemScrollPosition = 0;
        private Item? _hoveredItem = null;
        private DateTime _lastClickTime = DateTime.MinValue;
        private Item? _lastClickedItem = null;
        private float _soundVolume = 0.5f;
        private float _musicVolume = 0.5f;
        private bool _isSettingsPanelOpen = false;
        private bool _showAttackEffect = false;
        private float _attackEffectDuration = 0f;
        private bool _isPlayerAttacking = false;
        private Vector2 _attackEffectPos = new Vector2();
        private string _attackEffectText = "";
        private Raylib_cs.Color _attackEffectColor = Raylib_cs.Color.White;
        private List<(string Message, DateTime Time, double Duration, Raylib_cs.Color Color)> _notifications = new List<(string, DateTime, double, Raylib_cs.Color)>();
        private bool _isPlayerTurn = true;
        private string _battleLogText = "Battle started!";
        private List<string> _battleLog = new List<string>();
        private int _selectedSkillIndex = -1;
        private bool _showSkillSelection = false;
        private bool _showItemSelection = false;
        private float _turnTransitionTimer = 0f;
        private string _currentPlayerAttackingType = "";
        private string _currentMonsterAttackingType = "";
        private bool _isTurnInProgress = false;
        private string _currentTurnCharacter = "";
        private string _currentskillName = "";
        private float _turnDelay = 0.05f;
        private bool _showVictoryScreen = false;
        private Monster? _previousDeathMonster = null;
        private bool _victoryRewardsGranted = false;
        private bool _escapeAttemptInProgress = false;
        private float _escapeAnimationTimer = 0f;
        private bool _escapeSuccessful = false;
        private int _merchantFrame = 0;
        private float _merchantAnimTime = 0f;
        private const float _merchantFrameDuration = 1f;
        private const int _merchantFrameCount = 2;
        private const int _merchantFrameWidth = 130;
        private bool _inShopInteraction = false;
        private Merchant? _currentMerchant = null;
        private int _blacksmithFrame = 0;
        private float _blacksmithAnimTime = 0f;
        private const float _blacksmithFrameDuration = 1f;
        private const int _blacksmithFrameCount = 2;
        private const int _blacksmithFrameWidth = 130;
        private bool _inBlacksmithInteraction = false;
        private BlackSmith? _currentBlacksmith = null;
        private bool _victoryCreditsTransition = false;
        private string[] _creditLines = new string[]
        {
            "Game Developed by:",
            "Andrew Yii (ME)",
            "",
            "Programmer:",
            "Andrew Yii (ME)",
            "",
            "Art and Design:",
            "Andrew Yii (ME)",
            "",
            "Music and Sound Design:",
            "Still Finding",
            "Andrew Yii (ME)",
            "",
            "Special Thanks To:",
            "My Programming Lecturer, Miss Fu",
            "Free Itch.io Asset Packs Creators",
            "Still Finding Music Source",
            "Raylib Library Developer",
            "Family and Friends",
            "",
            "Additional Resources Provided by:",
            "Itch.io Asset Packs",
            "Still Finding Music Source",
            "",
            "Quality Assurance:",
            "Andrew Yii (ME)",
            "My Lecturer, Miss Fu",
            "My friends",
            "",
            "Game Testers:",
            "Andrew Yii (ME)",
            "My Friends",
            "",
            "With Gratitude:",
            "To all the fans and lecturers who supported us.",
            "Thank you for making this journey worthwhile!",
            "Grateful for this course's chance to explore custom programming",
            "",
            "",
            "Thanks for playing!",
            "Press any key to return to the main menu"
        };
        private float _creditsScrollY = 0f;
        private float _creditsScrollSpeed = 60f;
        /// <summary>
        /// Constructor for the Game class to initialize the game state, difficulty, maps, units, and player.
        /// </summary>
        public Game()
        {
            _currentState = GameState.MainMenu;
            _currentDifficulty = Difficulty.Medium;
            _map = new List<Map>();
            _units = new List<Unit>();
            _currentLevel = 1;
            _player = null;
            _characterSelectionPlayers = new Player[3];
            _characterSelectionPlayers[0] = new Knight("PreviewKnight", 100, 100, 10, 0.1, 5, 5, 50, 0, 1, 1, 1, true);
            _characterSelectionPlayers[1] = new Archer("PreviewArcher", 100, 100, 10, 0.1, 5, 5, 50, 0, 1, 1, 1, true);
            _characterSelectionPlayers[2] = new Axeman("PreviewAxeman", 100, 100, 10, 0.1, 5, 5, 50, 0, 1, 1, 1, true);
            Directory.CreateDirectory("log");
            Merchant.LoadMerchantTextures();
            BlackSmith.LoadBlacksmithTextures();
            Tile.LoadTextures();
        }
        /// <summary>
        /// Private method is used to draw the game credits on the screen.
        /// </summary>
        private void DrawCredits()
        {
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(10, 10, 25, 255));
            _creditsScrollY -= _creditsScrollSpeed * Raylib.GetFrameTime();
            float totalHeight = _creditLines.Length * 60;
            if (_creditsScrollY < -totalHeight)
            {
                _creditsScrollY = WINDOW_HEIGHT;
            }
            int lineHeight = 60;
            for (int i = 0; i < _creditLines.Length; i++)
            {
                string line = _creditLines[i];
                Raylib_cs.Color textColor = Raylib_cs.Color.White;
                int fontSize = 30;
                if (i % 3 == 0 && !string.IsNullOrEmpty(line) && line.Contains(":"))
                {
                    textColor = Raylib_cs.Color.Gold;
                    fontSize = 40;
                }
                else if (i >= _creditLines.Length - 5)
                {
                    textColor = new Raylib_cs.Color(150, 255, 150, 255);
                    fontSize = 36;
                }
                int textWidth = Raylib.MeasureText(line, fontSize);
                int x = WINDOW_WIDTH / 2 - textWidth / 2;
                int y = (int)(_creditsScrollY + i * lineHeight + 200);
                if (y > -50 && y < WINDOW_HEIGHT + 50)
                {
                    float offset = (float)Math.Sin(Raylib.GetTime() * 2 + i * 0.2) * 3;
                    Raylib.DrawText(line, x + 2, y + 2, fontSize, new Raylib_cs.Color(0, 0, 0, 180));
                    Raylib.DrawText(line, x, y + (int)offset, fontSize, textColor);
                }
            }
        }
        /// <summary>
        ///  Private method is used to handle the input for the game credit in the ending and resetgame after the input is executed.
        /// </summary>
        private void HandleCreditsInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                ResetGame();
                ChangeState(GameState.MainMenu);
                LogGameEvent("Navigation", "Credits -> Main Menu");
            }
        }
        /// <summary>
        /// Private method is used to check the equipment of the player and remove any broken equipment.
        /// </summary>
        private void CheckBrokenEquipment()
        {
            if (_player == null || _player.Inventory == null){
                return;
            }
            List<Equipment> brokenItems = _player.Inventory.CheckBrokenEquipment();
            _player.Inventory.RemoveBrokenEquipment(brokenItems, (brokenEquip) =>
            {
                _player.UpdateStats();
                _battleLog.Add("" + brokenEquip.Name + " broke and was removed from inventory.");
                LogGameEvent("Equipment", brokenEquip.Name + " broke and was removed");
            });
        }
        /// <summary>
        /// Private method is used to change the current game state.
        /// </summary>
        private void ChangeState(GameState newState)
        {
            GameState oldState = _currentState;
            _currentState = newState;
            _notifications.Clear();
            LogGameEvent("State change", "From " + oldState + " to " + _currentState);
            switch (_currentState)
            {
                case GameState.MainMenu:
                    Console.WriteLine("Welcome to Dungeon Adventure!");
                    break;
                case GameState.Exploring:
                    Console.WriteLine("Exploring the dungeon...");
                    break;
                case GameState.InBattle:
                    Console.WriteLine("A battle has begun!");
                    break;

                case GameState.Inventory:
                    Console.WriteLine("Opening inventory...");
                    break;

                case GameState.GameOver:
                    Console.WriteLine("Game over! You were defeated.");
                    break;

                case GameState.Credit:
                    Console.WriteLine("Game Credits\nThank you for playing!");
                    break;
            }
        }
        /// <summary>
        /// Public method is used to handle all the input for the game based on the current state.
        /// </summary>
        public void HandleInput()
        {
            if (_isSettingsPanelOpen)
            {
                HandleSettingsInput();
                return;
            }
            switch (_currentState)
            {
                case GameState.MainMenu:
                    HandleMainMenuInput();
                    break;

                case GameState.CharacterSelection:
                    HandleCharacterSelectionInput();
                    break;

                case GameState.NameInput:
                    HandleNameInputInput();
                    break;

                case GameState.StoryIntro:
                    HandleStoryIntroInput();
                    break;

                case GameState.Exploring:
                    HandleExploringInput();
                    break;

                case GameState.Inventory:
                    HandleInventoryInput();
                    break;
                case GameState.InBattle:
                    HandleBattleInput();
                    break;
                case GameState.GameOver:
                    HandleGameOverInput();
                    break;
                case GameState.Credit:
                    HandleCreditsInput();
                    break;

            }
        }
        /// <summary>
        /// Private method is used to handle the input for the main menu.
        /// </summary>
        private void HandleMainMenuInput()
        {
            Rectangle startButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2, 400, 60);
            Rectangle exitButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2 + 150, 400, 60);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), startButton))
                {
                    ChangeState(GameState.CharacterSelection);
                    LogGameEvent("Navigation", "Main Menu -> Character Selection");
                }
                else if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), exitButton))
                {
                    try
                    {
                        LogGameEvent("Exit", "Main Menu Exit (Exit button)");
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error exiting application: " + ex.Message);
                        Environment.Exit(1);
                    }
                }
            }
        }
        /// <summary>
        /// Private method is used to handle the input for the character selection screen.
        /// </summary>
        private void HandleCharacterSelectionInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                _selectedCharacterIndex = (_selectedCharacterIndex + 1) % _characterRoles.Length;
                LogGameEvent("Character Selection", "Selected: " + _characterRoles[_selectedCharacterIndex]);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                _selectedCharacterIndex = (_selectedCharacterIndex - 1 + _characterRoles.Length) % _characterRoles.Length;
                LogGameEvent("Character Selection", "Selected: " + _characterRoles[_selectedCharacterIndex]);
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Rectangle confirmButton = new Rectangle(WINDOW_WIDTH / 2 - 350, WINDOW_HEIGHT - 140, 250, 50);
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), confirmButton))
                {
                    ChangeState(GameState.NameInput);
                    LogGameEvent("Navigation", "Character Selection -> Name Input");
                    return;
                }
                Rectangle backButton = new Rectangle(WINDOW_WIDTH / 2 + 100, WINDOW_HEIGHT - 140, 250, 50);
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), backButton))
                {
                    ChangeState(GameState.MainMenu);
                    LogGameEvent("Navigation", "Character Selection -> Main Menu (back button)");
                    return;
                }
                for (int i = 0; i < _characterRoles.Length; i++)
                {
                    int xPos = WINDOW_WIDTH / 2 - 300 + (i * 300);
                    Rectangle characterBox = new Rectangle(xPos - 100, WINDOW_HEIGHT / 2 - 150, 200, 300);

                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), characterBox))
                    {
                        _selectedCharacterIndex = i;
                        LogGameEvent("Character Selection", "Selected: " + _characterRoles[_selectedCharacterIndex]);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Private method is used to handle the input for the item selection during battle.
        /// </summary>
        private void HandleItemSelectionInput()
        {
            if (_player == null)
            {
                return;
            }
            int panelWidth = 800;
            int panelHeight = 600;
            Rectangle panel = new Rectangle(WINDOW_WIDTH / 2 - panelWidth / 2,WINDOW_HEIGHT / 2 - panelHeight / 2,panelWidth,panelHeight);
            List<Item> usableItems = new List<Item>();
            foreach (Item item in _player.Inventory.Items)
            {
                if (item is Potion potion)
                {
                    if (potion.PotionType != PotionType.ExpBoost)
                    {
                        usableItems.Add(item);
                    }
                }
            }
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), panel))
            {
                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    int visibleItems = 5;
                    int maxScrollPosition = Math.Max(0, usableItems.Count - visibleItems);
                    _battleItemScrollPosition -= (int)wheel;
                    _battleItemScrollPosition = Math.Clamp(_battleItemScrollPosition, 0, maxScrollPosition);
                }
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                Rectangle closeButton = new Rectangle(panel.X + panel.Width - 60, panel.Y + 20, 40, 40);
                if (Raylib.CheckCollisionPointRec(mousePos, closeButton))
                {
                    _showItemSelection = false;
                    _currentPlayerAttackingType = "";
                    _isTurnInProgress = false;
                    return;
                }
                Rectangle itemsArea = new Rectangle(panel.X + 20, panel.Y + 80, panel.Width - 40, panel.Height - 100);
                if (usableItems.Count > 0)
                {                  
                    int itemButtonHeight = 80;
                    int itemSpacing = 100;
                    int visibleItems = 5;
                    int maxScrollPosition = Math.Max(0, usableItems.Count - visibleItems);
                    _battleItemScrollPosition = Math.Clamp(_battleItemScrollPosition, 0, maxScrollPosition);
                    int itemY = (int)itemsArea.Y + 20;
                    for (int i = 0; i < Math.Min(visibleItems, usableItems.Count); i++)
                    {
                        int itemIndex = i + _battleItemScrollPosition;
                        if (itemIndex >= usableItems.Count) break;
                        Item item = usableItems[itemIndex];
                        Rectangle itemButton = new Rectangle(itemsArea.X + 20,itemY + (i * itemSpacing),itemsArea.Width - 60, itemButtonHeight);
                        if (Raylib.CheckCollisionPointRec(mousePos, itemButton))
                        {
                            UseItemInBattle(item);
                            _showItemSelection = false;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Private method is used to use item in battle.
        /// </summary>
        private void UseItemInBattle(Item item)
        {
            if (_player == null) {
                return;
            }
            if (item is Potion potion)
            {
                (bool potionUsed, string message) = potion.UseInBattle(_player);
                if (potionUsed)
                {
                    _battleLogText = message;
                    _battleLog.Add(message);
                    LogGameEvent("Battle", message);
                    _showAttackEffect = true;
                    _attackEffectDuration = 0.5f;
                    _isPlayerAttacking = true;
                    _attackEffectPos = new Vector2(WINDOW_WIDTH / 4, WINDOW_HEIGHT / 2);
                    _attackEffectText = "";
                    if (potion.PotionType == PotionType.Healing)
                    {
                        _attackEffectColor = new Raylib_cs.Color(0, 255, 0, 255);
                    }
                    else if (potion.PotionType == PotionType.Mana)
                    {
                        _attackEffectColor = new Raylib_cs.Color(0, 100, 255, 255);
                    }
                    else
                    {
                        _attackEffectColor = Raylib_cs.Color.White;
                    }
                    _isPlayerTurn = false;
                    _turnTransitionTimer = 0.5f;
                    potion.Quantity--;
                    if (potion.Quantity <= 0)
                    {
                        _player.Inventory.RemoveItem(potion);
                    }
                }
                else
                {
                    _battleLogText = message;
                    _battleLog.Add(message);
                    _isTurnInProgress = false;
                }
            }
        }
        /// <summary>
        /// Private method is used to handle the input for the gameplay in the map / exploring state.
        /// </summary>
        private void HandleExploringInput()
        {
            if (_player != null && _map.Count > 0 && !_player.IsMoving)
            {
                Map currentMap = _map[_map.Count - 1];
                int newRow = _player.Row;
                int newCol = _player.Column;
                bool moveRequested = false;
                if (Raylib.IsKeyPressed(KeyboardKey.W))
                {
                    newRow--;
                    moveRequested = true;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.S))
                {
                    newRow++; 
                    moveRequested = true;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.A))
                {
                    newCol--; 
                    moveRequested = true;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.D))
                {
                    newCol++;
                    moveRequested = true;
                }
                if (moveRequested && currentMap.CheckInBounds(newCol, newRow))
                {
                    Tile targetTile = currentMap.Tiles[newCol, newRow];
                    if (targetTile.IsWalkable)
                    {
                        _player.StartMovement(newRow, newCol);
                        RevealTiles(currentMap, newCol, newRow, 2);
                    }
                }
            }
            if (Raylib.IsKeyPressed(KeyboardKey.E))
            {
                ChangeState(GameState.Inventory);
                LogGameEvent("Navigation", "Exploring -> Inventory");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.C) && _player != null && _map.Count > 0)
            {
                Map currentMap = _map[_map.Count - 1];
                if (currentMap.CheckInBounds(_player.Column, _player.Row))
                {
                    Tile currentTile = currentMap.Tiles[_player.Column, _player.Row];
                    if (currentTile.TileType == TileType.Merchant)
                    {
                        Merchant? merchant = null;
                        foreach (Unit unit in _units)
                        {
                            if (unit is Merchant m && m.Row == _player.Row && m.Column == _player.Column)
                            {
                                merchant = m;
                                break;
                            }
                        }
                        if (merchant != null)
                        {
                            _currentMerchant = merchant;
                            _inShopInteraction = true;
                            LogGameEvent("Shop", "Player entered merchant shop");
                        }
                    }
                    if (currentTile.TileType == TileType.Blacksmith)
                    {
                        BlackSmith? blacksmith = null;
                        foreach (Unit unit in _units)
                        {
                            if (unit is BlackSmith bs &&
                                bs.Column == _player.Column &&
                                bs.Row == _player.Row)
                            {
                                blacksmith = bs;
                                break;
                            }
                        }
                        if (blacksmith != null)
                        {
                            _currentBlacksmith = blacksmith;
                            _inBlacksmithInteraction = true;
                            LogGameEvent("Blacksmith", "Player entered blacksmith shop");
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Private method is used to handle the input for the inventory menu.
        /// </summary>
        private void HandleInventoryInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.E) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                ChangeState(GameState.Exploring);
                LogGameEvent("Navigation", "Inventory -> Exploring");
                return;
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                HandleInventoryMouseClick();
            }
            else if (_isDragging && Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                _currentDragPosition = Raylib.GetMousePosition();
                if (_player != null)
                {
                    _player.Inventory.UpdateDragPosition(_currentDragPosition);
                }
            }
            else if (_isDragging && Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                HandleItemDrop();
            }
        }
        /// <summary>
        /// Private method is used to handle the item click and drag in the inventory.
        /// </summary>
        private void HandleInventoryMouseClick()
        {
            if (_player != null && _player.Inventory.Items.Count > 0)
            {
                Rectangle itemsPanel = new Rectangle(WINDOW_WIDTH / 2 - 420, 170, 260, WINDOW_HEIGHT - 290);
                int rowHeight = 60;
                int visibleRowsCount = 8;

                Rectangle listArea = new Rectangle(itemsPanel.X + 10, itemsPanel.Y + 50, itemsPanel.Width - 30, visibleRowsCount * rowHeight);
                if (_player.Inventory.Items.Count > visibleRowsCount)
                {
                    Rectangle upArrowArea = new Rectangle(listArea.X + listArea.Width / 2 - 15,listArea.Y - 20,30,20);
                    Rectangle downArrowArea = new Rectangle(listArea.X + listArea.Width / 2 - 15,listArea.Y + listArea.Height,30,20);
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), upArrowArea) && _inventoryScrollPosition > 0)
                    {
                        _inventoryScrollPosition--;
                        return;
                    }
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), downArrowArea) &&
                        _inventoryScrollPosition < _player.Inventory.Items.Count - visibleRowsCount)
                    {
                        _inventoryScrollPosition++;
                        return;
                    }
                }
                for (int i = _inventoryScrollPosition; i < Math.Min(_player.Inventory.Items.Count, _inventoryScrollPosition + visibleRowsCount); i++)
                {
                    Item item = _player.Inventory.Items[i];
                    int rowIndex = i - _inventoryScrollPosition;
                    Rectangle rowRect = new Rectangle(listArea.X,listArea.Y + (rowIndex * rowHeight),listArea.Width,rowHeight - 5 );
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rowRect))
                    {
                        DateTime now = DateTime.Now;
                        double timeSinceLastClick = (now - _lastClickTime).TotalSeconds;
                        bool isDoubleClick = (timeSinceLastClick < DOUBLE_CLICK_TIME) && (_lastClickedItem == item);
                        _lastClickTime = now;
                        _lastClickedItem = item;
                        if (isDoubleClick)
                        {
                            if (item is Equipment)
                            {
                                Equipment eq = (Equipment)item;
                                _player.Inventory.EquipItem(eq);
                                LogGameEvent("Inventory", "Equipped " + eq.Name);
                            }
                            else if (item is Potion)
                            {
                                Potion p = (Potion)item;
                                (bool potionUsed, string message) = p.UseInBattle(_player);
                                if (potionUsed)
                                {
                                    LogGameEvent("Inventory", message);
                                    p.Quantity--;
                                    if (p.Quantity <= 0)
                                    {
                                        _player.Inventory.Items.Remove(p);
                                        if (_selectedItem == p)
                                            _selectedItem = null;
                                        if (_hoveredItem == p)
                                            _hoveredItem = null;
                                    }
                                }
                                else
                                {
                                    LogGameEvent("Inventory", message);
                                }
                            }
                            return;
                        }
                    }
                }
                if (_player != null)
                {
                    int slotSize = 80; 
                    Rectangle equipPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 170, 550, 450);
                    int slotX = (int)equipPanel.X + 30;
                    int equipStartY = (int)equipPanel.Y + 50;
                    int equipSpacing = slotSize + 15;
                    CheckEquipmentSlotClick(slotX + 210, equipStartY, slotSize, _player.Inventory.EquippedHelmet);
                    CheckEquipmentSlotClick(slotX + 100, equipStartY + equipSpacing, slotSize, _player.Inventory.EquippedWeapon);
                    CheckEquipmentSlotClick(slotX + 210, equipStartY + equipSpacing + 50, slotSize, _player.Inventory.EquippedChest);
                    CheckEquipmentSlotClick(slotX + equipSpacing * 2 + 130, equipStartY + equipSpacing, slotSize, _player.Inventory.EquippedBracelet);
                    CheckEquipmentSlotClick(slotX + 100, equipStartY + equipSpacing * 2 + 20, slotSize, _player.Inventory.EquippedGloves);
                    CheckEquipmentSlotClick(slotX + equipSpacing * 2 + 130, equipStartY + equipSpacing * 2 + 20, slotSize, _player.Inventory.EquippedRing);
                    CheckEquipmentSlotClick(slotX + 210, equipStartY + equipSpacing * 3, slotSize, _player.Inventory.EquippedLegs);
                }
            }
        }
        /// <summary>
        /// Private method to draw the game over screen.
        /// </summary>
        private void DrawGameOver()
        {
            float time = (float)Raylib.GetTime();
            int alpha = (int)Math.Min(200, time * 60); // Fade in effect
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, alpha));
            float pulseIntensity = (float)Math.Sin(time * 0.8f) * 0.3f + 0.7f;
            Raylib.DrawCircle(WINDOW_WIDTH / 2,WINDOW_HEIGHT / 2,400 + pulseIntensity * 50,new Raylib_cs.Color(120, 20, 20, (int)(30 * pulseIntensity)));
            string title = "GAME OVER";
            int titleFontSize = 120;
            float titleOffset = (float)Math.Sin(time * 2) * 5;
            int titleWidth = Raylib.MeasureText(title, titleFontSize);
            Raylib.DrawText(title,WINDOW_WIDTH / 2 - titleWidth / 2 + 8,100 + 8 + (int)titleOffset,titleFontSize,new Raylib_cs.Color(80, 0, 0, 200));
            Raylib.DrawText(title,WINDOW_WIDTH / 2 - titleWidth / 2,100 + (int)titleOffset,titleFontSize,new Raylib_cs.Color(200, 10, 10, 255));
            if (_player != null)
            {
                string[] trapDeathMessages = {
                    "Caught in a deadly trap, your life ebbs away...",
                    "The trap's cruel mechanism claims your final breath...",
                    "You failed to notice the deadly contraption until too late...",
                    "Ancient traps still guard these halls with lethal efficiency...",
                    "The dungeon's mechanical sentinel exacts its toll..."
                };
                string[] monsterDeathMessages = {
                    "Your strength fails as darkness claims your vision...",
                    "Your journey ends here, another legend lost to the depths...",
                    "Darkness consumes you as your final breath escapes...",
                    "The dungeon claims another soul for its eternal collection...",
                    "Your blood joins countless others on these ancient stones...",
                    "Your tale ends here, remembered only by these silent walls..."
                };
                string[] deathMessages = monsterDeathMessages;
                Map currentMap = _map[_currentLevel - 1];
                if (currentMap.CheckInBounds(_player.Column, _player.Row) &&
                    currentMap.Tiles[_player.Column, _player.Row].TileType == TileType.Trap)
                {
                    deathMessages = trapDeathMessages;
                }
                int messageCount = Math.Min(2, deathMessages.Length);
                int messageSpacing = 40; 
                for (int i = 0; i < messageCount; i++)
                {                   
                    string deathMessage = deathMessages[Math.Abs((_player.Name.GetHashCode() + _currentLevel + i) % deathMessages.Length)];
                    Monster.DrawTextWithShadow(deathMessage,WINDOW_WIDTH / 2 - Raylib.MeasureText(deathMessage, 32) / 2,250 + (i * messageSpacing), 32,new Raylib_cs.Color(200, 200, 200, 255));
                }
            }
            if (_player != null)
            {
                int centerX = WINDOW_WIDTH / 2;
                int centerY = WINDOW_HEIGHT / 2;
                float scale = 5.0f;
                Vector2 centeredPosition = new Vector2(centerX, centerY);
                _player.RenderBattle(centeredPosition, scale, AnimationType.Death, false, 0f);
            }
            if (_player != null)
            {
                string playerInfo = "Player: " + _player.Name;
                string levelInfo = "Level: " + _player.Level;
                Monster.DrawTextWithShadow(playerInfo, WINDOW_WIDTH / 2 - Raylib.MeasureText(playerInfo, 30) / 2, WINDOW_HEIGHT / 2 + 200, 30, Raylib_cs.Color.White);
                Monster.DrawTextWithShadow(levelInfo, WINDOW_WIDTH / 2 - Raylib.MeasureText(levelInfo, 30) / 2, WINDOW_HEIGHT / 2 + 240, 30, Raylib_cs.Color.White);
            }
            Rectangle menuButton = new Rectangle(WINDOW_WIDTH / 2 - 150, WINDOW_HEIGHT - 150, 300, 70);
            bool menuHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), menuButton);
            Rectangle scaledButton = menuButton;
            if (menuHovered)
            {
                float buttonScale = 1.1f;
                scaledButton = new Rectangle(menuButton.X - (menuButton.Width * (buttonScale - 1)) / 2,menuButton.Y - (menuButton.Height * (buttonScale - 1)) / 2,menuButton.Width * buttonScale,menuButton.Height * buttonScale);
            }
            Raylib.DrawRectangleRounded(new Rectangle(scaledButton.X + 8, scaledButton.Y + 8, scaledButton.Width, scaledButton.Height),0.3f, 6, new Raylib_cs.Color(0, 0, 0, 150));
            Raylib_cs.Color buttonColor = menuHovered ? new Raylib_cs.Color(180, 30, 30, 255) : new Raylib_cs.Color(120, 20, 20, 255);
            Raylib.DrawRectangleRounded(scaledButton, 0.3f, 6, buttonColor);
            Raylib.DrawRectangleRoundedLines(scaledButton,0.3f,6,menuHovered ? new Raylib_cs.Color(255, 200, 200, 255) : new Raylib_cs.Color(200, 100, 100, 255));
            string buttonText = "RETURN TO MAIN MENU";
            int buttonFontSize = menuHovered ? 26 : 24;
            int buttonTextWidth = Raylib.MeasureText(buttonText, buttonFontSize);
            if (menuHovered)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Raylib.DrawText(buttonText,(int)(scaledButton.X + scaledButton.Width / 2 - buttonTextWidth / 2),(int)(scaledButton.Y + scaledButton.Height / 2 - buttonFontSize / 2),buttonFontSize,new Raylib_cs.Color(255, 255, 255, 50 / i));
                }
            }
            Raylib.DrawText(buttonText,(int)(scaledButton.X + scaledButton.Width / 2 - buttonTextWidth / 2),(int)(scaledButton.Y + scaledButton.Height / 2 - buttonFontSize / 2),buttonFontSize,Raylib_cs.Color.White);
            if (menuHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                ResetGame(); 
                ChangeState(GameState.MainMenu);
                LogGameEvent("Navigation", "Game Over -> Main Menu");
            }
        }
        /// <summary>
        /// Private method is used to handle the input for the game over (die) screen.
        /// </summary>
        private void HandleGameOverInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                ResetGame();
                ChangeState(GameState.MainMenu);
                LogGameEvent("Navigation", "Game Over -> Main Menu (keyboard)");
            }
        }
        /// <summary>
        /// Private method is used to handle the equip panel to check if the item is dropped in the correct slot.
        /// </summary>
        private void HandleItemDrop()
        {
            if (_player == null || _draggingEquipment == null)
            {
                _isDragging = false;
                _draggingEquipment = null;
                return;
            }
            Rectangle equipPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 170, 550, 450);
            int slotSize = 80;
            int slotX = (int)equipPanel.X + 30;
            int equipStartY = (int)equipPanel.Y + 50;
            int equipSpacing = slotSize + 15;
            Dictionary<string, System.Drawing.RectangleF> equipSlots = new Dictionary<string, System.Drawing.RectangleF>
            {
                { "Helmet", new System.Drawing.RectangleF(slotX + 210, equipStartY, slotSize, slotSize) },
                { "Weapon", new System.Drawing.RectangleF(slotX + 100, equipStartY + equipSpacing, slotSize, slotSize) },
                { "Chest", new System.Drawing.RectangleF(slotX + 210, equipStartY + equipSpacing + 50, slotSize, slotSize) },
                { "Bracelet", new System.Drawing.RectangleF(slotX + equipSpacing*2 + 130, equipStartY + equipSpacing, slotSize, slotSize) },
                { "Gloves", new System.Drawing.RectangleF(slotX + 100, equipStartY + equipSpacing*2 +20, slotSize, slotSize) },
                { "Ring", new System.Drawing.RectangleF(slotX + equipSpacing*2 +130, equipStartY + equipSpacing*2 +20, slotSize, slotSize) },
                { "Legs", new System.Drawing.RectangleF(slotX + 210, equipStartY + equipSpacing*3, slotSize, slotSize) }
            };
            _player.Inventory.HandleItemDrop(_currentDragPosition, equipSlots);
            _isDragging = false;
            _draggingEquipment = null;
        }
        /// <summary>
        /// Private method to execute the monster's turn during battle.
        /// </summary>
        private void ExecuteMonsterTurn(Monster monster)
        {
            monster.UpdateBuffs();
            _showAttackEffect = false;
            _isPlayerAttacking = false;
            _currentMonsterAttackingType = "";
            if (_player == null){
                return;
            }
            if (monster is Boss boss)
            {
                double hpPercentage = boss.HP / boss.MaxHP;
                if (hpPercentage <= 0.45 && boss.Phase == 1)
                {
                    boss.ChangePhase();
                    _showAttackEffect = true;
                    _attackEffectDuration = 2.0f;
                    _attackEffectPos = new Vector2(3 * WINDOW_WIDTH / 4, WINDOW_HEIGHT / 2);
                    _attackEffectText = "PHASE 2";
                    _attackEffectColor = new Raylib_cs.Color(255, 0, 0, 255);
                    _battleLogText = boss.Name + " enters Phase 2!";
                    _battleLog.Add(_battleLogText);
                    _turnTransitionTimer = 2.0f;
                    return;
                }
                if (boss.TryAction(boss) != string.Empty)
                {
                    _battleLogText = boss.Name + " is buffing itself!";
                    _battleLog.Add(_battleLogText);
                }
            }
            if (monster is Goblin goblin)
            {
                if (goblin.TryAction(_player) != string.Empty)
                {
                    _battleLogText = goblin.Name + " is stealing money from you!";
                    _battleLog.Add(_battleLogText);
                }
            }
            if (monster is Slime slime)
            {
                if (slime.TryAction(_player) != string.Empty)
                {
                    _battleLogText = slime.Name + " is corroding your equipment!";
                    _battleLog.Add(_battleLogText);
                }
            }
            double skillChance = 0.3;
            if (monster is Boss bossMonster && bossMonster.Phase == 2)
            {
                skillChance = 0.6;
            }
            bool useSkill = monster.Skills.Count > 0 && new Random().NextDouble() < skillChance;
            if (useSkill)
            {
                Skill selectedSkill = monster.Skills[new Random().Next(monster.Skills.Count)];
                string battleMessage = "";
                double damage = monster.UseSkill(selectedSkill, _player);
                battleMessage = "" + monster.Name + " uses " + selectedSkill.Name + "";
                _battleLogText = battleMessage;
                _battleLog.Add(battleMessage);
                _showAttackEffect = true;
                _attackEffectDuration = 1.0f;
                _isPlayerAttacking = false;
                if (monster.Name.Contains("Goblin"))
                {
                    if (selectedSkill is AttackSkill)
                    {
                        monster.SetAnimation(AnimationType.AttackSkill);
                        _currentMonsterAttackingType = "GoblinAttackSkill";
                        _currentskillName = selectedSkill.Name.ToUpper();
                        if (_currentskillName == "LEFTRIGHTSPIN")
                        {
                            monster.SetAnimation(AnimationType.AttackStatueSkill);
                        }

                    }
                }
                else if (monster.Name.Contains("Slime"))
                {
                    if (selectedSkill is AttackSkill)
                    {
                        monster.SetAnimation(AnimationType.AttackSkill);
                        _currentMonsterAttackingType = "SlimeAttackSkill";
                        _currentskillName = selectedSkill.Name.ToUpper();

                    }
                }
                else if (monster.Name.Contains("Skeleton"))
                {
                    if (selectedSkill is AttackSkill)
                    {
                        monster.SetAnimation(AnimationType.AttackSkill);
                        _currentMonsterAttackingType = "SkeletonAttackSkill";
                        _currentskillName = selectedSkill.Name.ToUpper();
                        if (_currentskillName == "HEAVYSLASH")
                        {
                            monster.SetAnimation(AnimationType.AttackStatueSkill);
                        }
                    }
                }
            }
            else
            {
                double damage = monster.Attack(_player);
                string battleMessage = "" + monster.Name + " attacks you ";
                _battleLogText = battleMessage;
                _battleLog.Add(battleMessage);
                _showAttackEffect = true;
                _attackEffectDuration = 1.0f;
                _isPlayerAttacking = false;
                _attackEffectPos = new Vector2(WINDOW_WIDTH / 4, WINDOW_HEIGHT / 2);
                _attackEffectText = (damage - _player.Defense).ToString("F2");
                _attackEffectColor = Raylib_cs.Color.Red;
                if (monster.Name.Contains("Boss"))
                {
                    if (monster is Boss boss3)
                    {
                        if (boss3.Phase == 1)
                        {
                            _currentMonsterAttackingType = "Boss";
                        }
                        else
                        {
                            _currentMonsterAttackingType = "Boss";
                        }
                    }
                }
                else if (monster.Name.Contains("Goblin"))
                {
                    monster.SetAnimation(AnimationType.Attack);
                    _currentMonsterAttackingType = "Goblin";
                    _attackEffectDuration = 0.7f;
                }
                else if (monster.Name.Contains("Skeleton"))
                {
                    monster.SetAnimation(AnimationType.Attack);
                    _currentMonsterAttackingType = "Skeleton";
                }
                else if (monster.Name.Contains("Slime"))
                {
                    monster.SetAnimation(AnimationType.Attack);
                    _currentMonsterAttackingType = "Slime";
                    _attackEffectDuration = 0.6f;
                }
            }
            if (_player.HP <= 0)
            {
                _battleLogText = "You have been defeated!";
                _battleLog.Add(_battleLogText);
                _turnTransitionTimer = 2.0f;
                return;
            }
            _turnTransitionTimer = 1.0f;
        }
        /// <summary>
        /// Private method is used to draw the battle screen.
        /// </summary>
        private void DrawBattle()
        {
            if (_player == null) {
                return;
            }
            float time = (float)Raylib.GetTime() * 0.5f;
            float torchFlicker = (float)Math.Sin(time * 2.3f) * 15f + (float)Math.Sin(time * 5.7f) * 5f;
            float slowPulse = (float)Math.Sin(time * 0.4f) * 0.2f + 1.0f; 
            Raylib_cs.Color ceilingColor = new Raylib_cs.Color(25, 22, 38, 255);  
            Raylib_cs.Color wallColor = new Raylib_cs.Color(45, 40, 62, 255);     
            Raylib_cs.Color floorColor = new Raylib_cs.Color(65, 60, 80, 255);   
            for (int i = 0; i < WINDOW_HEIGHT / 3; i += 2)
            {
                float ratio = (float)i / (WINDOW_HEIGHT / 3);
                float lightFactor = ratio * (0.8f + slowPulse * 0.2f);
                int r = (int)Math.Min(ceilingColor.R + (ratio * 25) + (torchFlicker * ratio * 0.4f * lightFactor), 255);
                int g = (int)Math.Min(ceilingColor.G + (ratio * 18) + (torchFlicker * ratio * 0.3f * lightFactor), 255);
                int b = (int)Math.Min(ceilingColor.B + (ratio * 12), 255);
                Raylib.DrawRectangle(0, i, WINDOW_WIDTH, 2, new Raylib_cs.Color(r, g, b, 255));
            }
            for (int i = WINDOW_HEIGHT / 3; i < 2 * WINDOW_HEIGHT / 3; i += 2)
            {
                float heightRatio = (float)(i - WINDOW_HEIGHT / 3) / (WINDOW_HEIGHT / 3);
                float centerEffect = 1.0f - Math.Abs(heightRatio - 0.5f) * 2; 
                float flickerIntensity = torchFlicker * (0.3f + centerEffect * 0.2f);
                int r = (int)Math.Min(wallColor.R + flickerIntensity * 0.6f, 255);
                int g = (int)Math.Min(wallColor.G + flickerIntensity * 0.4f, 255);
                int b = (int)Math.Min(wallColor.B + flickerIntensity * 0.2f, 255);
                Raylib.DrawRectangle(0, i, WINDOW_WIDTH, 2, new Raylib_cs.Color(r, g, b, 255));
            }
            for (int i = 2 * WINDOW_HEIGHT / 3; i < WINDOW_HEIGHT; i += 2)
            {
                float ratio = (float)(i - 2 * WINDOW_HEIGHT / 3) / (WINDOW_HEIGHT / 3);
                float reflectionFactor = Math.Max(0, 1.0f - ratio * 2.5f);
                int r = (int)Math.Clamp(floorColor.R - (ratio * 35) + torchFlicker * 0.15f * reflectionFactor, 25, 255);
                int g = (int)Math.Clamp(floorColor.G - (ratio * 35) + torchFlicker * 0.1f * reflectionFactor, 20, 255);
                int b = (int)Math.Clamp(floorColor.B - (ratio * 35), 35, 255);
                Raylib.DrawRectangle(0, i, WINDOW_WIDTH, 2, new Raylib_cs.Color(r, g, b, 255));
            }
            int brickWidth = 60;
            int brickHeight = 30;
            Raylib_cs.Color brickLineColor = new Raylib_cs.Color(35, 32, 45, 255);
            for (int x = 0; x < WINDOW_WIDTH; x += brickWidth)
            {
                Raylib.DrawLine(x, WINDOW_HEIGHT / 3, x, 2 * WINDOW_HEIGHT / 3, brickLineColor);
            }
            for (int y = WINDOW_HEIGHT / 3; y < 2 * WINDOW_HEIGHT / 3; y += brickHeight)
            {
                Raylib.DrawLine(0, y, WINDOW_WIDTH, y, brickLineColor);
                int offsetRow = ((y - WINDOW_HEIGHT / 3) / brickHeight) % 2;
                if (offsetRow == 1)
                {
                    for (int x = brickWidth / 2; x < WINDOW_WIDTH; x += brickWidth)
                    {
                        Raylib.DrawLine(x, y, x, y + brickHeight, brickLineColor);
                    }
                }
            }
            Monster? currentMonster = null;
            if (_player != null)
            {
                foreach (Unit unit in _units)
                {
                    if (unit is Monster monster &&
                        monster.IsAlive &&
                        monster.Column == _player.Column &&
                        monster.Row == _player.Row)
                    {
                        currentMonster = monster;
                        break;
                    }
                }
            }
            if (currentMonster != null)
            {
                Monster.DrawBattleUI(currentMonster, WINDOW_WIDTH, WINDOW_HEIGHT, _currentLevel, _isPlayerTurn, _battleLog);
            }
            int playerX = WINDOW_WIDTH / 4;
            int playerY = WINDOW_HEIGHT / 2;
            int playerSize = 200;
            if (_escapeAttemptInProgress)
            {
                _escapeAnimationTimer -= Raylib.GetFrameTime();
                if (_escapeAnimationTimer <= 0)
                {
                    _escapeAttemptInProgress = false;
                    _showAttackEffect = false;
                    if (_escapeSuccessful)
                    {
                        string successMessage = "Escaped successfully!";
                        _battleLogText = successMessage;
                        _battleLog.Add(successMessage);
                        LogGameEvent("Battle", "Player escaped battle successfully");
                        _currentPlayerAttackingType = "";
                        _isPlayerAttacking = false;
                        _showAttackEffect = false;
                        if (_player != null && _map.Count > 0)
                        {
                            Map currentMap = _map[_currentLevel - 1];
                            int newColumn = _player.Column - 1;
                            if (currentMap.CheckInBounds(newColumn, _player.Row) &&
                                currentMap.Tiles[newColumn, _player.Row].IsWalkable)
                            {
                                _player.Column = newColumn;
                            }
                            else if (currentMap.CheckInBounds(_player.Column, _player.Row - 1) &&
                                    currentMap.Tiles[_player.Column, _player.Row - 1].IsWalkable)
                            {
                                _player.Row -= 1;
                            }
                            else if (currentMap.CheckInBounds(_player.Column, _player.Row + 1) &&
                                    currentMap.Tiles[_player.Column, _player.Row + 1].IsWalkable)
                            {
                                _player.Row += 1;
                            }
                            else if (currentMap.CheckInBounds(_player.Column + 1, _player.Row) &&
                                    currentMap.Tiles[_player.Column + 1, _player.Row].IsWalkable)
                            {
                                _player.Column += 1;
                            }
                            if (currentMap.CheckInBounds(_player.Column, _player.Row))
                            {
                                currentMap.Tiles[_player.Column, _player.Row].TileType = TileType.Empty;
                                currentMap.Tiles[_player.Column, _player.Row].IsWalkable = true;
                            }
                        }
                        _turnTransitionTimer = 0.5f;
                        ChangeState(GameState.Exploring);
                        _escapeSuccessful = false;
                    }
                    else
                    {
                        string failMessage = "Failed to escape!";
                        _battleLogText = failMessage;
                        _battleLog.Add(failMessage);
                        LogGameEvent("Battle", "Player failed to escape battle");                       
                        _showAttackEffect = false;
                        _isPlayerAttacking = false;
                        if (_player != null)
                        {
                            _player.SetAnimation(AnimationType.Idle);
                        }
                        foreach (Unit unit in _units)
                        {
                            if (unit is Monster monster && monster.IsAlive)
                            {
                                monster.SetAnimation(AnimationType.Idle);
                            }
                        }
                        _currentPlayerAttackingType = "";
                        _currentMonsterAttackingType = "";
                        _isPlayerTurn = false;
                        _turnTransitionTimer = 0.5f;
                        _isTurnInProgress = false;
                    }
                }
                else
                {
                    if (_player != null)
                    {
                        float progress = 1.0f - (_escapeAnimationTimer / 1.5f);
                        float scale = 200 / 100f * 4.5f; 
                        Vector2 playerPosition = new Vector2(
                            WINDOW_WIDTH / 4,
                            WINDOW_HEIGHT / 2
                        );
                        _player.RenderEscape(playerPosition, scale, progress, _escapeSuccessful);
                    }
                }
            }
            else
            {            
                if (_player != null)
                {
                    AnimationType battleAnimation = AnimationType.Idle;
                    if (_showAttackEffect && _isPlayerAttacking && _currentPlayerAttackingType != "Item")
                    {
                        if (_currentPlayerAttackingType == "AttackSkill")
                        {
                            battleAnimation = AnimationType.AttackSkill;
                        }
                        else if (_currentPlayerAttackingType == "AttackStatusSkill")
                        {
                            battleAnimation = AnimationType.AttackStatueSkill;
                        }
                        else if (_currentPlayerAttackingType == "StatusSkill")
                        {
                            battleAnimation = AnimationType.StatueSkill;
                        }
                        else if (_currentPlayerAttackingType == "Escape")
                        {
                            battleAnimation = AnimationType.Walk;
                        }
                        else
                        {
                            battleAnimation = AnimationType.Attack;
                        }
                    }
                    float attackProgress = 0f;
                    if (_showAttackEffect && _isPlayerAttacking && _currentPlayerAttackingType != "StatusSkill" && _currentPlayerAttackingType != "Escape" && _currentPlayerAttackingType != "Item")
                    {
                        attackProgress = Math.Max(0, Math.Min(1, 1 - (_attackEffectDuration / 1.5f)));
                    }
                    float scale = (playerSize / 100f) * 4.5f;
                    _player.RenderBattle(new Vector2(playerX, playerY),scale,battleAnimation,_showAttackEffect && _isPlayerAttacking,attackProgress);
                }
                if (_player != null)
                {
                    Monster.DrawTextWithGlow("(Lvl " + _player.Level + ") " + _player.Name,playerX - 100,playerY - playerSize / 2 - 40,24,Raylib_cs.Color.White,new Raylib_cs.Color(150, 150, 200, 100));
                    double playerHealthPercent = _player.HP / _player.MaxHP;
                    Monster.DrawEnhancedBar(playerX - playerSize / 2,playerY + playerSize / 2 + 20,playerSize,30,playerHealthPercent,new Raylib_cs.Color(200, 50, 50, 255),"HP: " + _player.HP.ToString("F2") + "/" + _player.MaxHP.ToString("F2"));
                    double playerManaPercent = _player.Mana / _player.MaxMana;
                    Monster.DrawEnhancedBar(playerX - playerSize / 2,playerY + playerSize / 2 + 55,playerSize,20,playerManaPercent,new Raylib_cs.Color(50, 50, 200, 255),"MP: " + _player.Mana.ToString("F2") + "/" + _player.MaxMana.ToString("F2"));
                }
                int monsterX = (WINDOW_WIDTH / 4) * 3;
                int monsterY = WINDOW_HEIGHT / 2;
                int monsterSize = 200;
                if (currentMonster != null)
                {                    
                    if (currentMonster.Name.Contains("Goblin"))
                    {
                        AnimationType battleAnimation = AnimationType.Idle;
                        if (_showAttackEffect && !_isPlayerAttacking)
                        {
                            if (_currentMonsterAttackingType == "GoblinAttackSkill")
                            {
                                battleAnimation = _currentskillName == "LEFTRIGHTSPIN" ? AnimationType.AttackStatueSkill : AnimationType.AttackSkill;
                            }
                            else if (_currentMonsterAttackingType == "Goblin")
                            {
                                battleAnimation = AnimationType.Attack;
                            }
                        }
                        float attackProgress = 0f;
                        if (_showAttackEffect && !_isPlayerAttacking && (_currentMonsterAttackingType == "Goblin" || _currentMonsterAttackingType == "GoblinAttackSkill"))
                        {
                            attackProgress = Math.Max(0, Math.Min(1, 1 - (_attackEffectDuration / 1.5f)));
                        }
                        currentMonster.RenderBattle(new Vector2(monsterX, monsterY),battleAnimation,"battle",_showAttackEffect && !_isPlayerAttacking,_currentMonsterAttackingType,_currentskillName,attackProgress);
                    }
                    else if (currentMonster.Name.Contains("Skeleton"))
                    {
                        AnimationType battleAnimation = AnimationType.Idle;
                        if (_showAttackEffect && !_isPlayerAttacking)
                        {
                            if (_currentMonsterAttackingType == "SkeletonAttackSkill")
                            {
                                battleAnimation = _currentskillName == "HEAVYSLASH" ? AnimationType.AttackStatueSkill : AnimationType.AttackSkill;
                            }
                            else if (_currentMonsterAttackingType == "Skeleton")
                            {
                                battleAnimation = AnimationType.Attack;
                            }
                        }
                        float attackProgress = 0f;
                        if (_showAttackEffect && !_isPlayerAttacking && (_currentMonsterAttackingType == "Skeleton" || _currentMonsterAttackingType == "SkeletonAttackSkill"))
                        {
                            attackProgress = Math.Max(0, Math.Min(1, 1 - (_attackEffectDuration / 1.5f)));
                        }
                        currentMonster.RenderBattle(new Vector2(monsterX, monsterY),battleAnimation,"battle",_showAttackEffect && !_isPlayerAttacking,_currentMonsterAttackingType,_currentskillName,attackProgress);
                    }                 
                    else if (currentMonster.Name.Contains("Slime"))
                    {
                        AnimationType battleAnimation = AnimationType.Idle;
                        if (_showAttackEffect && !_isPlayerAttacking)
                        {
                            if (_currentMonsterAttackingType == "SlimeAttackSkill")
                            {
                                battleAnimation = AnimationType.AttackSkill;
                            }
                            else if (_currentMonsterAttackingType == "Slime")
                            {
                                battleAnimation = AnimationType.Attack;
                            }
                        }
                        float attackProgress = 0f;
                        if (_showAttackEffect && !_isPlayerAttacking && (_currentMonsterAttackingType == "Slime" || _currentMonsterAttackingType == "SlimeAttackSkill"))
                        {
                            attackProgress = Math.Max(0, Math.Min(1, 1 - (_attackEffectDuration / 1.5f)));
                        }

                        currentMonster.RenderBattle(new Vector2(monsterX, monsterY), battleAnimation,"battle",_showAttackEffect && !_isPlayerAttacking,_currentMonsterAttackingType,_currentskillName,attackProgress);
                    }
                    else if (currentMonster.Name.Contains("Boss") && currentMonster is Boss boss)
                    {
                        AnimationType battleAnimation = AnimationType.Idle;
                        if (_showAttackEffect && !_isPlayerAttacking)
                        {
                            if (_currentMonsterAttackingType == "Boss" || _currentMonsterAttackingType == "BossAttackSkill")
                            {
                                battleAnimation = AnimationType.Attack;
                            }
                        }
                        float attackProgress = 0f;
                        if (_showAttackEffect && !_isPlayerAttacking && (_currentMonsterAttackingType == "Boss" || _currentMonsterAttackingType == "BossAttackSkill"))
                        {
                            attackProgress = Math.Max(0, Math.Min(1, 1 - (_attackEffectDuration / 1.5f)));
                        }
                        currentMonster.RenderBattle(new Vector2(monsterX, monsterY),battleAnimation,"battle",_showAttackEffect && !_isPlayerAttacking, _currentMonsterAttackingType, _currentskillName, attackProgress );
                    }
                    Monster.DrawTextWithShadow("Lvl " + currentMonster.Level.ToString() + " " + currentMonster.Name,  monsterX - 120, monsterY - monsterSize / 2 - 40, 24,Raylib_cs.Color.White );                  
                    double monsterHealthPercent = currentMonster.HP / currentMonster.MaxHP;
                    Monster.DrawEnhancedBar(monsterX - monsterSize / 2, monsterY + monsterSize / 2 + 20,monsterSize, 30, monsterHealthPercent, new Raylib_cs.Color(200, 50, 50, 255), "HP: " + currentMonster.HP.ToString("F2") + "/" + currentMonster.MaxHP.ToString("F2") );
                }
                else
                {
                    if (_previousDeathMonster != null)
                    {                        
                        _previousDeathMonster.SetAnimation(AnimationType.Death);
                        _previousDeathMonster.RenderBattle(  new Vector2(monsterX, monsterY),  AnimationType.Death,  "battle" );                       
                        Monster.DrawTextWithShadow( "Lvl " + _previousDeathMonster.Level.ToString() + " " + _previousDeathMonster.Name + " DEFEATED", monsterX - 200, monsterY - monsterSize / 2 - 40, 24,  Raylib_cs.Color.Red  );
                    }
                }
                if (_showVictoryScreen && _previousDeathMonster != null)
                {
                    Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, 100));
                    Rectangle panel = new Rectangle(WINDOW_WIDTH / 2 - 300, WINDOW_HEIGHT / 2 - 300, 600, 600);
                    Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(40, 40, 60, 200));
                    Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Gold);                    // Title
                    Monster.DrawTextWithGlow("VICTORY!", WINDOW_WIDTH / 2 - 120, (int)panel.Y + 40, 60, Raylib_cs.Color.Gold, new Raylib_cs.Color(150, 150, 0, 100));                    
                    string monsterName = "Defeated: " + _previousDeathMonster.Name + " (Level " + _previousDeathMonster.Level + ")";
                    Monster.DrawTextWithShadow(monsterName, (int)panel.X + 50, (int)panel.Y + 120, 24, Raylib_cs.Color.White);                
                    string expText = "Experience gained: " + _previousDeathMonster.ExpReward;
                    Monster.DrawTextWithShadow(expText, (int)panel.X + 50, (int)panel.Y + 160, 24, Raylib_cs.Color.Green);
                    string moneyText = "Gold earned: " + _previousDeathMonster.MoneyReward;
                    Monster.DrawTextWithShadow(moneyText, (int)panel.X + 50, (int)panel.Y + 190, 24, Raylib_cs.Color.Gold);
                    Monster.DrawTextWithShadow("Items dropped:", (int)panel.X + 50, (int)panel.Y + 230, 24, Raylib_cs.Color.White);                  
                    int itemY = (int)panel.Y + 270;
                    if (_previousDeathMonster?.DropList?.Count > 0)
                    {
                        foreach (Item item in _previousDeathMonster.DropList)
                        {
                            Raylib_cs.Color itemColor = Raylib_cs.Color.White;
                            if (item is Weapon) itemColor = Raylib_cs.Color.Red;
                            else if (item is Armor) itemColor = Raylib_cs.Color.Blue;
                            else if (item is Potion) itemColor = Raylib_cs.Color.Green;                           
                            Monster.DrawTextWithShadow("- " + item.Name, (int)panel.X + 70, itemY, 20, itemColor);
                            itemY += 30;
                        }
                    }
                    else
                    {                       
                        Monster.DrawTextWithShadow("No items dropped", (int)panel.X + 70, itemY, 20, Raylib_cs.Color.Gray);
                    }                   
                    if ((int)(Raylib.GetTime() * 2) % 2 == 0)
                    {
                        Monster.DrawTextWithShadow("Click anywhere to continue", WINDOW_WIDTH / 2 - 150, (int)panel.Y + 550, 20, Raylib_cs.Color.LightGray);
                    }
                    return;
                }
                if (_showSkillSelection && _player != null)
                {
                    Monster.DrawSkillSelectionUI(_player, WINDOW_WIDTH, WINDOW_HEIGHT, _selectedSkillIndex);
                }
                if (_showItemSelection && _player != null)
                {
                    Monster.DrawItemSelectionUI(_player, WINDOW_WIDTH, WINDOW_HEIGHT, _battleItemScrollPosition);
                }
            }
        }
        /// <summary>
        /// Private method is used to handle skill selection input during battle.
        /// </summary>
        private void HandleSkillSelectionInput()
        {
            if (_player == null)
            {
                return;
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                int panelWidth = 800;
                int panelHeight = 600;
                Rectangle panel = new Rectangle(WINDOW_WIDTH / 2 - panelWidth / 2,WINDOW_HEIGHT / 2 - panelHeight / 2,panelWidth,panelHeight);
                Rectangle closeButton = new Rectangle(panel.X + panel.Width - 60, panel.Y + 20, 40, 40);
                if (Raylib.CheckCollisionPointRec(mousePos, closeButton))
                {
                    _showSkillSelection = false;
                    _selectedSkillIndex = -1;
                    _currentPlayerAttackingType = "";
                    _isTurnInProgress = false;
                    return;
                }
                Rectangle skillsArea = new Rectangle(panel.X + 20, panel.Y + 80, panel.Width - 40, panel.Height - 100);
                if (_player.Skills.Count > 0)
                {
                    int skillY = (int)skillsArea.Y + 20;
                    int skillButtonHeight = 80;
                    int skillSpacing = 100;
                    for (int i = 0; i < _player.Skills.Count; i++)
                    {
                        Skill skill = _player.Skills[i];
                        Rectangle skillButton = new Rectangle(
                            skillsArea.X + 20,
                            skillY + (i * skillSpacing),
                            skillsArea.Width - 40,
                            skillButtonHeight
                        );
                        if (Raylib.CheckCollisionPointRec(mousePos, skillButton))
                        {
                            bool isOnCooldown = skill.Cooldown > 0;
                            bool hasEnoughMana = _player.Mana >= skill.ManaCost;
                            if (!isOnCooldown && hasEnoughMana)
                            {
                                _selectedSkillIndex = i;
                                UseSelectedSkill();
                                _showSkillSelection = false;
                            }
                            else if (isOnCooldown)
                            {
                                _battleLog.Add(skill.Name + " is on cooldown for " + skill.Cooldown + " more turns.");
                            }
                            else if (!hasEnoughMana)
                            {
                                _battleLog.Add("Not enough mana to use " + skill.Name + ".");
                            }
                            else
                            {
                                _player.UpdateBuffs();
                                _player.UpdateSkillCooldowns();
                            }
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Private method is used to perform the selected skill action.
        /// </summary>
        private void UseSelectedSkill()
        {
            if (_player == null || _selectedSkillIndex < 0 || _selectedSkillIndex >= _player.Skills.Count) { 
                return;
            }
            Skill selectedSkill = _player.Skills[_selectedSkillIndex];
            Monster? currentMonster = null;
            foreach (Unit unit in _units)
            {
                if (unit is Monster monster &&
                    monster.IsAlive &&
                    monster.Column == _player.Column &&
                    monster.Row == _player.Row)
                {
                    currentMonster = monster;
                    break;
                }
            }
            if (currentMonster != null)
            {
                double damage = _player.UseSkill(selectedSkill, currentMonster);
                selectedSkill.Cooldown = selectedSkill.Duration;
                string message = "You used " + selectedSkill.Name + " on " + currentMonster.Name;
                _battleLogText = message;
                _battleLog.Add(message);
                LogGameEvent("Battle", "Player used skill " + selectedSkill.Name);
                if (_player.Inventory != null && _player.Inventory.GetEquippedItems().Count > 0)
                {
                    foreach (Equipment equipment in _player.Inventory.GetEquippedItems())
                    {
                        int originalDurability = equipment.Durability;
                        equipment.Durability = Math.Max(0, equipment.Durability - 5);
                        if (equipment.Durability == 0 && originalDurability > 0)
                        {
                            _player.Inventory.RemoveItem(equipment);
                        }
                    }
                }
                if (selectedSkill is AttackSkill)
                {
                    _currentPlayerAttackingType = "AttackSkill";
                }
                else if (selectedSkill is AttackStatusSkill)
                {
                    _currentPlayerAttackingType = "AttackStatusSkill";
                }
                else if (selectedSkill is StatusSkill)
                {
                    _currentPlayerAttackingType = "StatusSkill";
                }
                _isTurnInProgress = true;
                _showAttackEffect = true;
                _attackEffectDuration = 1.0f;
                _isPlayerAttacking = true;
                _attackEffectPos = new Vector2(3 * WINDOW_WIDTH / 4, WINDOW_HEIGHT / 2);
                _attackEffectText = damage.ToString("F2");
                _attackEffectColor = Raylib_cs.Color.Red;
            }
            _selectedSkillIndex = -1;
        }
        /// <summary>
        /// Private method is used to handle the battle input during the player's turn.
        /// </summary>
        private void HandleBattleInput()
        {
            Monster? currentMonster = null;
            if (_player != null)
            {
                foreach (Unit unit in _units)
                {
                    if (unit is Monster monster && monster.IsAlive && monster.Column == _player.Column && monster.Row == _player.Row)
                    {
                        currentMonster = monster;
                        break;
                    }
                }
            }
            if (currentMonster != null)
            {
                _previousDeathMonster = currentMonster;
            }
            if (_previousDeathMonster is Skeleton skeleton)
            {
                if (skeleton.TryAction(skeleton) != string.Empty)
                {
                    currentMonster = skeleton;
                    _battleLogText = "Skeleton revived!";
                    _battleLog.Add(_battleLogText);
                    _showVictoryScreen = false;
                }
            }
            int buttonWidth = 200;
            int buttonHeight = 80;
            int centerX = WINDOW_WIDTH / 2;
            int centerY = WINDOW_HEIGHT / 2 + 460; 
            int spacing = 20;
            Rectangle attackButton = new Rectangle(centerX - buttonWidth - spacing / 2,  centerY - buttonHeight - spacing / 2,buttonWidth,buttonHeight);
            Rectangle skillButton = new Rectangle(centerX + spacing / 2, centerY - buttonHeight - spacing / 2,buttonWidth,buttonHeight);
            Rectangle itemButton = new Rectangle(centerX - buttonWidth - spacing / 2,  centerY + spacing / 2,buttonWidth,buttonHeight);
            Rectangle escapeButton = new Rectangle(centerX + spacing / 2, centerY + spacing / 2,buttonWidth,buttonHeight);
            if (_showSkillSelection)
            {
                HandleSkillSelectionInput();
                return;
            }
            if (_showItemSelection)
            {
                HandleItemSelectionInput();
                return;
            }
            if (_isPlayerTurn && !_isTurnInProgress)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 mousePos = Raylib.GetMousePosition();                    
                    if (Raylib.CheckCollisionPointRec(mousePos, attackButton))
                    {
                        _isTurnInProgress = true;
                        _currentTurnCharacter = "Player";
                        if (_player != null && currentMonster != null)
                        {
                            double damage = _player.Attack(currentMonster);
                            string attackMessage = "You attacked " + currentMonster.Name;
                            _battleLogText = attackMessage;
                            _battleLog.Add(attackMessage);
                            LogGameEvent("Battle", "Player attacked " + currentMonster.Name);
                            if (_player.Inventory != null && _player.Inventory.GetEquippedItems().Count > 0)
                            {
                                foreach (Equipment equipment in _player.Inventory.GetEquippedItems())
                                {
                                    int originalDurability = equipment.Durability;
                                    equipment.Durability = Math.Max(0, equipment.Durability - 5);
                                    if (equipment.Durability == 0 && originalDurability > 0)
                                    {
                                        _battleLog.Add(equipment.Name + " broke during battle!");
                                        _player.Inventory.RemoveItem(equipment);
                                    }
                                }
                            }
                            _showAttackEffect = true;
                            _attackEffectDuration = 1.0f;
                            _isPlayerAttacking = true;
                            _attackEffectPos = new Vector2(3 * WINDOW_WIDTH / 4, WINDOW_HEIGHT / 2);
                            _attackEffectText = (damage - currentMonster.Defense).ToString("F2");
                            _attackEffectColor = Raylib_cs.Color.Red;
                            _player.UpdateBuffs();
                            _player.UpdateSkillCooldowns();
                        }
                    }
                    else if (Raylib.CheckCollisionPointRec(mousePos, skillButton))
                    {
                        if (_player != null)
                        {
                            _showSkillSelection = true;
                        }
                    }
                    else if (Raylib.CheckCollisionPointRec(mousePos, itemButton))
                    {
                        string itemMessage = "Opening item menu...";
                        _battleLogText = itemMessage;
                        _battleLog.Add(itemMessage);
                        LogGameEvent("Battle", "Player opened item menu");
                        _showItemSelection = true;
                        _currentPlayerAttackingType = "Item";
                        if (_player != null)
                        {
                            _player.UpdateBuffs();
                            _player.UpdateSkillCooldowns();
                        }
                    }
                    else if (Raylib.CheckCollisionPointRec(mousePos, escapeButton))
                    {
                        if (_player != null && currentMonster != null)
                        {
                            double speedDifference = _player.Speed - currentMonster.Speed;
                            double escapeChance = 0.4 + (speedDifference * 0.05);
                            escapeChance = Math.Clamp(escapeChance, 0.1, 0.9); 
                            _escapeSuccessful = new Random().NextDouble() < escapeChance;
                            _currentPlayerAttackingType = "Escape";
                            _escapeAttemptInProgress = true;
                            _escapeAnimationTimer = 1.5f;
                            _isTurnInProgress = true;
                            string attemptMessage = "Try to escape";
                            _battleLogText = attemptMessage; _battleLog.Add(attemptMessage);
                            LogGameEvent("Battle", "Player attempting to escape");
                            if (_player != null)
                            {
                                _player.UpdateBuffs();
                                _player.UpdateSkillCooldowns();
                            }
                        }
                    }
                }
                if (_showVictoryScreen && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (_previousDeathMonster != null)
                    {
                        _units.Remove(_previousDeathMonster);
                    }
                    _showVictoryScreen = false;
                    ChangeState(GameState.Exploring);
                }
            }
            if (_showAttackEffect)
            {
                _attackEffectDuration -= Raylib.GetFrameTime();
                if (_attackEffectDuration <= 0)
                {
                    _showAttackEffect = false;
                    if (_isPlayerAttacking)
                    {
                        _isPlayerAttacking = false;
                        _isPlayerTurn = false;
                        _currentPlayerAttackingType = "";
                        _turnTransitionTimer = _turnDelay; 
                    }
                    else
                    {
                        _isPlayerTurn = true;
                    }
                    _isTurnInProgress = false;
                    if (currentMonster != null && !currentMonster.IsAlive)
                    {
                        _turnTransitionTimer = 1.0f;
                    }
                    else if (_currentLevel == 3 && _previousDeathMonster is Boss && currentMonster != null)
                    {
                        _victoryCreditsTransition = true;
                    }
                    else if (currentMonster == null && _player != null)
                    {
                        _showVictoryScreen = true;
                        if (!_victoryRewardsGranted && _previousDeathMonster != null && _player != null)
                        {
                            _player?.AddExp(_previousDeathMonster?.ExpReward ?? 0);
                            if (_player?.Inventory != null)
                            {
                                _player.Inventory.Money += _previousDeathMonster?.MoneyReward ?? 0;
                            }
                            if (_previousDeathMonster?.DropList != null && _player?.Inventory != null)
                            {
                                foreach (Item item in _previousDeathMonster.DropList)
                                {
                                    _player.Inventory.AddItem(item);
                                }
                            }
                            string victoryMessage = "You defeated " + (_previousDeathMonster?.Name ?? "Unknown Enemy");
                            _battleLogText = victoryMessage;
                            _battleLog.Add(victoryMessage);
                            _victoryRewardsGranted = true;
                        }

                        _turnTransitionTimer = 2.0f;
                    }

                }
            }
            if (!_isPlayerTurn && !_isTurnInProgress && _turnTransitionTimer <= 0 && currentMonster != null)
            {
                _isTurnInProgress = true;
                _currentTurnCharacter = currentMonster.Name;
                ExecuteMonsterTurn(currentMonster);
            }
            if (_turnTransitionTimer > 0)
            {
                _turnTransitionTimer -= Raylib.GetFrameTime();

            }
            if (_showVictoryScreen && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (_previousDeathMonster != null)
                {
                    _units.Remove(_previousDeathMonster);
                    if (_map.Count > 0 && _previousDeathMonster != null)
                    {
                        Map currentMap = _map[_currentLevel - 1];
                        if (currentMap.CheckInBounds(_previousDeathMonster.Column, _previousDeathMonster.Row))
                        {
                            currentMap.Tiles[_previousDeathMonster.Column, _previousDeathMonster.Row].TileType = TileType.Empty;
                            currentMap.Tiles[_previousDeathMonster.Column, _previousDeathMonster.Row].IsWalkable = true;
                        }
                    }
                }
                _showVictoryScreen = false;
                _victoryRewardsGranted = false;
                if (_victoryCreditsTransition)
                {
                    _victoryCreditsTransition = false;
                    _creditsScrollY = WINDOW_HEIGHT;
                    ChangeState(GameState.Credit);
                    LogGameEvent("Game", "Showing end game credits after boss defeat");
                }
                else
                {
                    ChangeState(GameState.Exploring);
                }
            }
        }
        /// <summary>
        /// Public method is used to draw the current scene based on the game state.
        /// </summary>
        public void DrawCurrentState()
        {
            try
            {
                try
                {
                    switch (_currentState)
                    {
                        case GameState.MainMenu:
                            DrawMainMenu();
                            break;
                        case GameState.CharacterSelection:
                            DrawCharacterSelection();
                            break;
                        case GameState.NameInput:
                            DrawNameInput();
                            break;
                        case GameState.StoryIntro:
                            DrawStoryIntro();
                            break;
                        case GameState.Exploring:
                            DrawGame();
                            break;
                        case GameState.Inventory:
                            DrawInventory();
                            break;
                        case GameState.InBattle:
                            DrawBattle();
                            break;
                        case GameState.GameOver:
                            DrawGameOver();
                            break;
                        case GameState.Credit:
                            DrawCredits();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing current state: " + ex.Message);
                }
                if (_inShopInteraction && _currentMerchant != null && _player != null)
                {
                    _currentMerchant.DrawUI(_player, this);
                }
                if (_inBlacksmithInteraction && _currentBlacksmith != null && _player != null)
                {
                    _currentBlacksmith.DrawUI(_player, this);
                }
                try
                {
                    DrawSettingsButton();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing settings button: " + ex.Message);
                }
                try
                {
                    DrawSettingsPanel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing settings panel: " + ex.Message);
                }
                try
                {
                    DrawNotifications();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing notifications: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Critical UI rendering error: " + ex.Message);
            }
        }
        /// <summary>
        /// Private method to draw the main menu UI.
        /// </summary>
        private void DrawMainMenu()
        {
            try
            {
                Raylib.DrawText("DUNGEON ADVENTURE", WINDOW_WIDTH / 2 - 300, 200, 60, Raylib_cs.Color.Gold);
                Rectangle startButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2, 400, 60);
                Rectangle exitButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2 + 150, 400, 60);
                bool startHovered = false;
                bool exitHovered = false;
                try
                {
                    startHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), startButton);
                    exitHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), exitButton);
                }
                catch (Exception mouseEx)
                {
                    Console.WriteLine("Mouse position error: " + mouseEx.Message);
                }
                Raylib.DrawRectangleRec(startButton, startHovered ? Raylib_cs.Color.Gold : Raylib_cs.Color.DarkGray);
                Raylib.DrawRectangleLinesEx(startButton, 3, Raylib_cs.Color.White);
                Raylib.DrawRectangleRec(exitButton, exitHovered ? Raylib_cs.Color.Red : Raylib_cs.Color.DarkGray);
                Raylib.DrawRectangleLinesEx(exitButton, 3, Raylib_cs.Color.White);
                try
                {
                    Raylib.DrawText("START GAME", (int)startButton.X + 60, (int)startButton.Y + 15, 40, Raylib_cs.Color.White);
                }
                catch (Exception textEx)
                {
                    Console.WriteLine("Error drawing start button text: " + textEx.Message);
                }
                try
                {
                    Raylib.DrawText("EXIT", (int)exitButton.X + 150, (int)exitButton.Y + 15, 40, Raylib_cs.Color.White);
                }
                catch (Exception textEx)
                {
                    Console.WriteLine("Error drawing exit button text: " + textEx.Message);
                    Environment.Exit(0);
                }
                try
                {
                    Raylib.DrawText("Created by: Andrew Teck Foon YII (104386568)", 20, WINDOW_HEIGHT - 30, 20, Raylib_cs.Color.Gray);
                }
                catch (Exception textEx)
                {
                    Console.WriteLine("Error drawing info text: " + textEx.Message);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Rectangle startButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2, 400, 60);
                    Rectangle exitButton = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2 + 150, 400, 60);
                    Raylib.DrawRectangleRec(startButton, Raylib_cs.Color.DarkGray);
                    Raylib.DrawRectangleLinesEx(startButton, 5, Raylib_cs.Color.White);
                    Raylib.DrawRectangleRec(exitButton, Raylib_cs.Color.DarkGray);
                    Raylib.DrawRectangleLinesEx(exitButton, 5, Raylib_cs.Color.Red);
                }
                catch
                {
                    Environment.Exit(0);
                }
                Console.WriteLine("Error in DrawMainMenu: " + ex.Message);
            }
        }
        /// <summary>
        /// Private method to draw the character selection screen.
        /// </summary>
        private void DrawCharacterSelection()
        {
            Raylib.DrawText("SELECT YOUR CHARACTER", WINDOW_WIDTH / 2 - 350, 100, 60, Raylib_cs.Color.Gold);
            float arrowBlinkTime = (float)(Raylib.GetTime() % 1.0);
            bool showArrow = arrowBlinkTime > 0.5f;              
            if (_characterSelectionPlayers != null)
            {
                foreach (Player player in _characterSelectionPlayers)
                {
                    if (player != null)
                    {
                        player.UpdateAnimation();
                    }
                }
            }

            for (int i = 0; i < _characterRoles.Length; i++)
            {
                int xPos = WINDOW_WIDTH / 2 - 300 + (i * 300);
                Raylib.DrawRectangle(xPos - 100, WINDOW_HEIGHT / 2 - 150, 200, 300,
                i == _selectedCharacterIndex ? Raylib_cs.Color.White : Raylib_cs.Color.DarkGray);
                Rectangle characterImageRect = new Rectangle(xPos - 75, WINDOW_HEIGHT / 2 - 125, 150, 250);
                Raylib.DrawRectangleRec(characterImageRect, Raylib_cs.Color.DarkGray);
                if (_characterSelectionPlayers != null && i < _characterSelectionPlayers.Length && _characterSelectionPlayers[i] != null)
                {
                    Player player = _characterSelectionPlayers[i];

                    if (i == _selectedCharacterIndex)
                    {
                        player.SetAnimation(AnimationType.Idle);
                    }
                    else
                    {
                        player.SetAnimation(AnimationType.Walk);
                    }
                    Vector2 centerPosition = new Vector2(characterImageRect.X + characterImageRect.Width / 2, characterImageRect.Y + characterImageRect.Height / 2);
                    float scale = Math.Min(characterImageRect.Width / 100f, characterImageRect.Height / 150f) * 3.0f;
                    player.Render(centerPosition, scale, false);
                }
                Raylib.DrawText(_characterRoles[i], xPos - 80, WINDOW_HEIGHT / 2 + 200, 30,
                i == _selectedCharacterIndex ? Raylib_cs.Color.Yellow : Raylib_cs.Color.White);
                if (i == _selectedCharacterIndex && showArrow)
                {
                    Raylib.DrawText("<", xPos - 130, WINDOW_HEIGHT / 2, 40, Raylib_cs.Color.White);
                    Raylib.DrawText(">", xPos + 110, WINDOW_HEIGHT / 2, 40, Raylib_cs.Color.White);
                }
            }
            Raylib.DrawText("Use LEFT/RIGHT arrows or click to select", WINDOW_WIDTH / 2 - 310, WINDOW_HEIGHT - 200, 30, Raylib_cs.Color.White);
            Rectangle confirmButton = new Rectangle(WINDOW_WIDTH / 2 - 350, WINDOW_HEIGHT - 140, 250, 50);
            bool confirmHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), confirmButton);
            Raylib.DrawRectangleRec(confirmButton, confirmHovered ? Raylib_cs.Color.Gold : Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(confirmButton, 2, Raylib_cs.Color.White);
            Raylib.DrawText("CONFIRM", (int)confirmButton.X + 75, (int)confirmButton.Y + 15, 24, Raylib_cs.Color.White);
            Rectangle backButton = new Rectangle(WINDOW_WIDTH / 2 + 100, WINDOW_HEIGHT - 140, 250, 50);
            bool backHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), backButton);
            Raylib.DrawRectangleRec(backButton, backHovered ? Raylib_cs.Color.Red : Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(backButton, 2, Raylib_cs.Color.White);
            Raylib.DrawText("BACK", (int)backButton.X + 95, (int)backButton.Y + 15, 24, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Private method to draw the name input screen.
        /// </summary>
        private void DrawNameInput()
        {
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(30, 30, 50, 255));
            Raylib.DrawText("ENTER YOUR NAME", WINDOW_WIDTH / 2 - 300, 120, 60, Raylib_cs.Color.Gold);
            Rectangle characterPanel = new Rectangle(WINDOW_WIDTH / 2 - 250, 220, 500, 120);
            Raylib.DrawRectangleRec(characterPanel, new Raylib_cs.Color(40, 40, 60, 200));
            Raylib.DrawRectangleLinesEx(characterPanel, 3, Raylib_cs.Color.Gold);
            Rectangle characterIconRect = new Rectangle((int)characterPanel.X + 30, (int)characterPanel.Y + 20, 80, 80);
            Raylib.DrawRectangleRec(characterIconRect, Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(characterIconRect, 2, Raylib_cs.Color.White);
            if (_characterSelectionPlayers != null && _selectedCharacterIndex < _characterSelectionPlayers.Length && _characterSelectionPlayers[_selectedCharacterIndex] != null)
            {
                Player player = _characterSelectionPlayers[_selectedCharacterIndex];
                Raylib_cs.Texture2D profileTexture = player.GetProfileTexture();

                if (profileTexture.Width == 0 || profileTexture.Height == 0)
                {
                    LogGameEvent("Texture Error", _characterRoles[_selectedCharacterIndex] + " profile texture has invalid dimensions");
                    string initial = _characterRoles[_selectedCharacterIndex][0].ToString();
                    Raylib.DrawText(initial, (int)characterIconRect.X + 30, (int)characterIconRect.Y + 25, 40, Raylib_cs.Color.White);
                }
                else
                {
                    float scale = Math.Min(characterIconRect.Width / profileTexture.Width,characterIconRect.Height / profileTexture.Height) * 1.2f;
                    float scaledWidth = profileTexture.Width * scale;
                    float scaledHeight = profileTexture.Height * scale;
                    float xOffset = characterIconRect.X + (characterIconRect.Width - scaledWidth) / 2;
                    float yOffset = characterIconRect.Y + (characterIconRect.Height - scaledHeight) / 2;
                    Raylib.DrawTextureEx(profileTexture,new Vector2(xOffset, yOffset),0, scale,Raylib_cs.Color.White);
                }
            }
            Raylib.DrawText("Role: " + _characterRoles[_selectedCharacterIndex], (int)characterPanel.X + 140, (int)characterPanel.Y + 30, 30, Raylib_cs.Color.White);
            string attributeText = "";
            if (_selectedCharacterIndex == 0) attributeText = "High Defense, Medium Damage";
            else if (_selectedCharacterIndex == 1) attributeText = "High Speed, Medium Damage";
            else if (_selectedCharacterIndex == 2) attributeText = "High Damage, Medium Defense";
            Raylib.DrawText(attributeText, (int)characterPanel.X + 140, (int)characterPanel.Y + 70, 20, Raylib_cs.Color.Yellow);
            Rectangle characterPreviewRect = new Rectangle(WINDOW_WIDTH / 2 - 100, WINDOW_HEIGHT / 2 - 200, 200, 300);
            Raylib.DrawRectangleRec(characterPreviewRect, Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(characterPreviewRect, 2, Raylib_cs.Color.White);
            if (_selectedCharacterIndex >= 0 && _selectedCharacterIndex < (_characterSelectionPlayers?.Length ?? 0) && _characterSelectionPlayers?[_selectedCharacterIndex] != null)
            {
                Player player = _characterSelectionPlayers[_selectedCharacterIndex];
                if (player is Knight knight){
                    knight.CurrentAnimation = AnimationType.Walk;
                } else if (player is Archer archer){
                    archer.CurrentAnimation = AnimationType.Walk;
                } else if (player is Axeman axeman){
                    axeman.CurrentAnimation = AnimationType.Walk;
                }
                player.UpdateAnimation();
                Raylib_cs.Texture2D texture = player.GetCurrentTexture();
                Raylib_cs.Rectangle sourceRect = player.GetSourceRectangle();
                float scale = Math.Min(characterPreviewRect.Width / sourceRect.Width, characterPreviewRect.Height / sourceRect.Height) * 2.5f; 
                float scaledWidth = sourceRect.Width * scale;
                float scaledHeight = sourceRect.Height * scale;
                float xOffset = characterPreviewRect.X + (characterPreviewRect.Width - scaledWidth) / 2;
                float yOffset = characterPreviewRect.Y + (characterPreviewRect.Height - scaledHeight) / 2;
                Raylib.DrawTexturePro( texture,sourceRect,  new Rectangle(xOffset, yOffset, scaledWidth, scaledHeight),new Vector2(0, 0), 0, Raylib_cs.Color.White);
            }
            Rectangle namePanel = new Rectangle(WINDOW_WIDTH / 2 - 300, WINDOW_HEIGHT / 2 + 170, 600, 150);
            Raylib.DrawRectangleRec(namePanel, new Raylib_cs.Color(40, 40, 60, 200));
            Raylib.DrawRectangleLinesEx(namePanel, 3, Raylib_cs.Color.Gold);
            Raylib.DrawText("Character Name:", (int)namePanel.X + 30, (int)namePanel.Y + 30, 25, Raylib_cs.Color.White);
            Rectangle inputBox = new Rectangle(WINDOW_WIDTH / 2 - 200, WINDOW_HEIGHT / 2 + 240, 400, 60);
            Raylib.DrawRectangleRec(inputBox, Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(inputBox, 3, Raylib_cs.Color.White);
            Raylib.DrawText(_playerName, (int)inputBox.X + 20, (int)inputBox.Y + 15, 30, Raylib_cs.Color.White);
            float time = (float)Raylib.GetTime();
            if ((time % 1.0f) < 0.5f && _playerName.Length < 15)
            {
                int cursorX = (int)inputBox.X + 20 + Raylib.MeasureText(_playerName, 30);
                Raylib.DrawText("|", cursorX, (int)inputBox.Y + 15, 30, Raylib_cs.Color.White);
            }
            Raylib.DrawText("(" + _playerName.Length + "/15)", WINDOW_WIDTH / 2 + 220, WINDOW_HEIGHT / 2 + 250, 20, Raylib_cs.Color.LightGray);
            Rectangle buttonsPanel = new Rectangle(WINDOW_WIDTH / 2 - 400, WINDOW_HEIGHT - 200, 800, 100);
            Raylib.DrawRectangleRec(buttonsPanel, new Raylib_cs.Color(40, 40, 60, 150));
            Rectangle confirmButton = new Rectangle(WINDOW_WIDTH / 2 - 350, WINDOW_HEIGHT - 180, 250, 50);
            bool confirmHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), confirmButton);
            bool canConfirm = _playerName.Length > 0;
            Raylib_cs.Color confirmColor = canConfirm ? (confirmHovered ? Raylib_cs.Color.Gold : Raylib_cs.Color.DarkGray) : new Raylib_cs.Color(100, 100, 100, 200);
            Raylib.DrawRectangleRec(confirmButton, confirmColor);
            Raylib.DrawRectangleLinesEx(confirmButton, 2, Raylib_cs.Color.White);
            Raylib.DrawText("CONFIRM", (int)confirmButton.X + 75, (int)confirmButton.Y + 15, 24, canConfirm ? Raylib_cs.Color.White : Raylib_cs.Color.Gray);
            Rectangle backButton = new Rectangle(WINDOW_WIDTH / 2 + 100, WINDOW_HEIGHT - 180, 250, 50);
            bool backHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), backButton);
            Raylib.DrawRectangleRec(backButton, backHovered ? Raylib_cs.Color.Red : Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(backButton, 2, Raylib_cs.Color.White);
            Raylib.DrawText("BACK", (int)backButton.X + 95, (int)backButton.Y + 15, 24, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Private method to handle the input for the name input screen.
        /// </summary>
        private void HandleNameInputInput()
        {
            int key = Raylib.GetKeyPressed();
            if (key >= 32 && key <= 125 && _playerName.Length < 15)
            {
                _playerName += (char)key;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _playerName.Length > 0)
            {
                _playerName = _playerName.Substring(0, _playerName.Length - 1);
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && _playerName.Length > 0)
            {
                ChangeState(GameState.StoryIntro);
                LogGameEvent("Player Creation", "Name: " + _playerName + ", Class: " + _characterRoles[_selectedCharacterIndex]);
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Rectangle confirmButton = new Rectangle(WINDOW_WIDTH / 2 - 350, WINDOW_HEIGHT - 180, 250, 50);
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), confirmButton) && _playerName.Length > 0)
                {
                    ChangeState(GameState.StoryIntro);
                    LogGameEvent("Player Creation", "Name: " + _playerName + ", Class: " + _characterRoles[_selectedCharacterIndex]);
                    return;
                }
                Rectangle backButton = new Rectangle(WINDOW_WIDTH / 2 + 100, WINDOW_HEIGHT - 180, 250, 50);
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), backButton))
                {
                    ChangeState(GameState.CharacterSelection);
                    LogGameEvent("Navigation", "Name Input -> Character Selection (back button)");
                    return;
                }
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                ChangeState(GameState.CharacterSelection);
                LogGameEvent("Navigation", "Name Input -> Character Selection (escape key)");
            }
        }
        /// <summary>
        /// Private method to draw the story intro screen.
        /// </summary>
        private void DrawStoryIntro()
        {
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(20, 20, 30, 255));
            Raylib.DrawText("THE DUNGEON AWAITS", WINDOW_WIDTH / 2 - 350, 100, 60, Raylib_cs.Color.Gold);
            int textY = 250;
            int lineHeight = 40;
            Raylib.DrawText("Welcome, brave " + _characterRoles[_selectedCharacterIndex] + " " + _playerName + "!", WINDOW_WIDTH / 2 - 400, textY, 30, Raylib_cs.Color.White);
            Raylib.DrawText("Your journey begins in a mysterious dungeon full of monsters,", WINDOW_WIDTH / 2 - 400, textY + lineHeight, 30, Raylib_cs.Color.White);
            Raylib.DrawText("treasures, and ancient secrets waiting to be discovered.", WINDOW_WIDTH / 2 - 400, textY + lineHeight * 2, 30, Raylib_cs.Color.White);
            Raylib.DrawText("Navigate through the halls, defeat enemies, collect loot,", WINDOW_WIDTH / 2 - 400, textY + lineHeight * 4, 30, Raylib_cs.Color.White);
            Raylib.DrawText("and find the exit to reach deeper levels of the dungeon.", WINDOW_WIDTH / 2 - 400, textY + lineHeight * 5, 30, Raylib_cs.Color.White);
            Raylib.DrawText("Your legend begins now...", WINDOW_WIDTH / 2 - 200, textY + lineHeight * 7, 30, Raylib_cs.Color.Gold);
            float time = (float)Raylib.GetTime();
            if ((time % 1.0f) < 0.7f)
            {
                Raylib.DrawText("Press any key to begin", WINDOW_WIDTH / 2 - 180, WINDOW_HEIGHT - 150, 30, Raylib_cs.Color.White);
            }
        }
        /// <summary>
        /// Private method to handle the input for the story intro screen.
        /// </summary>
        private void HandleStoryIntroInput()
        {
            if (Raylib.GetKeyPressed() != 0 || Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (_player == null)
                {
                    CreatePlayerAndMap();
                }

                ChangeState(GameState.Exploring);
                LogGameEvent("Navigation", "Story Intro -> Exploring");
            }
        }
        /// <summary>
        /// Private method to draw the game scene including the map, player, and UI elements.
        /// </summary>
        private void DrawGame()
        {
            if (_player == null || _map.Count == 0)
            {
                return;
            }
            Map currentMap = _map[_map.Count - 1];
            const int MAP_OFFSET_X = 200;
            const int MAP_OFFSET_Y = 250;
            for (int x = 0; x < currentMap.MapWidth; x++)
            {
                for (int y = 0; y < currentMap.MapLength; y++)
                {
                    Tile tile = currentMap.Tiles[x, y];
                    if (tile.IsVisible)
                    {
                        int tileX = MAP_OFFSET_X + (x * GRID_SIZE);
                        int tileY = MAP_OFFSET_Y + (y * GRID_SIZE);
                        Texture2D floorTexture = Tile.GetTexture(TileType.Empty);
                        Raylib.DrawTextureEx(floorTexture, new Vector2(tileX, tileY), 0.0f, (float)GRID_SIZE / floorTexture.Width, Raylib_cs.Color.White);
                        Raylib.DrawRectangleLines(tileX, tileY, GRID_SIZE, GRID_SIZE, Raylib_cs.Color.DarkGray);
                        DrawTileSymbol(tileX, tileY, tile.TileType);
                    }
                    else
                    {
                        int tileX = MAP_OFFSET_X + (x * GRID_SIZE);
                        int tileY = MAP_OFFSET_Y + (y * GRID_SIZE);
                        Raylib.DrawRectangle(tileX, tileY, GRID_SIZE, GRID_SIZE, Raylib_cs.Color.Black);
                    }
                }
            }
            if (_player != null && _map.Count > 0 && currentMap.CheckInBounds(_player.Column, _player.Row))
            {
                Tile currentTile = currentMap.Tiles[_player.Column, _player.Row];
                if (currentTile.TileType == TileType.Merchant)
                {
                    string interactPrompt = "Press 'C' to talk to merchant";
                    int promptWidth = Raylib.MeasureText(interactPrompt, 24);
                    Raylib.DrawText(interactPrompt, WINDOW_WIDTH / 2 - promptWidth / 2, WINDOW_HEIGHT - 100, 24, Raylib_cs.Color.White);
                }
                else if (currentTile.TileType == TileType.Blacksmith)
                {
                    string interactPrompt = "Press 'C' to talk to blacksmith";
                    int promptWidth = Raylib.MeasureText(interactPrompt, 24);
                    Raylib.DrawText(interactPrompt, WINDOW_WIDTH / 2 - promptWidth / 2, WINDOW_HEIGHT - 100, 24, Raylib_cs.Color.White);
                }
            }
            int playerMapX, playerMapY;
            if (_player != null && _player.IsMoving)
            {
                var (x, y) = _player.GetInterpolatedPosition(MAP_OFFSET_X, MAP_OFFSET_Y, GRID_SIZE);
                playerMapX = x;
                playerMapY = y;
            }
            else
            {
                playerMapX = MAP_OFFSET_X + ((_player?.Column ?? 0) * GRID_SIZE);
                playerMapY = MAP_OFFSET_Y + ((_player?.Row ?? 0) * GRID_SIZE);
            }
            if (_player != null)
            {
                AnimationType animationType = _player.IsPlayerMoving ? AnimationType.Walk : AnimationType.Idle;
                if (_player is Knight knight)
                {
                    knight.CurrentAnimation = animationType;
                }
                else if (_player is Archer archer)
                {
                    archer.CurrentAnimation = animationType;
                }
                else if (_player is Axeman axeman)
                {
                    axeman.CurrentAnimation = animationType;
                }
                _player.UpdateAnimation();
            }
            if (_player != null)
            {
                Raylib_cs.Texture2D texture = _player.GetCurrentTexture();
                Raylib_cs.Rectangle sourceRect = _player.GetSourceRectangle();
                float scale = (float)GRID_SIZE / sourceRect.Width * 4.5f;
                float scaledWidth = sourceRect.Width * scale;
                float scaledHeight = sourceRect.Height * scale;
                float xOffset = playerMapX + (GRID_SIZE - scaledWidth) / 2;
                float yOffset = playerMapY + (GRID_SIZE - scaledHeight) / 2;
                Raylib.DrawTexturePro(texture, sourceRect, new Rectangle(xOffset, yOffset, scaledWidth, scaledHeight), new Vector2(0, 0), 0, Raylib_cs.Color.White);
            }
            Raylib.DrawText("Player: " + (_player?.Name ?? "Unknown"), 20, 20, 30, Raylib_cs.Color.Red);
            double hp = _player?.HP ?? 0;
            double maxHp = _player?.MaxHP ?? 1;
            float healthPercentage = (float)(hp / maxHp);
            Monster.DrawEnhancedBar(20, 60, 300, 30, healthPercentage, Raylib_cs.Color.Red, "HP: " + hp.ToString("F2") + "/" + maxHp.ToString("F2"));
            double mana = _player?.Mana ?? 0;
            double maxMana = _player?.MaxMana ?? 1;
            float manaPercentage = (float)(mana / maxMana);
            Monster.DrawEnhancedBar(20, 100, 300, 20, manaPercentage, Raylib_cs.Color.Blue, "MP: " + mana.ToString("F2") + "/" + maxMana.ToString("F2"));
            int exp = _player?.Exp ?? 0;
            int level = _player?.Level ?? 1;
            int expToNextLevel = 75 + (level - 1) * 5;
            if (exp < 0)
            {
                exp = 0;
                if (_player != null)
                {
                    _player.Exp = 0;
                }
            }
            float expPercentage = Math.Clamp((float)exp / expToNextLevel, 0f, 1f);
            Monster.DrawEnhancedBar(20, 130, 300, 20, expPercentage, Raylib_cs.Color.Green, "EXP: " + exp.ToString() + "/" + expToNextLevel.ToString());
            Raylib.DrawText("LVL: " + level, 20, 160, 30, Raylib_cs.Color.White);
            Raylib.DrawText("STAGE LEVEL: " + _currentLevel, WINDOW_WIDTH - 280, 115, 30, Raylib_cs.Color.Gold);
        }
        /// <summary>
        /// Private method to draw the inventory UI.
        /// </summary>
        private void DrawInventory()
        {
            DrawGame();
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, 170));
            if (_player == null) {
                return;
            }
            Rectangle invPanel = new Rectangle(WINDOW_WIDTH / 2 - 450, 50, 900, WINDOW_HEIGHT - 100);
            Raylib.DrawRectangleRec(invPanel, new Raylib_cs.Color(45, 45, 65, 230));
            Raylib.DrawRectangleLinesEx(invPanel, 4, Raylib_cs.Color.Gold);
            Raylib.DrawText("INVENTORY", WINDOW_WIDTH / 2 - 200, 70, 80, Raylib_cs.Color.Gold);
            Rectangle itemsPanel = new Rectangle(WINDOW_WIDTH / 2 - 420, 170, 260, WINDOW_HEIGHT - 290);
            Raylib.DrawRectangleRec(itemsPanel, new Raylib_cs.Color(30, 30, 50, 200));
            Raylib.DrawRectangleLinesEx(itemsPanel, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("ITEMS", (int)itemsPanel.X + 90, (int)itemsPanel.Y + 10, 30, Raylib_cs.Color.White);
            if (_player.Inventory.Items.Count > 0)
            {
                int rowHeight = 60; 
                int visibleRowsCount = 14; 
                int totalRowsCount = _player.Inventory.Items.Count;
                Rectangle listArea = new Rectangle(itemsPanel.X + 10,itemsPanel.Y + 50,itemsPanel.Width - 30,visibleRowsCount * rowHeight);
                int maxScrollPosition = Math.Max(0, totalRowsCount - visibleRowsCount);
                _inventoryScrollPosition = Math.Clamp(_inventoryScrollPosition, 0, maxScrollPosition);
                if (totalRowsCount > visibleRowsCount)
                {
                    _player.Inventory.DrawScrollbar(listArea, _inventoryScrollPosition, maxScrollPosition, visibleRowsCount, totalRowsCount);
                    Vector2 mousePos = Raylib.GetMousePosition();
                    Rectangle scrollTrack = new Rectangle(listArea.X + listArea.Width + 5,listArea.Y,12,listArea.Height );
                    if (Raylib.CheckCollisionPointRec(mousePos, scrollTrack) && Raylib.IsMouseButtonDown(0))
                    {
                        float relativeY = mousePos.Y - scrollTrack.Y;
                        float scrollPercentage = relativeY / scrollTrack.Height;
                        _inventoryScrollPosition = (int)(maxScrollPosition * scrollPercentage);
                        _inventoryScrollPosition = Math.Clamp(_inventoryScrollPosition, 0, maxScrollPosition);
                    }
                    float wheelMove = Raylib.GetMouseWheelMove();
                    Rectangle wheelScrollArea = new Rectangle( itemsPanel.X, itemsPanel.Y, itemsPanel.Width, itemsPanel.Height );
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), wheelScrollArea) && wheelMove != 0)
                    {
                        _inventoryScrollPosition -= (int)(wheelMove * 1.5f);
                        _inventoryScrollPosition = Math.Clamp(_inventoryScrollPosition, 0, maxScrollPosition);
                    }
                }
                _hoveredItem = null;
                Item? itemToShowTooltip = _player.Inventory.DrawInventoryItems(listArea, _inventoryScrollPosition, rowHeight, visibleRowsCount);
                _hoveredItem = itemToShowTooltip;
            }
            else
            {
                Raylib.DrawText("No items in inventory", (int)itemsPanel.X + 20, (int)itemsPanel.Y + 150, 20, Raylib_cs.Color.Gray);
            }
            Rectangle equipPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 170, 550, 450);
            Raylib.DrawRectangleRec(equipPanel, new Raylib_cs.Color(30, 30, 50, 200));
            Raylib.DrawRectangleLinesEx(equipPanel, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("EQUIPMENT", (int)equipPanel.X + 205, (int)equipPanel.Y + 10, 30, Raylib_cs.Color.White);
            int slotSize = 80; 
            int slotX = (int)equipPanel.X + 30;
            int equipStartY = (int)equipPanel.Y + 50;
            int equipSpacing = slotSize + 15;
            _player.Inventory.DrawEquipmentSlot(slotX + 210, equipStartY, slotSize, "Helmet", _player.Inventory.EquippedHelmet);
            _player.Inventory.DrawEquipmentSlot(slotX + 100, equipStartY + equipSpacing, slotSize, "Weapon", _player.Inventory.EquippedWeapon);
            _player.Inventory.DrawEquipmentSlot(slotX + 210, equipStartY + equipSpacing + 50, slotSize, "Chest", _player.Inventory.EquippedChest);
            _player.Inventory.DrawEquipmentSlot(slotX + equipSpacing * 2 + 130, equipStartY + equipSpacing, slotSize, "Bracelet", _player.Inventory.EquippedBracelet);
            _player.Inventory.DrawEquipmentSlot(slotX + 100, equipStartY + equipSpacing * 2 + 20, slotSize, "Gloves", _player.Inventory.EquippedGloves);
            _player.Inventory.DrawEquipmentSlot(slotX + equipSpacing * 2 + 130, equipStartY + equipSpacing * 2 + 20, slotSize, "Ring", _player.Inventory.EquippedRing);
            _player.Inventory.DrawEquipmentSlot(slotX + 210, equipStartY + equipSpacing * 3, slotSize, "Legs", _player.Inventory.EquippedLegs);
            Rectangle bonusPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 640, 550, 90);
            Raylib.DrawRectangleRec(bonusPanel, new Raylib_cs.Color(40, 40, 60, 200));
            Raylib.DrawRectangleLinesEx(bonusPanel, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("EQUIPMENT BONUSES", (int)bonusPanel.X + 60, (int)bonusPanel.Y + 10, 20, Raylib_cs.Color.LightGray);
            double bonusDefense = 0;
            double bonusHP = 0;
            double bonusSpeed = 0;
            double bonusMana = 0;
            double bonusCriticalRate = 0;
            double weaponDamage = 0;
            double classWeaponDamage = 0;
            foreach (Equipment equip in _player.Inventory.GetEquippedItems())
            {
                if (equip is Armor armor)
                {
                    bonusDefense += armor.BonusDefense;
                    bonusHP += armor.BonusHP;
                    bonusSpeed += armor.BonusSpeed;
                    bonusMana += armor.BonusMana;
                }
                else if (equip is Weapon weapon)
                {
                    weaponDamage += weapon.BonusAttack;
                    bonusCriticalRate += weapon.BonusCriticalRate;
                    if (_player != null)
                    {
                        (double attackBonus, double critBonus) = _player.GetClassWeaponBonus(weapon);
                        classWeaponDamage += attackBonus;
                        bonusCriticalRate += critBonus;
                    }
                }
            }
            int bonusTextY = (int)bonusPanel.Y + 40;
            int bonusSpacing = 20;
            if (_player != null && classWeaponDamage > 0)
            {
                Raylib.DrawText("DMG +" + weaponDamage.ToString("F1") + " (+" + classWeaponDamage.ToString("F1") + " role)", (int)bonusPanel.X + 20, bonusTextY, 16, Raylib_cs.Color.Red);
            }
            else
            {
                Raylib.DrawText("DMG +" + weaponDamage.ToString("F1"), (int)bonusPanel.X + 20, bonusTextY, 16, Raylib_cs.Color.Red);
            }

            Raylib.DrawText("HP +" + bonusHP.ToString("F1"), (int)bonusPanel.X + 270, bonusTextY, 16, Raylib_cs.Color.Green);
            Raylib.DrawText("DEF +" + bonusDefense.ToString("F1"), (int)bonusPanel.X + 20, bonusTextY + bonusSpacing, 16, Raylib_cs.Color.Blue);
            Raylib.DrawText("SPD +" + bonusSpeed.ToString("F1"), (int)bonusPanel.X + 270, bonusTextY + bonusSpacing, 16, Raylib_cs.Color.Yellow);
            Raylib.DrawText("MANA +" + bonusMana.ToString("F1"), (int)bonusPanel.X + 400, bonusTextY, 16, Raylib_cs.Color.Purple);
            Raylib.DrawText("CRIT +" + (bonusCriticalRate * 100).ToString("F2") + "%", (int)bonusPanel.X + 400, bonusTextY + bonusSpacing, 16, Raylib_cs.Color.Orange);
            Rectangle statsPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 740, 550, 250);
            Raylib.DrawRectangleRec(statsPanel, new Raylib_cs.Color(30, 30, 50, 200));
            Raylib.DrawRectangleLinesEx(statsPanel, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("PLAYER STATS", (int)statsPanel.X + 80, (int)statsPanel.Y + 10, 30, Raylib_cs.Color.White);
            int statY = (int)statsPanel.Y + 50;
            int spacing = 25;
            if (_player != null)
            {
                Raylib.DrawText("Name: " + _player.Name, (int)statsPanel.X + 20, statY, 20, Raylib_cs.Color.White);
                Raylib.DrawText("Level: " + _player.Level, (int)statsPanel.X + 20, statY + spacing, 20, Raylib_cs.Color.White);
                Raylib.DrawText("HP: " + _player.HP.ToString("F2") + "/" + _player.MaxHP.ToString("F2"), (int)statsPanel.X + 20, statY + spacing * 2, 20, Raylib_cs.Color.White);
                Raylib.DrawText("DMG: " + _player.Damage, (int)statsPanel.X + 20, statY + spacing * 3, 20, Raylib_cs.Color.White);
                Raylib.DrawText("DEF: " + _player.Defense, (int)statsPanel.X + 20, statY + spacing * 4, 20, Raylib_cs.Color.White);
                Raylib.DrawText("SPD +" + _player.Speed.ToString("F1"), (int)statsPanel.X + 20, statY + spacing * 5, 20, Raylib_cs.Color.White);
                Raylib.DrawText("Mana +" + _player.Mana.ToString("F1"), (int)statsPanel.X + 20, statY + spacing * 6, 20, Raylib_cs.Color.White);
                Raylib.DrawText("CRIT +" + (_player.CriticalRate * 100).ToString("F2") + "%", (int)statsPanel.X + 20, statY + spacing * 7, 20, Raylib_cs.Color.White);
            }
            Rectangle moneyPanel = new Rectangle(WINDOW_WIDTH / 2 - 145, 1000, 550, 60);
            Raylib.DrawRectangleRec(moneyPanel, new Raylib_cs.Color(60, 50, 20, 200));
            Raylib.DrawRectangleLinesEx(moneyPanel, 2, Raylib_cs.Color.Gold);
            if (_player != null)
                Raylib.DrawText("GOLD: " + _player.Inventory.Money, (int)moneyPanel.X + 20, (int)moneyPanel.Y + 15, 30, Raylib_cs.Color.Gold);
            if (_isDragging && _draggingEquipment != null)
            {
                int dragSize = 60;
                int dragX = (int)_currentDragPosition.X - (dragSize / 2);
                int dragY = (int)_currentDragPosition.Y - (dragSize / 2);
                Raylib.DrawRectangle(dragX, dragY, dragSize, dragSize, new Raylib_cs.Color(200, 200, 200, 150));
                string itemCode = _draggingEquipment is Weapon ? "W" :(_draggingEquipment is Armor a ? a.ArmorType.ToString()[0].ToString() : "?");
                Raylib.DrawText(itemCode, dragX + 25, dragY + 25, 24, Raylib_cs.Color.White);
            }
            if (_hoveredItem != null)
            {
                DrawItemTooltip(_hoveredItem, Raylib.GetMousePosition());
            }
        }
        /// <summary>
        /// Private method to draw the radial gradient effect.
        /// </summary>
        private void DrawRadialGradient(int centerX, int centerY, float innerRadius, float outerRadius, Raylib_cs.Color innerColor, Raylib_cs.Color outerColor, int segments = 32)
        {
            float angleStep = 2.0f * (float)Math.PI / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = (i + 1) * angleStep;
                float x1Inner = centerX + innerRadius * (float)Math.Cos(angle1);
                float y1Inner = centerY + innerRadius * (float)Math.Sin(angle1);
                float x1Outer = centerX + outerRadius * (float)Math.Cos(angle1);
                float y1Outer = centerY + outerRadius * (float)Math.Sin(angle1);
                float x2Inner = centerX + innerRadius * (float)Math.Cos(angle2);
                float y2Inner = centerY + innerRadius * (float)Math.Sin(angle2);
                float x2Outer = centerX + outerRadius * (float)Math.Cos(angle2);
                float y2Outer = centerY + outerRadius * (float)Math.Sin(angle2);
                Raylib.DrawTriangle(new Vector2(x1Inner, y1Inner),new Vector2(x1Outer, y1Outer),new Vector2(x2Outer, y2Outer),outerColor);
                Raylib.DrawTriangle(new Vector2(x1Inner, y1Inner),new Vector2(x2Outer, y2Outer),new Vector2(x2Inner, y2Inner),innerColor);
            }
        }
        /// <summary>
        /// Private method to draw a shimmering effect.
        /// </summary>
        private void DrawShimmerEffect(int centerX, int centerY, float radius, float time, int sparkleCount, Raylib_cs.Color color, float minSize = 1.0f, float maxSize = 3.0f)
        {
            Random rand = new Random((int)(time * 100));
            for (int i = 0; i < sparkleCount; i++)
            {
                float angle = (float)(i * Math.PI * 2 / sparkleCount + time % (2 * Math.PI));
                float wobble = (float)Math.Sin(time * 3 + i * 1.1f) * 0.2f; 
                angle += wobble;
                float distanceRatio = 0.2f + 0.8f * (float)Math.Sin(time * 2 + i * 0.7f);
                if (Math.Sin(time * 0.8f + i * 0.3f) > 0.85f)
                {
                    distanceRatio *= 1.2f;
                }
                float distance = radius * distanceRatio;
                int sparkleX = (int)(centerX + Math.Cos(angle) * distance);
                int sparkleY = (int)(centerY + Math.Sin(angle) * distance);
                float oscillation = (float)Math.Sin(time * 3 + i * 1.3f);
                float sizeVariation = (float)Math.Sin(time * 1.5f + i * 0.5f) * 0.2f + 1.0f;
                float sparkleSize = (minSize + (maxSize - minSize) * ((oscillation + 1) / 2)) * sizeVariation;
                byte baseAlpha = (byte)(150 + rand.Next(0, 106));
                if (Math.Sin(time * 5 + i * 2.5f) > 0.9f)
                {
                    baseAlpha = 255;
                    sparkleSize *= 1.3f;
                }
                Raylib.DrawCircle(sparkleX, sparkleY, sparkleSize, new Raylib_cs.Color(color.R, color.G, color.B, baseAlpha));
                if (sparkleSize > 1.5f)
                {
                    Raylib.DrawCircle(sparkleX, sparkleY, sparkleSize * 0.5f,
                        new Raylib_cs.Color(
                            (byte)Math.Min(255, color.R + 30),
                            (byte)Math.Min(255, color.G + 30),
                            (byte)Math.Min(255, color.B + 30),
                            (byte)Math.Min(255, baseAlpha + 40)
                        )
                    );
                }
                if (rand.NextDouble() > 0.7)
                {
                    float glintSize = sparkleSize * 0.7f;
                    Raylib.DrawLine(sparkleX - (int)(glintSize), sparkleY,sparkleX + (int)(glintSize), sparkleY,new Raylib_cs.Color(255, 255, 255, (int)baseAlpha));
                }
            }
        }
        /// <summary>
        /// Private method to draw a related tile graphic based on the tile type.
        /// </summary>
        private void DrawTileSymbol(int x, int y, TileType tileType)
        {
            int centerX = x + (GRID_SIZE / 2) - 8;
            int centerY = y + (GRID_SIZE / 2) - 10;
            Texture2D floorTexture = Tile.GetTexture(TileType.Empty);
            Raylib.DrawTextureEx(floorTexture, new Vector2(x, y), 0.0f, (float)GRID_SIZE / floorTexture.Width, Raylib_cs.Color.White);
            Map currentMap = _map[_map.Count - 1];
            int tileCol = (x - 200) / GRID_SIZE;
            int tileRow = (y - 250) / GRID_SIZE;
            bool isInteracted = false;
            if (tileCol >= 0 && tileCol < currentMap.MapWidth && tileRow >= 0 && tileRow < currentMap.MapLength)
            {
                Tile currentTile = currentMap.Tiles[tileCol, tileRow];
                isInteracted = currentTile.IsInteracted;
            }
            if (isInteracted && (tileType == TileType.RandomBuff || tileType == TileType.Trap || tileType == TileType.Treasure))
            {
                return;
            }
            switch (tileType)
            {
                case TileType.Wall:
                    Texture2D wallTexture = Tile.GetTexture(TileType.Wall);
                    Raylib.DrawTextureEx(wallTexture, new Vector2(x, y), 0.0f, (float)GRID_SIZE / wallTexture.Width, Raylib_cs.Color.White);
                    break;
                case TileType.Merchant:
                    Rectangle merchantFrameRect = new Rectangle( _merchantFrame * _merchantFrameWidth, 0, _merchantFrameWidth, Merchant.GetMerchantTexture().Height );
                    float merchantScale = (float)GRID_SIZE / _merchantFrameWidth * 0.85f;
                    float merchantScaledWidth = _merchantFrameWidth * merchantScale;
                    float merchantScaledHeight = Merchant.GetMerchantTexture().Height * merchantScale;
                    float merchantxOffset = x + (GRID_SIZE - merchantScaledWidth) / 2;
                    float merchantyOffset = y + (GRID_SIZE - merchantScaledHeight) / 2;
                    try
                    {
                        Raylib.DrawTexturePro( Merchant.GetMerchantTexture(), merchantFrameRect, new Rectangle(merchantxOffset, merchantyOffset, merchantScaledWidth, merchantScaledHeight), new Vector2(0, 0),  0, Raylib_cs.Color.White );
                    }
                    catch (Exception ex)
                    {
                        LogGameEvent("TextureDrawError", "Error drawing merchant texture: " + ex.Message);
                        Raylib.DrawText("$", centerX, centerY, 20, Raylib_cs.Color.White);
                    }
                    break;
                case TileType.Blacksmith:
                    Rectangle blacksmithFrameRect = new Rectangle(_blacksmithFrame * _blacksmithFrameWidth,0,_blacksmithFrameWidth,BlackSmith.GetBlacksmithTexture().Height);
                    float blacksmithScale = (float)GRID_SIZE / _blacksmithFrameWidth * 0.85f;
                    float blacksmithScaledWidth = _blacksmithFrameWidth * blacksmithScale;
                    float blacksmithScaledHeight = BlackSmith.GetBlacksmithTexture().Height * blacksmithScale;
                    float blacksmithXOffset = x + (GRID_SIZE - blacksmithScaledWidth) / 2;
                    float blacksmithYOffset = y + (GRID_SIZE - blacksmithScaledHeight) / 2;
                    try
                    {
                        Raylib.DrawTexturePro( BlackSmith.GetBlacksmithTexture(), blacksmithFrameRect, new Rectangle(blacksmithXOffset, blacksmithYOffset, blacksmithScaledWidth, blacksmithScaledHeight), new Vector2(0, 0),  0, Raylib_cs.Color.White);
                    }
                    catch (Exception ex)
                    {
                        LogGameEvent("TextureDrawError", "Error drawing blacksmith texture: " + ex.Message);
                        Raylib.DrawText("Bl", centerX, centerY, 20, Raylib_cs.Color.White);
                    }
                    break;
                case TileType.Monster:
                    Monster? monsterAtTile = null;
                    foreach (Unit unit in _units)
                    {
                        if (unit is Monster monster &&
                            monster.IsAlive &&
                            monster.Column == tileCol &&
                            monster.Row == tileRow)
                        {
                            monsterAtTile = monster;
                            break;
                        }
                    }
                    if (monsterAtTile != null)
                    {
                        monsterAtTile.SetAnimation(AnimationType.Idle);
                        monsterAtTile.UpdateAnimation();
                        float xOffset = x + (GRID_SIZE / 2);
                        float yOffset = y + (GRID_SIZE / 2);
                        monsterAtTile.RenderBattle(new Vector2(xOffset, yOffset),AnimationType.Idle, "tilemap" );
                    }
                    else
                    {
                        if (tileCol >= 0 && tileCol < currentMap.MapWidth &&
                            tileRow >= 0 && tileRow < currentMap.MapLength)
                        {
                            currentMap.Tiles[tileCol, tileRow].TileType = TileType.Empty;
                            currentMap.Tiles[tileCol, tileRow].IsWalkable = true;
                            LogGameEvent("Map fix", "Changed monster tile without monster at [" + tileCol + "," + tileRow + "] to empty");
                        }
                    }
                    break;
                case TileType.RandomBuff:
                    float currentTime = (float)Raylib.GetTime();
                    float floatingOffset = (float)Math.Sin(currentTime * 1.5f) * 6.0f;
                    Texture2D buffTexture = Tile.GetTexture(TileType.RandomBuff);
                    Raylib.DrawTextureEx( buffTexture, new Vector2(x, y - floatingOffset), 0.0f,(float)GRID_SIZE / buffTexture.Width,Raylib_cs.Color.White);
                    break;
                case TileType.Trap:
                    float trapTime = (float)Raylib.GetTime();
                    int frameWidth = 145;
                    int frameIndex = (int)(trapTime * 2) % 3;
                    int[] frameOffsets = new int[] { 20, 168, 328 };
                    Texture2D trapTexture = Tile.GetTexture(TileType.Trap);
                    Raylib_cs.Rectangle sourceRect = new Raylib_cs.Rectangle( frameOffsets[frameIndex], 0, frameWidth, trapTexture.Height );
                    Raylib_cs.Rectangle destRect = new Raylib_cs.Rectangle( x, y, GRID_SIZE, GRID_SIZE );
                    Raylib.DrawTexturePro(trapTexture,sourceRect,destRect,new Vector2(0, 0), 0.0f,  Raylib_cs.Color.White);
                    break;
                case TileType.Exit:
                    Texture2D escapeTexture = Tile.GetTexture(TileType.Exit);
                    Raylib.DrawTextureEx(escapeTexture,new Vector2(x, y),0.0f, (float)GRID_SIZE / escapeTexture.Width, Raylib_cs.Color.White);
                    break;
                case TileType.Treasure:
                    float treasureTime = (float)Raylib.GetTime();
                    int treasureCenterX = x + GRID_SIZE / 2;
                    int treasureCenterY = y + GRID_SIZE / 2;
                    float treasureGlowSpeed = 1.5f;
                    float treasureGlowSize = (float)(Math.Sin(treasureTime * treasureGlowSpeed) * 5 + 30);
                    float ambientGlowSize = treasureGlowSize + 35;
                    DrawRadialGradient( treasureCenterX, treasureCenterY, GRID_SIZE / 2 + 15, GRID_SIZE / 2 + ambientGlowSize, new Raylib_cs.Color(100, 50, 200, 2),new Raylib_cs.Color(50, 30, 120, 8));
                    float outerGlowSize = treasureGlowSize + 25;
                    DrawRadialGradient( treasureCenterX, treasureCenterY,GRID_SIZE / 2, GRID_SIZE / 2 + outerGlowSize, new Raylib_cs.Color(255, 223, 0, 10), new Raylib_cs.Color(255, 223, 0, 35) );
                    float middleGlowSize = (float)(Math.Sin(treasureTime * treasureGlowSpeed + (float)Math.PI) * 10 + 28);
                    DrawRadialGradient( treasureCenterX, treasureCenterY, GRID_SIZE / 2, GRID_SIZE / 2 + middleGlowSize, new Raylib_cs.Color(255, 215, 0, 70), new Raylib_cs.Color(255, 215, 0, 15) );
                    float innerGlowSize = (float)(Math.Sin(treasureTime * (treasureGlowSpeed * 2)) * 4 + 12);
                    DrawRadialGradient( treasureCenterX,  treasureCenterY, GRID_SIZE / 2 - 5,  GRID_SIZE / 2 + innerGlowSize, new Raylib_cs.Color(255, 255, 180, 140), new Raylib_cs.Color(255, 215, 50, 50));
                    float rotationSpeed = treasureTime * 0.5f;
                    int magicCirclePoints = 6;
                    float magicCircleRadius = GRID_SIZE / 2 + 10;
                    for (int i = 0; i < magicCirclePoints; i++)
                    {
                        float angle = rotationSpeed + ((float)i / magicCirclePoints * 2 * (float)Math.PI);
                        float nextAngle = rotationSpeed + ((float)(i + 1) / magicCirclePoints * 2 * (float)Math.PI);
                        float x1 = treasureCenterX + (float)Math.Cos(angle) * magicCircleRadius;
                        float y1 = treasureCenterY + (float)Math.Sin(angle) * magicCircleRadius;
                        float x2 = treasureCenterX + (float)Math.Cos(nextAngle) * magicCircleRadius;
                        float y2 = treasureCenterY + (float)Math.Sin(nextAngle) * magicCircleRadius;
                        Raylib.DrawLineEx(  new Vector2(x1, y1), new Vector2(x2, y2), 1.0f, new Raylib_cs.Color(255, 223, 50, 50));
                    }
                    DrawShimmerEffect(  treasureCenterX, treasureCenterY, GRID_SIZE / 2 + 15,  treasureTime,  16,  new Raylib_cs.Color(255, 255, 200, 255),  0.5f,  3.5f );
                    DrawShimmerEffect( treasureCenterX, treasureCenterY, GRID_SIZE / 2 + 25,  treasureTime * 0.7f,  8,  new Raylib_cs.Color(180, 230, 255, 200), 0.3f, 2.5f );
                    float ringPulse = (float)(Math.Sin(treasureTime * 1.2f) * 0.3f + 0.7f);
                    float ringWidth = 3.5f * ringPulse;
                    Raylib.DrawRing( new Vector2(treasureCenterX, treasureCenterY), GRID_SIZE / 2 + 8, GRID_SIZE / 2 + 8 + ringWidth,  0, 360,0, new Raylib_cs.Color(255, 215, 0, (int)(160 * ringPulse)));
                    if (Math.Sin(treasureTime * 0.5f) > 0.88f)
                    {
                        float flashSize = (float)(Math.Sin(treasureTime * 8) * 12 + 15);
                        Raylib.DrawCircle( treasureCenterX, treasureCenterY, GRID_SIZE / 2 + flashSize, new Raylib_cs.Color(255, 255, 220, 35) );
                    }
                    int lightCount = 3;
                    float lightOrbitRadius = GRID_SIZE / 2 + 16;
                    float lightOrbitSpeed = treasureTime * 0.8f;
                    for (int i = 0; i < lightCount; i++)
                    {
                        float lightAngle = lightOrbitSpeed + ((float)i / lightCount * 2 * (float)Math.PI);
                        float x1 = treasureCenterX + (float)Math.Cos(lightAngle) * lightOrbitRadius;
                        float y1 = treasureCenterY + (float)Math.Sin(lightAngle) * lightOrbitRadius;
                        float pulseSize = (float)Math.Sin(treasureTime * 3 + i) * 1.5f + 3.5f;
                        Raylib.DrawCircle( (int)x1,  (int)y1, pulseSize, new Raylib_cs.Color(255, 255, 200, 150) );
                    }
                    Texture2D treasureTexture = Tile.GetTexture(TileType.Treasure);
                    Raylib.DrawTextureEx(treasureTexture, new Vector2(x, y), 0.0f,  (float)GRID_SIZE / treasureTexture.Width,Raylib_cs.Color.White);
                    break;
            }
        }
        /// <summary>
        /// Public method to track the game events.
        /// </summary>
        public void LogGameEvent(string eventType, string description)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText("log/game_events.txt", timestamp + " - " + eventType + ": " + description + "\n");
        }
        /// <summary>
        /// Public method to update the game scene and handle game logic.
        /// </summary>
        public void Update()
        {
            if (_player != null && _player.HP <= 0 && _currentState != GameState.GameOver)
            {
                if (_player != null)
                {
                    _player.SetAnimation(AnimationType.Death);
                }
                ChangeState(GameState.GameOver);
                LogGameEvent("Player Death", "Player has been defeated");
                return;
            }
            _merchantAnimTime += Raylib.GetFrameTime();
            if (_merchantAnimTime >= _merchantFrameDuration)
            {
                _merchantFrame = (_merchantFrame + 1) % _merchantFrameCount;
                _merchantAnimTime = 0;
            }
            _blacksmithAnimTime += Raylib.GetFrameTime();
            if (_blacksmithAnimTime >= _blacksmithFrameDuration)
            {
                _blacksmithFrame = (_blacksmithFrame + 1) % _blacksmithFrameCount;
                _blacksmithAnimTime = 0;
            }
            if (_inShopInteraction && _currentMerchant != null && _player != null)
            {
                if (_currentMerchant.HandleUIInput(_player, this))
                {
                    _units.Remove(_currentMerchant);
                    if (_map.Count > 0)
                    {
                        Map currentMap = _map[_currentLevel - 1];
                        if (currentMap.CheckInBounds(_currentMerchant.Column, _currentMerchant.Row))
                        {
                            currentMap.Tiles[_currentMerchant.Column, _currentMerchant.Row].TileType = TileType.Empty;
                            currentMap.Tiles[_currentMerchant.Column, _currentMerchant.Row].IsWalkable = true;
                        }
                    }
                    _inShopInteraction = false;
                    _currentMerchant = null;
                    LogGameEvent("Shop", "Merchant dismissed and removed from game");
                }
            }
            if (_inBlacksmithInteraction && _currentBlacksmith != null && _player != null)
            {
                if (_currentBlacksmith.HandleUIInput(_player, this))
                {
                    _units.Remove(_currentBlacksmith);
                    if (_map.Count > 0)
                    {
                        Map currentMap = _map[_currentLevel - 1];
                        if (currentMap.CheckInBounds(_currentBlacksmith.Column, _currentBlacksmith.Row))
                        {
                            currentMap.Tiles[_currentBlacksmith.Column, _currentBlacksmith.Row].TileType = TileType.Empty;
                            currentMap.Tiles[_currentBlacksmith.Column, _currentBlacksmith.Row].IsWalkable = true;
                        }
                    }
                    _inBlacksmithInteraction = false;
                    _currentBlacksmith = null;
                    LogGameEvent("Blacksmith", "Blacksmith dismissed and removed from game");
                }
            }
            CheckBrokenEquipment();
            if (_player != null && _player.IsMoving)
            {
                _player.UpdateMovement();
            }
            if (_currentState != GameState.Exploring && _currentState != GameState.InBattle)
                return;
            foreach (Unit unit in _units)
            {
                if (!unit.IsAlive)
                    continue;
                if (unit is Monster monster)
                {
                    monster.UpdateAnimation();
                }
            }
            if (_player != null && _player.IsAlive && _map.Count > 0)
            {
                Map currentMap = _map[_map.Count - 1]; if (currentMap.CheckInBounds(_player.Column, _player.Row))
                {
                    Tile currentTile = currentMap.Tiles[_player.Column, _player.Row];
                    if (_currentState == GameState.Exploring && !_player.IsMoving)
                    {
                        switch (currentTile.TileType)
                        {
                            case TileType.Monster:
                                Monster? encounterMonster = null;
                                foreach (Unit unit in _units)
                                {
                                    if (unit is Monster monster && monster.IsAlive && monster.Column == _player.Column && monster.Row == _player.Row)
                                    {
                                        encounterMonster = monster;
                                        break;
                                    }
                                }
                                if (encounterMonster != null)
                                {
                                    InitializeBattle(encounterMonster);
                                }
                                ChangeState(GameState.InBattle);
                                break;
                            case TileType.Exit:
                                NextLevel();
                                break;
                            default:
                                string tileMessage = currentTile.HandleTileType(_player);
                                if (!string.IsNullOrEmpty(tileMessage))
                                {
                                    AddNotification(tileMessage, 3.0, Raylib_cs.Color.White);
                                }
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Public method to initialize the game objects and map.
        /// </summary>
        public void InitializeObjects()
        {
            int mapWidth = 20;
            int mapHeight = 15;
            Map firstMap = new Map(mapWidth, mapHeight, _currentLevel);
            firstMap.GenerateMap();
            _map.Add(firstMap);
            int monsterCount = firstMap.CountMonsterTiles();
            firstMap.AddMonsters(monsterCount, _currentDifficulty, _units);
            foreach (Unit unit in _units)
            {
                if (unit is Monster monster)
                {
                    monster.AddItemsToDropList(monster.Level);
                }
            }
            foreach (Map map in _map)
            {
                foreach (Tile tile in map.Tiles)
                {
                    if (tile.TileType == TileType.Merchant)
                    {
                        Merchant merchant = new Merchant( "Merchant", 100, 100,0, 0, 0, 0, 100,0, 1,tile.Row, tile.Column, true);
                        _units.Add(merchant);
                    }
                    if (tile.TileType == TileType.Blacksmith)
                    {
                        BlackSmith blacksmith = new BlackSmith("BlackSmith", 100, 100, 0, 0, 0, 0, 100,0, 1,tile.Row, tile.Column, true);
                        _units.Add(blacksmith);
                    }
                }

            }
            LogGameEvent("Map generated", "Level: " + _currentLevel + ", Size: " + mapWidth + "x" + mapHeight + ", Monsters: " + monsterCount);
        }
        /// <summary>
        /// Public method to create a new map once the player reaches the exit tile and go to the next level.
        /// </summary>
        public void NextLevel()
        {
            _currentLevel++;
            _units.RemoveAll(unit => unit is Monster);
            int mapWidth = 20;
            int mapHeight = 15;
            mapWidth = Math.Min(mapWidth, 40);
            mapHeight = Math.Min(mapHeight, 30);
            Map newMap = new Map(mapWidth, mapHeight, _currentLevel);
            newMap.GenerateMap();
            _map.Add(newMap);
            if (_player != null)
            {
                _player.Row = 1;
                _player.Column = 1;
            }
            newMap.AddMonsters(newMap.CountMonsterTiles(), _currentDifficulty, _units);
            foreach (Tile tile in newMap.Tiles)
            {
                if (tile.TileType == TileType.Merchant)
                {
                    Merchant merchant = new Merchant("Merchant", 100, 100, 0, 0, 0, 0, 100, 0, 1, tile.Row, tile.Column, true);
                    _units.Add(merchant);
                }
                if (tile.TileType == TileType.Blacksmith)
                {
                    BlackSmith blacksmith = new BlackSmith("BlackSmith", 100, 100, 0, 0, 0, 0, 100, 0, 1, tile.Row, tile.Column, true);
                    _units.Add(blacksmith);
                }
            }
        }
        /// <summary>
        /// Public method to reveal tiles around a given position on the map.
        /// </summary>
        public void RevealTiles(Map map, int centerX, int centerY, int radius)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (map.CheckInBounds(x, y))
                    {
                        map.Tiles[x, y].IsVisible = true;
                    }
                }
            }
        }
        /// <summary>
        /// Private method to create the player and initialize the map.
        /// </summary>
        private void CreatePlayerAndMap()
        {
            _player = PlayerFactory.CreatePlayerFromSelection(_playerName, _characterRoles[_selectedCharacterIndex]);
            _player.SetupStartingInventory();
            if (_map.Count == 0)
            {
                InitializeObjects();
            }
            if (_map.Count > 0)
            {
                Map currentMap = _map[0];
                for (int x = 0; x < currentMap.MapWidth; x++)
                {
                    for (int y = 0; y < currentMap.MapLength; y++)
                    {
                        if (currentMap.Tiles[x, y].TileType == TileType.Spawn)
                        {
                            _player.Column = x;
                            _player.Row = y;
                            break;
                        }
                    }
                }
                if (_player.Column == 1 && _player.Row == 1)
                {
                    currentMap.Tiles[1, 1].TileType = TileType.Empty;
                    currentMap.Tiles[1, 1].IsWalkable = true;
                }
                RevealTiles(currentMap, _player.Column, _player.Row, 3);
            }
            LogGameEvent("Player Created", "Name: " + _player.Name + ", Role: " + _characterRoles[_selectedCharacterIndex] + ", HP: " + _player.HP + ", ATK: " + _player.Damage + ", DEF: " + _player.Defense);
        }
        /// <summary>
        /// Private method to draw the settings button and handle its interactions.
        /// </summary>
        private void DrawSettingsButton()
        {
            int normalSize = 60;
            int hoverSize = 70;
            int currentSize;
            Rectangle settingsButton;
            bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(WINDOW_WIDTH - 75, 10, 70, 70) );
            currentSize = isHovered ? hoverSize : normalSize;
            settingsButton = new Rectangle(WINDOW_WIDTH - 15 - currentSize,15 + (normalSize - currentSize) / 2,currentSize, currentSize );
            int centerX = (int)settingsButton.X + currentSize / 2;
            int centerY = (int)settingsButton.Y + currentSize / 2;
            float circleScale = currentSize / (float)normalSize;
            Raylib.DrawCircle(centerX, centerY, 20 * circleScale, Raylib_cs.Color.White); 
            Raylib.DrawCircle(centerX, centerY, 14 * circleScale, isHovered ? new Raylib_cs.Color(100, 100, 100, 200) : new Raylib_cs.Color(40, 40, 40, 180)); 
            Raylib.DrawCircle(centerX, centerY, 6 * circleScale, Raylib_cs.Color.White); 
            for (int i = 0; i < 8; i++)
            {
                float angle = i * MathF.PI / 4.0f;
                float innerRadius = 14 * circleScale;
                float outerRadius = 26 * circleScale;
                float lineThickness = 5 * circleScale;
                float x1 = centerX + MathF.Cos(angle) * innerRadius;
                float y1 = centerY + MathF.Sin(angle) * innerRadius;
                float x2 = centerX + MathF.Cos(angle) * outerRadius;
                float y2 = centerY + MathF.Sin(angle) * outerRadius;
                Raylib.DrawLineEx(new Vector2(x1, y1), new Vector2(x2, y2), lineThickness, Raylib_cs.Color.White);
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && isHovered)
            {
                _isSettingsPanelOpen = !_isSettingsPanelOpen;
            }
        }
        /// <summary>
        /// Private method to draw the settings panel.
        /// </summary>
        private void DrawSettingsPanel()
        {
            if (!_isSettingsPanelOpen)
            {
                return;
            }
            Raylib.DrawRectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, 150));
            Rectangle panel = new Rectangle(WINDOW_WIDTH / 2 - 300, WINDOW_HEIGHT / 2 - 300, 600, 550);
            Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(40, 40, 60, 255));
            Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Gold);          
            Raylib.DrawText("SETTINGS", (int)panel.X + 150, (int)panel.Y + 30, 50, Raylib_cs.Color.Gold);
            Rectangle closeButton = new Rectangle((int)panel.X + panel.Width - 60, (int)panel.Y + 20, 40, 40);
            bool closeHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), closeButton);
            Raylib.DrawRectangleRec(closeButton, closeHovered ? new Raylib_cs.Color(200, 60, 60, 255) : new Raylib_cs.Color(60, 60, 80, 255));
            Raylib.DrawRectangleLinesEx(closeButton, closeHovered ? 3 : 2, Raylib_cs.Color.White);
            int centerX = (int)closeButton.X + (int)closeButton.Width / 2;
            int centerY = (int)closeButton.Y + (int)closeButton.Height / 2;
            int iconSize = closeHovered ? 14 : 12;
            int thickness = closeHovered ? 3 : 2;
            Raylib.DrawLineEx(new Vector2(centerX - iconSize, centerY - iconSize),new Vector2(centerX + iconSize, centerY + iconSize),thickness,Raylib_cs.Color.White);
            Raylib.DrawLineEx( new Vector2(centerX + iconSize, centerY - iconSize), new Vector2(centerX - iconSize, centerY + iconSize), thickness, Raylib_cs.Color.White );
            Raylib.DrawText("Sound Volume:", (int)panel.X + 30, (int)panel.Y + 100, 30, Raylib_cs.Color.White);
            Rectangle soundSlider = new Rectangle((int)panel.X + 50, (int)panel.Y + 140, 500, 30);
            Raylib.DrawRectangleRec(soundSlider, Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleRec(new Rectangle((int)panel.X + 50, (int)panel.Y + 140, 500 * _soundVolume, 30), Raylib_cs.Color.Gold);
            Raylib.DrawRectangleLinesEx(soundSlider, 2, Raylib_cs.Color.White);
            Raylib.DrawText("Music Volume:", (int)panel.X + 30, (int)panel.Y + 190, 30, Raylib_cs.Color.White);
            Rectangle musicSlider = new Rectangle((int)panel.X + 50, (int)panel.Y + 230, 500, 30);
            Raylib.DrawRectangleRec(musicSlider, Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleRec(new Rectangle((int)panel.X + 50, (int)panel.Y + 230, 500 * _musicVolume, 30), Raylib_cs.Color.Gold);
            Raylib.DrawRectangleLinesEx(musicSlider, 2, Raylib_cs.Color.White);
            Raylib.DrawText("Game Controls:", (int)panel.X + 30, (int)panel.Y + 280, 30, Raylib_cs.Color.Gold);
            Raylib.DrawText("WASD - Move Character", (int)panel.X + 30, (int)panel.Y + 320, 22, Raylib_cs.Color.White);
            Raylib.DrawText("E - Open Inventory", (int)panel.X + 30, (int)panel.Y + 350, 22, Raylib_cs.Color.White);
            Raylib.DrawText("C - Interact with NPC", (int)panel.X + 30, (int)panel.Y + 380, 22, Raylib_cs.Color.White);
            Rectangle mainMenuButton = new Rectangle(WINDOW_WIDTH / 2 - 150, panel.Y + panel.Height - 80, 240, 50);
            bool mainMenuHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), mainMenuButton);
            Raylib.DrawRectangleRec(mainMenuButton, mainMenuHovered ? Raylib_cs.Color.Gold : Raylib_cs.Color.DarkGray);
            Raylib.DrawRectangleLinesEx(mainMenuButton, 2, Raylib_cs.Color.White);
            Raylib.DrawText("MAIN MENU", (int)mainMenuButton.X + 60, (int)mainMenuButton.Y + 15, 24, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Private method to handle input for the settings panel.
        /// </summary>
        private void HandleSettingsInput()
        {
            if (!_isSettingsPanelOpen) return;
            Rectangle panel = new Rectangle(WINDOW_WIDTH / 2 - 300, WINDOW_HEIGHT / 2 - 300, 600, 550);
            Rectangle closeButton = new Rectangle((int)panel.X + panel.Width - 60, (int)panel.Y + 20, 40, 40);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), closeButton))
            {
                _isSettingsPanelOpen = false;
                LogGameEvent("Settings", "Closed settings panel");
            }
            Rectangle soundSlider = new Rectangle((int)panel.X + 50, (int)panel.Y + 140, 500, 30);
            if (Raylib.IsMouseButtonDown(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), soundSlider))
            {
                _soundVolume = Math.Clamp((Raylib.GetMousePosition().X - soundSlider.X) / soundSlider.Width, 0f, 1f);
                LogGameEvent("Settings", "Sound volume changed to " + _soundVolume.ToString("F2"));
            }         
            Rectangle musicSlider = new Rectangle((int)panel.X + 50, (int)panel.Y + 230, 500, 30);
            if (Raylib.IsMouseButtonDown(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), musicSlider))
            {
                _musicVolume = Math.Clamp((Raylib.GetMousePosition().X - musicSlider.X) / musicSlider.Width, 0f, 1f);
                LogGameEvent("Settings", "Music volume changed to " + _musicVolume.ToString("F2"));
            }
            Rectangle mainMenuButton = new Rectangle(WINDOW_WIDTH / 2 - 150, panel.Y + panel.Height - 80, 240, 50);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), mainMenuButton))
            {
                _isSettingsPanelOpen = false;
                ResetGame(); 
                ChangeState(GameState.MainMenu);
                LogGameEvent("Navigation", "Settings -> Main Menu (Game Reset)");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                _isSettingsPanelOpen = false;
                LogGameEvent("Settings", "Closed settings panel (ESC key)");
            }
        }
        /// <summary>
        /// Private method to check the equipment slot click and handle dragging or unequipping.
        /// </summary>
        private void CheckEquipmentSlotClick(int x, int y, int size, Equipment? equipment)
        {
            if (_player == null)
            {
                return;
            }
            Rectangle slotRect = new Rectangle(x, y, size, size);
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), slotRect))
            {
                if (equipment != null)
                {
                    _hoveredItem = equipment;
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    DateTime now = DateTime.Now;
                    double timeSinceLastClick = (now - _lastClickTime).TotalSeconds;
                    bool isDoubleClick = (timeSinceLastClick < DOUBLE_CLICK_TIME) && (_lastClickedItem == equipment);
                    _lastClickTime = now;
                    _lastClickedItem = equipment;
                    if (equipment != null)
                    {
                        if (isDoubleClick)
                        {
                            _player.Inventory.UnequipItem(equipment);
                            LogGameEvent("Inventory", "Unequipped " + equipment.Name);
                            return;
                        }
                        _draggingEquipment = equipment;
                        _isDragging = true;
                        _dragStartPosition = Raylib.GetMousePosition();
                        _currentDragPosition = _dragStartPosition;
                        _selectedItem = equipment;
                    }
                }
            }
        }
        /// <summary>
        /// Private method to draw the item tooltip when hovering over an item.
        /// </summary>
        private void DrawItemTooltip(Item item, Vector2 mousePosition)
        {
            if (item == null)
            {
                return;
            }
            string name = item.Name;
            string description = item.Description;
            string type = "";
            string stats = "";
            if (item is Weapon weapon)
            {
                type = "Weapon (" + weapon.WeaponType + ")";
                double bonusAttack = weapon.BonusAttack;
                double bonusCritical = weapon.BonusCriticalRate;
                stats = "Damage: +" + bonusAttack;
                if (bonusCritical > 0) stats += "\nCritical: +" + bonusCritical.ToString("P0");
                if (_player != null)
                {
                    (double attackBonus, double critBonus) = _player.GetClassWeaponBonus(weapon);
                    if (attackBonus > 0)
                    {
                        stats += "\nRole Damage Bonus: +" + attackBonus;
                    }
                    if (critBonus > 0)
                    {
                        stats += "\nRole Critical Bonus: +" + critBonus.ToString("P0");
                    }
                }
                stats += "\nDouble-click to equip";
            }
            else if (item is Armor armor)
            {
                type = "Armor (" + armor.ArmorType + ")";
                stats = "Defense: +" + armor.BonusDefense;
                if (armor.BonusHP > 0) stats += "\nHP: +" + armor.BonusHP;
                if (armor.BonusSpeed > 0) stats += "\nSpeed: +" + armor.BonusSpeed;
                if (armor.BonusMana > 0) stats += "\nMana: +" + armor.BonusMana;
                stats += "\nDouble-click to equip";
            }
            else if (item is Potion potion)
            {
                type = "Potion";
                if (potion.HealingAmount > 0){
                    stats = "Healing: +" + potion.HealingAmount + " HP";
                }
                if (potion.Mana > 0){
                    stats += (stats != "" ? "\n" : "") + "Mana: +" + potion.Mana + " MP";
                }
                if (potion.ReducingCooldown > 0){
                    stats += (stats != "" ? "\n" : "") + "Cooldown Reduction: -" + potion.ReducingCooldown + " turns";
                }
                if (potion.ExpAmount > 0){
                    stats += (stats != "" ? "\n" : "") + "Experience: +" + potion.ExpAmount;
                }
                stats += "\nDouble-click to use";
            }
            if (item is Equipment equipment && equipment.Durability > 0)
            {
                stats += "\nDurability: " + equipment.Durability + "/100";
            }
            int padding = 10;
            int lineHeight = 22; 
            int titleHeight = 28; 
            int statLines = stats.Split('\n').Length;
            int descLines = 0;
            if (!string.IsNullOrEmpty(description))
            {
                descLines = (int)Math.Ceiling(description.Length / 25.0) + 1; 
            }
            int tooltipHeight = padding * 4 + titleHeight + lineHeight * 3 +(statLines > 0 ? lineHeight * statLines : 0) +(descLines > 0 ? lineHeight * descLines + padding : 0) +20; 
            int nameWidth = Math.Max(120, name.Length * 12); 
            int descWidth = Math.Max(200, description.Length * 9);
            int typeWidth = type.Length * 9;
            int statsWidth = 0;
            foreach (string line in stats.Split('\n'))
            {
                statsWidth = Math.Max(statsWidth, line.Length * 9);
            }
            int tooltipWidth = Math.Max(250, Math.Max(nameWidth, Math.Max(descWidth, Math.Max(typeWidth, statsWidth)))) + padding * 3;
            int tooltipX = (int)mousePosition.X + 20;
            int tooltipY = (int)mousePosition.Y;
            if (tooltipX + tooltipWidth > WINDOW_WIDTH)
            {
                tooltipX = (int)mousePosition.X - tooltipWidth - 20;
                if (tooltipX < 5)
                {
                    tooltipX = Math.Max(5, WINDOW_WIDTH - tooltipWidth - 5);
                }
            }
            if (tooltipY + tooltipHeight > WINDOW_HEIGHT)
            {
                tooltipY = (int)mousePosition.Y - tooltipHeight - 10;
                if (tooltipY < 5)
                {
                    tooltipY = 5;
                }
            }
            Rectangle tooltipRect = new Rectangle(tooltipX, tooltipY, tooltipWidth, tooltipHeight);
            Rectangle shadowRect = new Rectangle(tooltipX + 4, tooltipY + 4, tooltipWidth, tooltipHeight);
            Raylib.DrawRectangleRec(shadowRect, new Raylib_cs.Color(20, 20, 20, 180));
            Raylib.DrawRectangleRec(tooltipRect, new Raylib_cs.Color(50, 50, 70, 240));
            Raylib.DrawRectangleLinesEx(tooltipRect, 2, Raylib_cs.Color.Gold);
            Rectangle innerBorder = new Rectangle(tooltipX + 3, tooltipY + 3, tooltipWidth - 6, tooltipHeight - 6);
            Raylib.DrawRectangleLinesEx(innerBorder, 1, new Raylib_cs.Color(80, 80, 100, 255));
            int currentY = tooltipY + padding;
            Raylib.DrawText(name, tooltipX + padding + 1, currentY + 1, 20, new Raylib_cs.Color(120, 100, 0, 128));
            Raylib.DrawText(name, tooltipX + padding, currentY, 20, Raylib_cs.Color.Gold);
            currentY += titleHeight;
            Raylib.DrawText("Type: " + type, tooltipX + padding, currentY, 16, Raylib_cs.Color.LightGray);
            currentY += lineHeight;
            string tierText = "Tier: " + item.Tier;
            Raylib.DrawText(tierText, tooltipX + padding, currentY, 16, Raylib_cs.Color.Yellow);
            currentY += lineHeight;
            Raylib.DrawText("Value: " + item.Price + " gold", tooltipX + padding, currentY, 16, item.Price > 0 ? Raylib_cs.Color.Gold : Raylib_cs.Color.LightGray);
            currentY += lineHeight;
            Raylib.DrawLine(tooltipX + padding, currentY, tooltipX + tooltipWidth - padding, currentY, new Raylib_cs.Color(100, 100, 120, 200));
            currentY += lineHeight / 2;
            if (!string.IsNullOrEmpty(stats))
            {
                foreach (string line in stats.Split('\n'))
                {
                    Raylib_cs.Color statColor = Raylib_cs.Color.White;
                    if (line.Contains("Damage")) statColor = Raylib_cs.Color.Red;
                    else if (line.Contains("Defense")) statColor = Raylib_cs.Color.Blue;
                    else if (line.Contains("HP") || line.Contains("Healing")) statColor = Raylib_cs.Color.Green;
                    else if (line.Contains("Speed")) statColor = Raylib_cs.Color.Yellow;
                    else if (line.Contains("Mana")) statColor = Raylib_cs.Color.Purple;
                    else if (line.Contains("Experience")) statColor = Raylib_cs.Color.SkyBlue;
                    else if (line.Contains("Critical")) statColor = new Raylib_cs.Color(255, 150, 50, 255); // Orange
                    else if (line.Contains("Double-click")) statColor = new Raylib_cs.Color(255, 200, 100, 255);
                    {
                        if (item is Equipment equipItem)
                        {
                            float durabilityRatio = (float)equipItem.Durability / 100f;
                            if (durabilityRatio > 0.7f) statColor = Raylib_cs.Color.Green;
                            else if (durabilityRatio > 0.3f) statColor = Raylib_cs.Color.Yellow;
                            else statColor = Raylib_cs.Color.Red;
                        }
                    }
                    Raylib.DrawText(line, tooltipX + padding, currentY, 16, statColor);
                    currentY += lineHeight;
                }
                currentY += lineHeight / 2;
                Raylib.DrawLine(tooltipX + padding, currentY, tooltipX + tooltipWidth - padding, currentY, new Raylib_cs.Color(100, 100, 120, 200));
                currentY += lineHeight / 2;
            }
            if (!string.IsNullOrEmpty(description))
            {
                int descFontSize = 14;
                int maxCharsPerLine = (tooltipWidth - padding * 3) / 7; 
                for (int i = 0; i < description.Length;)
                {
                    int length = Math.Min(maxCharsPerLine, description.Length - i);
                    if (i + length < description.Length && length == maxCharsPerLine)
                    {
                        int lastSpace = description.Substring(i, length).LastIndexOf(' ');
                        if (lastSpace > 0)
                        {
                            length = lastSpace + 1; 
                        }
                    }
                    string line = description.Substring(i, length);
                    Raylib.DrawText(line, tooltipX + padding, currentY, descFontSize, Raylib_cs.Color.LightGray);
                    i += length;
                    currentY += lineHeight - 2; 
                }
                currentY += padding;
            }
        }
        /// <summary>
        /// Private method to reset the game state and clear all game data.
        /// </summary>
        private void ResetGame()
        {
            _victoryCreditsTransition = false;
            _currentState = GameState.MainMenu;
            _currentLevel = 1;
            _player = null;
            _map.Clear();
            _units.Clear();
            _selectedCharacterIndex = 0;
            _playerName = "";
            _currentDifficulty = Difficulty.Medium;
            _inShopInteraction = false;
            _inBlacksmithInteraction = false;
            _selectedItem = null;
            _draggingEquipment = null;
            _isDragging = false; _dragStartPosition = new Vector2(0, 0);
            _currentDragPosition = new Vector2(0, 0);
            _inventoryScrollPosition = 0;
            _battleItemScrollPosition = 0;
            _hoveredItem = null;
            _lastClickTime = DateTime.MinValue;
            _lastClickedItem = null;
            LogGameEvent("Game State", "Game fully reset");
        }
        /// <summary>
        /// Public method to clean up all game resources and textures.
        /// </summary>
        public void Cleanup()
        {

            Tile.UnloadAllTextures();
            Weapon.UnloadAllTextures();
            Armor.UnloadAllTextures();
            Potion.UnloadAllTextures();
            foreach (Unit unit in _units)
            {
                if (unit is Monster monster)
                {
                    monster.UnloadTextures();
                }
            }
            if (_player != null)
            {
                _player.UnloadTextures();
            }
            Merchant.UnloadMerchantTextures();
            BlackSmith.UnloadBlacksmithTextures();
            LogGameEvent("Game State", "Resources cleaned up");
        }
        /// <summary>
        /// Private method to initialize the battle state with the given monster.
        /// </summary>
        private void InitializeBattle(Monster monster)
        {
            if (_player == null)
            {
                return;
            }
            _isPlayerTurn = _player.Speed >= monster.Speed;
            _showSkillSelection = false;
            _showItemSelection = false;
            _selectedSkillIndex = -1;
            _turnTransitionTimer = 0f;
            _showAttackEffect = false;
            _attackEffectDuration = 0f;
            _isPlayerAttacking = false;
            _isTurnInProgress = false;
            _currentMonsterAttackingType = "";
            _showVictoryScreen = false;
            _victoryRewardsGranted = false;
            _escapeSuccessful = false;
            _escapeAttemptInProgress = false;
            string firstAttacker = _isPlayerTurn ? "You" : monster.Name;
            _battleLogText = firstAttacker + " will move first!";
            _battleLog.Clear();
            _battleLog.Add(_battleLogText);
        }
        /// <summary>
        /// Public method to add a notification (small chat box) to the game.
        /// </summary>
        public void AddNotification(string message, double duration = 3.0, Raylib_cs.Color? color = null)
        {
            if (string.IsNullOrEmpty(message)){
                return;
            }
            Raylib_cs.Color notificationColor = color ?? Raylib_cs.Color.White;
            _notifications.Add((message, DateTime.Now, duration, notificationColor));
            LogGameEvent("Notification", message);
        }
        /// <summary>
        /// Private method to update and remove expired notifications.
        /// </summary>
        private void UpdateNotifications()
        {
            _notifications.RemoveAll(n => (DateTime.Now - n.Time).TotalSeconds >= n.Duration);
        }
        /// <summary>
        /// Public method to clear all notifications.
        /// </summary>
        public void ClearNotifications()
        {
            _notifications.Clear();
        }
        /// <summary>
        /// Private method to draw all active notifications.
        /// </summary>
        private void DrawNotifications()
        {
            if (_notifications.Count == 0){
                return;
            }
            UpdateNotifications();
            const int MAX_NOTIFICATIONS = 5;
            const int PADDING = 15;
            const int BASE_Y = WINDOW_HEIGHT - 200;
            const int NOTIFICATION_HEIGHT = 130;
            const int SPACING = 120;
            const float ANIMATION_DURATION = 0.3f;
            List<(string Message, DateTime Time, double Duration, Raylib_cs.Color Color)> sortedNotifications = _notifications.OrderByDescending(n => n.Time).Take(MAX_NOTIFICATIONS).ToList();
            for (int i = 0; i < sortedNotifications.Count; i++)
            {
                (string Message, DateTime Time, double Duration, Raylib_cs.Color Color) notification = sortedNotifications[i];
                double elapsed = (DateTime.Now - notification.Time).TotalSeconds;
                int yPos = BASE_Y - (SPACING * i);
                float entryOffset = 0;
                if (elapsed < ANIMATION_DURATION)
                {
                    entryOffset = (float)(WINDOW_WIDTH * 0.2 * (1 - (elapsed / ANIMATION_DURATION)));
                }
                float opacity = 1.0f;
                if (elapsed > notification.Duration - 1.0)
                {
                    opacity = (float)Math.Max(0, (notification.Duration - elapsed));
                }
                else if (elapsed < ANIMATION_DURATION)
                {
                    opacity = (float)(elapsed / ANIMATION_DURATION);
                }
                int textWidth = Raylib.MeasureText(notification.Message, 20);
                int rectWidth = textWidth + (PADDING * 2);
                Raylib_cs.Color bgColor = new Raylib_cs.Color(40, 40, 60, (int)(200 * opacity));
                Raylib_cs.Color borderColor = new Raylib_cs.Color( notification.Color.R, notification.Color.G,notification.Color.B, (byte)(opacity * 200) );
                float staggerOffset = i * 5.0f;
                Rectangle notificationRect = new Rectangle(WINDOW_WIDTH / 2 - rectWidth / 2 - PADDING + entryOffset + staggerOffset, yPos - NOTIFICATION_HEIGHT / 2, rectWidth, NOTIFICATION_HEIGHT );
                Rectangle shadowRect = new Rectangle(notificationRect.X + 3,notificationRect.Y + 3, notificationRect.Width,notificationRect.Height );
                Raylib.DrawRectangleRounded(shadowRect, 0.3f, 6, new Raylib_cs.Color(0, 0, 0, (int)(70 * opacity)));
                Raylib.DrawRectangleRounded(notificationRect, 0.3f, 6, bgColor);
                Raylib.DrawRectangleLinesEx(notificationRect, 2, borderColor);
                Raylib_cs.Color textColor = new Raylib_cs.Color(notification.Color.R,notification.Color.G,notification.Color.B,(byte)(opacity * 255) );
                Raylib.DrawText( notification.Message,(int)(WINDOW_WIDTH / 2 - textWidth / 2 + entryOffset + staggerOffset),yPos - 20, 20, textColor );
                if (i == 0)
                {
                    Rectangle highlightRect = new Rectangle( notificationRect.X - 2, notificationRect.Y - 2, notificationRect.Width + 4, notificationRect.Height + 4);
                    Raylib.DrawRectangleLinesEx(highlightRect, 1, new Raylib_cs.Color( borderColor.R, borderColor.G, borderColor.B, (byte)(opacity * 100) ));
                }
            }
        }
        /// <summary>
        /// Property to get or set the last click time for double-click detection.
        /// </summary>
        public DateTime LastClickTime
        {
            get
            {
                return _lastClickTime;
            }
            set
            {
                _lastClickTime = value;
            }
        }
        /// <summary>
        /// Property to get or set the last clicked item.
        /// </summary>
        public Item? LastClickedItem
        {
            get
            {
                return _lastClickedItem;
            }
            set
            {
                _lastClickedItem = value;
            }
        }
        /// <summary>
        /// Property to get the double-click time threshold.
        /// </summary>
        public static double DoubleClickTime
        {
            get
            {
                return DOUBLE_CLICK_TIME;
            }
        }
    }
}
