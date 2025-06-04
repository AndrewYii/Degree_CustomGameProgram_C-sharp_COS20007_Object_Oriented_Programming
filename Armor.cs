using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Armor class, inherits from Equipment, it has 18 extra properties.
    /// </summary>
    public class Armor : Equipment{
        private double _bonusDefense;
        private double _bonusHP;
        private double _bonusSpeed;
        private double _bonusMana;
        private ArmorType _armorType;
        private static Raylib_cs.Texture2D _helmetTexture;
        private static Raylib_cs.Texture2D _chestTexture;
        private static Raylib_cs.Texture2D _glovesTexture;
        private static Raylib_cs.Texture2D _legsTexture;
        private static Raylib_cs.Texture2D _braceletTexture;
        private static Raylib_cs.Texture2D _ringTexture;
        private static bool _helmetTextureLoaded = false;
        private static bool _chestTextureLoaded = false;
        private static bool _glovesTextureLoaded = false;
        private static bool _legsTextureLoaded = false;
        private static bool _braceletTextureLoaded = false;
        private static bool _ringTextureLoaded = false;
        /// <summary>
        /// Parameterized constructor for the Armor class that sets itemID, quantity, name, description, tier, price, durability, bonusDefense, bonusHP, bonusSpeed, bonusMana, and armorType.
        /// </summary>
        public Armor(int itemID, int quantity, string name, string description, int tier, int price, int durability, double bonusDefense, double bonusHP, double bonusSpeed, double bonusMana, ArmorType armorType) : base(itemID, quantity, name, description, tier, price, durability)
        {
            _bonusDefense = bonusDefense;
            _bonusHP = bonusHP;
            _bonusSpeed = bonusSpeed;
            _bonusMana = bonusMana;
            _armorType = armorType;
        }
        /// <summary>
        /// Property method to get or set the bonus defense of the armor.
        /// </summary>
        public double BonusDefense
        {
            get { return _bonusDefense; }
            set { _bonusDefense = value; }
        }
        /// <summary>
        /// Property method to get or set the bonus HP of the armor.
        /// </summary>
        public double BonusHP
        {
            get { return _bonusHP; }
            set { _bonusHP = value; }
        }
        /// <summary>
        /// Property method to get or set the bonus speed of the armor.
        /// </summary>
        public double BonusSpeed
        {
            get { return _bonusSpeed; }
            set { _bonusSpeed = value; }
        }
        /// <summary>
        /// Property method to get or set the bonus mana of the armor.
        /// </summary>
        public double BonusMana
        {
            get { return _bonusMana; }
            set { _bonusMana = value; }
        }
        /// <summary>
        /// Property method to get or set the armor type of the armor.
        /// </summary>
        public ArmorType ArmorType
        {
            get { return _armorType; }
            set { _armorType = value; }
        }
        /// <summary>
        /// Override method to get the equipped armor details.
        /// </summary>
        public override string Equipped()
        {
            return "Equipped " + base.Name + " of type " + _armorType + " with bonus defense: " + _bonusDefense + ", HP: " + _bonusHP + ", speed: " + _bonusSpeed + ", and mana: " + _bonusMana + " durability: " + base.Durability;
        }
        /// <summary>
        /// Override method to load the armor texture based on its type.
        /// </summary>
        public override void LoadTexture()
        {
            switch (_armorType)
            {
                case ArmorType.Helmet:
                    if (!_helmetTextureLoaded)
                    {
                        _helmetTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Helmet.png");
                        _helmetTextureLoaded = true;
                    }
                    break;
                case ArmorType.Chest:
                    if (!_chestTextureLoaded)
                    {
                        _chestTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Chest.png");
                        _chestTextureLoaded = true;
                    }
                    break;
                case ArmorType.Glove:
                    if (!_glovesTextureLoaded)
                    {
                        _glovesTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Gloves.png");
                        _glovesTextureLoaded = true;
                    }
                    break;
                case ArmorType.Leg:
                    if (!_legsTextureLoaded)
                    {
                        _legsTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Leg.png");
                        _legsTextureLoaded = true;
                    }
                    break;
                case ArmorType.Bracelet:
                    if (!_braceletTextureLoaded)
                    {
                        _braceletTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Bracelet.png");
                        _braceletTextureLoaded = true;
                    }
                    break;
                case ArmorType.Ring:
                    if (!_ringTextureLoaded)
                    {
                        _ringTexture = Raylib.LoadTexture("picture/Item/Equipment/Armor/Ring.png");
                        _ringTextureLoaded = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Override method to get the texture of the armor based on its type.
        /// </summary>
        public override Raylib_cs.Texture2D GetTexture()
        {
            LoadTexture();
            switch (_armorType)
            {
                case ArmorType.Helmet:
                    return _helmetTexture;
                case ArmorType.Chest:
                    return _chestTexture;
                case ArmorType.Glove:
                    return _glovesTexture;
                case ArmorType.Leg:
                    return _legsTexture;
                case ArmorType.Bracelet:
                    return _braceletTexture;
                case ArmorType.Ring:
                    return _ringTexture;
                default:
                    return _helmetTexture;
            }
        }
        /// <summary>
        /// Static method to unload all armor textures.
        /// </summary>
        public static void UnloadAllTextures()
        {
            if (_helmetTextureLoaded)
            {
                Raylib.UnloadTexture(_helmetTexture);
                _helmetTextureLoaded = false;
            }
            if (_chestTextureLoaded)
            {
                Raylib.UnloadTexture(_chestTexture);
                _chestTextureLoaded = false;
            }
            if (_glovesTextureLoaded)
            {
                Raylib.UnloadTexture(_glovesTexture);
                _glovesTextureLoaded = false;
            }
            if (_legsTextureLoaded)
            {
                Raylib.UnloadTexture(_legsTexture);
                _legsTextureLoaded = false;
            }
            if (_braceletTextureLoaded)
            {
                Raylib.UnloadTexture(_braceletTexture);
                _braceletTextureLoaded = false;
            }
            if (_ringTextureLoaded)
            {
                Raylib.UnloadTexture(_ringTexture);
                _ringTextureLoaded = false;
            }
        }
    }
}