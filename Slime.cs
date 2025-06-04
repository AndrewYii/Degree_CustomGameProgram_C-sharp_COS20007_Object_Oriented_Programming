using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask
{
    /// <summary>
    /// This is the slime class, inheriting from the Monster class, has extra 17 attributes.
    /// </summary>
    public class Slime : Monster
    {
        private double _corrodeChance;
        private Raylib_cs.Texture2D _idleTexture;
        private Raylib_cs.Texture2D _attackTexture;
        private Raylib_cs.Texture2D _attackSkillTexture;
        private Raylib_cs.Texture2D _deathTexture;
        private int _frame = 0;
        private float _animTime = 0f;
        private const float _frameDuration = 0.2f;
        private const int _idleFrameCount = 6;
        private const int _attackFrameCount = 6;
        private const int _attackSkillFrameCount = 12;
        private const int _deathFrameCount = 4;
        private const int _frameWidth = 100;
        private AnimationType _currentAnimation = AnimationType.Idle;
        /// <summary>
        /// Parameterized constructor for Slime class, that sets the name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive ,expReward ,initializes the corrode chance and loads textures.
        /// </summary>
        public Slime(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int money, int level, int row, int column, bool isAlive, int expReward) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column, isAlive, expReward)
        {
            _corrodeChance = 1;
            LoadTextures();
        }
        /// <summary>
        /// Loads the textures for the Slime animations.
        /// </summary>
        private void LoadTextures()
        {
            _idleTexture = Raylib.LoadTexture("picture/Monster/Slime/Slime-Idle.png");
            _attackTexture = Raylib.LoadTexture("picture/Monster/Slime/Slime-Attack.png");
            _attackSkillTexture = Raylib.LoadTexture("picture/Monster/Slime/Slime-AttackSkill.png");
            _deathTexture = Raylib.LoadTexture("picture/Monster/Slime/Slime-Death.png");
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
        /// Override to set the current animation type and resets the frame and animation time.
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
        /// Override to unload the textures when the Slime instance is no longer needed.
        /// </summary>
        public override void UnloadTextures()
        {
            Raylib.UnloadTexture(_idleTexture);
            Raylib.UnloadTexture(_attackTexture);
            Raylib.UnloadTexture(_attackSkillTexture);
            Raylib.UnloadTexture(_deathTexture);
        }
        /// <summary>
        /// Renders the Slime in battle, handling specific attack animations based on the attack type.
        /// </summary>
        public override void RenderBattle(Vector2 position, float scale, AnimationType battleAnimation, bool showAttackEffect = false, string attackType = "", string skillName = "", float attackProgress = 0f)
        {
            if (showAttackEffect && !string.IsNullOrEmpty(attackType))
            {
                if (attackType == "SlimeAttackSkill")
                {
                    SetAnimation(AnimationType.AttackSkill);
                }
                else if (attackType == "Slime")
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
        /// Override to handle the slime's special action of corroding a player's equipment.
        /// </summary>
        public override string TryAction(Unit target)
        {
            Random random = new Random();
            if (target is not Player player)
            {
                return string.Empty;
            }
            double chance = random.NextDouble();
            double adjustedCorrodeChance = _corrodeChance * (1 + (Level * 0.05));
            adjustedCorrodeChance -= (player.Defense * 0.005);
            //adjustedCorrodeChance = Math.Max(0.05, Math.Min(0.9, adjustedCorrodeChance));
            if (chance < adjustedCorrodeChance)
            {
                if (player.Inventory != null && player.Inventory.GetEquippedItems().Count > 0)
                {
                    List<Equipment> equippedItems = player.Inventory.GetEquippedItems();
                    int itemIndex = random.Next(equippedItems.Count);
                    Equipment targetItem = equippedItems[itemIndex];
                    int additionalReduction = random.Next(10, 26);
                    int totalReduction = 5 + additionalReduction; 
                    int originalDurability = targetItem.Durability;
                    targetItem.Durability = Math.Max(0, targetItem.Durability - totalReduction);
                    if (targetItem.Durability == 0)
                    {
                        player.Inventory.RemoveItem(targetItem); 
                    }
                    using (StreamWriter writer = new StreamWriter("log/slime_corrodes.txt", true))
                    {
                        writer.WriteLine(DateTime.Now + ": Slime '" + Name + "' (Level " + Level + ") corroded " + targetItem.Name + " from " + target.Name);
                        writer.WriteLine("Corrode chance: " + adjustedCorrodeChance.ToString("P2") + " (Base: " + _corrodeChance.ToString("P2") + ")");
                        writer.WriteLine("Additional durability reduction: " + additionalReduction + " (beyond normal 5)");
                        writer.WriteLine("Total reduction: " + originalDurability + " -> " + targetItem.Durability + " (-" + totalReduction + ")");
                        if (targetItem.Durability == 0)
                        {
                            writer.WriteLine("Item dissolved completely!");
                        }
                        writer.WriteLine();
                    }
                }
                return "Slime successfully corroded equipment";
            }
            else
            {
                if (player.Inventory != null && player.Inventory.GetEquippedItems().Count > 0)
                {
                    List<Equipment> equippedItems = player.Inventory.GetEquippedItems();
                    int itemIndex = random.Next(equippedItems.Count);
                    Equipment targetItem = equippedItems[itemIndex];
                    int originalDurability = targetItem.Durability;
                    targetItem.Durability = Math.Max(0, targetItem.Durability - 5);
                    if (targetItem.Durability == 0)
                    {
                        player.Inventory.RemoveItem(targetItem);
                    }
                }
                return string.Empty;
            }
        }
    }
}