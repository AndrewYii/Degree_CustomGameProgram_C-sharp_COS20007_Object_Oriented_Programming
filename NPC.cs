using System;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the NPC class, inherits from Unit.
    public abstract class NPC : Unit
    {
        /// <summary>
        /// Constructor for the NPC class.
        /// </summary>
        public NPC(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
        }
        /// <summary>
        /// Abstract method to draw the NPC's UI.
        /// </summary>
        public abstract string DrawUI(Player player, Game game);
        /// <summary>
        /// Abstract method to handle UI input from the player.
        /// </summary>
        public abstract bool HandleUIInput(Player player, Game game);
    }
}