using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask
{
    /// <summary>
    /// This is the Merchant class, which inherits from the NPC class, that has 8 extra attributes.
    /// </summary>
    public class Merchant : NPC
    {
        private static Raylib_cs.Texture2D _merchantTexture;
        private static Raylib_cs.Texture2D _merchantShopTexture;
        private static bool _merchantTextureLoaded = false;
        private static bool _merchantShopTextureLoaded = false;
        private List<Item> _shopItems;
        private int _shopScrollPosition = 0;
        private Item? _selectedShopItem = null;
        private ShopTab _activeShopTab = ShopTab.Buy;
        /// <summary>
        /// Parameterized constructor for the merchant that sets the name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column , isAlive and initializes the shop items.
        /// </summary>
        public Merchant(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
            _shopItems = new List<Item>();
            GenerateShopInventory(level);
        }
        /// <summary>
        /// Generates the shop inventory based on the player's level.
        /// </summary>
        public void GenerateShopInventory(int playerLevel)
        {
            _shopItems.Clear();
            Random random = new Random();
            int itemCount = random.Next(6, 12);
            int potionCount = Math.Max(2, itemCount / 3);
            int equipmentCount = Math.Max(2, itemCount - potionCount);
            for (int i = 0; i < potionCount; i++)
            {
                int potionTier = Math.Max(1, playerLevel / 2);
                string[] potionTypes = { "HEALING", "MANA", "EXPBOOST", "REDUCECOOLDOWN" };
                string potionType = potionTypes[random.Next(potionTypes.Length)];
                string potionName;
                string description;
                switch (potionType)
                {
                    case "HEALING":
                        potionName = "Level " + potionTier + " Healing Potion";
                        description = "Restores health when used";
                        break;
                    case "MANA":
                        potionName = "Level " + potionTier + " Mana Potion";
                        description = "Restores mana when used";
                        break;
                    case "EXPBOOST":
                        potionName = "Level " + potionTier + " Exp Potion";
                        description = "Grants additional experience";
                        break;
                    case "REDUCECOOLDOWN":
                        potionName = "Level " + potionTier + " ReduceCD Potion";
                        description = "Reduces skill cooldowns";
                        break;
                    default:
                        potionName = "Level " + potionTier + " Mystery Potion";
                        description = "Effect unknown";
                        break;
                }
                Item potion = Item.CreatePotionFromData(potionType,potionTier,100 + random.Next(50),potionName,description,(12 + (potionTier * 6)) * 2);
                if (potion != null)
                {
                    _shopItems.Add(potion);
                    int additionalPotions = random.Next(3);
                    for (int j = 0; j < additionalPotions; j++)
                    {
                        Item additionalPotion = Item.CreatePotionFromData( potionType,potionTier,100 + random.Next(50),potionName,description,(12 + (potionTier * 6)) * 2);
                        if (additionalPotion != null)
                        {
                            _shopItems.Add(additionalPotion);
                        }
                    }
                }
            }
            for (int i = 0; i < equipmentCount; i++)
            {
                int equipTier = Math.Max(1, playerLevel / 2);
                if (random.Next(2) == 0)
                {
                    WeaponType weaponType = (WeaponType)random.Next(3);
                    string weaponTypeName = weaponType.ToString();
                    Weapon weapon = Item.CreateWeaponFromData(weaponType,equipTier,200 + random.Next(100),weaponTypeName,"A well-crafted " + weaponTypeName.ToLower() + " sold by a reputable merchant",(20 + (equipTier * 12)) * 2);
                    _shopItems.Add(weapon);
                }
                else
                {
                    ArmorType armorType = (ArmorType)random.Next(6);
                    string armorTypeName = armorType.ToString();
                    Armor armor = Item.CreateArmorFromData(armorType,equipTier,300 + random.Next(100),armorTypeName,"A well-crafted " + armorTypeName.ToLower() + " sold by a reputable merchant",(15 + (equipTier * 10)) * 2);
                    _shopItems.Add(armor);
                }
            }
            _shopItems = _shopItems.OrderBy(item => item.GetType().Name).ThenBy(item => item.Price) .ToList();
        }
        /// <summary>
        /// Loads the merchant textures.
        /// </summary>
        public static void LoadMerchantTextures()
        {
            if (!_merchantTextureLoaded)
            {
                _merchantTexture = Raylib.LoadTexture("picture/NPC/Merchant.png");
                _merchantTextureLoaded = true;
            }

            if (!_merchantShopTextureLoaded)
            {
                _merchantShopTexture = Raylib.LoadTexture("picture/NPC/Merchant_Shop.png");
                _merchantShopTextureLoaded = true;
            }
        }
        /// <summary>
        /// Unloads the merchant textures.
        /// </summary>
        public static void UnloadMerchantTextures()
        {
            if (_merchantTextureLoaded)
            {
                Raylib.UnloadTexture(_merchantTexture);
                _merchantTextureLoaded = false;
            }

            if (_merchantShopTextureLoaded)
            {
                Raylib.UnloadTexture(_merchantShopTexture);
                _merchantShopTextureLoaded = false;
            }
        }
        /// <summary>
        /// Gets the merchant texture.
        /// </summary>
        public static Raylib_cs.Texture2D GetMerchantTexture()
        {
            return _merchantTexture;
        }
        /// <summary>
        /// Gets the merchant shop texture.
        /// </summary>
        public static Raylib_cs.Texture2D GetMerchantShopTexture()
        {
            return _merchantShopTexture;
        }
        /// <summary>
        /// Draws the UI for the merchant shop, including the shop items, buy/sell tabs, and item details.
        /// </summary>
        public override string DrawUI(Player player, Game game)
        {
            if (player == null)
            {
                return string.Empty;
            }
            game.ClearNotifications();
            Raylib.DrawRectangle(0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, 180));
            int merchantX = 250;
            int merchantY = Game.WINDOW_HEIGHT / 2 + 300;
            Rectangle frameRect = new Rectangle(0,0,500,500 );
            Raylib.DrawTexturePro(Merchant.GetMerchantShopTexture(), frameRect, new Rectangle(merchantX, merchantY, 500, 500), new Vector2(500 / 2, 500 / 2),0, Raylib_cs.Color.White );
            int bubbleX = merchantX - 150;
            int bubbleY = merchantY - 230;
            int bubbleWidth = 300;
            int bubbleHeight = 120;
            float time = (float)Raylib.GetTime();
            float bounceEffect = (float)Math.Sin(time * 2.0f) * 3.0f;
            Raylib.DrawRectangleRounded( new Rectangle(bubbleX, bubbleY + bounceEffect, bubbleWidth, bubbleHeight), 0.3f, 8, new Raylib_cs.Color(255, 255, 255, 230) );
            Raylib.DrawRectangleRoundedLines( new Rectangle(bubbleX, bubbleY + bounceEffect, bubbleWidth, bubbleHeight), 0.3f, 8, new Raylib_cs.Color(100, 100, 100, 255) );
            Raylib.DrawTriangle(  new Vector2(bubbleX + bubbleWidth / 2 - 10, bubbleY + bubbleHeight + bounceEffect),  new Vector2(bubbleX + bubbleWidth / 2 + 10, bubbleY + bubbleHeight + bounceEffect),  new Vector2(bubbleX + bubbleWidth / 2, bubbleY + bubbleHeight + 20 + bounceEffect),  new Raylib_cs.Color(255, 255, 255, 230) );
            string fullText = "Welcome to my shop!";
            int charCount = Math.Min(fullText.Length, (int)(time * 10) % (fullText.Length + 30));
            if (charCount > fullText.Length) charCount = fullText.Length; // Handle the pause at the end
            string visibleText = fullText.Substring(0, charCount);
            Raylib.DrawText(this.Name, bubbleX + 20, bubbleY + 20, 20, Raylib_cs.Color.DarkBlue);
            Raylib.DrawLine(bubbleX + 20, bubbleY + 45, bubbleX + bubbleWidth - 20, bubbleY + 45, Raylib_cs.Color.Gray);
            Vector2 textPos = new Vector2(bubbleX + 20, bubbleY + 55);
            Raylib.DrawText(visibleText, (int)textPos.X, (int)textPos.Y, 18, Raylib_cs.Color.Black);
            Rectangle panel = new Rectangle(Game.WINDOW_WIDTH / 2 - 300, 100, 1000, 1050);
            Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(30, 30, 50, 240));
            Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Gold);
            Raylib.DrawText($"Your Gold: {player.Inventory.Money}", Game.WINDOW_WIDTH / 2 - 180, 170, 36, Raylib_cs.Color.Gold);
            Rectangle buyTabRect = new Rectangle(Game.WINDOW_WIDTH / 2, 250, 200, 40);
            Rectangle sellTabRect = new Rectangle(Game.WINDOW_WIDTH / 2 + 200, 250, 200, 40);
            Raylib.DrawRectangleRec(buyTabRect, _activeShopTab == ShopTab.Buy ? new Raylib_cs.Color(80, 80, 120, 255) :new Raylib_cs.Color(50, 50, 70, 255));
            Raylib.DrawRectangleRec(sellTabRect, _activeShopTab == ShopTab.Sell ?  new Raylib_cs.Color(80, 80, 120, 255) : new Raylib_cs.Color(50, 50, 70, 255));
            Raylib.DrawRectangleLinesEx(buyTabRect, 2, Raylib_cs.Color.Gold);
            Raylib.DrawRectangleLinesEx(sellTabRect, 2, Raylib_cs.Color.Gold);
            Raylib.DrawText("BUY", (int)buyTabRect.X + 70, (int)buyTabRect.Y + 10, 24, Raylib_cs.Color.White);
            Raylib.DrawText("SELL", (int)sellTabRect.X + 70, (int)sellTabRect.Y + 10, 24, Raylib_cs.Color.White);
            Rectangle itemsArea = new Rectangle(Game.WINDOW_WIDTH / 2 - 200, 320, 800, 500);
            Raylib.DrawRectangleRec(itemsArea, new Raylib_cs.Color(20, 20, 35, 200));
            Raylib.DrawRectangleLinesEx(itemsArea, 2, Raylib_cs.Color.LightGray);
            List<Item> itemsList = _activeShopTab == ShopTab.Buy ?  _shopItems : player.Inventory.Items;
            if (itemsList.Count > 0)
            {
                int itemHeight = 80;
                int visibleItems = 6;
                if (itemsList.Count > visibleItems)
                {
                    Rectangle scrollTrack = new Rectangle( itemsArea.X + itemsArea.Width - 20, itemsArea.Y + 10, 10, itemsArea.Height - 20 );
                    Raylib.DrawRectangleRec(scrollTrack, new Raylib_cs.Color(50, 50, 70, 255));
                    float handleRatio = (float)visibleItems / itemsList.Count;
                    float handleHeight = Math.Max(30, scrollTrack.Height * handleRatio);
                    int maxScroll = Math.Max(0, itemsList.Count - visibleItems);
                    float scrollProgress = maxScroll > 0 ? (float)_shopScrollPosition / maxScroll : 0;
                    float handleY = scrollTrack.Y + scrollProgress * (scrollTrack.Height - handleHeight);
                    Rectangle scrollHandle = new Rectangle( scrollTrack.X, handleY, 10, handleHeight );
                    Raylib.DrawRectangleRec(scrollHandle, Raylib_cs.Color.Gold);
                }
                for (int i = 0; i < Math.Min(visibleItems, itemsList.Count - _shopScrollPosition); i++)
                {
                    int itemIndex = i + _shopScrollPosition;
                    if (itemIndex >= itemsList.Count) break;
                    Item item = itemsList[itemIndex];
                    bool isSelected = item == _selectedShopItem;
                    Rectangle itemRect = new Rectangle( itemsArea.X + 20, itemsArea.Y + (i * itemHeight) + 10, itemsArea.Width - 40, itemHeight - 10 );
                    Raylib_cs.Color bgColor = isSelected ? new Raylib_cs.Color(80, 70, 50, 255) : new Raylib_cs.Color(40, 40, 55, 255);
                    Raylib.DrawRectangleRec(itemRect, bgColor);
                    Raylib.DrawRectangleLinesEx(itemRect, isSelected ? 2 : 1, isSelected ? Raylib_cs.Color.Gold : Raylib_cs.Color.Gray);
                    Rectangle iconRect = new Rectangle(itemRect.X + 10, itemRect.Y + 10, 60, 60);
                    if (item is Weapon weapon)
                    {
                        try
                        {
                            Raylib_cs.Texture2D texture = weapon.GetTexture();
                            float weaponscale = 60f / Math.Max(texture.Width, texture.Height);
                            Raylib.DrawTextureEx(texture, new Vector2(iconRect.X, iconRect.Y), 0, weaponscale, Raylib_cs.Color.White);
                        }
                        catch
                        {
                            Raylib.DrawText("W", (int)iconRect.X + 25, (int)iconRect.Y + 20, 30, Raylib_cs.Color.White);
                        }
                    }
                    else if (item is Armor armor)
                    {
                        try
                        {
                            Raylib_cs.Texture2D texture = armor.GetTexture();
                            float armorscale = 60f / Math.Max(texture.Width, texture.Height);
                            Raylib.DrawTextureEx(texture,  new Vector2(iconRect.X, iconRect.Y), 0, armorscale, Raylib_cs.Color.White);
                        }
                        catch
                        {
                            Raylib.DrawText("A", (int)iconRect.X + 25, (int)iconRect.Y + 20, 30, Raylib_cs.Color.White);
                        }
                    }
                    else if (item is Potion potion)
                    {
                        try
                        {
                            Raylib_cs.Texture2D texture = potion.GetTexture();
                            float potionscale = 60f / Math.Max(texture.Width, texture.Height);
                            Raylib.DrawTextureEx(texture,  new Vector2(iconRect.X, iconRect.Y), 0, potionscale, Raylib_cs.Color.White);
                        }
                        catch
                        {
                            Raylib.DrawText("P", (int)iconRect.X + 25, (int)iconRect.Y + 20, 30, Raylib_cs.Color.White);
                        }
                    }
                    Raylib.DrawText(item.Name, (int)itemRect.X + 80, (int)itemRect.Y + 10, 20, Raylib_cs.Color.White);
                    string priceText = _activeShopTab == ShopTab.Buy ? "Price: " + item.Price + " gold" : "Sell for: " + ((int)(item.Price * 0.5)) + " gold";

                    Raylib.DrawText(priceText, (int)itemRect.X + 80, (int)itemRect.Y + 35, 16, Raylib_cs.Color.Gold);
                    if (item is Potion && item.Quantity > 1)
                    {
                        Raylib.DrawText("Quantity: " + item.Quantity, (int)itemRect.X + 300, (int)itemRect.Y + 35, 16, Raylib_cs.Color.LightGray);
                    }

                }
            }
            else
            {
                string noItemsText = _activeShopTab == ShopTab.Buy ? "Merchant has no items to sell!" : "You have no items to sell!";
                int textWidth = Raylib.MeasureText(noItemsText, 24);
                Raylib.DrawText(noItemsText,  (int)(itemsArea.X + itemsArea.Width / 2 - textWidth / 2),(int)(itemsArea.Y + itemsArea.Height / 2),  24, Raylib_cs.Color.Gray);
            }
            if (_selectedShopItem != null)
            {
                Rectangle detailRect = new Rectangle(Game.WINDOW_WIDTH / 2 - 200, 840, 800, 150);
                Raylib.DrawRectangleRec(detailRect, new Raylib_cs.Color(40, 40, 60, 255));
                Raylib.DrawRectangleLinesEx(detailRect, 2, Raylib_cs.Color.Gold);
                string actionText = _activeShopTab == ShopTab.Buy ? "BUY" : "SELL";
                Rectangle actionButton = new Rectangle(Game.WINDOW_WIDTH / 2 + 400, 1030, 180, 60);
                Raylib_cs.Color buttonColor;
                if (_activeShopTab == ShopTab.Buy && player.Inventory.Money < _selectedShopItem.Price)
                {
                    buttonColor = new Raylib_cs.Color(100, 50, 50, 255);
                }
                else
                {
                    buttonColor = new Raylib_cs.Color(50, 100, 50, 255);
                }
                Raylib.DrawRectangleRec(actionButton, buttonColor);
                Raylib.DrawRectangleLinesEx(actionButton, 2, Raylib_cs.Color.White);
                int textWidth = Raylib.MeasureText(actionText, 30);
                Raylib.DrawText(actionText, (int)(actionButton.X + actionButton.Width / 2 - textWidth / 2), (int)(actionButton.Y + 15),  35,  Raylib_cs.Color.White);
                Raylib.DrawText(_selectedShopItem.Description,  (int)(detailRect.X + 20), (int)(detailRect.Y + 20), 25, Raylib_cs.Color.White);
                if (_activeShopTab == ShopTab.Buy && player.Inventory.Money < _selectedShopItem.Price)
                {
                    string warningText = "Not enough gold!";
                    int warningWidth = Raylib.MeasureText(warningText, 20);
                    Raylib.DrawText(warningText, (int)(detailRect.X + detailRect.Width - warningWidth - 20), (int)(detailRect.Y + 80), 20, Raylib_cs.Color.Red);
                }
            }
            Rectangle exitButton = new Rectangle(Game.WINDOW_WIDTH - 200, 140, 60, 60);
            bool exitHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), exitButton);
            Raylib.DrawRectangleRec(exitButton, exitHovered ? new Raylib_cs.Color(120, 40, 40, 255) : new Raylib_cs.Color(80, 30, 30, 255));
            Raylib.DrawRectangleLinesEx(exitButton, 2, Raylib_cs.Color.White);
            int centerX = (int)(exitButton.X + exitButton.Width / 2);
            int centerY = (int)(exitButton.Y + exitButton.Height / 2);
            int xSize = 15;
            Raylib.DrawLine(centerX - xSize, centerY - xSize, centerX + xSize, centerY + xSize, Raylib_cs.Color.White);
            Raylib.DrawLine(centerX + xSize, centerY - xSize, centerX - xSize, centerY + xSize, Raylib_cs.Color.White);
            return "Opening Merchant Shop";
        }
        /// <summary>
        /// Handles the UI input for the merchant shop, including buying and selling items, scrolling through items, and exiting the shop.
        /// </summary>
        public override bool HandleUIInput(Player player, Game game)
        {
            if (player == null) return false;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                Rectangle buyTabRect = new Rectangle(Game.WINDOW_WIDTH / 2, 250, 200, 40);
                Rectangle sellTabRect = new Rectangle(Game.WINDOW_WIDTH / 2 + 200, 250, 200, 40);
                if (Raylib.CheckCollisionPointRec(mousePos, buyTabRect))
                {
                    _activeShopTab = ShopTab.Buy;
                    _shopScrollPosition = 0;
                    _selectedShopItem = null;
                }
                else if (Raylib.CheckCollisionPointRec(mousePos, sellTabRect))
                {
                    _activeShopTab = ShopTab.Sell;
                    _shopScrollPosition = 0;
                    _selectedShopItem = null;
                }
                List<Item> itemsList = _activeShopTab == ShopTab.Buy ? _shopItems : player.Inventory.Items;
                Rectangle itemsArea = new Rectangle(Game.WINDOW_WIDTH / 2 - 200, 320, 800, 700);
                int itemHeight = 80;
                int visibleItems = 6;
                for (int i = 0; i < Math.Min(visibleItems, itemsList.Count - _shopScrollPosition); i++)
                {
                    int itemIndex = i + _shopScrollPosition;
                    if (itemIndex >= itemsList.Count) break;
                    Rectangle itemRect = new Rectangle( itemsArea.X + 20, itemsArea.Y + (i * itemHeight) + 10, itemsArea.Width - 40, itemHeight - 10 );
                    if (Raylib.CheckCollisionPointRec(mousePos, itemRect))
                    {
                        _selectedShopItem = itemsList[itemIndex];
                    }
                }
                if (_selectedShopItem != null)
                {
                    Rectangle actionButton = new Rectangle(Game.WINDOW_WIDTH / 2 + 400, 1030, 180, 60);
                    if (Raylib.CheckCollisionPointRec(mousePos, actionButton))
                    {
                        if (_activeShopTab == ShopTab.Buy)
                        {
                            if (player.Inventory.Money >= _selectedShopItem.Price)
                            {
                                player.Inventory.Money -= _selectedShopItem.Price;
                                Item boughtItem = _selectedShopItem;
                                player.Inventory.AddItem(boughtItem);
                                game.AddNotification("Bought " + _selectedShopItem.Name + " for " + _selectedShopItem.Price + " gold", 2.0, Raylib_cs.Color.Green);
                                game.LogGameEvent("Shop", "Player bought " + _selectedShopItem.Name + " for " + _selectedShopItem.Price + " gold");
                            }
                            else
                            {
                                game.AddNotification("Not enough gold!", 2.0, Raylib_cs.Color.Red);
                                game.LogGameEvent("Shop", "Player attempted purchase but had insufficient gold");
                            }
                        }
                        else
                        {
                            int sellPrice = (int)(_selectedShopItem.Price * 0.5);
                            player.Inventory.Money += sellPrice;
                            player.Inventory.RemoveItem(_selectedShopItem);
                            game.AddNotification("Sold " + _selectedShopItem.Name + " for " + sellPrice + " gold", 2.0, Raylib_cs.Color.Green);
                            game.LogGameEvent("Shop", "Player sold " + _selectedShopItem.Name + " for " + sellPrice + " gold");
                            _selectedShopItem = null;
                        }
                    }
                }
                Rectangle exitButton = new Rectangle(Game.WINDOW_WIDTH - 200, 140, 60, 60);
                if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
                {
                    return true;
                }
            }
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(Game.WINDOW_WIDTH / 2 - 400, 220, 800, 500)))
            {
                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    List<Item> itemsList = _activeShopTab == ShopTab.Buy ? _shopItems : player.Inventory.Items;
                    int maxScroll = Math.Max(0, itemsList.Count - 6);
                    _shopScrollPosition = Math.Clamp(_shopScrollPosition - (int)wheel, 0, maxScroll);
                }
            }
            return false;
        }
    }
}