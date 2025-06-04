using System;

namespace DistinctionTask
{
    /// <summary>
    /// This is the player factory class which follow the factory design pattern.
    /// </summary>
    public static class PlayerFactory
    {
        /// <summary>
        /// Creates a player based on the selected character role.
        /// </summary>
        public static Player CreatePlayerFromSelection(string playerName, string characterRole)
        {
            switch (characterRole.ToUpper())
            {
                case "KNIGHT":
                    return CreateKnight(playerName);
                case "ARCHER":
                    return CreateArcher(playerName);
                case "AXEMAN":
                    return CreateAxeman(playerName);
                default:
                    return CreateAxeman(playerName); 
            }
        }
        /// <summary>
        /// Creates a Knight player with base stats calculated from the class methods.
        /// </summary>
        private static Knight CreateKnight(string playerName)
        {
            Knight temp = new Knight(playerName, 1, 1, 1, 1, 1, 1, 100, 0, 1, 1, 1, true);
            double baseHP = temp.GetBaseHP();
            double baseDamage = temp.GetBaseDamage();
            double baseDefense = temp.GetBaseDefense();
            double baseSpeed = temp.GetBaseSpeed();
            double baseCriticalRate = temp.GetBaseCriticalRate();
            return new Knight(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, 100.0, 0, 1, 1, 1, true);
        }
        /// <summary>
        /// Creates an Archer player with base stats calculated from the class methods.
        /// </summary>
        private static Archer CreateArcher(string playerName)
        {
            Archer temp = new Archer(playerName, 1, 1, 1, 1, 1, 1, 100, 0, 1, 1, 1, true);
            double baseHP = temp.GetBaseHP();
            double baseDamage = temp.GetBaseDamage();
            double baseDefense = temp.GetBaseDefense();
            double baseSpeed = temp.GetBaseSpeed();
            double baseCriticalRate = temp.GetBaseCriticalRate();
            return new Archer(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, 100.0, 0, 1, 1, 1, true);
        }
        /// <summary>
        /// Creates an Axeman player with base stats calculated from the class methods.
        /// </summary>
        private static Axeman CreateAxeman(string playerName)
        {
            Axeman temp = new Axeman(playerName, 1, 1, 1, 1, 1, 1, 100, 0, 1, 1, 1, true);
            double baseHP = temp.GetBaseHP();
            double baseDamage = temp.GetBaseDamage();
            double baseDefense = temp.GetBaseDefense();
            double baseSpeed = temp.GetBaseSpeed();
            double baseCriticalRate = temp.GetBaseCriticalRate();
            return new Axeman(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, 100.0, 0, 1, 1, 1, true);
        }
    }
}
