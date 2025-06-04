using System;

namespace DistinctionTask
{
    /// <summary>
    /// This is the monster factory class which follows the factory design pattern.
    /// </summary>
    public static class MonsterFactory
    {
        /// <summary>
        /// Creates a monster based on a string type with complete stat calculation logic.
        /// </summary>
        public static Monster? CreateMonsterFromSelection(string monsterTypeString, int level, string prefix = "", Difficulty difficulty = Difficulty.Medium, int row = 0, int column = 0)
        {
            try
            {
                (double baseHP, double baseAttack, double baseCriticalRate, double baseDefense, double baseSpeed, double baseMana, int baseExpReward)? baseStats = GetBaseStats(monsterTypeString);
                if (baseStats == null){
                    return null;
                }
                double finalHP = baseStats.Value.baseHP + (level * 10);
                double finalMaxHP = baseStats.Value.baseHP + (level * 10);
                double finalAttack = baseStats.Value.baseAttack + (level * 2);
                double finalDefense = baseStats.Value.baseDefense + level;
                double finalCritRate = baseStats.Value.baseCriticalRate + (level * 0.01);
                double finalSpeed = baseStats.Value.baseSpeed;
                double finalMana = baseStats.Value.baseMana;
                int moneyReward = 5 * level + 30;
                if (!string.IsNullOrEmpty(prefix))
                {
                    ApplyPrefixModifiers(prefix, ref finalAttack, ref finalHP, ref finalMaxHP, ref finalDefense, ref finalCritRate);
                }
                ApplyDifficultyModifiers(difficulty, ref finalHP, ref finalMaxHP, ref finalAttack);
                Monster monster = CreateMonster(monsterTypeString, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, prefix, baseStats.Value.baseExpReward);
                if (monster != null)
                {
                    AssignMonsterSkills(monster, monsterTypeString);
                }
                return monster;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the base stats for a monster type based on the original implementation.
        /// </summary>
        private static (double baseHP, double baseAttack, double baseCriticalRate, double baseDefense, double baseSpeed, double baseMana, int baseExpReward)? GetBaseStats(string monsterType)
        {
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    return (40.0, 5.0, 0.05, 2.0, 8.0, 20.0, 25);
                case "SLIME":
                    return (40.0, 5.0, 0.05, 2.0, 3.0, 15.0, 20);
                case "SKELETON":
                    return (40.0, 5.0, 0.05, 2.0, 5.0, 30.0, 30);
                case "BOSS_PHASE1":
                    return (300.0, 10.0, 0.08, 5.0, 5.0, 100.0, 100);
                default:
                    return null;
            }
        }
        /// <summary>
        /// Applies prefix modifiers to monster stats based on the original implementation.
        /// </summary>
        private static void ApplyPrefixModifiers(string prefix, ref double finalAttack, ref double finalHP, ref double finalMaxHP, ref double finalDefense, ref double finalCritRate)
        {
            switch (prefix.ToUpper())
            {
                case "FERAL":
                    finalAttack *= 1.2;
                    finalHP *= 0.9;
                    finalMaxHP *= 0.9;
                    finalDefense *= 1.0;
                    finalCritRate += 0.0;
                    break;
                case "DARK":
                    finalAttack *= 1.1;
                    finalHP *= 1.0;
                    finalMaxHP *= 1.0;
                    finalDefense *= 1.0;
                    finalCritRate += 0.05;
                    break;
                case "SAVAGE":
                    finalAttack *= 1.3;
                    finalHP *= 1.0;
                    finalMaxHP *= 1.0;
                    finalDefense *= 0.8;
                    finalCritRate += 0.0;
                    break;
                case "VENOMOUS":
                    finalAttack *= 1.1;
                    finalHP *= 1.0;
                    finalMaxHP *= 1.0;
                    finalDefense *= 1.0;
                    finalCritRate += 0.1;
                    break;
                case "SHADOW":
                    finalAttack *= 1.0;
                    finalHP *= 1.0;
                    finalMaxHP *= 1.0;
                    finalDefense *= 0.9;
                    finalCritRate += 0.15;
                    break;
                case "CURSED":
                    finalAttack *= 1.2;
                    finalHP *= 1.2;
                    finalMaxHP *= 1.2;
                    finalDefense *= 0.8;
                    finalCritRate += 0.0;
                    break;
                case "ANCIENT":
                    finalAttack *= 1.0;
                    finalHP *= 1.5;
                    finalMaxHP *= 1.5;
                    finalDefense *= 1.3;
                    finalCritRate += 0.0;
                    break;
                case "GIANT":
                    finalAttack *= 1.2;
                    finalHP *= 1.8;
                    finalMaxHP *= 1.8;
                    finalDefense *= 1.1;
                    finalCritRate += 0.0;
                    break;
            }
        }
        /// <summary>
        /// Applies difficulty modifiers to monster stats.
        /// </summary>
        private static void ApplyDifficultyModifiers(Difficulty difficulty, ref double finalHP, ref double finalMaxHP, ref double finalAttack)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    finalHP *= 0.8;
                    finalMaxHP *= 0.8;
                    finalAttack *= 0.8;
                    break;
                case Difficulty.Hard:
                    finalHP *= 1.2;
                    finalMaxHP *= 1.2;
                    finalAttack *= 1.2;
                    break;
            }
        }
        /// <summary>
        /// Creates the appropriate monster with calculated stats.
        /// </summary>
        private static Monster CreateMonster(string monsterType, double finalHP, double finalMaxHP, double finalAttack, double finalCritRate, double finalDefense, double finalSpeed, double finalMana, int moneyReward, int level, int row, int column, string prefix, int baseExpReward)
        {
            string fullName = !string.IsNullOrEmpty(prefix) ? prefix + " " + monsterType : monsterType;
            bool isAlive = true;
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    return new Goblin(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward + level * 5);
                case "SLIME":
                    return new Slime(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward + level * 5);
                case "SKELETON":
                    return new Skeleton(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward + level * 5);
                case "BOSS_PHASE1":
                    return new Boss(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward);
                default:
                    throw new ArgumentException("Unknown monster type: " + monsterType);
            }
        }
        /// <summary>
        /// Assigns appropriate skills to a monster based on its type and level.
        /// </summary>
        private static void AssignMonsterSkills(Monster monster, string monsterType)
        {
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    if (monster.Level >= 1)
                    {
                        monster.Skills.Add(new AttackSkill(1, "UpDownBleeding", "Up Down Bleeding Attack", 12, 5, 0, 0, 1, 1.2, 0.0));
                    }
                    if (monster.Level >= 3)
                    {
                        monster.Skills.Add(new AttackSkill(3, "LeftRightSpin", "Increases attack and defense power", 35, 0, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
                case "SLIME":
                    if (monster.Level >= 1)
                    {
                        monster.Skills.Add(new AttackSkill(4, "IhaveKnife", "Has a knife to kill", 0, 10, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
                case "SKELETON":
                    if (monster.Level >= 2)
                    {
                        monster.Skills.Add(new AttackSkill(6, "AheadChop ", "Move the knife ahead to attack", 15, 4, 0, 0, 1, 1.0, 0.0));
                    }
                    if (monster.Level >= 3)
                    {
                        monster.Skills.Add(new AttackSkill(7, "HeavySlash", "Increases attack and defense power", 30, 0, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
            }
        }
    }
}
