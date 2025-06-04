using System;
using System.Collections.Generic;
using System.IO;

namespace DistinctionTask{
    /// <summary>
    /// This is the abstract Unit class, which serves as a base class for all units in the game.
    /// </summary>
    public abstract class Unit
    {
        private string _name;
        private double _HP;
        private double _maxHP;
        private double _damage;
        private double _criticalRate;
        private double _defense;
        private double _speed;
        private double _mana;
        private double _maxMana;
        private int _exp;
        private int _level;
        private int _row;
        private int _column;
        private bool _isAlive;
        private List<Buff> _buffs;
        private List<Skill> _skills;
        /// <summary>
        /// Parameterized constructor that sets the name, HP, maxHP, damage, criticalRate, defense, speed, mana, exp, level, row, column, isAlive and initializes buffs and skills lists.
        /// </summary>
        public Unit(string name, double HP, double maxHP, double damage, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive)
        {
            _name = name;
            _HP = HP;
            _maxHP = maxHP;
            _damage = damage;
            _criticalRate = criticalRate;
            _defense = defense;
            _speed = speed;
            _mana = mana;
            _maxMana = 100;
            _exp = exp;
            _level = level;
            _row = row;
            _column = column;
            _isAlive = isAlive;
            _buffs = new List<Buff>();
            _skills = new List<Skill>();
        }
        /// <summary>
        /// Updates the unit's hp lost after each turn.
        /// </summary>
        public void TakeDamage(double damage)
        {
            double actualDamage = damage - _defense;
            actualDamage = Math.Max(1, actualDamage);
            _HP -= actualDamage;
            if (_HP <= 0)
            {
                _HP = 0;
                if (_isAlive)
                {
                    Die();
                }
            }
        }
        /// <summary>
        /// Virtual method to handle the death of the unit, setting isAlive to false.
        /// </summary>
        public virtual void Die()
        {
            _isAlive = false;
        } 
        /// <summary>
        /// Adds a buff to the unit, applies its effects, and updates the buffs list.
        /// </summary>
        public void AddBuff(Buff buff)
        {
            _buffs.Add(buff);
            buff.ApplyBuff(this);
        }
        /// <summary>
        /// Updates all buffs after each turn, checking for expiration and removing them if necessary.
        /// </summary>
        public void UpdateBuffs()
        {
            foreach (Buff buff in _buffs.ToList())
            {
                buff.UpdateAfterTurn();
                if (buff.CheckExpired())
                {
                    buff.RemoveBuff(this);
                    _buffs.Remove(buff);
                }
            }
        }
        /// <summary>
        /// Uses a skill on a target unit, checking if the unit has enough mana to use the skill.
        /// </summary>
        public double UseSkill(Skill skill, Unit targetUnit)
        {
            if (_mana >= skill.ManaCost)
            {
                _mana -= skill.ManaCost;
                return skill.Used(targetUnit);
            }
            return 0;
        }
        /// <summary>
        /// Property method to get or set the name of the unit.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Property method to get or set the HP of the unit.
        /// </summary>
        public double HP
        {
            get { return _HP; }
            set { _HP = Math.Min(value, _maxHP); }
        }
        /// <summary>
        /// Property method to get or set the maximum HP of the unit.
        /// </summary>
        public double MaxHP
        {
            get { return _maxHP; }
            set { _maxHP = value; }
        }
        /// <summary>
        /// Property method to get or set the damage of the unit.
        /// </summary>
        public double Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }
        /// <summary>
        /// Property method to get or set the critical rate of the unit.
        /// </summary>
        public double CriticalRate
        {
            get { return _criticalRate; }
            set { _criticalRate = value; }
        }
        /// <summary>
        /// Property method to get or set the defense of the unit.
        /// </summary>
        public double Defense
        {
            get { return _defense; }
            set { _defense = value; }
        }
        /// <summary>
        /// Property method to get or set the speed of the unit.
        /// </summary>
        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        /// <summary>
        /// Property method to get or set the mana of the unit, ensuring it does not exceed maxMana.
        /// </summary>
        public double Mana
        {
            get { return _mana; }
            set { _mana = Math.Min(value, _maxMana); }
        }
        /// <summary>
        /// Property method to get or set the maximum mana of the unit.
        /// </summary>
        public double MaxMana
        {
            get { return _maxMana; }
            set { _maxMana = value; }
        }
        /// <summary>
        /// Property method to get or set the experience points of the unit.
        /// </summary>
        public int Exp
        {
            get { return _exp; }
            set { _exp = value; }
        }
        /// <summary>
        /// Property method to get or set the level of the unit.
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        /// <summary>
        /// Property method to get or set the row position of the unit.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        /// <summary>
        /// Property method to get or set the column position of the unit.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }
        /// <summary>
        /// Property method to get or set the alive status of the unit.
        /// </summary>
        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }
        /// <summary>
        /// Property method to get or set the buffs of the unit.
        /// </summary>
        public List<Buff> Buffs
        {
            get { return _buffs; }
            set { _buffs = value; }
        }
        /// <summary>
        /// Property method to get or set the skills of the unit.
        /// </summary>
        public List<Skill> Skills
        {
            get { return _skills; }
            set { _skills = value; }
        }
    }
}