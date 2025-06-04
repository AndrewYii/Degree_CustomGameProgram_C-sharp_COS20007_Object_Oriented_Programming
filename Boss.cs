using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Boss class, inherits from monster class, has the extra 16 attributes.
    /// </summary>
    public class Boss : Monster
    {
        private int _phase;
        private double _buffChance;
        private Raylib_cs.Texture2D _phase1IdleTexture;
        private Raylib_cs.Texture2D _phase1AttackTexture;
        private Raylib_cs.Texture2D _phase2IdleTexture;
        private Raylib_cs.Texture2D _phase2AttackTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.2f;
        private const int _phase1IdleFrameCount = 6;
        private const int _phase1AttackFrameCount = 8;
        private const int _phase1FrameWidth = 100;
        private const int _phase2IdleFrameCount = 6;
        private const int _phase2AttackFrameCount = 8;
        private const int _phase2FrameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterised constructor for Boss class that set name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive ,expReward and initialises the phase,buffchance and loads the textures.
        /// </summary>
        public Boss(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int money, int level, int row, int column, bool isAlive, int expReward) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive, expReward)
        {
            _phase = 1;
            _buffChance = 1;
            LoadTextures();
        }
        /// <summary>
        /// Loads the textures for the boss animations.
        ///  </summary>
        private void LoadTextures()
        {
            _phase1IdleTexture = Raylib.LoadTexture("picture/Monster/Boss/phase1/BossPhase1-Idle.png");
            _phase1AttackTexture = Raylib.LoadTexture("picture/Monster/Boss/phase1/BossPhase1-Attack.png");
            _phase2IdleTexture = Raylib.LoadTexture("picture/Monster/Boss/phase2/BossPhase2-Idle.png");
            _phase2AttackTexture = Raylib.LoadTexture("picture/Monster/Boss/phase2/BossPhase2-Attack.png");
        }
        /// <summary>
        /// Override methode to update the animation frame based on the elapsed time and current animation type.
        /// </summary>
        public override void UpdateAnimation()
        {
            _animTime += Raylib.GetFrameTime();

            if (_animTime >= _frameDuration)
            {
                int frameCount = GetCurrentFrameCount();
                _frame = (_frame + 1) % frameCount;
                _animTime = 0;
            }
        }
        /// <summary>
        /// Method to get the current frame count based on the boss phase and animation type.
        /// </summary>
        private int GetCurrentFrameCount()
        {
            if (_phase == 1)
            {
                switch (_currentAnimation)
                {
                    case AnimationType.Idle:
                        return _phase1IdleFrameCount;
                    case AnimationType.Attack:
                        return _phase1AttackFrameCount;
                    default:
                        return _phase1IdleFrameCount;
                }
            }
            else
            {
                switch (_currentAnimation)
                {
                    case AnimationType.Idle:
                        return _phase2IdleFrameCount;
                    case AnimationType.Attack:
                        return _phase2AttackFrameCount;
                    default:
                        return _phase2IdleFrameCount;
                }
            }
        }
        /// <summary>
        /// Override method to get the current texture based on the boss phase and animation type.
        /// </summary>
        public override Raylib_cs.Texture2D GetCurrentTexture()
        {
            if (_phase == 1)
            {
                switch (_currentAnimation)
                {
                    case AnimationType.Idle:
                        return _phase1IdleTexture;
                    case AnimationType.Attack:
                        return _phase1AttackTexture;
                    default:
                        return _phase1IdleTexture;
                }
            }
            else
            {
                switch (_currentAnimation)
                {
                    case AnimationType.Idle:
                        return _phase2IdleTexture;
                    case AnimationType.Attack:
                        return _phase2AttackTexture;
                    default:
                        return _phase2IdleTexture;
                }
            }
        }
        /// <summary>
        /// Override method to get the source rectangle for the current animation frame based on the boss phase.
        /// </summary>
        public override Raylib_cs.Rectangle GetSourceRectangle()
        {
            int frameWidth = _phase == 1 ? _phase1FrameWidth : _phase2FrameWidth;
            return new Raylib_cs.Rectangle(_frame * frameWidth, 0, frameWidth, GetCurrentTexture().Height);
        }
        /// <summary>
        /// Override method to set the animation type for the boss.
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
        /// Override method to unload textures for the boss.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_phase1IdleTexture);
            Raylib.UnloadTexture(_phase1AttackTexture);
            Raylib.UnloadTexture(_phase2IdleTexture);
            Raylib.UnloadTexture(_phase2AttackTexture);
        }
        /// <summary>
        /// Override method to render the boss during battle.
        /// </summary>
        public override void RenderBattle(Vector2 position, float scale, AnimationType battleAnimation, bool showAttackEffect = false, string attackType = "", string skillName = "", float attackProgress = 0f)
        {
            // Handle specific boss attack animations based on attack type
            if (showAttackEffect && !string.IsNullOrEmpty(attackType))
            {
                if (attackType == "Boss" || attackType == "BossAttackSkill")
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
        /// <summary>
        /// Method to change the boss phase when certain conditions are met.
        /// </summary>
        public void ChangePhase()
        {
            if (_phase == 1)
            {
                _phase = 2;
                base.Name = base.Name.Replace("Phase 1", "Phase 2");
                base.MaxHP *= 0.8; 
                base.HP = base.MaxHP;   
                base.Damage *= 2; 
                base.Defense *= 0.7; 
                base.Speed *= 1.5; 
                base.Skills.Clear();
                using (StreamWriter writer = new StreamWriter("log/boss_phase_change.txt", true))
                {
                    writer.WriteLine(DateTime.Now + ": Boss '" + base.Name + "' has changed to Phase 2!");
                    writer.WriteLine("New stats: HP=" + base.MaxHP + ", DMG=" + base.Damage + ", DEF=" + base.Defense + ", SPD=" + base.Speed);
                    writer.WriteLine();
                }
            }
        }
        /// <summary>
        /// Override method to allow the boss to perform a behaviour action, which in this case is to buff a target unit.
        /// </summary>
        public override string TryAction(Unit target)
        {

            Random random = new Random();
            double roll = random.NextDouble();
            double adjustedBuffChance = _buffChance * (1 + (Level * 0.04));
            //adjustedBuffChance = Math.Min(0.7, adjustedBuffChance);
            if (roll < adjustedBuffChance)
            {
                double damageIncrease = target.Damage * (0.2 + random.NextDouble() * 0.2);
                double defenseIncrease = target.Defense * (0.2 + random.NextDouble() * 0.2);
                double speedIncrease = target.Speed * (0.1 + random.NextDouble() * 0.2);
                target.Damage += damageIncrease;
                target.Defense += defenseIncrease;
                target.Speed += speedIncrease;
                using (StreamWriter writer = new StreamWriter("log/boss_buffs.txt", true))
                {
                    writer.WriteLine(DateTime.Now + ": Boss '" + base.Name + "' (Phase " + _phase + ", Level " + Level + ") buffed '" + target.Name + "'");
                    writer.WriteLine("Buff chance: " + adjustedBuffChance.ToString("P2") + " (Base: " + _buffChance.ToString("P2") + ")");
                    writer.WriteLine("Increases: DMG +" + damageIncrease.ToString("F1") + ", DEF +" + defenseIncrease.ToString("F1") + ", SPD +" + speedIncrease.ToString("F1"));
                    writer.WriteLine();
                }
                return "Boss buff successfully";
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Override method to handle boss death.
        /// </summary>
        public override void Die()
        {
            if (_phase == 1)
            {
                IsAlive = true;
                ChangePhase();
            }
            else
            {
                base.Die();
                using (StreamWriter writer = new StreamWriter("log/boss_defeated.txt", true))
                {
                    writer.WriteLine(DateTime.Now + ": Boss '" + base.Name + "' (Phase " + _phase + ", Level " + Level + ") has been defeated!");
                    writer.WriteLine();
                }
            }
        }
        /// <summary>
        /// Property to get the current phase of the boss.
        /// </summary>
        public int Phase
        {
            get { return _phase; }
        }
    }
}