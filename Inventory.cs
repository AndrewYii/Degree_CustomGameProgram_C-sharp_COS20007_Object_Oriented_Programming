using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

namespace DistinctionTask{    
    /// <summary>
    /// This is the Inventory class, which manages the player's items, money, and equipped items.
    /// </summary>
    public class Inventory
    {
        private List<Item> _items;
        private int _money;
        private Player? _owner;
        private Armor? _equippedHelmet;
        private Armor? _equippedChest;
        private Armor? _equippedLegs;
        private Armor? _equippedGloves;
        private Weapon? _equippedWeapon;
        private Equipment? _equippedRing;
        private Equipment? _equippedBracelet;
        private Equipment? _draggingEquipment = null;
        private bool _isDragging = false;
        private Vector2 _dragStartPosition;
        private Vector2 _currentDragPosition;
     private Item? _setHoveredItem;
        /// <summary>
        /// Constructor for the Inventory class.
        /// </summary>
        public Inventory()
        {
            _items = new List<Item>();
            _money = 0;
            _isDragging = false;
            _draggingEquipment = null;
            _dragStartPosition = new System.Numerics.Vector2(0, 0);
            _currentDragPosition = new System.Numerics.Vector2(0, 0);
        }
        // Set the owner of this inventory
        public void SetOwner(Player owner)
        {
            _owner = owner;
        }
        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        public void AddItem(Item item)
        {
            _items.Add(item);
        }
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        public void RemoveItem(Item item)
        {
            _items.Remove(item);
        }
        /// <summary>
        /// Adds money to the inventory.
        /// </summary>
        public void AddMoney(int amount)
        {
            _money += amount;
        }
        /// <summary>
        /// Equips an item from the inventory.
        /// </summary>
        public bool EquipItem(Equipment equipment)
        {
            bool equipped = false;
            if (equipment is Weapon weapon)
            {
                if (_equippedWeapon != null)
                {
                    _items.Add(_equippedWeapon);
                }
                _equippedWeapon = weapon;
                _items.Remove(weapon);
                equipped = true;
            }
            else if (equipment is Armor armor)
            {
                switch (armor.ArmorType)
                {
                    case ArmorType.Helmet:
                        if (_equippedHelmet != null) _items.Add(_equippedHelmet);
                        _equippedHelmet = armor;
                        break;
                    case ArmorType.Chest:
                        if (_equippedChest != null) _items.Add(_equippedChest);
                        _equippedChest = armor;
                        break;
                    case ArmorType.Glove:
                        if (_equippedGloves != null) _items.Add(_equippedGloves);
                        _equippedGloves = armor;
                        break;
                    case ArmorType.Leg:
                        if (_equippedLegs != null) _items.Add(_equippedLegs);
                        _equippedLegs = armor;
                        break;
                    case ArmorType.Ring:
                        if (_equippedRing != null) _items.Add(_equippedRing);
                        _equippedRing = armor;
                        break;
                    case ArmorType.Bracelet:
                        if (_equippedBracelet != null) _items.Add(_equippedBracelet);
                        _equippedBracelet = armor;
                        break;
                    default:
                        return false;
                }
                _items.Remove(armor);
                equipped = true;
            }
            if (equipped)
            {
                equipment.Equipped();
                if (_owner != null)
                {
                    _owner.UpdateStats();
                }
            }
            return equipped;
        }
        /// <summary>
        /// Unequips an item from the inventory.
        /// </summary>
        public bool UnequipItem(Equipment equipment)
        {
            if (equipment == _equippedHelmet || equipment == _equippedChest ||
                equipment == _equippedLegs || equipment == _equippedGloves ||
                equipment == _equippedWeapon || equipment == _equippedRing ||
                equipment == _equippedBracelet)
            {
                _items.Add(equipment);

                if (equipment == _equippedHelmet) _equippedHelmet = null;
                else if (equipment == _equippedChest) _equippedChest = null;
                else if (equipment == _equippedLegs) _equippedLegs = null;
                else if (equipment == _equippedGloves) _equippedGloves = null;
                else if (equipment == _equippedWeapon) _equippedWeapon = null;
                else if (equipment == _equippedRing) _equippedRing = null;
                else if (equipment == _equippedBracelet) _equippedBracelet = null;
                if (_owner != null)
                {
                    _owner.UpdateStats();
                }

                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets a list of all equipped items.
        /// /// </summary>
        public List<Equipment> GetEquippedItems()
        {
            List<Equipment> equipped = new List<Equipment>();
            if (_equippedHelmet != null) equipped.Add(_equippedHelmet);
            if (_equippedChest != null) equipped.Add(_equippedChest);
            if (_equippedLegs != null) equipped.Add(_equippedLegs);
            if (_equippedGloves != null) equipped.Add(_equippedGloves);
            if (_equippedWeapon != null) equipped.Add(_equippedWeapon);
            if (_equippedRing != null) equipped.Add(_equippedRing);
            if (_equippedBracelet != null) equipped.Add(_equippedBracelet);
            return equipped;
        }
        /// <summary>
        /// Starts dragging an equipment item.
        /// </summary>
        public void UpdateDragPosition(System.Numerics.Vector2 newPosition)
        {
            if (_isDragging)
            {
                _currentDragPosition = newPosition;
            }
        }
        /// <summary>
        /// Handles dropping an item onto the equipment slots.
        /// </summary>
        public bool HandleItemDrop(System.Numerics.Vector2 dropPosition, Dictionary<string, System.Drawing.RectangleF> equipSlots)
        {
            if (_owner == null || _draggingEquipment == null)
            {
                _isDragging = false;
                _draggingEquipment = null;
                return false;
            }
            foreach (var slot in equipSlots)
            {
                if (CheckCollisionPointRect(dropPosition, slot.Value))
                {
                    bool correctSlot = false;
                    if (_draggingEquipment is Weapon && slot.Key == "Weapon")
                    {
                        correctSlot = true;
                    }
                    else if (_draggingEquipment is Armor armor)
                    {
                        if ((armor.ArmorType == ArmorType.Helmet && slot.Key == "Helmet") ||
                            (armor.ArmorType == ArmorType.Chest && slot.Key == "Chest") ||
                            (armor.ArmorType == ArmorType.Glove && slot.Key == "Gloves") ||
                            (armor.ArmorType == ArmorType.Leg && slot.Key == "Legs"))
                        {
                            correctSlot = true;
                        }
                    }
                    else if (_draggingEquipment.Name.Contains("Ring") && slot.Key == "Ring")
                    {
                        correctSlot = true;
                    }
                    else if (_draggingEquipment.Name.Contains("Bracelet") && slot.Key == "Bracelet")
                    {
                        correctSlot = true;
                    }

                    if (correctSlot)
                    {
                        EquipItem(_draggingEquipment);
                        LogItemAction("Equipped " + _draggingEquipment.Name + " by drag and drop");
                        _isDragging = false;
                        _draggingEquipment = null;
                        return true;
                    }
                    else
                    {
                        LogItemAction("Cannot equip " + _draggingEquipment.Name + " in " + slot.Key + " slot");
                    }
                }
            }
            _isDragging = false;
            _draggingEquipment = null;
            return false;
        }
        /// <summary>
        /// Checks the selection of an item in the inventory.
        /// </summary>
        private bool CheckCollisionPointRect(System.Numerics.Vector2 point, System.Drawing.RectangleF rect)
        {
            return (point.X >= rect.X && point.X <= rect.X + rect.Width && point.Y >= rect.Y && point.Y <= rect.Y + rect.Height);
        }
        /// <summary>
        /// Logs an action performed on an item in the inventory.
        /// </summary>
        private void LogItemAction(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                System.IO.File.AppendAllText("log/game_events.txt", timestamp + " - Inventory: " + message + "\n");
            }
            catch (Exception ex)
            {
               Console.WriteLine("Error logging item action: " + ex.Message);
            }
        }
        /// <summary>
        /// Gets a list of broken items in the inventory.
        /// </summary>
        public List<Equipment> CheckBrokenEquipment()
        {
            List<Equipment> brokenEquipment = new List<Equipment>();
            foreach (Equipment equipment in GetEquippedItems())
            {
                if (equipment.Durability <= 0)
                {
                    brokenEquipment.Add(equipment);
                }
            }
            foreach (Item item in Items)
            {
                if (item is Equipment invEquip && invEquip.Durability <= 0)
                {
                    brokenEquipment.Add(invEquip);
                }
            }
            return brokenEquipment;
        }
        /// <summary>
        /// Removes broken equipment from the inventory and calls a callback if provided.
        /// </summary>
        public void RemoveBrokenEquipment(List<Equipment> brokenItems, Action<Equipment>? onEquipmentBroken = null)
        {
            foreach (Equipment equipment in brokenItems)
            {
                onEquipmentBroken?.Invoke(equipment);
                UnequipItem(equipment);
                RemoveItem(equipment);
            }
        }
        /// <summary>
        /// Gets the list of items in the inventory.
        /// </summary>
        public List<Item> Items
        {
            get { return _items; }
        }
        /// <summary>
        /// Gets or sets the amount of money in the inventory.
        /// </summary>
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped items.
        /// </summary>
        public Armor? EquippedHelmet
        {
            get { return _equippedHelmet; }
            set { _equippedHelmet = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped armor items.
        /// </summary>
        public Armor? EquippedChest
        {
            get { return _equippedChest; }
            set { _equippedChest = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped legs armor.
        /// </summary>
        public Armor? EquippedLegs
        {
            get { return _equippedLegs; }
            set { _equippedLegs = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped gloves armor.
        /// </summary>
        public Armor? EquippedGloves
        {
            get { return _equippedGloves; }
            set { _equippedGloves = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped weapon.
        /// </summary>
        public Weapon? EquippedWeapon
        {
            get { return _equippedWeapon; }
            set { _equippedWeapon = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped ring.
        /// </summary>
        public Equipment? EquippedRing
        {
            get { return _equippedRing; }
            set { _equippedRing = value; }
        }
        /// <summary>
        /// Gets or sets the currently equipped bracelet.
        /// </summary>
        public Equipment? EquippedBracelet
        {
            get { return _equippedBracelet; }
            set { _equippedBracelet = value; }
        }
        /// <summary>
        /// Draws the equipment slots for the inventory UI
        /// </summary>
        public void DrawEquipmentSlot(int x, int y, int size, string label, Equipment? equipment)
        {

            Raylib.DrawRectangle(x, y, size, size, Color.DarkGray);
            Raylib.DrawRectangleLines(x, y, size, size, Color.White);
            Raylib.DrawText(label, x + (size / 2) - (label.Length * 4), y + size + 5, 15, Color.LightGray);
            Rectangle slotRect = new Rectangle(x, y, size, size);
            bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), slotRect);
            if (isHovered && equipment != null)
            {
                _setHoveredItem = equipment;
            }
            if (equipment != null)
            {

                Color slotColor = equipment is Weapon ?new Color(150, 100, 100, 255) :new Color(100, 100, 150, 255);
                Raylib.DrawRectangle(x, y, size, size, slotColor);
                try
                {
                    int padding = 10;
                    int drawableSize = size - (padding * 2); 
                    Texture2D texture = equipment.GetTexture();
                    float scaleX = (float)drawableSize / texture.Width;
                    float scaleY = (float)drawableSize / texture.Height;
                    float scale = Math.Min(scaleX, scaleY); 
                    int scaledWidth = (int)(texture.Width * scale);
                    int scaledHeight = (int)(texture.Height * scale);
                    int centerOffsetX = (size - scaledWidth) / 2;
                    int centerOffsetY = (size - scaledHeight) / 2;
                    Raylib.DrawTextureEx( texture, new Vector2(x + centerOffsetX, y + centerOffsetY),0.0f, scale, Color.White );
                    string equipmentTypeName = equipment is Weapon weapon ? weapon.WeaponType.ToString() :equipment is Armor armor ? armor.ArmorType.ToString() : "Unknown";
                    Raylib.DrawRectangle(x + 2, y + 2, 10, 10, Color.Green);
                }
                catch (Exception ex)
                {
                    string equipmentTypeName = equipment is Weapon weapon ? weapon.WeaponType.ToString() :equipment is Armor armor ? armor.ArmorType.ToString() : "Unknown";
                    Console.WriteLine("Error drawing " + equipmentTypeName + " texture: " + ex.Message);
                    string fallbackText = equipment is Weapon ? "W" : equipment is Armor armorItem ? armorItem.ArmorType.ToString()[0].ToString() : "?";
                    Raylib.DrawText(fallbackText, x + (size / 2) - 5, y + (size / 2) - 10, 30, Color.White);
                    Raylib.DrawRectangle(x + 2, y + 2, 10, 10, Color.Red);
                }
                if (equipment.Name.Length > 10)
                {
                    Raylib.DrawText(equipment.Name.Substring(0, 7) + "...", x + 5, y + size - 15, 10, Color.Yellow);
                }
                else
                {
                    Raylib.DrawText(equipment.Name, x + 5, y + size - 15, 10, Color.Yellow);
                }
            }
            else if (isHovered)
            {
                Raylib.DrawRectangleLinesEx(slotRect, 3, Color.Gold);
            }
        }
        /// <summary>
        /// Public method to draw the item icon in the inventory
        /// </summary>
        public Color DrawItemIcon(Item item, Rectangle iconRect)
        {
            Color iconColor = Color.White;
            if (item is Weapon weapon)
            {
                iconColor = new Color(200, 50, 50, 255);
                Raylib.DrawRectangleRec(iconRect, iconColor);
                try
                {
                    int padding = 4;
                    int drawableSize = (int)iconRect.Width - (padding * 2);
                    Texture2D texture = weapon.GetTexture();
                    float scaleX = (float)drawableSize / texture.Width;
                    float scaleY = (float)drawableSize / texture.Height;
                    float scale = Math.Min(scaleX, scaleY) * 0.8f; 
                    int scaledWidth = (int)(texture.Width * scale);
                    int scaledHeight = (int)(texture.Height * scale);
                    int centerOffsetX = ((int)iconRect.Width - scaledWidth) / 2;
                    int centerOffsetY = ((int)iconRect.Height - scaledHeight) / 2;
                    Raylib.DrawTextureEx( texture,new Vector2(iconRect.X + centerOffsetX, iconRect.Y + centerOffsetY), 0.0f,  scale,Color.White);
                    Raylib.DrawRectangle((int)iconRect.X + 2, (int)iconRect.Y + 2, 6, 6, Color.Green);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing " + weapon.WeaponType + " texture: " + ex.Message);
                    Raylib.DrawText("W", (int)iconRect.X + 12, (int)iconRect.Y + 10, 20, Color.White);
                    Raylib.DrawRectangle((int)iconRect.X + 2, (int)iconRect.Y + 2, 6, 6, Color.Red);
                }
            }
            else if (item is Armor armor)
            {
                iconColor = new Color(50, 50, 200, 255);
                Raylib.DrawRectangleRec(iconRect, iconColor);
                try
                {
                    int padding = 4;
                    int drawableSize = (int)iconRect.Width - (padding * 2); 
                    Texture2D texture = armor.GetTexture();
                    float scaleX = (float)drawableSize / texture.Width;
                    float scaleY = (float)drawableSize / texture.Height;
                    float scale = Math.Min(scaleX, scaleY) * 0.8f; 
                    int scaledWidth = (int)(texture.Width * scale);
                    int scaledHeight = (int)(texture.Height * scale);
                    int centerOffsetX = ((int)iconRect.Width - scaledWidth) / 2;
                    int centerOffsetY = ((int)iconRect.Height - scaledHeight) / 2;
                    Raylib.DrawTextureEx(texture, new Vector2(iconRect.X + centerOffsetX, iconRect.Y + centerOffsetY),0.0f,scale, Color.White);
                    Raylib.DrawRectangle((int)iconRect.X + 2, (int)iconRect.Y + 2, 6, 6, Color.Green);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing " + armor.ArmorType + " texture: " + ex.Message);
                    Raylib.DrawText(armor.ArmorType.ToString()[0].ToString(), (int)iconRect.X + 12, (int)iconRect.Y + 10, 20, Color.White);
                }
            }
            else if (item is Potion potion)
            {
                switch (potion.PotionType)
                {
                    case PotionType.Healing:
                        iconColor = new Color(50, 200, 50, 255); 
                        break;
                    case PotionType.Mana:
                        iconColor = new Color(50, 50, 200, 255); 
                        break;
                    case PotionType.ExpBoost:
                        iconColor = new Color(200, 200, 50, 255); 
                        break;
                    case PotionType.ReduceCooldown:
                        iconColor = new Color(200, 50, 200, 255);
                        break;
                    default:
                        iconColor = new Color(150, 150, 150, 255);
                        break;
                }
                Raylib.DrawRectangleRec(iconRect, iconColor);
                try
                {
                    int padding = 4;
                    int drawableSize = (int)iconRect.Width - (padding * 2);
                    Texture2D texture = potion.GetTexture();
                    float scaleX = (float)drawableSize / texture.Width;
                    float scaleY = (float)drawableSize / texture.Height;
                    float scale = Math.Min(scaleX, scaleY) * 0.8f;
                    float scaledWidth = texture.Width * scale;
                    float scaledHeight = texture.Height * scale;
                    float xOffset = iconRect.X + (iconRect.Width - scaledWidth) / 2;
                    float yOffset = iconRect.Y + (iconRect.Height - scaledHeight) / 2;
                    Raylib.DrawTextureEx(texture,new Vector2(xOffset, yOffset),0.0f, scale, Color.White);
                    Raylib.DrawRectangle((int)iconRect.X + 2, (int)iconRect.Y + 2, 6, 6, Color.Green);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error drawing potion texture: " + ex.Message);
                    Raylib.DrawText("P", (int)iconRect.X + 12, (int)iconRect.Y + 10, 20, Color.White);
                    Raylib.DrawRectangle((int)iconRect.X + 2, (int)iconRect.Y + 2, 6, 6, Color.Red);
                }
            }
            else
            {
                iconColor = new Color(100, 100, 100, 255);
                string itemType = "?";
                Raylib.DrawRectangleRec(iconRect, iconColor);
                Raylib.DrawText(itemType, (int)iconRect.X + 12, (int)iconRect.Y + 10, 20, Color.White);
            }

            return iconColor;
        }
        /// <summary>
        /// Draws the inventory items in a scrollable list
        /// </summary>
        public Item? DrawInventoryItems(Rectangle listArea, int scrollPosition, int rowHeight, int visibleRowsCount)
        {
            Item? itemToShowTooltip = null;
            for (int i = scrollPosition; i < Math.Min(_items.Count, scrollPosition + visibleRowsCount); i++)
            {
                Item item = _items[i];
                int rowIndex = i - scrollPosition;
                Rectangle rowRect = new Rectangle(listArea.X,listArea.Y + (rowIndex * rowHeight),listArea.Width,rowHeight);
                Color rowColor = (i % 2 == 0) ?new Color(40, 40, 60, 255) : new Color(50, 50, 70, 255);
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rowRect))
                {
                    itemToShowTooltip = item;
                    rowColor = new Color(60, 60, 100, 255);
                }
                Raylib.DrawRectangleRec(rowRect, rowColor);
                Rectangle itemIcon = new Rectangle( rowRect.X + 5, rowRect.Y + 5, rowHeight - 15, rowHeight - 15 );
                DrawItemIcon(item, itemIcon);
                Raylib.DrawText(item.Name, (int)rowRect.X + (int)rowHeight, (int)rowRect.Y + 5, 16, Color.White);
                if (item.Quantity > 1)
                {
                    Raylib.DrawText($"x{item.Quantity}", (int)rowRect.X + (int)rowHeight, (int)rowRect.Y + 25, 16, Color.Yellow);
                }
                string tierStars = "";
                for (int t = 0; t < item.Tier; t++) tierStars += "*";
                Raylib.DrawText(tierStars, (int)rowRect.X + (int)rowRect.Width - 10 - (tierStars.Length * 10), (int)rowRect.Y + 25, 16, Color.Gold);
            }
            return itemToShowTooltip;
        }
        /// <summary>
        /// Public method to draw the scrollbar for the inventory list
        /// </summary>
        public void DrawScrollbar(Rectangle scrollTrack, int scrollPosition, int maxScrollPosition, int visibleRowsCount, int totalRowsCount)
        {
            Raylib.DrawRectangleRec(scrollTrack, new Color(40, 40, 50, 255));
            float handleRatio = (float)visibleRowsCount / totalRowsCount;
            float handleHeight = Math.Max(30, scrollTrack.Height * handleRatio); 
            float scrollProgress = maxScrollPosition > 0 ? (float)scrollPosition / maxScrollPosition : 0;
            float handleY = scrollTrack.Y + scrollProgress * (scrollTrack.Height - handleHeight);
            Rectangle scrollHandle = new Rectangle(scrollTrack.X, handleY, scrollTrack.Width, handleHeight);
            Raylib.DrawRectangleRec(scrollHandle, Color.LightGray);
        }
    }
}