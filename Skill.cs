using System;

namespace DistinctionTask{
    /// <summary>
    /// This is skill class that control the skills of the player.
    /// </summary>
    public abstract class Skill
    {
        private int _skillID;
        private string _name;
        private string _description;
        private int _manaCost;
        private double _damage;
        private double _cooldown;
        private double _duration;
        private int _levelRequired;
        /// <summary>
        /// Parameterized constructor for Skill that sets the skill id, name, description, mana cost, damage, cooldown, duration, and level required.
        /// </summary>
        public Skill(int skillID, string name, string description, int manaCost, double damage, double cooldown, double duration, int levelRequired)
        {
            _skillID = skillID;
            _name = name;
            _description = description;
            _manaCost = manaCost;
            _damage = damage;
            _cooldown = cooldown;
            _duration = duration;
            _levelRequired = levelRequired;
        }
        /// <summary>
        /// Abstract method to be implemented by derived classes to define the skill's effect on a target unit.
        /// </summary>
        public abstract double Used(Unit target);
        /// <summary>
        /// Property method to get or set the skill name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Property method to get or set the skill description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// Property method to get or set the skill mana cost.
        /// </summary>
        public int ManaCost
        {
            get { return _manaCost; }
            set { _manaCost = value; }
        }
        /// <summary>
        /// Property method to get or set the skill damage.
        /// </summary>
        public double Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }
        /// <summary>
        /// Property method to get or set the skill cool down.
        /// </summary>
        public double Cooldown
        {
            get { return _cooldown; }
            set { _cooldown = value; }
        }
        /// <summary>
        /// Property method to get or set the skill duration need to be used.
        /// </summary>    
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
    }
}