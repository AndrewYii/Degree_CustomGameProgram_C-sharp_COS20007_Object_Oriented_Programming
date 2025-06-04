using System;
using System.Collections.Generic;
using System.IO;

namespace DistinctionTask
{
    /// <summary>
    /// This is Buff class that has the 14 attributes.
    /// </summary> 
    public class Buff
    {
        private int _buffID;
        private string _name;
        private string _description;
        private int _remainingTurns;
        private double _attackBonus;
        private double _defenseBonus;
        private double _speedBonus;
        private double _criticalRateBonus;
        private BuffType _buffType;
        private int _tier;
        private static Dictionary<string, string> _buffDataCache = new Dictionary<string, string>();
        private static bool _dataLoaded = false;
        /// <summary>
        /// Parameterized constructor of Buff class to set the buffid, name, description, remaining turns, attack bonus, defense bonus, speed bonus, critical rate bonus, buff type and tier.
        /// </summary>
        public Buff(int buffID, string name, string description, int remainingTurns, double attackBonus, double defenseBonus, double speedBonus, double criticalRateBonus, BuffType buffType = BuffType.Positive, int tier = 1)
        {
            _buffID = buffID;
            _name = name;
            _description = description;
            _remainingTurns = remainingTurns;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            _speedBonus = speedBonus;
            _criticalRateBonus = criticalRateBonus;
            _buffType = buffType;
            _tier = tier;
        }
        /// <summary>
        /// Method to load buff data from a file into a dictionary cache.
        /// </summary>
        /// <summary>
        private static void LoadBuffData()
        {
            if (_dataLoaded) return;

            try
            {
                string path = "log/buff_reference_data.txt";
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || !line.Contains("="))
                        {
                            continue;
                        }
                        int eqIndex = line.IndexOf('=');
                        if (eqIndex > 0)
                        {
                            string key = line.Substring(0, eqIndex).Trim();
                            string value = line.Substring(eqIndex + 1).Trim();
                            _buffDataCache[key] = value;
                        }
                    }
                    _dataLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading buff data: " + ex.Message);
            }
        }
        /// <summary>
        /// Method to create a buff from a reference type and tier.
        /// </summary>
        public static Buff CreateFromReference(string buffType, int tier, bool isNegative = false)
        {
            if (!_dataLoaded)
            {
                LoadBuffData();
            }
            Random random = new Random();
            string key = buffType.ToUpper() + "_" + tier;
            double value = 0;
            int duration = 3;
            if (_buffDataCache.TryGetValue(key, out string? data) && data != null)
            {
                string[] parts = data.Split(',');
                if (parts.Length >= 2)
                {
                    double.TryParse(parts[0], out value);
                    int.TryParse(parts[1], out duration);
                }
            }
            if (isNegative)
            {
                value = -value;
            }
            double attackBonus = 0, defenseBonus = 0;
            double speedBonus = 0, critBonus = 0;
            switch (buffType.ToUpper())
            {
                case "ATTACK_BUFF":
                    attackBonus = value;
                    break;
                case "DEFENSE_BUFF":
                    defenseBonus = value;
                    break;
                case "SPEED_BUFF":
                    speedBonus = value;
                    break;
                case "CRIT_BUFF":
                    critBonus = value / 100.0; 
                    break;
            }
            string name = GetBuffName(buffType, tier, isNegative);
            string description = GetBuffDescription(buffType, Math.Abs(value), duration, isNegative);
            return new Buff(
                random.Next(1000),  
                name,
                description,
                duration,
                attackBonus,
                defenseBonus,
                speedBonus,
                critBonus,
                isNegative ? BuffType.Negative : BuffType.Positive,
                tier
            );
        }
        /// <summary>
        /// Method to create a random buff based on player level.
        /// </summary>
        public static Buff CreateRandomBuff(int playerLevel = 1)
        {
            if (!_dataLoaded){
                LoadBuffData();
            }
            Random random = new Random();
            string[] buffTypes = { "ATTACK_BUFF", "DEFENSE_BUFF", "SPEED_BUFF", "CRIT_BUFF" };
            string selectedType = buffTypes[random.Next(buffTypes.Length)];
            int maxTier = Math.Min(5, 1 + (playerLevel / 5));
            int tier = random.Next(1, maxTier + 1);
            return CreateFromReference(selectedType, tier, false);
        }
        /// <summary>
        /// Method to create a random debuff based on enemy level.
        /// </summary>
        public static Buff CreateRandomDebuff(int enemyLevel = 1)
        {
            if (!_dataLoaded)
            {
                LoadBuffData();
            }
            Random random = new Random();
            string[] buffTypes = { "ATTACK_BUFF", "DEFENSE_BUFF", "SPEED_BUFF" };
            string selectedType = buffTypes[random.Next(buffTypes.Length)];
            int maxTier = Math.Min(5, 1 + (enemyLevel / 5));
            int tier = random.Next(1, maxTier + 1);
            return CreateFromReference(selectedType, tier, true);
        }
        /// <summary>
        /// Method to generate a buff name based on type, tier, and negativity.
        /// </summary>
        private static string GetBuffName(string buffType, int tier, bool isNegative = false)
        {
            string prefix;
            switch (tier)
            {
                case 1: prefix = "Minor"; break;
                case 2: prefix = "Standard"; break;
                case 3: prefix = "Greater"; break;
                case 4: prefix = "Major"; break;
                case 5: prefix = "Supreme"; break;
                default: prefix = ""; break;
            }
            string suffix;
            switch (buffType.ToUpper())
            {
                case "ATTACK_BUFF":
                    suffix = isNegative ? "Weakness" : "Strength";
                    break;
                case "DEFENSE_BUFF":
                    suffix = isNegative ? "Vulnerability" : "Protection";
                    break;
                case "SPEED_BUFF":
                    suffix = isNegative ? "Slowness" : "Swiftness";
                    break;
                case "CRIT_BUFF":
                    suffix = isNegative ? "Inaccuracy" : "Precision";
                    break;
                default:
                    suffix = isNegative ? "Debuff" : "Effect";
                    break;
            }
            return prefix + " " + suffix;
        }
        /// <summary>
        /// Method to generate a buff description based on type, value, duration, and negativity.
        /// </summary>
        private static string GetBuffDescription(string buffType, double value, int duration, bool isNegative = false)
        {
            string effect;
            string direction = isNegative ? "-" : "+";

            switch (buffType.ToUpper())
            {
                case "ATTACK_BUFF":
                    effect = direction + value + " Attack";
                    break;
                case "DEFENSE_BUFF":
                    effect = direction + value + " Defense";
                    break;
                case "SPEED_BUFF":
                    effect = direction + value + " Speed";
                    break;
                case "CRIT_BUFF":
                    effect = direction + value + "% Critical chance";
                    break;
                default:
                    effect = direction + value + " Effect";
                    break;
            }
            return effect + " for " + duration + " turns";
        }
        /// <summary>
        /// Method to apply the buff effect to a unit.
        /// /// </summary>
        public void ApplyBuff(Unit unit)
        {
            unit.Damage += _attackBonus;
            unit.Defense += _defenseBonus;
            unit.Speed += _speedBonus;
            unit.CriticalRate += _criticalRateBonus;
        }
        /// <summary>
        /// Method to remove the buff effect from a unit.
        /// </summary>
        public void RemoveBuff(Unit unit)
        {
            unit.Damage -= _attackBonus;
            unit.Defense -= _defenseBonus;
            unit.Speed -= _speedBonus;
            unit.CriticalRate -= _criticalRateBonus;
        }
        /// <summary>
        /// Method to update the buff after each turn.
        ///  </summary>
        public void UpdateAfterTurn()
        {
            if (_remainingTurns > 0)
            {
                _remainingTurns--;
            }
        }
        /// <summary>
        /// Public method to check if the buff has expired.
        /// </summary>
        public bool CheckExpired()
        {
            return _remainingTurns <= 0;
        }
        /// <summary>
        /// Property to get or set the Name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Property to get or set the description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        ///  Property to get or set the attack bonus.
        /// </summary>
        public double AttackBonus
        {
            get { return _attackBonus; }
            set { _attackBonus = value; }
        }
        /// <summary>
        ///  Property to get or set the defense bonus.
        /// </summary>
        public double DefenseBonus
        {
            get { return _defenseBonus; }
            set { _defenseBonus = value; }
        }
        /// <summary>
        ///  Property to get or set the speed bonus.
        /// </summary>
        public double SpeedBonus
        {
            get { return _speedBonus; }
            set { _speedBonus = value; }
        }
        /// <summary>
        ///  Property to get or set the critical rate bonus.
        /// </summary>
        public double CriticalRateBonus
        {
            get { return _criticalRateBonus; }
            set { _criticalRateBonus = value; }
        }
    }
}