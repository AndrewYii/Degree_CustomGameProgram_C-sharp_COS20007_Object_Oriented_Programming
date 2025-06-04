using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Knight class, which inherits from the Player class, that has extra 21 attribues.
    public class Knight : Player
    {
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _walkTexture;
        private Raylib_cs.Texture2D _statueSkillTexture;
        private Raylib_cs.Texture2D _attackTexture;
        private Raylib_cs.Texture2D _attackSkillTexture;
        private Raylib_cs.Texture2D _attackStatueSkillTexture;
        private Raylib_cs.Texture2D _deathTexture;
        private Raylib_cs.Texture2D _profileTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.1f;
        private const int _idleFrameCount = 6;
        private const int _walkFrameCount = 8;
        private const int _statueSkillFrameCount = 4;
        private const int _attackFrameCount = 7;
        private const int _attackSkillFrameCount = 10;
        private const int _attackStatueSkillFrameCount = 11;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for the Knight that sets the name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive and loads the textures.
        /// </summary>
        public Knight(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
            LoadTextures();
        }
        /// <summary>
        /// Override method to load Knight-specific textures.
        /// </summary>
        public override void LoadTextures()
        {
            _profileTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-profile.png");
            _idleTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-Idle.png");
            _walkTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-Walk.png");
            _statueSkillTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-StatueSkill.png");
            _attackTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-Attack.png");
            _attackSkillTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-AttackSkill.png");
            _attackStatueSkillTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-AttackStatueSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Player/Knight/Knight-Death.png");
        }
        /// <summary>
        /// Override method to set up the starting inventory for the Knight which includes the basic equipments, items (handled by virtual method) and an extra sword.
        /// </summary>
        public override void SetupStartingInventory()
        {
            base.SetupStartingInventory();
            Weapon knightSword = Item.CreateWeaponFromData(WeaponType.Sword,1,100,"Sword","A sturdy sword for a brave knight",25);
            base.Inventory.AddItem(knightSword);
        }
        /// <summary>
        /// Override method to update the animation based on the current animation type and frame duration.
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
                case AnimationType.Walk:
                    return _walkFrameCount;
                case AnimationType.StatueSkill:
                    return _statueSkillFrameCount;
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
                case AnimationType.Walk:
                    return _walkTexture;
                case AnimationType.StatueSkill:
                    return _statueSkillTexture;
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
            return new Raylib_cs.Rectangle( _frame * _frameWidth,0,_frameWidth,GetCurrentTexture().Height);
        }
        /// <summary>
        /// Override to get the profile texture for the Knight.
        /// </summary>
        public override Raylib_cs.Texture2D GetProfileTexture()
        {
            return _profileTexture;
        }
        /// <summary>
        /// Override to set the current animation and reset the frame and animation time.
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
        /// Override to unload the Knight-specific textures.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_profileTexture);
            Raylib.UnloadTexture(_idleTexture);
            Raylib.UnloadTexture(_walkTexture);
            Raylib.UnloadTexture(_statueSkillTexture);
            Raylib.UnloadTexture(_attackTexture);
            Raylib.UnloadTexture(_attackSkillTexture);
            Raylib.UnloadTexture(_attackStatueSkillTexture);
            Raylib.UnloadTexture(_deathTexture);
        }
        /// <summary>
        /// Override to get the base HP.
        /// </summary>
        public override double GetBaseHP()
        {
            return 120.0 + 10 * (base.Level - 1);
        }
        /// <summary>
        /// Override to get the base damage.
        /// </summary>
        public override double GetBaseDamage()
        {
            return 12.0 + 5 * (base.Level - 1);
        }
        /// <summary>
        /// Override to get the base defense.
        /// </summary>
        public override double GetBaseDefense()
        {
            return 8.0 + 3 * (base.Level - 1);
        }
        /// <summary>
        /// Override to get the base speed.
        /// </summary>
        public override double GetBaseSpeed()
        {
            return 10.0 + 2 * (base.Level - 1);
        }
        /// <summary>
        /// Override to get the base critical rate.
        /// </summary>
        public override double GetBaseCriticalRate()
        {
            return 0.05 + 0.02 * (base.Level - 1);
        }
        /// <summary>
        /// Override to level up the Knight, increasing its stats and learning new skills.
        /// </summary>
        public override void LevelUp()
        {
            base.Level++;
            base.MaxHP += 10;
            base.HP = base.MaxHP;
            base.Damage += 5;
            base.Defense += 3;
            base.Speed += 2;
            base.Mana = base.MaxMana;
            base.CriticalRate += 0.02;
            Exp = Exp - (75 + (Level - 1) * 5);
            LearnSkills();

        }
        /// <summary>
        /// Override method to learn skills based on the Knight's level.
        /// </summary>
        public override void LearnSkills()
        {
            if (Level == 2)
            {
                AttackSkill upDownChop = new AttackSkill(1,"Up-Down Chop","A powerful vertical sword strike that can deal critical damage",15,20, 0, 3, 2,25,0.15);
                base.Skills.Add(upDownChop);
            }

            else if (Level == 3)
            {
                Buff defenseBuff = new Buff(2,"Knight's Stance","Increased defense posture",3,0,15,0,0);
                StatusSkill defenseSetUp = new StatusSkill(2,"Defense Set-Up","Enter a defensive stance that greatly increases defense for 3 turns",20, 0, 0, 4, 3,defenseBuff);
                base.Skills.Add(defenseSetUp);
            }

            else if (Level == 5)
            {
                Buff burnBuff = new Buff(3,"Burning Blade","Applies a burning effect that deals damage over time", 3, 0, -5, 0, 0, BuffType.Negative);
                AttackStatusSkill fireDance = new AttackStatusSkill(3,"Fire Dance","A fiery sword dance that damages enemies and applies a burning effect", 35,30,0,5,5,40,0.2,burnBuff);
                base.Skills.Add(fireDance);
            }
        }
        /// <summary>
        /// Override method to get the class weapon bonus for the Knight, which is specific to swords.
        /// </summary>
        public override (double attackBonus, double critBonus) GetClassWeaponBonus(Weapon weapon)
        {
            if (weapon.WeaponType == WeaponType.Sword)
            {
                double attackBonus = 5.0 * weapon.Tier;
                double critBonus = 0.05 * weapon.Tier;

                return (attackBonus, critBonus);
            }
            return (0, 0);
        }
        /// <summary>
        /// Property to get or set the current animation type.
        /// </summary>
        public AnimationType CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }
    }
}