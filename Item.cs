using System;
using System.Collections.Generic;

namespace DistinctionTask{
    /// <summary>
    /// This is item class that has properties and methods to create different types of items which are Potions, Weapons, and Armors.
    /// </summary>
    public class Item
    {
        private int _itemID;
        private int _quantity;
        private string _name;
        private string _description;
        private int _tier;
        private int _price;
        /// <summary>
        /// Parameterized constructor to initialize an item that sets the item ID, quantity, name, description, tier, and price.
        /// </summary>
        public Item(int itemID, int quantity, string name, string description, int tier, int price)
        {
            _itemID = itemID;
            _quantity = quantity;
            _name = name;
            _description = description;
            _tier = tier;
            _price = price;
        }
        /// <summary>
        /// Static method to create a potion from the provided data.
        /// </summary>
        public static Potion CreatePotionFromData(string potionType, int tier, int itemId, string name, string description, int price)
        {
            (double value, double duration) = GetPotionData(potionType, tier);

            string finalName = name;
            if (!name.Contains("Level " + tier))
            {
                finalName = "Level " + tier + " " + name;
            }
            double healingAmount = 0;
            int expAmount = 0;
            int reducingCooldown = 0;
            double strength = 0;
            double mana = 0;
            PotionType pType;
            switch (potionType.ToUpper())
            {
                case "HEALING":
                    pType = PotionType.Healing;
                    healingAmount = value;
                    break;
                case "MANA":
                    pType = PotionType.Mana;
                    mana = value;
                    break;
                case "REDUCECOOLDOWN":
                    pType = PotionType.ReduceCooldown;
                    reducingCooldown = (int)value;
                    break;
                case "EXPBOOST":
                    pType = PotionType.ExpBoost;
                    expAmount = (int)value;
                    break;
                default:
                    pType = PotionType.Healing;
                    healingAmount = value;
                    break;
            }
            return new Potion(healingAmount, expAmount, reducingCooldown, strength, mana, pType, itemId, 1, finalName, description, tier, price);
        }
        /// <summary>
        /// Static method to get potion data based on the potion type and tier.
        /// </summary>
        public static (double value, double duration) GetPotionData(string potionType, int tier)
        {
            switch (potionType.ToUpper())
            {
                case "HEALING":
                    return (10.0 * tier, 0);
                case "MANA":
                    return (10.0 * tier, 0);
                case "REDUCECOOLDOWN":
                    return (1.0 * tier, 0);
                case "EXPBOOST":
                    return (10.0 * tier, 0);
                default:
                    return (10.0 * tier, 0);
            }
        }
        /// <summary>
        /// Static method to get weapon and armor data based on the type and tier.
        /// </summary>
        private static (double attack, double critical) GetWeaponData(WeaponType type, int tier)
        {
            switch (type)
            {
                case WeaponType.Sword:
                    return (8.0 + 1 * tier, 0.08 + 0.01 * tier);

                case WeaponType.Bow:
                    return (6.0 + 0.5 * tier, 0.12 + 0.02 * tier);

                case WeaponType.Axe:
                    return (10.0 + 2 * tier, 0.06 + 0.005 * tier);

                default:
                    return (5.0 + 0.5 * tier, 0.05 + 0.5 * tier);
            }
        }
        /// <summary>
        /// Static method to get armor data based on the armor type and tier.
        /// </summary>
        private static (double defense, double hp, double speed, double mana) GetArmorData(ArmorType type, int tier)
        {
            switch (type)
            {
                case ArmorType.Helmet:
                    return (3.0 + 0.5 * tier, 8.0 + 0.5 * tier, 0, 2.0 + 0.5 * tier);

                case ArmorType.Chest:
                    return (5.0 + 0.5 * tier, 15.0 + 0.5 * tier, 1.0 * tier, 0);

                case ArmorType.Leg:
                    return (2.0 + 0.5 * tier, 6.0 + 0.5 * tier, 1.0 * tier, 1.0 * tier);

                case ArmorType.Glove:
                    return (1.5 + 0.5 * tier, 4.0 + 0.5 * tier, 1.0 * tier, 1.5 + 0.5 * tier);

                case ArmorType.Bracelet:
                    return (1.0 + 0.5 * tier, 3.0 + 0.5 * tier, 3.0 + 0.5 * tier, 0);

                case ArmorType.Ring:
                    return (0.5 + 0.5 * tier, 2.0 + 0.5 * tier, 0, 3.0 + 0.5 * tier);

                default:
                    return (2.0 + 0.5 * tier, 5.0 + 0.5 * tier, 0, 0);
            }
        }
        /// <summary>
        /// Static method to create a weapon from the provided data.
        /// </summary>
        public static Weapon CreateWeaponFromData(WeaponType weaponType, int tier, int itemId, string name, string description, int price)
        {
            (double attack, double critical) = GetWeaponData(weaponType, tier);

            string finalName = name;
            if (!name.Contains("Level " + tier))
            {
                finalName = "Level " + tier + " " + name;
            }

            return new Weapon(attack,critical,weaponType,itemId,1,finalName,description,tier,price,100);
        }
        /// <summary>
        /// Static method to create armor from the provided data.
        /// </summary>
        public static Armor CreateArmorFromData(ArmorType armorType, int tier, int itemId, string name, string description, int price)
        {
            (double defense, double hp, double speed, double mana) = GetArmorData(armorType, tier);
            string finalName = name;
            if (!name.Contains("Level " + tier))
            {
                finalName = "Level " + tier + " " + name;
            }
            return new Armor(itemId, 1, finalName, description, tier, price, 100, defense, hp, speed, mana, armorType);
        }
        /// <summary>
        /// Property to get or set the item ID.
        /// </summary>
        public int ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
        /// <summary>
        /// Property to get or set the quantity of the item.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        /// <summary>
        /// Property to get or set the name of the item.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Property to get or set the description of the item.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// Property to get or set the tier of the item.
        /// </summary>
        public int Tier
        {
            get { return _tier; }
            set { _tier = value; }
        }
        /// <summary>
        /// Property to get or set the price of the item.
        /// </summary>
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }
    }
}