using System;
using Raylib_cs;
using System.Numerics;
using System.IO;

namespace DistinctionTask{
    /// <summary>
    /// This is the Player class, inherits from Unit, it has extra 9 attributes
    /// </summary>
    public abstract class Player : Unit
    {
        private Inventory _inventory;
        private bool _isPlayerMoving = false;
        private float _lastMovementTime = 0f;
        private int _targetRow = 1;
        private int _targetColumn = 1;
        private float _moveStartTime = 0f;
        private float _moveDuration = 0.3f;
        private bool _isMoving = false;
        private const float MOVEMENT_COOLDOWN = 0.5f;
        /// <summary>
        /// Constructor for the Player class which set the name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column ,isAlive attributes and set the inventory.
        /// </summary>
        public Player(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int exp, int level, int row, int column, bool isAlive) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, exp, level, row, column, isAlive)
        {
            _inventory = new Inventory();
            _inventory.SetOwner(this);
        }
        /*
        public static Player CreatePlayerFromSelection(string playerName, string characterRole)
        {
            double baseHP = 100.0;
            double baseDamage = 10.0;
            double baseDefense = 5.0;
            double baseSpeed = 10.0;
            double baseMana = 50.0;
            double baseCriticalRate = 0.05;

            switch (characterRole)
            {
                case "Knight":
                    baseHP = 120.0;
                    baseDamage = 12.0;
                    baseDefense = 8.0;
                    baseSpeed = 10.0;
                    baseMana = 100.0;
                    baseCriticalRate = 0.05;
                    break;

                case "Archer":
                    baseHP = 90.0;
                    baseDamage = 15.0;
                    baseDefense = 4.0;
                    baseSpeed = 12.0;
                    baseMana = 100.0;
                    baseCriticalRate = 0.12;
                    break;

                case "Axeman":
                    baseHP = 130.0;
                    baseDamage = 18.0;
                    baseDefense = 9.0;
                    baseSpeed = 8.0;
                    baseMana = 100.0;
                    baseCriticalRate = 0.07;
                    break;
            }

            Player player;
            switch (characterRole)
            {
                case "Knight":
                    player = new Knight(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, baseMana, 0, 1, 1, 1, true);
                    break;
                case "Archer":
                    player = new Archer(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, baseMana, 0, 1, 1, 1, true);
                    break;
                case "Axeman":
                    player = new Axeman(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, baseMana, 0, 1, 1, 1, true);
                    break;
                default:
                    player = new Axeman(playerName, baseHP, baseHP, baseDamage, baseCriticalRate, baseDefense, baseSpeed, baseMana, 0, 1, 1, 1, true);
                    break;
            }
            return player;
        }
        */
        /// <summary>
        /// Updates the player's stats based on equipped items and buffs.
        /// </summary>
        public void UpdateStats()
        {
            double baseHP = GetBaseHP();
            double baseMana = 100;
            double baseDamage = GetBaseDamage();
            double baseDefense = GetBaseDefense();
            double baseSpeed = GetBaseSpeed();
            double totalBonusHP = 0;
            double totalBonusMana = 0;
            double totalBonusDamage = 0;
            double totalBonusDefense = 0;
            double totalBonusSpeed = 0;
            foreach (Equipment equip in _inventory.GetEquippedItems())
            {
                if (equip is Weapon weapon)
                {
                    totalBonusDamage += weapon.BonusAttack;
                    (double attackBonus, double critBonus) = GetClassWeaponBonus(weapon);
                    totalBonusDamage += attackBonus;
                }
                else if (equip is Armor armor)
                {
                    totalBonusHP += armor.BonusHP;
                    totalBonusDefense += armor.BonusDefense;
                    totalBonusSpeed += armor.BonusSpeed;
                    totalBonusMana += armor.BonusMana;
                }
            }
            double baseCriticalRate = GetBaseCriticalRate();
            CriticalRate = baseCriticalRate;
            foreach (Equipment equip in _inventory.GetEquippedItems())
            {
                if (equip is Weapon weapon)
                {
                    CriticalRate += weapon.BonusCriticalRate;
                    (double _, double critBonus) = GetClassWeaponBonus(weapon);
                    CriticalRate += critBonus;
                }
            }
            double oldMaxHP = base.MaxHP;
            double oldMaxMana = base.MaxMana;
            base.MaxHP = baseHP + totalBonusHP;
            base.MaxMana = baseMana + totalBonusMana;
            base.Damage = baseDamage + totalBonusDamage;
            base.Defense = baseDefense + totalBonusDefense;
            base.Speed = baseSpeed + totalBonusSpeed;
            foreach (Buff buff in Buffs)
            {
                if (buff.CheckExpired()) continue;
                base.Damage += buff.AttackBonus;
                base.Defense += buff.DefenseBonus;
                base.Speed += buff.SpeedBonus;
                base.CriticalRate += buff.CriticalRateBonus;
            }
            if (base.HP == oldMaxHP)
            {
                base.HP = base.MaxHP;
            }
            if (base.Mana == oldMaxMana)
            {
                base.Mana = base.MaxMana;
            }
        }
        /// <summary>
        /// Virtual to get the damage base for the player class.
        /// </summary>
        public virtual double GetBaseDamage()
        {
            return 10.0;
        }
        /// <summary>
        /// Virtual to get the defense base for the player class.
        /// </summary>
        public virtual double GetBaseDefense()
        {
            return 5.0;
        }
        /// <summary>
        /// Virtual to get the speed base for the player class.
        /// </summary>
        public virtual double GetBaseSpeed()
        {
            return 10.0;
        }
        /// <summary>
        /// Virtual to get the base HP for the player class.
        /// </summary>
        public virtual double GetBaseHP()
        {
            return 100.0;
        }
        /// <summary>
        /// Virtual to get the base critical rate for the player class.
        /// </summary>
        public virtual double GetBaseCriticalRate()
        {
            return 0.05;
        }
        /// <summary>
        /// Virtual to get the class-specific weapon bonus for the player class.
        /// </summary>
        public virtual (double attackBonus, double critBonus) GetClassWeaponBonus(Weapon weapon)
        {
            return (0, 0);
        }
        /// <summary>
        /// Gets the required experience points for the next level based on the current level.
        /// </summary>
        private int GetRequiredExpForLevel(int level)
        {
            return 75 + (level - 1) * 5;
        }
        /// <summary>
        /// Adds experience points to the player and checks if they can level up.
        /// </summary>
        public void AddExp(int amount)
        {
            Exp += amount;
            if (Exp >= GetRequiredExpForLevel(Level))
            {
                LevelUp();
            }
        }
        /// <summary>
        /// Levels up the player, increasing their stats and resetting HP.
        /// </summary>
        public virtual void LevelUp()
        {
            Level++;
            base.MaxHP += 10;
            base.HP = base.MaxHP;
            base.Damage += 5;
            base.Defense += 3;
            base.Speed += 2;
            base.CriticalRate += 0.01;
            Exp = Exp - (75 + (Level - 1) * 5);
        }
        /// <summary>
        /// Sets up the starting inventory for the player. Can be overridden by character classes to add role-specific items.
        /// </summary>
        public virtual void SetupStartingInventory()
        {
            _inventory.Money = 100;
            for (int i = 0; i < 3; i++)
            {
                Potion healingPotion = Item.CreatePotionFromData("HEALING", 1, 1 + i, "Level 1 Healing Potion", "Restores HP when used", 10);
                _inventory.AddItem(healingPotion);
            }
            for (int i = 0; i < 5; i++)
            {
                Potion expPotion = Item.CreatePotionFromData("EXPBOOST", 5, 4 + i, "Level 5 EXP Potion", "Gain EXP when used", 50);
                _inventory.AddItem(expPotion);
            }
            for (int i = 0; i < 3; i++)
            {
                Potion manaPotion = Item.CreatePotionFromData("MANA", 5, 9 + i, "Level 5 Mana Potion", "Restores mana when used", 50);
                _inventory.AddItem(manaPotion);
            }
            for (int i = 0; i < 3; i++)
            {
                Potion cooldownPotion = Item.CreatePotionFromData("REDUCECOOLDOWN", 5, 12 + i, "Level 5 ReduceCD Potion", "Reduces skill cooldowns", 50);
                _inventory.AddItem(cooldownPotion);
            }
            Armor startingHelmet = Item.CreateArmorFromData(ArmorType.Helmet, 1, 16, "Level 1 Helmet", "Can be equipped at the helmet slot", 0);
            Armor startingChest = Item.CreateArmorFromData(ArmorType.Chest, 1, 17, "Level 1 Chest", "Can be equipped at the chest slot", 0);
            Armor startingGlove = Item.CreateArmorFromData(ArmorType.Glove, 1, 18, "Level 1 Glove", "Can be equipped at the glove slot", 0);
            Armor startingLeg = Item.CreateArmorFromData(ArmorType.Leg, 1, 19, "Level 1 Leg", "Can be equipped at the leg slot", 0);
            Armor startingBracelet = Item.CreateArmorFromData(ArmorType.Bracelet, 1, 20, "Level 1 Bracelet", "Can be equipped at the bracelet slot", 0);
            Armor startingRing = Item.CreateArmorFromData(ArmorType.Ring, 1, 21, "Level 1 Ring", "Can be equipped at the ring slot", 0);
            _inventory.AddItem(startingHelmet);
            _inventory.AddItem(startingGlove);
            _inventory.AddItem(startingChest);
            _inventory.AddItem(startingLeg);
            _inventory.AddItem(startingBracelet);
            _inventory.AddItem(startingRing);
            UpdateStats();
        }
        /// <summary>
        /// Method to handle the player attacking a monster.
        /// </summary>
        public double Attack(Monster target)
        {
            if (!IsAlive || !target.IsAlive)
            {
                return 0;
            }
            double damage = CalculateDamage(target);
            target.TakeDamage(damage);
            if (!target.IsAlive)
            {
                Console.WriteLine("" + target.Name + " was defeated by " + Name + "!");
            }
            return (int)damage;
        }
        /// <summary>
        /// Method to update the cooldowns of the player's skills.
        /// </summary>
        public void UpdateSkillCooldowns()
        {
            foreach (Skill skill in Skills)
            {
                if (skill.Cooldown > 0)
                {
                    skill.Cooldown -= 1;
                }
            }
        }
        /// <summary>
        /// Calculates the damage dealt to a target unit based on the player's stats and the target's defense.
        /// </summary>
        public virtual double CalculateDamage(Unit target)
        {
            double totalAttack = Damage;
            double critModifier = CriticalRate;
            double damage = totalAttack * (100 / (100 + target.Defense));
            bool isCritical = new Random().NextDouble() < critModifier;
            if (isCritical)
            {
                damage *= 2;
            }
            return Math.Max(1, damage);
        }
        /// <summary>
        /// Abstract methods to update the animation.
        /// </summary>
        public abstract void UpdateAnimation();
        /// <summary>
        /// Abstract methods to get the current texture.
        /// </summary>
        public abstract Raylib_cs.Texture2D GetCurrentTexture();
        /// <summary>
        /// Abstract methods to get the source rectangle for the texture.
        /// </summary>
        public abstract Raylib_cs.Rectangle GetSourceRectangle();
        /// <summary>
        /// Abstract methods to get the profile texture.
        /// </summary>
        public abstract Raylib_cs.Texture2D GetProfileTexture();
        /// <summary>
        /// Abstract methods to set the animation type.
        /// </summary>
        public abstract void SetAnimation(AnimationType animation);
        /// <summary>
        /// Abstract methods to load textures.
        /// </summary>
        public abstract void LoadTextures();
        /// <summary>
        /// Abstract methods to unload textures.
        /// </summary>
        public abstract void UnloadTextures();
        /// <summary>
        /// Abstract methods to learn skills.
        /// </summary>
        public abstract void LearnSkills();
        /// <summary>
        /// Renders the player on the screen at a specified position and scale.
        /// </summary>
        public virtual void Render(Vector2 position, float scale, bool isMoving = false)
        {
            if (isMoving)
            {
                SetAnimation(AnimationType.Walk);
            }
            else
            {
                SetAnimation(AnimationType.Idle);
            }
            UpdateAnimation();
            Raylib_cs.Texture2D texture = GetCurrentTexture();
            Raylib_cs.Rectangle sourceRect = GetSourceRectangle();
            Raylib_cs.Rectangle destRect = new Raylib_cs.Rectangle(position.X - (sourceRect.Width * scale) / 2, position.Y - (sourceRect.Height * scale) / 2, sourceRect.Width * scale, sourceRect.Height * scale);
            Raylib.DrawTexturePro(texture, sourceRect, destRect, new Vector2(0, 0), 0, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Renders the player's battle animation at a specified position and scale.
        /// </summary>
        public virtual void RenderBattle(Vector2 position, float scale, AnimationType battleAnimation, bool showAttackEffect = false, float attackProgress = 0f)
        {
            SetAnimation(battleAnimation);
            UpdateAnimation();
            Raylib_cs.Texture2D texture = GetCurrentTexture();
            Raylib_cs.Rectangle sourceRect = GetSourceRectangle();
            Vector2 adjustedPosition = position;
            if (showAttackEffect && attackProgress > 0)
            {
                float moveFactor = attackProgress < 0.5f ? attackProgress * 2 : (1 - attackProgress) * 2;
                float moveDistance;
                switch (this)
                {
                    case Knight:
                        moveDistance = 600f;
                        break;
                    case Archer:
                        moveDistance = 200f;
                        break;
                    case Axeman:
                        moveDistance = 600f;
                        break;
                    default:
                        moveDistance = 400f;
                        break;
                }
                adjustedPosition.X += moveDistance * moveFactor;
            }
            Raylib_cs.Rectangle destRect = new Raylib_cs.Rectangle(adjustedPosition.X - (sourceRect.Width * scale) / 2, adjustedPosition.Y - (sourceRect.Height * scale) / 2, sourceRect.Width * scale, sourceRect.Height * scale);
            Raylib.DrawTexturePro(texture, sourceRect, destRect, new Vector2(0, 0), 0, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Renders the escape animation for the player at a specified position and scale.
        /// </summary>
        public virtual void RenderEscape(Vector2 position, float scale, float escapeProgress, bool isSuccessful)
        {
            SetAnimation(AnimationType.Walk);
            UpdateAnimation();
            Raylib_cs.Texture2D texture = GetCurrentTexture();
            Raylib_cs.Rectangle sourceRect = GetSourceRectangle();
            Raylib_cs.Rectangle flippedSourceRect = new Raylib_cs.Rectangle(sourceRect.X + sourceRect.Width, sourceRect.Y, -sourceRect.Width, sourceRect.Height);
            float escapeOffset = isSuccessful ? -300 * escapeProgress : 0;
            Vector2 adjustedPosition = new Vector2(position.X + escapeOffset, position.Y);
            Raylib_cs.Rectangle destRect = new Raylib_cs.Rectangle(adjustedPosition.X - (sourceRect.Width * scale) / 2, adjustedPosition.Y - (sourceRect.Height * scale) / 2, sourceRect.Width * scale, sourceRect.Height * scale);
            Raylib.DrawTexturePro(texture, flippedSourceRect, destRect, new Vector2(0, 0), 0, Raylib_cs.Color.White);
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                int particleX = (int)(position.X + rand.Next(-50, 50));
                int particleY = (int)(position.Y + rand.Next(-30, 30));
                int particleSize = rand.Next(3, 10);
                Raylib.DrawCircle(particleX, particleY, particleSize, new Raylib_cs.Color(200, 200, 200, (int)(255 * (1.0f - escapeProgress))));
            }
        }
        /// <summary>
        /// Starts the movement of the player to a target position on the map.
        /// /// </summary>
        public void StartMovement(int targetRow, int targetColumn, float duration = 0.3f)
        {
            _targetRow = targetRow;
            _targetColumn = targetColumn;
            _isMoving = true;
            _isPlayerMoving = true;
            _moveStartTime = (float)Raylib.GetTime();
            _moveDuration = duration;
        }
        /// <summary>
        /// Updates the player's movement state, checking if the movement is complete and resetting the position if necessary.
        /// </summary>
        public void UpdateMovement()
        {
            if (_isMoving)
            {
                float currentTime = (float)Raylib.GetTime();
                float elapsedTime = currentTime - _moveStartTime;
                float moveProgress = elapsedTime / _moveDuration;

                if (moveProgress >= 1.0f)
                {
                    Row = _targetRow;
                    Column = _targetColumn;
                    _isMoving = false;
                    _lastMovementTime = currentTime;
                    _isPlayerMoving = true;
                }
            }

            if (_isPlayerMoving && !_isMoving && Raylib.GetTime() - _lastMovementTime > MOVEMENT_COOLDOWN)
            {
                _isPlayerMoving = false;
            }
        }
        /// <summary>
        /// Gets the interpolated position of the player on the map based on the current row, column, and movement state.
        /// </summary>
        public (int x, int y) GetInterpolatedPosition(int mapOffsetX, int mapOffsetY, int gridSize)
        {
            if (!_isMoving)
            {
                return (mapOffsetX + (Column * gridSize), mapOffsetY + (Row * gridSize));
            }
            float currentTime = (float)Raylib.GetTime();
            float elapsedTime = currentTime - _moveStartTime;
            float moveProgress = Math.Min(1.0f, elapsedTime / _moveDuration);
            float easedProgress = EaseInOutQuad(moveProgress);
            int startX = mapOffsetX + (Column * gridSize);
            int startY = mapOffsetY + (Row * gridSize);
            int targetX = mapOffsetX + (_targetColumn * gridSize);
            int targetY = mapOffsetY + (_targetRow * gridSize);
            int playerMapX = (int)Lerp(startX, targetX, easedProgress);
            int playerMapY = (int)Lerp(startY, targetY, easedProgress);
            return (playerMapX, playerMapY);
        }
        /// <summary>
        /// Eases the movement progress using an ease-in-out quadratic function.
        /// /// </summary>
        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }
        /// <summary>
        /// Linearly interpolates between two values based on a given progress factor.
        /// </summary>
        private float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }
        /// <summary>
        /// Gets or sets whether the player is currently moving in battle.
        /// /// </summary>
        public bool IsPlayerMoving
        {
            get { return _isPlayerMoving; }
            set { _isPlayerMoving = value; }
        }
        /// <summary>
        /// Gets or sets whether the player is currently moving in map.
        /// /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }
        /// <summary>
        /// Gets the inventory of the player.
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }
        }
    }
}