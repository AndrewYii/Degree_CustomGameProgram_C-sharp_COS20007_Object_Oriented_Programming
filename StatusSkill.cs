using System;

namespace DistinctionTask{
    /// <summary>
    /// This is the status skill class, inheriting from the Skill class, used to apply buffs to units.
    /// </summary> 
    public class StatusSkill : Skill
    {
        private Buff _buff;
        /// <summary>
        /// Parameterized constructor for StatusSkill that sets the skill ID, name, description, mana cost, damage, cooldown, duration, level required, and initializes the buff.
        /// </summary>
        public StatusSkill(int skillID, string name, string description, int manaCost, double damage, double cooldown, double duration, int levelRequired, Buff buff) : base(skillID, name, description, manaCost, damage, cooldown, duration, levelRequired)
        {
            _buff = buff;
        }
        /// <summary>
        /// Applies the buff to the target unit and returns 0 as the used value.
        /// </summary>
        public override double Used(Unit target)
        {
            _buff.ApplyBuff(target);
            return 0;
        }
    }
}