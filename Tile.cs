using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is Tile class, representing a tile in the dungeon with properties such as column, row, tile width, tile length, tile type, walkability, visibility, and interaction status.
    /// </summary>
    public class Tile
    {
        private int _column;
        private int _row;
        private int _tileWidth;
        private int _tileLength;
        private TileType _tileType;
        private bool _isWalkable;
        private bool _isVisible;
        private bool _isInteracted;
        private static Raylib_cs.Texture2D _wallTexture;
        private static Raylib_cs.Texture2D _floorTexture;
        private static Raylib_cs.Texture2D _treasureTexture;
        private static Raylib_cs.Texture2D _buffTexture;
        private static Raylib_cs.Texture2D _trapTexture;
        private static Raylib_cs.Texture2D _escapeTexture;
        private static bool _wallTextureLoaded = false;
        private static bool _floorTextureLoaded = false;
        private static bool _treasureTextureLoaded = false;
        private static bool _buffTextureLoaded = false;
        private static bool _trapTextureLoaded = false;
        private static bool _escapeTextureLoaded = false;
        /// <summary>
        /// Parameterized constructor for Tile class, that sets the column, row, tile width, tile length, tile type, walkability, visibility, and interaction status.
        /// </summary>
        public Tile(int column, int row, int tileWidth, int tileLength, TileType tileType, bool isWalkable, bool isVisible, bool isInteracted)
        {
            _column = column;
            _row = row;
            _tileWidth = tileWidth;
            _tileLength = tileLength;
            _tileType = tileType;
            _isWalkable = isWalkable;
            _isVisible = isVisible;
            _isInteracted = isInteracted;
        }
        /// <summary>
        /// Handles the tile type interaction based on the unit that steps on it, returning a message indicating the result of the interaction.
        /// </summary>
        public string HandleTileType(Unit unit)
        {
            string message = "";

            switch (_tileType)
            {
                case TileType.Empty:
                    break;
                case TileType.Wall:
                    break;
                case TileType.Player:
                    break;
                case TileType.Spawn:
                    break;
                case TileType.Merchant:
                    if (unit is Player && !_isInteracted)
                    {
                        message = "You've encountered a merchant! Would you like to trade?";
                        _isInteracted = true;
                    }
                    break;
                case TileType.Blacksmith:
                    if (unit is Player && !_isInteracted)
                    {
                        message = "You've found a blacksmith! Would you like to upgrade your equipment?";
                        _isInteracted = true;
                    }
                    break;
                case TileType.Treasure:
                    if (unit is Player && !_isInteracted)
                    {
                        Player player = (Player)unit;
                        _isInteracted = true;
                        message = player.Name + " found a treasure chest!";
                        Random random = new Random();
                        int treasureType = random.Next(3);
                        switch (treasureType)
                        {
                            case 0: 
                                int goldAmount = random.Next(30, 70) + (player.Level * 10);
                                message += "\nYou found " + goldAmount + " gold!";
                                player.Inventory.AddMoney(goldAmount);
                                break;
                            case 1:
                                int potionTier = Math.Max(1, player.Level / 3);
                                Random potionRandom = new Random();
                                string[] potionTypes = { "HEALING", "MANA", "EXPBOOST", "REDUCECOOLDOWN" };
                                string potionType = potionTypes[potionRandom.Next(potionTypes.Length)];
                                string potionName;
                                string description;
                                switch (potionType)
                                {
                                    case "HEALING":
                                        potionName = "Healing Potion";
                                        description = "Restores health when used";
                                        break;
                                    case "MANA":
                                        potionName = "Mana Potion";
                                        description = "Restores mana when used";
                                        break;
                                    case "EXPBOOST":
                                        potionName = "Exp Potion";
                                        description = "Grants additional experience";
                                        break;
                                    case "REDUCECOOLDOWN":
                                        potionName = "ReduceCD Potion";
                                        description = "Reduces skill cooldowns";
                                        break;
                                    default:
                                        potionName = "Mystery Potion";
                                        description = "Effect unknown";
                                        break;
                                }
                                Item potion = Item.CreatePotionFromData(potionType,potionTier,100 + potionRandom.Next(10),potionName,description,10 + (potionTier * 5));
                                if (potion != null)
                                {
                                    player.Inventory.AddItem(potion);
                                    message += "\n You found a " + potion.Name + "!";
                                }
                                break;
                            case 2:
                                Random equipRandom = new Random();
                                int equipTier = Math.Max(1, player.Level / 2);
                                if (equipRandom.NextDouble() < 0.5)
                                {
                                    WeaponType weaponType = (WeaponType)equipRandom.Next(3);
                                    string weaponTypeName = weaponType.ToString();
                                    Weapon weapon = Item.CreateWeaponFromData(weaponType,equipTier,200 + equipRandom.Next(10),weaponTypeName,"A quality " + weaponTypeName.ToLower() + " found in a treasure chest",20 + (equipTier * 10));
                                    player.Inventory.AddItem(weapon);
                                    message += "\nYou found a " + weapon.Name + "!";
                                    message += "\nThis will boost your attack by " + weapon.BonusAttack.ToString("F1") + " and critical rate by " + weapon.BonusCriticalRate.ToString("P0") + "!";
                                }
                                else
                                {
                                    ArmorType armorType = (ArmorType)equipRandom.Next(6);
                                    string armorTypeName = armorType.ToString();
                                    Armor armor = Item.CreateArmorFromData(armorType,equipTier,300 + equipRandom.Next(10),armorTypeName,"A sturdy " + armorTypeName.ToLower() + " found in a treasure chest", 15 + (equipTier * 8));
                                    player.Inventory.AddItem(armor);
                                    message += "\nYou found a " + armor.Name + "!";
                                    string benefitMessage = " This will provide " + armor.BonusDefense.ToString("F1") + " defense";
                                    if (armor.BonusHP > 0) benefitMessage += ", " + armor.BonusHP.ToString("F0") + " HP";
                                    if (armor.BonusSpeed != 0) benefitMessage += ", " + (armor.BonusSpeed > 0 ? "+" : "") + armor.BonusSpeed.ToString("F1") + " speed";
                                    if (armor.BonusMana > 0) benefitMessage += ", " + armor.BonusMana.ToString("F0") + " mana";
                                    message += benefitMessage + "!";
                                }
                                break;
                        }
                    }
                    break;
                case TileType.RandomBuff:
                    if (!_isInteracted)
                    {
                        _isInteracted = true;
                        message = unit.Name + " steps on a magical tile!";
                        Random random = new Random();
                        bool isDebuff = random.NextDouble() < 0.3;
                        if (unit is Player player)
                        {
                            Buff buff = isDebuff? Buff.CreateRandomDebuff(player.Level): Buff.CreateRandomBuff(player.Level);
                            player.AddBuff(buff);
                            message += "\n" + player.Name + " received " + buff.Name + ": " + buff.Description;
                        }
                    }
                    break;
                case TileType.Trap:
                    if (!_isInteracted)
                    {
                        message = unit.Name + " triggered a trap!";
                        double damage = unit.MaxHP * 0.15;
                        unit.HP -= damage;
                        message += "\n" + unit.Name + " takes " + damage + " damage from the trap!";
                        if (new Random().Next(2) == 0)
                        {
                            unit.Speed = Math.Max(1, unit.Speed - 3);
                            message += "\n" + unit.Name + " is slowed by the trap!";
                        }
                        if (unit.HP <= 0)
                        {
                            unit.IsAlive = false;
                            message += "\n" + unit.Name + " was killed by the trap!";
                        }
                        _isInteracted = true;
                    }
                    break;
                case TileType.Exit:
                    if (unit is Player)
                    {
                        message = "You've reached the exit! Ready to proceed to the next level.";
                    }
                    break;
                default:
                    break;
            }
            return message;
        }
        /// <summary>
        /// Loads the textures for the different tile types if they haven't been loaded yet.
        /// </summary>
        public static void LoadTextures()
        {
            if (!_wallTextureLoaded)
            {
                _wallTexture = Raylib.LoadTexture("picture/Dungeon/Wall.png");
                _wallTextureLoaded = true;
            }

            if (!_floorTextureLoaded)
            {
                _floorTexture = Raylib.LoadTexture("picture/Dungeon/Floor.png");
                _floorTextureLoaded = true;
            }

            if (!_treasureTextureLoaded)
            {
                _treasureTexture = Raylib.LoadTexture("picture/Dungeon/Treasure.png");
                _treasureTextureLoaded = true;
            }

            if (!_buffTextureLoaded)
            {
                _buffTexture = Raylib.LoadTexture("picture/Dungeon/Buff.png");
                _buffTextureLoaded = true;
            }

            if (!_trapTextureLoaded)
            {
                _trapTexture = Raylib.LoadTexture("picture/Dungeon/Trap.png");
                _trapTextureLoaded = true;
            }

            if (!_escapeTextureLoaded)
            {
                _escapeTexture = Raylib.LoadTexture("picture/Dungeon/Escape.png");
                _escapeTextureLoaded = true;
            }
        }
        /// <summary>
        /// Gets the texture associated with the specified tile type, ensuring that textures are loaded before returning them.
        /// </summary>
        public static Raylib_cs.Texture2D GetTexture(TileType tileType)
        {
            LoadTextures(); 
            switch (tileType)
            {
                case TileType.Wall:
                    return _wallTexture;
                case TileType.Treasure:
                    return _treasureTexture;
                case TileType.RandomBuff:
                    return _buffTexture;
                case TileType.Trap:
                    return _trapTexture;
                case TileType.Exit:
                    return _escapeTexture;
                default:
                    return _floorTexture;
            }
        }
        /// <summary>
        /// Unloads all textures associated with the Tile class to free up resources.
        /// </summary>
        public static void UnloadAllTextures()
        {
            if (_wallTextureLoaded)
            {
                Raylib.UnloadTexture(_wallTexture);
                _wallTextureLoaded = false;
            }
            if (_floorTextureLoaded)
            {
                Raylib.UnloadTexture(_floorTexture);
                _floorTextureLoaded = false;
            }
            if (_treasureTextureLoaded)
            {
                Raylib.UnloadTexture(_treasureTexture);
                _treasureTextureLoaded = false;
            }
            if (_buffTextureLoaded)
            {
                Raylib.UnloadTexture(_buffTexture);
                _buffTextureLoaded = false;
            }
            if (_trapTextureLoaded)
            {
                Raylib.UnloadTexture(_trapTexture);
                _trapTextureLoaded = false;
            }
            if (_escapeTextureLoaded)
            {
                Raylib.UnloadTexture(_escapeTexture);
                _escapeTextureLoaded = false;
            }
        }
        /// <summary>
        /// Property method to get or set the tile column.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }
        /// <summary>
        /// Property method to get or set the tile row.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        /// <summary>
        /// Property method to get or set the tile type.
        /// </summary>
        public TileType TileType
        {
            get { return _tileType; }
            set { _tileType = value; }
        }
        /// <summary>
        /// Property method to get or set the walkable of tile.
        /// </summary>
        public bool IsWalkable
        {
            get { return _isWalkable; }
            set { _isWalkable = value; }
        }
        /// <summary>
        /// Property method to get or set the visibility of tile.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        /// <summary>
        /// Property method to get or set the interaction status of tile.
        /// </summary>
        public bool IsInteracted
        {
            get { return _isInteracted; }
            set { _isInteracted = value; }
        }
    }
}