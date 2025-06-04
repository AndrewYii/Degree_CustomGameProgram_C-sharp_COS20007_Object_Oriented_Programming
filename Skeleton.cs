using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask
{
    /// <summary>
    /// This is the Skeleton class, inheriting from the Monster class, has extra 17 attributes.
    /// </summary>
    public class Skeleton : Monster
    {
        private double _reviveChance;
        private bool _hasRevived = false;
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _attackTexture;
        private Raylib_cs.Texture2D _attackSkillTexture;
        private Raylib_cs.Texture2D _attackStatueSkillTexture;
        private Raylib_cs.Texture2D _deathTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.2f;
        private const int _idleFrameCount = 6;
        private const int _attackFrameCount = 9;
        private const int _attackSkillFrameCount = 8;
        private const int _attackStatueSkillFrameCount = 12;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for Skeleton class, that sets the name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive ,expReward ,initializes the revive chance and loads textures.
        /// </summary>
        public Skeleton(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int money, int level, int row, int column, bool isAlive, int expReward) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive, expReward)
        {
            _reviveChance = 1;
            LoadTextures();
        }
        /// <summary>
        /// Loads the textures for the Skeleton animations.
        /// </summary>
        private void LoadTextures()
        {
            _idleTexture = Raylib.LoadTexture("picture/Monster/Skeleton/Skeleton-Idle.png");
            _attackTexture = Raylib.LoadTexture("picture/Monster/Skeleton/Skeleton-Attack.png");
            _attackSkillTexture = Raylib.LoadTexture("picture/Monster/Skeleton/Skeleton-AttackSkill.png");
            _attackStatueSkillTexture = Raylib.LoadTexture("picture/Monster/Skeleton/Skeleton-AttackStatueSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Monster/Skeleton/Skeleton-Death.png");
        }
        /// <summary>
        /// Overrides to update the animation frame based on the current animation type and elapsed time.
        /// </summary>
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
            return new Raylib_cs.Rectangle(_frame * _frameWidth, 0, _frameWidth, GetCurrentTexture().Height);
        }
        /// <summary>
        /// Override to set the current animation type and reset frame and animation time.
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
        /// Override to unload the textures when the Skeleton is no longer needed.
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
        /// Override to handle the action of reviving the Skeleton.
        /// </summary>
        public override string TryAction(Unit target)
        {
            if (_hasRevived || IsAlive)
            {
                return string.Empty;
            }
            double adjustedReviveChance = _reviveChance * (1 + (Level * 0.05));
            //adjustedReviveChance = Math.Max(0.05, Math.Min(0.75, adjustedReviveChance));
            Random random = new Random();
            double roll = random.NextDouble();
            if (roll < adjustedReviveChance)
            {
                IsAlive = true;
                HP = MaxHP * 0.5; 
                _hasRevived = true; 
                using (StreamWriter writer = new StreamWriter("log/skeleton_revives.txt", true))
                {
                    writer.WriteLine(DateTime.Now + ": Skeleton '" + Name + "' (Level " + Level + ") revived!");
                    writer.WriteLine("Revive chance: " + adjustedReviveChance.ToString("P2") + " (Base: " + _reviveChance.ToString("P2") + ")");
                    writer.WriteLine("HP restored: " + HP.ToString("F1") + " (" + (HP / MaxHP).ToString("P1") + " of max)");
                    writer.WriteLine();
                }
                return "Skeleton revived successfully";
            }
            else
            {
                _hasRevived = true; 
                using (StreamWriter writer = new StreamWriter("log/skeleton_revives.txt", true))
                {
                    writer.WriteLine(DateTime.Now + ": Skeleton '" + Name + "' (Level " + Level + ") failed to revive");
                    writer.WriteLine("Revive chance: " + adjustedReviveChance.ToString("P2") + " (Base: " + _reviveChance.ToString("P2") + ")");
                    writer.WriteLine();
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Override the Die method to handle Skeleton-specific death logic.
        /// </summary>
        public override void Die()
        {
            if (!IsAlive){
                return;
            }
            base.Die();
            TryAction(this);
        }
        /// <summary>
        /// Override to render the Skeleton in battle, handling specific animations based on attack type and skill name.
        /// </summary>
        public override void RenderBattle(Vector2 position, float scale, AnimationType battleAnimation, bool showAttackEffect = false, string attackType = "", string skillName = "", float attackProgress = 0f)
        {
            if (showAttackEffect && !string.IsNullOrEmpty(attackType))
            {
                if (attackType == "SkeletonAttackSkill")
                {
                    if (skillName == "HEAVYSLASH")
                    {
                        SetAnimation(AnimationType.AttackStatueSkill);
                    }
                    else
                    {
                        SetAnimation(AnimationType.AttackSkill);
                    }
                }
                else if (attackType == "Skeleton")
                {
                    SetAnimation(AnimationType.Attack);
                }
                else
                {
                    SetAnimation(battleAnimation);
                }
            }
            else
            {
                SetAnimation(battleAnimation);
            }
            base.RenderBattle(position, scale, battleAnimation, showAttackEffect, attackType, skillName, attackProgress);
        }
    }
}