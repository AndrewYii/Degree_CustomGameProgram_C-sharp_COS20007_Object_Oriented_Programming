using System;
using Raylib_cs;

namespace DistinctionTask{
    /// <summary>
    /// This is the Archer class, which inherits from the Player class and has extra 16 attributes.
    /// </summary>
    public class Archer : Player{
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _walkTexture;
        private Raylib_cs.Texture2D _attackTexture;
        private Raylib_cs.Texture2D _attackStatueSkillTexture;
        private Raylib_cs.Texture2D _deathTexture;
        private Raylib_cs.Texture2D _profileTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.1f;
        private const int _idleFrameCount = 6;
        private const int _walkFrameCount = 8;
        private const int _attackFrameCount = 9;
        private const int _attackStatueSkillFrameCount = 12;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for the Archer class that set name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive and has function to load textures.
        /// </summary>
        public Archer(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
            LoadTextures();
        }
        /// <summary>
        /// Override method to load textures for the Archer.
        /// </summary>
        public override void LoadTextures()
        {
            _profileTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-profile.png");
            _idleTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-Idle.png");
            _walkTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-Walk.png");
            _attackTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-Attack.png");
            _attackStatueSkillTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-AttackStatueSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Player/Archer/Archer-Death.png");
        }
        /// <summary>
        /// Override method to set up the starting inventory for the Archer which includes the basic equipments, items (handled by virtual method) and an extra bow
        /// </summary>
        public override void SetupStartingInventory()
        {
            base.SetupStartingInventory();
            Weapon archerBow = Item.CreateWeaponFromData(WeaponType.Bow,1,101,"Bow","A precise bow for a skilled archer",25);
            base.Inventory.AddItem(archerBow);
        }
        /// <summary>
        /// Override method to update the Archer's animation based on the current state.
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
        public override Raylib_cs.Texture2D GetCurrentTexture(){
            switch (_currentAnimation){
                case AnimationType.Idle:
                    return _idleTexture;
                case AnimationType.Walk:
                    return _walkTexture;
                case AnimationType.Attack:
                    return _attackTexture;
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
        /// Override method to get the profile texture for the Archer.
        /// </summary>
        public override Raylib_cs.Texture2D GetProfileTexture()
        {
            return _profileTexture;
        }
        /// <summary>
        /// Override method to set the current animation for the Archer.
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
        /// Override method to unload all textures used by the Archer.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_profileTexture);
            Raylib.UnloadTexture(_idleTexture);
            Raylib.UnloadTexture(_walkTexture);
            Raylib.UnloadTexture(_attackTexture);
            Raylib.UnloadTexture(_attackStatueSkillTexture);
            Raylib.UnloadTexture(_deathTexture);
        }
        /// <summary>
        /// Override method to get the base HP for the Archer.
        /// </summary>
        public override double GetBaseHP()
        {
            return 90.0 + (base.Level - 1) * 8;
        }
        /// <summary>
        /// Override method to get the base damage for the Archer.
        /// </summary>
        public override double GetBaseDamage()
        {
            return 15.0 + (base.Level - 1) * 5;
        }
        /// <summary>
        /// Override method to get the base defense for the Archer.
        /// </summary>
        public override double GetBaseDefense()
        {
            return 4.0 + (base.Level - 1) * 2;
        }
        /// <summary>
        /// Override method to get the base speed for the Archer.
        /// </summary>
        public override double GetBaseSpeed()
        {
            return 12.0 + (base.Level - 1) * 3;
        }
        /// <summary>
        /// Override method to get the base critical rate for the Archer.
        /// </summary>
        public override double GetBaseCriticalRate()
        {
            return 0.12 + (base.Level - 1) * 0.025;
        }
        /// <summary>
        /// Override method to handle leveling up the Archer, increasing stats and learning new skills.
        /// </summary>
        public override void LevelUp()
        {
            base.Level++;
            base.MaxHP += 8;
            base.HP = base.MaxHP;
            base.Damage += 5;
            base.Defense += 2;
            base.Speed += 3;
            base.CriticalRate += 0.025;
            base.Mana = base.MaxMana;
            base.Exp = base.Exp - (75 + (Level - 1) * 5);
            LearnSkills();
        }
        /// <summary>
        /// Override method to learn new skills based on the Archer's level.
        /// </summary>
        public override void LearnSkills()
        {
            if (Level == 3)
            {
                Buff precisionBuff = new Buff(4,"Eagle Eye","Enhanced focus increases attack and critical hit chance",2,10,0,5,0.1);

                AttackStatusSkill focusShoot = new AttackStatusSkill(4,"Focus Shoot","A precisely aimed shot that deals high damage and enhances the archer's focus",25,30,0,4,3,35,0.3,precisionBuff);
                base.Skills.Add(focusShoot);
            }
        }
        /// <summary>
        /// Override method to get the class weapon bonus for the Archer based on the weapon type.
        /// </summary>
        public override (double attackBonus, double critBonus) GetClassWeaponBonus(Weapon weapon)
        {
            if (weapon.WeaponType == WeaponType.Bow)
            {
                double attackBonus = 4.0 * weapon.Tier;
                double critBonus = 0.15 * weapon.Tier;
                return (attackBonus, critBonus);
            }
            return (0, 0);
        }
        /// <summary>
        /// Override method to calculate the damage dealt by the Archer to a target unit.
        /// </summary>
        public override double CalculateDamage(Unit target)
        {
            double totalAttack = Damage;
            double critModifier = CriticalRate;

            double damage = totalAttack * (100 / (100 + target.Defense));
            bool isUltimateCritical = new Random().NextDouble() + 0.1 < critModifier;
            bool isCritical = new Random().NextDouble() < critModifier;
            if (isUltimateCritical)
            {
                damage *= 3;
            }
            else if (isCritical)
            {
                damage *= 2;
            }

            return Math.Max(1, damage);
        }
        /// <summary>
        /// Property method to get or set the current animation type for the Archer.
        /// </summary>
        public AnimationType CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }
    }
}