using System;

namespace DistinctionTask{
    /// <summary>
    /// This is the AttackStatusSkill class, inherits from Skill, it has 3 extra attributes.
    /// </summary>
    public class AttackStatusSkill : Skill
    {
        private double _attackPower;
        private double _criticalRate;
        private Buff _buff;
        /// <summary>
        /// Parameterized constructor for the AttackStatusSkill class that sets skillID, name, description, manaCost, damage, cooldown, duration, levelRequired, attackPower, criticalRate, and buff.
        /// </summary>
        public AttackStatusSkill(int skillID, string name, string description, int manaCost, double damage, double cooldown, double duration, int levelRequired, double attackPower, double criticalRate, Buff buff) : base(skillID, name, description, manaCost, damage, cooldown, duration, levelRequired)
        {
            _attackPower = attackPower;
            _criticalRate = criticalRate;
            _buff = buff;
        }
        /// <summary>
        /// Override method to use the attack status skill on a target, applying damage ,considering critical hits and applying buff.
        /// </summary>
        public override double Used(Unit target)
        {
            Random random = new Random();
            double finalDamage = _attackPower + base.Damage;
            if (random.NextDouble() < _criticalRate)
            {
                finalDamage *= 2;
            }
            finalDamage = Math.Max(1, finalDamage);
            target.TakeDamage(finalDamage);
            _buff.ApplyBuff(target);
            return finalDamage;
        }
    }
}