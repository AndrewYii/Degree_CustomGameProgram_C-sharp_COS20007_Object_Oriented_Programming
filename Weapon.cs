using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Weapon class, inherits from Equipment class, has extra 9 attributes.
    /// </summary>
    public class Weapon : Equipment
    {
        private double _bonusAttack;
        private double _bonusCriticalRate;
        private WeaponType _weaponType;
        private static Raylib_cs.Texture2D _swordTexture;
        private static Raylib_cs.Texture2D _bowTexture;
        private static Raylib_cs.Texture2D _axeTexture;
        private static bool _swordTextureLoaded = false;
        private static bool _bowTextureLoaded = false;
        private static bool _axeTextureLoaded = false;
        /// <summary>
        /// Parameterized constructor for Weapon class, that sets the bonus attack, bonus critical rate, weapon type, item ID, quantity, name, description, tier, price and durability.
        /// </summary>
        public Weapon(double bonusAttack, double bonusCriticalRate, WeaponType weaponType, int itemID, int quantity, string name, string description, int tier, int price, int durability) : base(itemID, quantity, name, description, tier, price, durability)
        {
            _bonusAttack = bonusAttack;
            _bonusCriticalRate = bonusCriticalRate;
            _weaponType = weaponType;
        }
        /// <summary>
        /// Override to return the equipped weapon's bonus attack, critical rate, and durability.
        /// </summary>
        public override string Equipped()
        {
            return "Equipped " + base.Name + " of type " + _weaponType + " with bonus attack: " + _bonusAttack + ", critical rate: " + _bonusCriticalRate + " and durability: " + base.Durability;
        }
        /// <summary>
        /// Override to load the texture for the weapon based on its type, ensuring that each texture is loaded only once.
        /// </summary>
        public override void LoadTexture()
        {
            switch (_weaponType)
            {
                case WeaponType.Sword:
                    if (!_swordTextureLoaded)
                    {
                        _swordTexture = Raylib.LoadTexture("picture/Item/Equipment/Weapon/Sword.png");
                        _swordTextureLoaded = true;
                    }
                    break;
                case WeaponType.Bow:
                    if (!_bowTextureLoaded)
                    {
                        _bowTexture = Raylib.LoadTexture("picture/Item/Equipment/Weapon/Bow.png");
                        _bowTextureLoaded = true;
                    }
                    break;
                case WeaponType.Axe:
                    if (!_axeTextureLoaded)
                    {
                        _axeTexture = Raylib.LoadTexture("picture/Item/Equipment/Weapon/Axe.png");
                        _axeTextureLoaded = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Override to return the texture of the weapon based on its type.
        /// </summary>
        public override Raylib_cs.Texture2D GetTexture()
        {
            LoadTexture();
            switch (_weaponType)
            {
                case WeaponType.Sword:
                    return _swordTexture;
                case WeaponType.Bow:
                    return _bowTexture;
                case WeaponType.Axe:
                    return _axeTexture;
                default:
                    return _swordTexture;
            }
        }
        /// <summary>
        /// Static method to unload all textures associated with the weapon class.
        /// </summary>
        public static void UnloadAllTextures()
        {
            if (_swordTextureLoaded)
            {
                Raylib.UnloadTexture(_swordTexture);
                _swordTextureLoaded = false;
            }
            if (_bowTextureLoaded)
            {
                Raylib.UnloadTexture(_bowTexture);
                _bowTextureLoaded = false;
            }
            if (_axeTextureLoaded)
            {
                Raylib.UnloadTexture(_axeTexture);
                _axeTextureLoaded = false;
            }
        }
        /// <summary>
        /// Property method to get or set the bonus attack of the weapon.
        /// </summary>
        public double BonusAttack
        {
            get { return _bonusAttack; }
            set { _bonusAttack = value; }
        }
        /// <summary>
        /// Property method to get or set the bonus critical rate of the weapon.
        /// </summary>
        public double BonusCriticalRate
        {
            get { return _bonusCriticalRate; }
            set { _bonusCriticalRate = value; }
        }
        /// <summary>
        /// Property method to get or set the weapon type of the weapon.
        /// </summary>
        public WeaponType WeaponType
        {
            get { return _weaponType; }
            set { _weaponType = value; }
        }
    }
}