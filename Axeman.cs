using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Axeman class, which inherits from the Player class and has extra 16 attributes.
    /// </summary>
    public class Axeman : Player
    {
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _walkTexture;
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
        private const int _attackFrameCount = 9;
        private const int _attackSkillFrameCount = 9;
        private const int _attackStatueSkillFrameCount = 12;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for the Axeman class that set the name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column , isAlive and load the textures.
        /// </summary>
        public Axeman(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
            LoadTextures();
        }
        /// <summary>
        /// Override method to load texture for the Axeman.
        /// </summary>
        public override void LoadTextures()
        {
            _profileTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-profile.png");
            _idleTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-Idle.png");
            _walkTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-Walk.png");
            _attackTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-Attack.png");
            _attackSkillTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-AttackSkill.png");
            _attackStatueSkillTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-AttackStatueSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Player/Axeman/Axeman-Death.png");
        }
        /// <summary>
        /// Override method to set up the starting inventory for the Axeman which includes the basic equipments, items (handled by virtual method) and an extra axe.
        /// </summary>
        public override void SetupStartingInventory()
        {
            base.SetupStartingInventory();
            Weapon axemanAxe = Item.CreateWeaponFromData(WeaponType.Axe,1,102,"Axe","A powerful axe for a mighty axeman",25);
            base.Inventory.AddItem(axemanAxe);
        }
        /// <summary>
        /// Override method to update the Axeman's animation based on the current state.
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
        /// Private method to get the current frame count based on the animation type.
        /// </summary>
        private int GetCurrentFrameCount()
        {
            switch (_currentAnimation)
            {
                case AnimationType.Idle:
                    return _idleFrameCount;
                case AnimationType.Walk:
                    return _walkFrameCount;
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
        /// Override method to get the current texture based on the animation type.
        /// </summary>
        public override Raylib_cs.Texture2D GetCurrentTexture()
        {
            switch (_currentAnimation)
            {
                case AnimationType.Idle:
                    return _idleTexture;
                case AnimationType.Walk:
                    return _walkTexture;
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
        /// Override method to get the source rectangle for the current animation frame.
        /// </summary>
        public override Raylib_cs.Rectangle GetSourceRectangle()
        {
            return new Raylib_cs.Rectangle(_frame * _frameWidth, 0, _frameWidth, GetCurrentTexture().Height);
        }
        /// <summary>
        /// Override method to get the profile texture for the Axeman.
        /// </summary>        
        public override Raylib_cs.Texture2D GetProfileTexture()
        {
            return _profileTexture;
        }
        /// <summary>
        /// Override method to set the current animation for the Axeman.
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
        /// Override method to unload the textures for the Axeman.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_profileTexture);
            Raylib.UnloadTexture(_idleTexture);
            Raylib.UnloadTexture(_walkTexture);
            Raylib.UnloadTexture(_attackTexture);
            Raylib.UnloadTexture(_attackSkillTexture);
            Raylib.UnloadTexture(_attackStatueSkillTexture);
            Raylib.UnloadTexture(_deathTexture);
        }
        /// <summary>
        /// Override method to get the base attributes for the Axeman.
        /// </summary>
        public override double GetBaseHP()
        {
            return 130.0 + (base.Level - 1) * 12;
        }
        /// <summary>
        /// Override method to get the base damage for the Axeman.
        /// </summary>
        public override double GetBaseDamage()
        {
            return 18.0 + (base.Level - 1) * 6;
        }
        /// <summary>
        /// Override method to get the base defense for the Axeman.
        /// </summary>
        public override double GetBaseDefense()
        {
            return 9.0 + (base.Level - 1) * 4;
        }
        /// <summary>
        /// Override method to get the base speed for the Axeman.
        /// </summary>
        public override double GetBaseSpeed()
        {
            return 8.0 + (base.Level - 1) * 2;
        }
        /// <summary>
        /// Override method to get the base critical rate for the Axeman.
        /// </summary>
        public override double GetBaseCriticalRate()
        {
            return 0.07 + (base.Level - 1) * 0.01;
        }
        /// <summary>
        /// Override method to handle leveling up the Axeman, increasing stats and learning new skills.
        /// </summary>
        public override void LevelUp()
        {
            base.Level++;
            base.MaxHP += 12;
            base.HP = base.MaxHP;
            base.Damage += 6;
            base.Defense += 4;
            base.Speed += 2;
            base.CriticalRate += 0.01;
            base.Mana = base.MaxMana;
            base.Exp = base.Exp - (75 + (Level - 1) * 5);
            LearnSkills();
        }
        /// <summary>
        /// Override method to learn skills specific to the Axeman class based on the current level.
        /// </summary>
        public override void LearnSkills()
        {

            {
                if (Level == 2)
                {
                    AttackSkill roundedChop = new AttackSkill(5, "Rounded Chop","A devastating spinning axe attack that hits with tremendous force",20,25,0,3,2,35,0.1);
                    base.Skills.Add(roundedChop);
                }

                else if (Level == 4)
                {
                    Buff bleedingEffect = new Buff(6,"Deep Wound","Target suffers from a severe wound that causes bleeding",3,-8,0,-5,0,BuffType.Negative);
                    AttackStatusSkill bleedingChop = new AttackStatusSkill(6,"Bleeding Chop","A vicious axe strike that causes a deep wound, bleeding damage, and weakens the target",30,35,0,5,4,40,0.15,bleedingEffect);
                    base.Skills.Add(bleedingChop);
                }
            }
        }
        /// <summary>
        /// Override method to get the class weapon bonus for the Axeman, which is specific to axes.
        /// </summary>
        public override (double attackBonus, double critBonus) GetClassWeaponBonus(Weapon weapon)
        {
            if (weapon.WeaponType == WeaponType.Axe)
            {
                double attackBonus = 10.0 * weapon.Tier;
                double critBonus = 0.02 * weapon.Tier;

                return (attackBonus, critBonus);
            }
            return (0, 0);
        }
        /// <summary>
        /// Property method to get or set the current animation type for the Axeman.
        /// </summary>
        public AnimationType CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }
    }
}