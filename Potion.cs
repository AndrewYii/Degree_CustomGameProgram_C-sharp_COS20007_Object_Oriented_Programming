using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the potion class, inherits from Item and it has 14 extra attributes.
    /// </summary> 
    public class Potion : Item
    {
        private double _healingAmount;
        private int _expAmount;
        private int _reducingCooldown;
        private double _strength;
        private double _mana;
        private PotionType _potionType;
        private static Raylib_cs.Texture2D _hpPotionTexture;
        private static Raylib_cs.Texture2D _manaPotionTexture;
        private static Raylib_cs.Texture2D _reduceCooldownPotionTexture;
        private static Raylib_cs.Texture2D _expPotionTexture;
        private static bool _hpPotionTextureLoaded = false;
        private static bool _manaPotionTextureLoaded = false;
        private static bool _reduceCooldownPotionTextureLoaded = false;
        private static bool _expPotionTextureLoaded = false;
        /// <summary>
        /// Parameterized constructor for Potion class , sets the healing amount, experience amount, reducing cooldown, strength, mana, potion type, item ID, quantity, name, description, tier and price.
        /// </summary>
        public Potion(double healingAmount, int expAmount, int reducingCooldown, double strength, double mana, PotionType potionType, int itemID, int quantity, string name, string description, int tier, int price) : base(itemID, quantity, name, description, tier, price)
        {
            _healingAmount = healingAmount;
            _expAmount = expAmount;
            _reducingCooldown = reducingCooldown;
            _strength = strength;
            _mana = mana;
            _potionType = potionType;
        }
        /// <summary>
        /// This method is used to apply the potion effects in battle.
        /// </summary>
        public (bool, string) UseInBattle(Player player)
        {
            bool potionUsed = false;
            string message = "";
            switch (_potionType)
            {
                case PotionType.Healing:
                    double oldHP = player.HP;
                    player.HP = Math.Min(player.HP + _healingAmount, player.MaxHP);
                    message = "Used " + base.Name + " and restored " + (player.HP - oldHP).ToString("F2") + " HP!";
                    potionUsed = true;
                    break;
                case PotionType.Mana:
                    double oldMana = player.Mana;
                    player.Mana = Math.Min(player.Mana + _mana, player.MaxMana);
                    message = "Used " + base.Name + " and restored " + (player.Mana - oldMana).ToString("F2") + " MP!";
                    potionUsed = true;
                    break;
                case PotionType.ReduceCooldown:
                    bool anyCooldownReduced = false;
                    foreach (Skill skill in player.Skills)
                    {
                        if (skill.Cooldown > 0)
                        {
                            skill.Cooldown = Math.Max(0, skill.Cooldown - _reducingCooldown);
                            anyCooldownReduced = true;
                        }
                    }
                    if (anyCooldownReduced)
                    {
                        message = "Used " + base.Name + " and reduced skill cooldowns!";
                        potionUsed = true;
                    }
                    else
                    {
                        message = "No skills are on cooldown!";
                    }
                    break;
                case PotionType.ExpBoost:
                    player.AddExp(_expAmount);
                    message = "Used " + base.Name + " and gained " + _expAmount + " experience!";
                    potionUsed = true;
                    break;
            }
            return (potionUsed, message);
        }
        /// <summary>
        /// Method to load the texture of the potion based on its type.
        /// </summary>
        public void LoadTexture()
        {
            switch (_potionType)
            {
                case PotionType.Healing:
                    if (!_hpPotionTextureLoaded)
                    {
                        _hpPotionTexture = Raylib.LoadTexture("picture/Item/Potion/hppotion.png");
                        _hpPotionTextureLoaded = true;
                    }
                    break;
                case PotionType.Mana:
                    if (!_manaPotionTextureLoaded)
                    {
                        _manaPotionTexture = Raylib.LoadTexture("picture/Item/Potion/manapotion.png");
                        _manaPotionTextureLoaded = true;
                    }
                    break;
                case PotionType.ReduceCooldown:
                    if (!_reduceCooldownPotionTextureLoaded)
                    {
                        _reduceCooldownPotionTexture = Raylib.LoadTexture("picture/Item/Potion/reduceskillturnpotion.png");
                        _reduceCooldownPotionTextureLoaded = true;
                    }
                    break;
                case PotionType.ExpBoost:
                    if (!_expPotionTextureLoaded)
                    {
                        _expPotionTexture = Raylib.LoadTexture("picture/Item/Potion/xppotion.png");
                        _expPotionTextureLoaded = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Method to get the texture of the potion based on its type.
        /// </summary>
        public Raylib_cs.Texture2D GetTexture()
        {
            LoadTexture();
            switch (_potionType)
            {
                case PotionType.Healing:
                    return _hpPotionTexture;
                case PotionType.Mana:
                    return _manaPotionTexture;
                case PotionType.ReduceCooldown:
                    return _reduceCooldownPotionTexture;
                case PotionType.ExpBoost:
                    return _expPotionTexture;
                default:
                    return _hpPotionTexture;
            }
        }
        /// <summary>
        /// Method to unload all potion textures.
        /// </summary>
        public static void UnloadAllTextures()
        {
            if (_hpPotionTextureLoaded)
            {
                Raylib.UnloadTexture(_hpPotionTexture);
                _hpPotionTextureLoaded = false;
            }
            if (_manaPotionTextureLoaded)
            {
                Raylib.UnloadTexture(_manaPotionTexture);
                _manaPotionTextureLoaded = false;
            }
            if (_reduceCooldownPotionTextureLoaded)
            {
                Raylib.UnloadTexture(_reduceCooldownPotionTexture);
                _reduceCooldownPotionTextureLoaded = false;
            }
            if (_expPotionTextureLoaded)
            {
                Raylib.UnloadTexture(_expPotionTexture);
                _expPotionTextureLoaded = false;
            }
        }
        /// <summary>
        /// Property to get or set the healing amount of the potion.
        /// </summary>
        public double HealingAmount
        {
            get { return _healingAmount; }
            set { _healingAmount = value; }
        }
        /// <summary>
        /// Property to get or set the experience amount of the potion.
        /// </summary>
        public int ExpAmount
        {
            get { return _expAmount; }
            set { _expAmount = value; }
        }
        /// <summary>
        /// Property to get or set the mana of the potion.
        /// </summary>
        public double Mana
        {
            get { return _mana; }
            set { _mana = value; }
        }
        /// <summary>
        /// Property to get or set the reducing cool down of the potion.
        ///  </summary>
        public int ReducingCooldown
        {
            get { return _reducingCooldown; }
            set { _reducingCooldown = value; }
        }
        /// <summary>
        /// Property to get or set the potion type of the potion.
        /// </summary>
        public PotionType PotionType
        {
            get { return _potionType; }
            set { _potionType = value; }
        }
    }
}