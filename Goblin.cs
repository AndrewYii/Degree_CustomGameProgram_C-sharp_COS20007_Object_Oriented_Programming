using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask
{
    /// <summary>
    /// This is the Goblin class, which inherits from the Monster class and has extra 17 attributes.
    /// </summary> 
    public class Goblin : Monster
    {
        private double _stealChance;
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _attackTexture;
        private Raylib_cs.Texture2D _attackSkillTexture;
        private Raylib_cs.Texture2D _attackStatueSkillTexture;
        private Raylib_cs.Texture2D _deathTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.2f;
        private const int _idleFrameCount = 6;
        private const int _attackFrameCount = 7;
        private const int _attackSkillFrameCount = 9;
        private const int _attackStatueSkillFrameCount = 11;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for the Goblin class that set the name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive , expReward , initializes the steal chance and loads the textures.
        /// </summary>
        public Goblin(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int money, int level, int row, int column, bool isAlive, int expReward) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive, expReward)
        {
            _stealChance = 1;
            LoadTextures();
        }
        /// <summary>
        /// Loads the textures for the Goblin animations.
        /// </summary>
        private void LoadTextures()
        {
            _idleTexture = Raylib.LoadTexture("picture/Monster/Goblin/Goblin-Idle.png");
            _attackTexture = Raylib.LoadTexture("picture/Monster/Goblin/Goblin-Attack.png");
            _attackSkillTexture = Raylib.LoadTexture("picture/Monster/Goblin/Goblin-AttackSkill.png");
            _attackStatueSkillTexture = Raylib.LoadTexture("picture/Monster/Goblin/Goblin-AttackStatueSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Monster/Goblin/Goblin-Death.png");
        }
        /// <summary>
        /// Updates the animation frame based on the current animation type and elapsed time.
        /// /// </summary>
        public override void UpdateAnimation()
        {
            _animTime += Raylib.GetFrameTime();

            float duration = _currentAnimation == AnimationType.Death ? _frameDuration + 0.3f : _frameDuration;

            if (_animTime >= duration)
            {
                int frameCount = GetCurrentFrameCount();
                _frame = (_frame + 1) % frameCount;
                _animTime = 0;
            }
        }
        /// <summary>
        /// Gets the current frame count based on the current animation type.
        /// </summary>
        private int GetCurrentFrameCount()
        {
            switch (_currentAnimation)
            {
                case AnimationType.Idle:
                    return _idleFrameCount;
                case AnimationType.Attack:
                    return _attackFrameCount;
                case AnimationType.AttackSkill:
                    return _attackSkillFrameCount;
                case AnimationType.AttackStatueSkill:
                    return _attackStatueSkillFrameCount;
                case AnimationType.Death:
                    return _deathFrameCount;
                default:
                    return _idleFrameCount;
            }
        }
        /// <summary>
        /// Override to get the current texture based on the current animation type.
        /// </summary>
        public override Raylib_cs.Texture2D GetCurrentTexture()
        {
            switch (_currentAnimation)
            {
                case AnimationType.Idle:
                    return _idleTexture;
                case AnimationType.Attack:
                    return _attackTexture;
                case AnimationType.AttackSkill:
                    return _attackSkillTexture;
                case AnimationType.AttackStatueSkill:
                    return _attackStatueSkillTexture;
                case AnimationType.Death:
                    return _deathTexture;
                default:
                    return _idleTexture;
            }
        }
        /// <summary>
        /// Override to get the source rectangle for the current animation frame.
        /// </summary>
        public override Raylib_cs.Rectangle GetSourceRectangle()
        {
            return new Raylib_cs.Rectangle(_frame * _frameWidth,0,_frameWidth,GetCurrentTexture().Height);
        }
        /// <summary>
        /// Override to set the animation type and reset the frame and animation time.
        /// </summary>
        public override void SetAnimation(AnimationType animation)
        {
            if (_currentAnimation != animation)
            {
                _currentAnimation = animation;
                _frame = 0;
                _animTime = 0f;
            }
        }
        /// <summary>
        /// Override to unload the textures when the Goblin is no longer needed.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_idleTexture);
            Raylib.UnloadTexture(_attackTexture);
            Raylib.UnloadTexture(_attackSkillTexture);
            Raylib.UnloadTexture(_attackStatueSkillTexture);
            Raylib.UnloadTexture(_deathTexture);
        }
        /// <summary>
        /// Override to handle the action of stealing money from a player (it will return the text to ease the unit test).
        /// </summary>
        public override string TryAction(Unit target)
        {
            Random random = new Random();
            double chance = random.NextDouble();
            double adjustedStealChance = _stealChance * (1 + (Level * 0.05));
            adjustedStealChance -= (target.Level * 0.02);
            //adjustedStealChance = Math.Max(0.05, Math.Min(0.95, adjustedStealChance));
            if (target is not Player player)
            {
                return string.Empty;
            }
            if (chance < adjustedStealChance)
            {
                if (player.Inventory != null && player.Inventory.Money > 0)
                {
                    int stealPercentage = random.Next(10, 26);
                    int amountToSteal = (int)(player.Inventory.Money * stealPercentage / 100.0);
                    amountToSteal = Math.Max(1, Math.Min(amountToSteal, player.Inventory.Money));
                    player.Inventory.Money -= amountToSteal;
                    base.MoneyReward += amountToSteal;
                    using (StreamWriter writer = new StreamWriter("log/goblin_steals.txt", true))
                    {
                        writer.WriteLine(DateTime.Now + ": Goblin '" + base.Name + "' (Level " + base.Level + ") stole " + amountToSteal + " money from " + target.Name);
                        writer.WriteLine("Steal chance: " + adjustedStealChance.ToString("P2") + " (Base: " + _stealChance.ToString("P2") + ")");
                        writer.WriteLine("Money stolen: " + amountToSteal);
                        writer.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine(base.Name + " tried to steal from " + player.Name + " but found no money!");
                }
                return "Goblin steal successful";
            }
            else
            {
                return string.Empty;
            }
        }

    }
}