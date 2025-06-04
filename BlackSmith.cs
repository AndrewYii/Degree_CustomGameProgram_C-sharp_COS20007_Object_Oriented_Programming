using System;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the BlackSmith class, inherits from the NPC class, has extra 11 attributes.
    /// </summary>
    public class BlackSmith : NPC
    {
        private static Raylib_cs.Texture2D _blacksmithTexture;
        private static Raylib_cs.Texture2D _blacksmithShopTexture;
        private static bool _blacksmithTextureLoaded = false;
        private static bool _blacksmithShopTextureLoaded = false;
        private int _shopScrollPosition = 0;
        private Item? _firstCombineItem = null;
        private Item? _secondCombineItem = null;
        private bool _isDraggingForCrafting = false;
        private Item? _draggingItem = null;
        private Vector2 _draggingItemPosition = new Vector2();
        private bool _canCombine = false;
        /// <summary>
        /// Parameterized constructor for the BlackSmith class to set the name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column and isAlive.
        /// </summary>
        public BlackSmith(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
        }
        /// <summary>
        /// Method to check if two items can be combined.
        /// </summary>
        public bool CanCombineItems(Item first, Item second)
        {
            if (first == null || second == null)
            {
                return false;
            }
            if (first.GetType() != second.GetType())
            {
                return false;
            }
            if (first is Weapon && second is Weapon)
            {
                Weapon weapon1 = (Weapon)first;
                Weapon weapon2 = (Weapon)second;
                bool sameType = weapon1.WeaponType == weapon2.WeaponType;
                if (!sameType)
                {
                    return false;
                }

                return sameType && first.Tier == second.Tier;
            }
            else if (first is Armor && second is Armor)
            {
                Armor armor1 = (Armor)first;
                Armor armor2 = (Armor)second;
                bool sameType = armor1.ArmorType == armor2.ArmorType;
                if (!sameType)
                {
                    return false;
                }
                return sameType && first.Tier == second.Tier;
            }
            else if (first is Potion && second is Potion)
            {
                Potion potion1 = (Potion)first;
                Potion potion2 = (Potion)second;
                bool sameType = potion1.PotionType == potion2.PotionType;
                bool sameTier = first.Tier == second.Tier;
                return sameType && sameTier;
            }
            return false;
        }
        /// <summary>
        /// Method to combine two items into a new item of a higher tier.
        /// </summary>
        public Item? CombineItems(Item first, Item second, int newTier)
        {
            if (!CanCombineItems(first, second)){
                return null;
            }
            Item? newItem = null;
            if (first is Weapon)
            {
                Weapon oldWeapon = (Weapon)first;
                newItem = Item.CreateWeaponFromData(oldWeapon.WeaponType,newTier,oldWeapon.ItemID + 1000,"Level " + newTier + " " + oldWeapon.WeaponType.ToString(),"A tier " + newTier + " weapon crafted by the blacksmith",CalculateUpgradedPrice(oldWeapon.Price, newTier));
            }
            else if (first is Armor)
            {
                Armor oldArmor = (Armor)first;
                newItem = Item.CreateArmorFromData(oldArmor.ArmorType,newTier,oldArmor.ItemID + 1000,"Level " + newTier + " " + oldArmor.ArmorType.ToString(),"A tier " + newTier + " armor crafted by the blacksmith",CalculateUpgradedPrice(oldArmor.Price, newTier));
            }
            else if (first is Potion)
            {
                Potion oldPotion = (Potion)first;
                string potionTypeName;
                switch (oldPotion.PotionType)
                {
                    case PotionType.Healing:
                        potionTypeName = "Healing Potion";
                        break;
                    case PotionType.Mana:
                        potionTypeName = "Mana Potion";
                        break;
                    case PotionType.ReduceCooldown:
                        potionTypeName = "ReduceCD Potion";
                        break;
                    case PotionType.ExpBoost:
                        potionTypeName = "EXP Potion";
                        break;
                    default:
                        potionTypeName = "Unknown Potion";
                        break;
                }
                string potionTypeString;
                switch (oldPotion.PotionType)
                {
                    case PotionType.Healing:
                        potionTypeString = "HEALING";
                        break;
                    case PotionType.Mana:
                        potionTypeString = "MANA";
                        break;
                    case PotionType.ReduceCooldown:
                        potionTypeString = "REDUCECOOLDOWN";
                        break;
                    case PotionType.ExpBoost:
                        potionTypeString = "EXPBOOST";
                        break;
                    default:
                        potionTypeString = "HEALING";
                        break;
                }
                newItem = Item.CreatePotionFromData(potionTypeString,newTier,oldPotion.ItemID + 1000,"Level " + newTier + " " + potionTypeName,"A tier " + newTier + " potion crafted by the blacksmith",CalculateUpgradedPrice(oldPotion.Price, newTier));
            }
            return newItem;
        }
        /// <summary>
        /// Method to calculate the upgraded price for an item based on its original price and new tier.
        /// </summary>
        private int CalculateUpgradedPrice(int originalPrice, int newTier)
        {
            return Math.Max(originalPrice * 2, 50 + (newTier * 25));
        }
        /// <summary>
        /// Method to load the blacksmith textures.
        /// </summary>
        public static void LoadBlacksmithTextures()
        {
            if (!_blacksmithTextureLoaded)
            {
                _blacksmithTexture = Raylib.LoadTexture("picture/NPC/BlackSmith.png");
                _blacksmithTextureLoaded = true;
            }

            if (!_blacksmithShopTextureLoaded)
            {
                _blacksmithShopTexture = Raylib.LoadTexture("picture/NPC/BlackSmith_Shop.png");
                _blacksmithShopTextureLoaded = true;
            }
        }
        /// <summary>
        /// Method to unload the blacksmith textures.
        /// </summary>
        public static void UnloadBlacksmithTextures()
        {
            if (_blacksmithTextureLoaded)
            {
                Raylib.UnloadTexture(_blacksmithTexture);
                _blacksmithTextureLoaded = false;
            }

            if (_blacksmithShopTextureLoaded)
            {
                Raylib.UnloadTexture(_blacksmithShopTexture);
                _blacksmithShopTextureLoaded = false;
            }
        }
        /// <summary>
        /// Method to get the blacksmith texture.
        /// </summary>
        public static Raylib_cs.Texture2D GetBlacksmithTexture()
        {
            return _blacksmithTexture;
        }
        /// <summary>
        /// Method to get the blacksmith shop texture.
        /// </summary>
        public static Raylib_cs.Texture2D GetBlacksmithShopTexture()
        {
            return _blacksmithShopTexture;
        }
        /// <summary>
        /// Override method to draw the blacksmith shop's/ service's UI.
        /// </summary>
        public override string DrawUI(Player player, Game game)
        {
            game.ClearNotifications();
            if (player == null)
            {
                return string.Empty;
            }
            float time = (float)Raylib.GetTime();
            Raylib.DrawRectangle(0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, new Raylib_cs.Color(0, 0, 0, 180));
            int blacksmithX = 250;
            int blacksmithY = Game.WINDOW_HEIGHT / 2 + 300;
            Rectangle frameRect = new Rectangle(0, 0, 500, 500);
            Raylib.DrawTexturePro(GetBlacksmithShopTexture(), frameRect, new Rectangle(blacksmithX, blacksmithY, 500, 500), new Vector2(500 / 2, 500 / 2), 0, Raylib_cs.Color.White);
            int bubbleWidth = 400;
            int bubbleHeight = 120;
            int bubbleX = 50;
            int bubbleY = 550;
            Raylib.DrawRectangleRounded(new Rectangle(bubbleX, bubbleY, bubbleWidth, bubbleHeight), 0.3f, 10, new Raylib_cs.Color(240, 240, 240, 220));
            string fullText = "Upgrade Your Item !";
            int charCount = Math.Min(fullText.Length, (int)(time * 10) % (fullText.Length + 30));
            if (charCount > fullText.Length) charCount = fullText.Length;
            string visibleText = fullText.Substring(0, charCount);
            Raylib.DrawText(Name, bubbleX + 20, bubbleY + 20, 20, Raylib_cs.Color.DarkBlue);
            Raylib.DrawLine(bubbleX + 20, bubbleY + 45, bubbleX + bubbleWidth - 20, bubbleY + 45, Raylib_cs.Color.Gray);
            Vector2 textPos = new Vector2(bubbleX + 20, bubbleY + 55);
            Raylib.DrawText(visibleText, (int)textPos.X, (int)textPos.Y, 18, Raylib_cs.Color.Black);
            Rectangle panel = new Rectangle(Game.WINDOW_WIDTH / 2 - 300, 100, 1000, 1050);
            Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(30, 30, 50, 240));
            Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Gold);
            Raylib.DrawText("Your Gold: " + player.Inventory.Money, Game.WINDOW_WIDTH / 2 - 250, 150, 36, Raylib_cs.Color.Gold);
            Raylib.DrawText("ITEM COMBINATION", Game.WINDOW_WIDTH / 2 - 50, 230, 48, Raylib_cs.Color.White);
            Rectangle inventoryArea = new Rectangle(Game.WINDOW_WIDTH / 2 - 180, 350, 350, 600);
            Raylib.DrawRectangleRec(inventoryArea, new Raylib_cs.Color(20, 20, 35, 200));
            Raylib.DrawRectangleLinesEx(inventoryArea, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("YOUR ITEMS", (int)inventoryArea.X + 120, (int)inventoryArea.Y + 10, 24, Raylib_cs.Color.White);
            Rectangle craftingArea = new Rectangle(Game.WINDOW_WIDTH / 2 + 230, 350, 350, 600);
            Raylib.DrawRectangleRec(craftingArea, new Raylib_cs.Color(20, 20, 35, 200));
            Raylib.DrawRectangleLinesEx(craftingArea, 2, Raylib_cs.Color.LightGray);
            Raylib.DrawText("CRAFTING", (int)craftingArea.X + 120, (int)craftingArea.Y + 10, 24, Raylib_cs.Color.White);
            if (player.Inventory.Items.Count > 0)
            {
                int itemHeight = 80;
                int visibleItems = 7;
                int inventoryStartY = (int)inventoryArea.Y + 40;
                if (player.Inventory.Items.Count > visibleItems)
                {
                    Rectangle scrollTrack = new Rectangle(inventoryArea.X + inventoryArea.Width - 20, inventoryArea.Y + 40, 10, inventoryArea.Height - 50);
                    Raylib.DrawRectangleRec(scrollTrack, new Raylib_cs.Color(50, 50, 70, 255));
                    float handleRatio = (float)visibleItems / player.Inventory.Items.Count;
                    float handleHeight = Math.Max(30, scrollTrack.Height * handleRatio);
                    int maxScroll = Math.Max(0, player.Inventory.Items.Count - visibleItems);
                    float scrollProgress = maxScroll > 0 ? (float)_shopScrollPosition / maxScroll : 0;
                    float handleY = scrollTrack.Y + scrollProgress * (scrollTrack.Height - handleHeight);
                    Rectangle scrollHandle = new Rectangle(scrollTrack.X, handleY, 10, handleHeight);
                    Raylib.DrawRectangleRec(scrollHandle, Raylib_cs.Color.Gold);
                }
                float wheel = Raylib.GetMouseWheelMove();
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), inventoryArea))
                {
                    if (wheel != 0)
                    {
                        _shopScrollPosition -= (int)wheel;
                        _shopScrollPosition = Math.Clamp(_shopScrollPosition, 0, Math.Max(0, player.Inventory.Items.Count - visibleItems));
                    }
                }
                for (int i = 0; i < Math.Min(visibleItems, player.Inventory.Items.Count - _shopScrollPosition); i++)
                {
                    int itemIndex = i + _shopScrollPosition;
                    if (itemIndex >= player.Inventory.Items.Count)
                    {
                        continue;
                    }
                    Item item = player.Inventory.Items[itemIndex];
                    bool canBeCombined = item is Equipment || item is Potion;
                    Rectangle itemRect = new Rectangle(inventoryArea.X + 10, inventoryStartY + (i * itemHeight), inventoryArea.Width - 30, itemHeight - 5);
                    bool itemHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), itemRect);
                    Raylib_cs.Color bgColor = itemHovered ? new Raylib_cs.Color(60, 60, 100, 255) : new Raylib_cs.Color(40, 40, 60, 255);
                    if ((item == _firstCombineItem || item == _secondCombineItem) && canBeCombined)
                    {
                        bgColor = new Raylib_cs.Color(100, 80, 40, 255);
                    }
                    else if (!canBeCombined)
                    {
                        bgColor = new Raylib_cs.Color(30, 30, 40, 255);
                    }
                    Raylib.DrawRectangleRec(itemRect, bgColor);
                    Raylib.DrawRectangleLinesEx(itemRect, 1, Raylib_cs.Color.LightGray);
                    Rectangle iconRect = new Rectangle(itemRect.X + 10, itemRect.Y + 10, 60, 60);
                    if (item is Weapon weapon)
                    {
                        Raylib_cs.Color iconColor = new Raylib_cs.Color(200, 50, 50, 255);
                        Raylib.DrawRectangleRec(iconRect, iconColor);
                        try
                        {
                            Raylib_cs.Texture2D texture = weapon.GetTexture();
                            float scale = Math.Min(iconRect.Width / texture.Width, iconRect.Height / texture.Height) * 0.8f;
                            Raylib.DrawTextureEx(texture, new Vector2(iconRect.X + (iconRect.Width - texture.Width * scale) / 2, iconRect.Y + (iconRect.Height - texture.Height * scale) / 2), 0, scale, Raylib_cs.Color.White);
                        }
                        catch { }
                    }
                    else if (item is Armor armor)
                    {
                        Raylib_cs.Color iconColor = new Raylib_cs.Color(50, 50, 200, 255);
                        Raylib.DrawRectangleRec(iconRect, iconColor);
                        try
                        {
                            Raylib_cs.Texture2D texture = armor.GetTexture();
                            float scale = Math.Min(iconRect.Width / texture.Width, iconRect.Height / texture.Height) * 0.8f;
                            Raylib.DrawTextureEx(texture, new Vector2(iconRect.X + (iconRect.Width - texture.Width * scale) / 2, iconRect.Y + (iconRect.Height - texture.Height * scale) / 2), 0, scale, Raylib_cs.Color.White);
                        }
                        catch { }
                    }
                    else if (item is Potion potion)
                    {
                        Raylib_cs.Color iconColor;
                        switch (potion.PotionType)
                        {
                            case PotionType.Healing:
                                iconColor = new Raylib_cs.Color(50, 200, 50, 255);
                                break;
                            case PotionType.Mana:
                                iconColor = new Raylib_cs.Color(50, 50, 200, 255);
                                break;
                            case PotionType.ExpBoost:
                                iconColor = new Raylib_cs.Color(200, 200, 50, 255);
                                break;
                            case PotionType.ReduceCooldown:
                                iconColor = new Raylib_cs.Color(200, 50, 200, 255);
                                break;
                            default:
                                iconColor = new Raylib_cs.Color(150, 150, 150, 255);
                                break;
                        }

                        Raylib.DrawRectangleRec(iconRect, iconColor);
                        try
                        {
                            Raylib_cs.Texture2D texture = potion.GetTexture();
                            float scale = Math.Min(iconRect.Width / texture.Width, iconRect.Height / texture.Height) * 0.8f;
                            Raylib.DrawTextureEx(texture, new Vector2(iconRect.X + (iconRect.Width - texture.Width * scale) / 2, iconRect.Y + (iconRect.Height - texture.Height * scale) / 2), 0, scale, Raylib_cs.Color.White);
                        }
                        catch { }
                    }
                    else
                    {
                        Raylib_cs.Color iconColor = new Raylib_cs.Color(150, 150, 150, 255);
                        Raylib.DrawRectangleRec(iconRect, iconColor);
                    }
                    Raylib.DrawText(item.Name, (int)itemRect.X + 80, (int)itemRect.Y + 15, 18, canBeCombined ? Raylib_cs.Color.White : Raylib_cs.Color.Gray);
                    if (item is Equipment equipment)
                    {
                        string tierText = "Tier: " + equipment.Tier;
                        Raylib.DrawText(tierText, (int)itemRect.X + 80, (int)itemRect.Y + 40, 16, Raylib_cs.Color.Gold);
                    }
                    else if (item is Potion pot)
                    {
                        string effectText = "";
                        switch (pot.PotionType)
                        {
                            case PotionType.Healing:
                                effectText = "Heals " + pot.HealingAmount + " HP";
                                break;
                            case PotionType.Mana:
                                effectText = "Restores " + pot.Mana + " MP";
                                break;
                            case PotionType.ReduceCooldown:
                                effectText = "Reduces cooldown by " + pot.ReducingCooldown;
                                break;
                            case PotionType.ExpBoost:
                                effectText = "Grants " + pot.ExpAmount + " EXP";
                                break;
                        }
                        Raylib.DrawText(effectText, (int)itemRect.X + 80, (int)itemRect.Y + 40, 16, Raylib_cs.Color.LightGray);
                    }
                }
            }
            else
            {
                Raylib.DrawText("No items in inventory", (int)inventoryArea.X + 70, (int)inventoryArea.Y + 200, 20, Raylib_cs.Color.Gray);
            }
            int slotSize = 120;
            int combinationRow = (int)craftingArea.Y + 100;
            Rectangle firstSlot = new Rectangle(craftingArea.X + 30, combinationRow, slotSize, slotSize);
            Raylib.DrawRectangleRec(firstSlot, new Raylib_cs.Color(30, 30, 45, 255));
            Raylib.DrawRectangleLinesEx(firstSlot, 2, Raylib_cs.Color.Gold);
            Raylib.DrawText("Item 1", (int)firstSlot.X + 40, (int)firstSlot.Y - 25, 18, Raylib_cs.Color.White);
            Raylib.DrawText("+", (int)craftingArea.X + 165, combinationRow + 50, 40, Raylib_cs.Color.White);
            Rectangle secondSlot = new Rectangle(craftingArea.X + 200, combinationRow, slotSize, slotSize);
            Raylib.DrawRectangleRec(secondSlot, new Raylib_cs.Color(30, 30, 45, 255));
            Raylib.DrawRectangleLinesEx(secondSlot, 2, Raylib_cs.Color.Gold);
            Raylib.DrawText("Item 2", (int)secondSlot.X + 30, (int)secondSlot.Y - 25, 18, Raylib_cs.Color.White);
            Rectangle resultSlot = new Rectangle(craftingArea.X + 115, combinationRow + 250, slotSize, slotSize);
            Raylib.DrawRectangleRec(resultSlot, new Raylib_cs.Color(30, 30, 45, 255));
            Raylib.DrawRectangleLinesEx(resultSlot, 2, Raylib_cs.Color.Gold);
            Raylib.DrawText("Result", (int)resultSlot.X + 35, (int)resultSlot.Y - 25, 18, Raylib_cs.Color.White);
            if (_firstCombineItem != null && _secondCombineItem != null)
            {
                _canCombine = CanCombineItems(_firstCombineItem, _secondCombineItem);
                if (_canCombine)
                {
                    int newTier = _firstCombineItem.Tier + 1;
                    int combineCost = 30 + (_firstCombineItem.Tier - 1) * 10;
                    Raylib.DrawText("Combination Cost: " + combineCost + " Gold", (int)craftingArea.X + 75, (int)resultSlot.Y + 140, 18, Raylib_cs.Color.Gold);
                    bool canAfford = player.Inventory.Money >= combineCost;
                    if (canAfford)
                    {
                        Raylib_cs.Texture2D resultTexture; Item? previewItem = null;
                        if (_firstCombineItem is Weapon weapon)
                        {
                            previewItem = Item.CreateWeaponFromData(weapon.WeaponType, newTier, weapon.ItemID + 1000, "Level " + newTier + " " + weapon.WeaponType.ToString(), "A tier " + newTier + " weapon crafted by the blacksmith", CalculateUpgradedPrice(weapon.Price, newTier));
                        }
                        else if (_firstCombineItem is Armor armor)
                        {
                            previewItem = Item.CreateArmorFromData(armor.ArmorType, newTier, armor.ItemID + 1000, "Level " + newTier + " " + armor.ArmorType.ToString(), "A tier " + newTier + " armor crafted by the blacksmith", CalculateUpgradedPrice(armor.Price, newTier));
                        }
                        else if (_firstCombineItem is Potion potion)
                        {
                            string potionTypeName;
                            switch (potion.PotionType)
                            {
                                case PotionType.Healing:
                                    potionTypeName = "Healing Potion";
                                    break;
                                case PotionType.Mana:
                                    potionTypeName = "Mana Potion";
                                    break;
                                case PotionType.ReduceCooldown:
                                    potionTypeName = "ReduceCD Potion";
                                    break;
                                case PotionType.ExpBoost:
                                    potionTypeName = "EXP Potion";
                                    break;
                                default:
                                    potionTypeName = "Unknown Potion";
                                    break;
                            }
                            string potionTypeString;
                            switch (potion.PotionType)
                            {
                                case PotionType.Healing:
                                    potionTypeString = "HEALING";
                                    break;
                                case PotionType.Mana:
                                    potionTypeString = "MANA";
                                    break;
                                case PotionType.ReduceCooldown:
                                    potionTypeString = "REDUCECOOLDOWN";
                                    break;
                                case PotionType.ExpBoost:
                                    potionTypeString = "EXPBOOST";
                                    break;
                                default:
                                    potionTypeString = "HEALING";
                                    break;
                            }
                            previewItem = Item.CreatePotionFromData(potionTypeString, newTier, potion.ItemID + 1000, potionTypeName, "A tier " + newTier + " potion crafted by the blacksmith", CalculateUpgradedPrice(potion.Price, newTier));
                        }
                        if (previewItem is Equipment previewEquipment)
                        {
                            resultTexture = previewEquipment.GetTexture();
                        }
                        else if (previewItem is Potion previewPotion)
                        {
                            resultTexture = previewPotion.GetTexture();
                        }
                        else
                        {
                            resultTexture = new Raylib_cs.Texture2D();
                        }
                        Raylib_cs.Color resultColor;
                        if (_firstCombineItem is Weapon)
                        {
                            resultColor = new Raylib_cs.Color(255, 100, 100, 255);
                        }
                        else if (_firstCombineItem is Armor)
                        {
                            resultColor = new Raylib_cs.Color(100, 100, 255, 255);
                        }
                        else if (_firstCombineItem is Potion pot)
                        {
                            switch (pot.PotionType)
                            {
                                case PotionType.Healing:
                                    resultColor = new Raylib_cs.Color(100, 255, 100, 255);
                                    break;
                                case PotionType.Mana:
                                    resultColor = new Raylib_cs.Color(100, 100, 255, 255);
                                    break;
                                case PotionType.ExpBoost:
                                    resultColor = new Raylib_cs.Color(255, 255, 100, 255);
                                    break;
                                case PotionType.ReduceCooldown:
                                    resultColor = new Raylib_cs.Color(255, 100, 255, 255);
                                    break;
                                default:
                                    resultColor = new Raylib_cs.Color(255, 255, 255, 255);
                                    break;
                            }
                        }
                        else
                        {
                            resultColor = new Raylib_cs.Color(200, 200, 200, 255);
                        }
                        Raylib.DrawRectangleRec(new Rectangle(resultSlot.X + 10, resultSlot.Y + 10, slotSize - 20, slotSize - 20), resultColor);
                        if (resultTexture.Width > 0 && resultTexture.Height > 0)
                        {
                            float scale = (slotSize - 30) / (float)Math.Max(resultTexture.Width, resultTexture.Height);
                            Raylib.DrawTextureEx(resultTexture, new Vector2(resultSlot.X + (slotSize / 2) - (resultTexture.Width * scale / 2), resultSlot.Y + (slotSize / 2) - (resultTexture.Height * scale / 2)), 0, scale, Raylib_cs.Color.White);
                        }
                        Raylib.DrawText("Tier " + newTier, (int)resultSlot.X + 15, (int)resultSlot.Y + slotSize - 25, 14, Raylib_cs.Color.Black);
                        Rectangle combineButton = new Rectangle(craftingArea.X + 95, resultSlot.Y + 170, 160, 50);
                        bool buttonHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), combineButton);
                        Raylib.DrawRectangleRec(combineButton, buttonHovered ? new Raylib_cs.Color(100, 180, 100, 255) : new Raylib_cs.Color(60, 120, 60, 255));
                        Raylib.DrawRectangleLinesEx(combineButton, 2, Raylib_cs.Color.White);
                        Raylib.DrawText("COMBINE", (int)combineButton.X + 35, (int)combineButton.Y + 15, 24, Raylib_cs.Color.White);
                    }
                    else
                    {
                        Raylib.DrawText("Not enough gold!", (int)craftingArea.X + 75, (int)resultSlot.Y + 170, 24, Raylib_cs.Color.Red);
                    }
                }
                else
                {
                    Raylib.DrawText("Requirement not matched!", (int)craftingArea.X + 25, (int)resultSlot.Y + 140, 24, Raylib_cs.Color.Red);
                }
            }
            if (_isDraggingForCrafting && _draggingItem != null)
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                Raylib_cs.Color dragColor;
                if (_draggingItem is Weapon)
                    dragColor = new Raylib_cs.Color(200, 50, 50, 200);
                else if (_draggingItem is Armor)
                    dragColor = new Raylib_cs.Color(50, 50, 200, 200);
                else if (_draggingItem is Potion potion)
                {
                    switch (potion.PotionType)
                    {
                        case PotionType.Healing:
                            dragColor = new Raylib_cs.Color(50, 200, 50, 200);
                            break;
                        case PotionType.Mana:
                            dragColor = new Raylib_cs.Color(50, 50, 200, 200);
                            break;
                        case PotionType.ExpBoost:
                            dragColor = new Raylib_cs.Color(200, 200, 50, 200);
                            break;
                        case PotionType.ReduceCooldown:
                            dragColor = new Raylib_cs.Color(200, 50, 200, 200);
                            break;
                        default:
                            dragColor = new Raylib_cs.Color(150, 150, 150, 200);
                            break;
                    }
                }
                else
                {
                    dragColor = new Raylib_cs.Color(150, 150, 150, 200);
                }
                Raylib.DrawRectangle((int)mousePos.X - 30, (int)mousePos.Y - 30, 60, 60, dragColor);
                Raylib.DrawRectangleLines((int)mousePos.X - 30, (int)mousePos.Y - 30, 60, 60, Raylib_cs.Color.White);
            }
            if (_firstCombineItem != null)
            {
                Raylib_cs.Color firstItemColor = GetItemColor(_firstCombineItem);
                Raylib.DrawRectangleRec(new Rectangle(firstSlot.X + 10, firstSlot.Y + 10, slotSize - 20, slotSize - 20), firstItemColor);
                DrawItemInSlot(_firstCombineItem, firstSlot, slotSize);
                Raylib.DrawText($"Tier: {_firstCombineItem.Tier}", (int)firstSlot.X + 15, (int)firstSlot.Y + slotSize - 25, 14, Raylib_cs.Color.Black);
            }
            if (_secondCombineItem != null)
            {
                Raylib_cs.Color secondItemColor = GetItemColor(_secondCombineItem);
                Raylib.DrawRectangleRec(new Rectangle(secondSlot.X + 10, secondSlot.Y + 10, slotSize - 20, slotSize - 20), secondItemColor);
                DrawItemInSlot(_secondCombineItem, secondSlot, slotSize);
                Raylib.DrawText($"Tier: {_secondCombineItem.Tier}", (int)secondSlot.X + 15, (int)secondSlot.Y + slotSize - 25, 14, Raylib_cs.Color.Black);
            }
            Raylib.DrawText("Double click items of the same tier to combine them into a higher tier item.", (int)panel.X + 60, (int)panel.Y + 950, 24, Raylib_cs.Color.LightGray);
            Rectangle exitButton = new Rectangle(Game.WINDOW_WIDTH - 200, 140, 60, 60);
            bool exitHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), exitButton);
            Raylib.DrawRectangleRec(exitButton, exitHovered ? new Raylib_cs.Color(120, 40, 40, 255) : new Raylib_cs.Color(80, 30, 30, 255));
            Raylib.DrawRectangleLinesEx(exitButton, 2, Raylib_cs.Color.White);
            int centerX = (int)(exitButton.X + exitButton.Width / 2);
            int centerY = (int)(exitButton.Y + exitButton.Height / 2);
            int xSize = 15;
            Raylib.DrawLine(centerX - xSize, centerY - xSize, centerX + xSize, centerY + xSize, Raylib_cs.Color.White);
            Raylib.DrawLine(centerX + xSize, centerY - xSize, centerX - xSize, centerY + xSize, Raylib_cs.Color.White);
            
            return "Opening Blacksmith Shop";
        }
        /// <summary>
        /// Handles the UI input for the blacksmith.
        /// </summary>
        public override bool HandleUIInput(Player player, Game game)
        {
            if (player == null){
                return false;
            }
            Vector2 mousePos = Raylib.GetMousePosition();
            Rectangle exitButton = new Rectangle(Game.WINDOW_WIDTH - 200, 140, 60, 60);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, exitButton))
            {
                _firstCombineItem = null;
                _secondCombineItem = null;
                _isDraggingForCrafting = false;
                _draggingItem = null;
                return true;
            }
            Rectangle inventoryArea = new Rectangle(Game.WINDOW_WIDTH / 2 - 180, 350, 350, 600);
            int slotSize = 120;
            int combinationRow = (int)(Game.WINDOW_WIDTH / 2 + 30) + 100;
            Rectangle firstSlot = new Rectangle((Game.WINDOW_WIDTH / 2 + 30) + 30, combinationRow, slotSize, slotSize);
            Rectangle secondSlot = new Rectangle((Game.WINDOW_WIDTH / 2 + 30) + 200, combinationRow, slotSize, slotSize);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, inventoryArea))
            {
                int itemHeight = 80;
                int visibleItems = 7;
                int inventoryStartY = (int)inventoryArea.Y + 40;
                for (int i = 0; i < Math.Min(visibleItems, player.Inventory.Items.Count - _shopScrollPosition); i++)
                {
                    int itemIndex = i + _shopScrollPosition;
                    if (itemIndex >= player.Inventory.Items.Count){
                        continue;
                    }
                    Item item = player.Inventory.Items[itemIndex];
                    if (!(item is Equipment || item is Potion)){
                        continue;
                    }
                    Rectangle itemRect = new Rectangle(inventoryArea.X + 10, inventoryStartY + (i * itemHeight), inventoryArea.Width - 30, itemHeight - 5);
                    if (Raylib.CheckCollisionPointRec(mousePos, itemRect))
                    {
                        DateTime now = DateTime.Now;
                        double timeSinceLastClick = (now - game.LastClickTime).TotalSeconds;
                        bool isDoubleClick = (timeSinceLastClick < Game.DoubleClickTime) && (game.LastClickedItem == item);
                        game.LastClickTime = now;
                        game.LastClickedItem = item;

                        if (isDoubleClick)
                        {
                            if (_firstCombineItem == item || _secondCombineItem == item)
                            {
                                game.AddNotification("Item is already in a crafting slot", 1.0, Raylib_cs.Color.Yellow);
                            }
                            else
                            {
                                if (_firstCombineItem == null)
                                {
                                    _firstCombineItem = item;
                                }
                                else if (_secondCombineItem == null)
                                {
                                    _secondCombineItem = item;
                                }
                                if (_firstCombineItem != null && _secondCombineItem != null)
                                {
                                    _canCombine = CanCombineItems(_firstCombineItem, _secondCombineItem);
                                }

                                game.AddNotification("Item added to crafting slot", 1.0, Raylib_cs.Color.Green);
                            }
                        }
                        else
                        {
                            _isDraggingForCrafting = true;
                            _draggingItem = item;
                            _draggingItemPosition = mousePos;
                        }
                        break;
                    }
                }
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Right) && Raylib.CheckCollisionPointRec(mousePos, inventoryArea))
            {
                int itemHeight = 80;
                int visibleItems = 7;
                int inventoryStartY = (int)inventoryArea.Y + 40;
                for (int i = 0; i < Math.Min(visibleItems, player.Inventory.Items.Count - _shopScrollPosition); i++)
                {
                    int itemIndex = i + _shopScrollPosition;
                    if (itemIndex >= player.Inventory.Items.Count){
                        continue;
                    }
                    Item item = player.Inventory.Items[itemIndex];
                    if (!(item is Equipment || item is Potion))
                    {
                        continue;
                    }
                    Rectangle itemRect = new Rectangle(inventoryArea.X + 10, inventoryStartY + (i * itemHeight), inventoryArea.Width - 30, itemHeight - 5);
                    if (Raylib.CheckCollisionPointRec(mousePos, itemRect))
                    {
                        DateTime now = DateTime.Now;
                        double timeSinceLastClick = (now - game.LastClickTime).TotalSeconds;
                        bool isDoubleClick = (timeSinceLastClick < Game.DoubleClickTime) && (game.LastClickedItem == item);

                        game.LastClickTime = now;
                        game.LastClickedItem = item;

                        if (isDoubleClick)
                        {
                            if (_firstCombineItem != null)
                            {
                                _firstCombineItem = null;
                            }
                            else if (_secondCombineItem != null)
                            {
                                _secondCombineItem = null;
                            }
                        }
                    }
                }
            }
            if (_isDraggingForCrafting && _draggingItem != null)
            {
                _draggingItemPosition = mousePos;
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    bool itemPlaced = false;
                    if (Raylib.CheckCollisionPointRec(mousePos, firstSlot))
                    {
                        if (_secondCombineItem == _draggingItem)
                        {
                            game.AddNotification("Item is already in the other slot", 1.0, Raylib_cs.Color.Yellow);
                        }
                        else
                        {
                            _firstCombineItem = _draggingItem;
                            itemPlaced = true;
                        }
                    }
                    else if (Raylib.CheckCollisionPointRec(mousePos, secondSlot))
                    {
                        if (_firstCombineItem == _draggingItem)
                        {
                            game.AddNotification("Item is already in the other slot", 1.0, Raylib_cs.Color.Yellow);
                        }
                        else
                        {
                            _secondCombineItem = _draggingItem;
                            itemPlaced = true;
                        }
                    }
                    _isDraggingForCrafting = false;
                    _draggingItem = null;
                    if (itemPlaced && _firstCombineItem != null && _secondCombineItem != null)
                    {
                        _canCombine = CanCombineItems(_firstCombineItem, _secondCombineItem);
                    }
                }
            }
            if (_firstCombineItem != null && _secondCombineItem != null && _canCombine)
            {
                int newTier = _firstCombineItem.Tier + 1;
                int combineCost = 30 + (_firstCombineItem.Tier - 1) * 10;
                if (player.Inventory.Money >= combineCost)
                {
                    Rectangle craftingArea = new Rectangle(Game.WINDOW_WIDTH / 2 + 230, 350, 350, 600);
                    Rectangle resultSlot = new Rectangle(craftingArea.X + 115,(int)craftingArea.Y + 100 + 250,slotSize,slotSize);
                    Rectangle combineButton = new Rectangle(craftingArea.X + 95,resultSlot.Y + 170,160,50);
                    bool buttonHovered = Raylib.CheckCollisionPointRec(mousePos, combineButton);
                    if (buttonHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        Item? newItem = CombineItems(_firstCombineItem, _secondCombineItem, newTier);
                        if (newItem != null)
                        {
                            player.Inventory.Items.Remove(_firstCombineItem);
                            player.Inventory.Items.Remove(_secondCombineItem);
                            player.Inventory.Items.Add(newItem);
                            player.Inventory.Money -= combineCost;
                            _firstCombineItem = null;
                            _secondCombineItem = null;
                            _canCombine = false;
                            game.AddNotification("Successfully combined items! Created " + newItem.Name, 2.0, Raylib_cs.Color.Green);
                        }
                        else
                        {
                            game.AddNotification("Failed to combine items!", 1.0, Raylib_cs.Color.Red);
                        }
                    }
                }
            }
            float wheel = Raylib.GetMouseWheelMove();
            if (Raylib.CheckCollisionPointRec(mousePos, inventoryArea))
            {
                if (wheel != 0)
                {
                    int visibleItems = 7;
                    _shopScrollPosition -= (int)wheel;
                    _shopScrollPosition = Math.Clamp(_shopScrollPosition, 0, Math.Max(0, player.Inventory.Items.Count - visibleItems));
                }
            }
            return false;
        }
        /// <summary>
        /// Method to get the color for an item based on its type.
        /// </summary>
        private Raylib_cs.Color GetItemColor(Item item)
        {
            if (item is Weapon)
            {
                return new Raylib_cs.Color(200, 50, 50, 255);
            }
            else if (item is Armor)
            {
                return new Raylib_cs.Color(50, 50, 200, 255);
            }
            else if (item is Potion potion)
            {
                switch (potion.PotionType)
                {
                    case PotionType.Healing:
                        return new Raylib_cs.Color(50, 200, 50, 255);
                    case PotionType.Mana:
                        return new Raylib_cs.Color(50, 50, 200, 255);
                    case PotionType.ExpBoost:
                        return new Raylib_cs.Color(200, 200, 50, 255);
                    case PotionType.ReduceCooldown:
                        return new Raylib_cs.Color(200, 50, 200, 255);
                    default:
                        return new Raylib_cs.Color(150, 150, 150, 255);
                }
            }
            else
            {
                return new Raylib_cs.Color(150, 150, 150, 255);
            }
        }
        /// <summary>
        /// Method to draw an item in a specific slot.
        /// </summary>
        private void DrawItemInSlot(Item item, Rectangle slot, int slotSize)
        {
            if (item is Weapon weapon)
            {
                try
                {
                    Raylib_cs.Texture2D texture = weapon.GetTexture();
                    float scale = (slotSize - 30) / (float)Math.Max(texture.Width, texture.Height);
                    Raylib.DrawTextureEx(texture,new Vector2(slot.X + (slotSize / 2) - (texture.Width * scale / 2),slot.Y + (slotSize / 2) - (texture.Height * scale / 2)),0,scale,Raylib_cs.Color.White);
                }
                catch
                {
                    Raylib.DrawText("W", (int)slot.X + 50, (int)slot.Y + 45, 30, Raylib_cs.Color.White);
                }
            }
            else if (item is Armor armor)
            {
                try
                {
                    Raylib_cs.Texture2D texture = armor.GetTexture();
                    float scale = (slotSize - 30) / (float)Math.Max(texture.Width, texture.Height);
                    Raylib.DrawTextureEx(texture,new Vector2(slot.X + (slotSize / 2) - (texture.Width * scale / 2),slot.Y + (slotSize / 2) - (texture.Height * scale) / 2),0,scale,Raylib_cs.Color.White);
                }
                catch
                {
                    Raylib.DrawText("A", (int)slot.X + 50, (int)slot.Y + 45, 30, Raylib_cs.Color.White);
                }
            }
            else if (item is Potion potion)
            {
                try
                {
                    Raylib_cs.Texture2D texture = potion.GetTexture();
                    float scale = (slotSize - 30) / (float)Math.Max(texture.Width, texture.Height);
                    Raylib.DrawTextureEx(texture,new Vector2(slot.X + (slotSize / 2) - (texture.Width * scale / 2),slot.Y + (slotSize / 2) - (texture.Height * scale) / 2),0,scale,Raylib_cs.Color.White);
                }
                catch
                {
                    Raylib.DrawText("P", (int)slot.X + 50, (int)slot.Y + 45, 30, Raylib_cs.Color.White);
                }
            }
        }
    }
}