using System;

namespace DistinctionTask{
    /// <summary>
    /// This is the AttackSkill class, inherits from Skill, it has extra 2 attributes.
    /// </summary>
    public class AttackSkill : Skill{
        private double _attackPower;
        private double _criticalHitChance;
        /// <summary>
        /// Parameterized constructor for the AttackSkill class that sets skillID, name, description, manaCost, damage, cooldown, duration, levelRequired, attackPower, and criticalHitChance.
        /// </summary>
        public AttackSkill(int skillID, string name, string description, int manaCost, double damage, double cooldown, double duration, int levelRequired, double attackPower, double criticalHitChance) : base(skillID, name, description, manaCost, damage, cooldown, duration, levelRequired)
        {
            _attackPower = attackPower;
            _criticalHitChance = criticalHitChance;
        }
        /// <summary>
        /// Override method to use the attack skill on a target, applying damage and considering critical hits.
        /// </summary>
        public override double Used(Unit target)
        {
            Random random = new Random();
            double finalDamage = _attackPower + base.Damage;
            if (random.NextDouble() < _criticalHitChance)
            {
                finalDamage *= 2;
            }
            finalDamage = Math.Max(1, finalDamage);
            target.TakeDamage(finalDamage);
            return finalDamage;
        }
    }
}