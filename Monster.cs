using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace DistinctionTask
{
    /// <summary>
    /// This is abstract monster class that inherits from Unit, it has extra 14 attributes.
    /// </summary>
    public abstract class Monster : Unit
    {
        private List<Item> _dropList;
        private int _expReward;
        private int _moneyReward;
        public const int BATTLE_BUTTON_WIDTH = 200;
        public const int BATTLE_BUTTON_HEIGHT = 80;
        public const int BATTLE_BUTTON_SPACING = 40;
        public const int BATTLE_LOG_HEIGHT = 125;
        public const int ITEMS_PANEL_WIDTH = 800;
        public const int ITEMS_PANEL_HEIGHT = 600;
        public const int SKILL_BUTTON_HEIGHT = 80;
        public const int SKILL_SPACING = 100;
        public const int ITEM_BUTTON_HEIGHT = 80;
        public const int ITEM_SPACING = 100;
        public const int VISIBLE_ITEMS = 5;
        /// <summary>
        /// Parameterized constructor for Monster class that set name, HP, maxHP, attack, criticalRate, defense, speed, mana, money, level, row, column and isAlive.
        /// </summary>
        public Monster(string name, double HP, double maxHP, double attack, double criticalRate, double defense, double speed, double mana, int money, int level, int row, int column, bool isAlive, int expReward) : base(name, HP, maxHP, attack, criticalRate, defense, speed, mana, 0, level, row, column, isAlive)
        {
            _expReward = expReward;
            _moneyReward = money;
            _dropList = new List<Item>();
        }
        /// <summary>
        /// Abstract method to update the monster's animation.
        /// </summary>
        public abstract void UpdateAnimation();
        /// <summary>
        /// Abstract method to get the current texture of the monster.
        /// </summary>
        public abstract Raylib_cs.Texture2D GetCurrentTexture();
        /// <summary>
        /// Abstract method to get the source rectangle of the monster's texture.
        /// </summary>
        public abstract Raylib_cs.Rectangle GetSourceRectangle();
        /// <summary>
        /// Abstract method to set the animation type for the monster.
        /// </summary>
        public abstract void SetAnimation(AnimationType animation);
        /// <summary>
        /// Abstract method to load textures for the monster.
        /// </summary>
        public abstract void UnloadTextures();
        /// <summary>
        /// Abstract method to try an action on a target unit.
        /// </summary>
        public abstract string TryAction(Unit target);
        /// <summary>
        /// Adds items to the monster's drop list based on its level.
        /// </summary>
        public void AddItemsToDropList(int monsterLevel)
        {
            Random random = new Random();
            bool isBoss = base.Name.Contains("Boss");
            if (isBoss)
            {
                return;
            }
            double potionChance = 0.3;
            double weaponChance = 0.15;
            double armorChance = 0.15;
            if (random.NextDouble() < potionChance)
            {
                int potionTier = Math.Max(1, monsterLevel / 3);
                string[] potionTypes = { "HEALING", "MANA", "EXPBOOST", "REDUCECOOLDOWN" };
                string potionType = potionTypes[random.Next(potionTypes.Length)];
                string potionName;
                string description;
                switch (potionType)
                {
                    case "HEALING":
                        potionName = "Healing Potion";
                        description = "Restores health when used";
                        break;
                    case "MANA":
                        potionName = "Mana Potion";
                        description = "Restores mana when used";
                        break;
                    case "EXPBOOST":
                        potionName = "Exp Potion";
                        description = "Grants additional experience";
                        break;
                    case "REDUCECOOLDOWN":
                        potionName = "ReduceCD Potion";
                        description = "Reduces skill cooldowns";
                        break;
                    default:
                        potionName = "Mystery Potion";
                        description = "Effect unknown";
                        break;
                }
                Item potion = Item.CreatePotionFromData(potionType,potionTier,100 + random.Next(10),potionName,description,10 + (potionTier * 5));
                if (potion != null)
                {
                    _dropList.Add(potion);
                }
            }
            if (random.NextDouble() < weaponChance)
            {
                int equipTier = Math.Max(1, monsterLevel / 3);
                WeaponType weaponType = (WeaponType)random.Next(3);
                string weaponTypeName = weaponType.ToString();
                Weapon weapon = Item.CreateWeaponFromData(weaponType,equipTier,200 + random.Next(10),weaponTypeName,"A " + weaponTypeName.ToLower() + " dropped by a monster",15 + (equipTier * 8));
                _dropList.Add(weapon);
            }
            if (random.NextDouble() < armorChance)
            {
                int equipTier = Math.Max(1, monsterLevel / 3);
                ArmorType armorType = (ArmorType)random.Next(6);
                string armorTypeName = armorType.ToString();
                Armor armor = Item.CreateArmorFromData(armorType,equipTier,300 + random.Next(10),armorTypeName,"A " + armorTypeName.ToLower() + " dropped by a monster",12 + (equipTier * 7));
                _dropList.Add(armor);
            }
        }
        /// <summary>
        /// Renders the monster in battle based on the context (tilemap or battle).
        /// </summary>
        public virtual void RenderBattle(Vector2 position, AnimationType battleAnimation, string context = "battle", bool showAttackEffect = false, string attackType = "", string skillName = "", float attackProgress = 0f)
        {
            float scale;
            switch (context)
            {
                case "tilemap":
                    scale = 2.5f;
                    break;
                case "battle":
                    scale = 8.0f;
                    break;
                default:
                    scale = 3.5f;
                    break;
            }
            RenderBattle(position, scale, battleAnimation, showAttackEffect, attackType, skillName, attackProgress);
        }
        /// <summary>
        /// Renders the monster in battle with specified position, scale, animation type, and attack effects.
        /// </summary>
        public virtual void RenderBattle(Vector2 position, float scale, AnimationType battleAnimation, bool showAttackEffect = false, string attackType = "", string skillName = "", float attackProgress = 0f)
        {
            SetAnimation(battleAnimation);
            UpdateAnimation();
            Raylib_cs.Texture2D texture = GetCurrentTexture();
            Raylib_cs.Rectangle sourceRect = GetSourceRectangle();
            Vector2 adjustedPosition = position;
            if (showAttackEffect && attackProgress > 0)
            {
                float moveFactor = attackProgress < 0.5f ? attackProgress * 2 : (1 - attackProgress) * 2;
                adjustedPosition.X -= 600 * moveFactor; // Move left toward player
            }
            Raylib_cs.Rectangle flippedSourceRect = new Raylib_cs.Rectangle(sourceRect.X + sourceRect.Width,sourceRect.Y,-sourceRect.Width,sourceRect.Height);
            Raylib_cs.Rectangle destRect = new Raylib_cs.Rectangle(adjustedPosition.X - (sourceRect.Width * scale) / 2,adjustedPosition.Y - (sourceRect.Height * scale) / 2,sourceRect.Width * scale,sourceRect.Height * scale);
            Raylib.DrawTexturePro(texture,flippedSourceRect,destRect,new Vector2(0, 0), 0, Raylib_cs.Color.White );
        }
        /// <summary>
        /// Uses a skill on a target player, applying damage if the skill is known.
        /// </summary>
        public double UseSkill(Skill skill, Player target)
        {
            if (!IsAlive || !target.IsAlive)
            {
                return 0;
            }
            if (Skills.Contains(skill))
            {
                skill.Used(target);
                return skill.Damage;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Attacks a target player, calculating damage based on the monster's stats and the target's defense.
        /// </summary>
        public double Attack(Player target)
        {
            if (!IsAlive || !target.IsAlive)
            {
                return 0;
            }
            double damage = CalculateDamage(target);
            target.TakeDamage(damage);
            return (int)damage;
        }
        /// <summary>
        ///  Handles the monster's death, setting its HP to 0 and marking it as not alive.
        /// </summary>
        public override void Die()
        {
            if (IsAlive)
            {
                IsAlive = false;
                HP = 0;
            }
        }
        /// <summary>
        /// Calculates the damage dealt to a target based on the monster's attack value and the target's defense.
        /// </summary>
        private double CalculateDamage(Unit target)
        {
            double attackVal = base.Damage;
            double damage = attackVal * (100 / (100 + target.Defense));
            Random random = new Random();
            bool isCritical = random.NextDouble() < CriticalRate;
            if (isCritical)
            {
                damage *= 1.5;
            }
            double variation = 0.9 + (random.NextDouble() * 0.2);
            damage *= variation;
            return Math.Max(1, Math.Round(damage));
        }
        /// <summary>
        /// Generates a monster based on the provided selection, level, prefix, difficulty, row, and column.
        /// </summary>
        public static Monster? GenerateRandomMonster(int stageLevel, Difficulty difficulty, int row, int column)
        {
            try
            {
                Random random = new Random();
                string[] monsterTypes = { "Goblin", "Slime", "Skeleton" };
                string monsterName = monsterTypes[random.Next(monsterTypes.Length)];
                int minLevel = Math.Max(1, stageLevel - 1);
                int maxLevel = stageLevel + 1;
                int level = random.Next(minLevel, maxLevel + 1);
                double prefixChance = 0.3 + (stageLevel * 0.1); 
                string prefix = "";
                if (random.NextDouble() < prefixChance)
                {
                    string[] prefixes = { "Feral", "Dark", "Savage", "Venomous", "Shadow", "Cursed", "Ancient", "Giant" };
                    prefix = prefixes[random.Next(prefixes.Length)];                }
                return MonsterFactory.CreateMonsterFromSelection(monsterName, level, prefix, difficulty, row, column);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Creates a monster based on the provided selection, level, prefix, difficulty, row, and column.
        /// </summary>
        public static List<Monster> GenerateStageMonsters(int stageLevel, Difficulty difficulty, int monsterCount, bool includeBoss)
        {
            List<Monster> monsters = new List<Monster>();
            for (int i = 0; i < monsterCount; i++)
            {
                int row = i / 4;  
                int column = i % 4;
                Monster? monster = GenerateRandomMonster(stageLevel, difficulty, row, column);
                if (monster != null)
                {
                    monsters.Add(monster);
                }
            }
            return monsters;
        }
        /// <summary>
        /// Creates the battle UI for the monster, including the arena name, turn indicator, battle log, and action buttons.
        /// </summary>
        public static void DrawBattleUI(Monster monster,int windowWidth,int windowHeight,int currentLevel,bool isPlayerTurn,List<string> battleLog){
            string arenaName = "Dungeon Battle - Level " + currentLevel.ToString();
            int titleFontSize = 36;
            int titleWidth = Raylib.MeasureText(arenaName, titleFontSize);
            Raylib.DrawRectangle(windowWidth / 2 - titleWidth / 2 - 20,15,titleWidth + 40,50,new Raylib_cs.Color(40, 40, 70, 200) );
            DrawPixelBorder(windowWidth / 2 - titleWidth / 2 - 20,15, titleWidth + 40, 50,new Raylib_cs.Color(100, 100, 255, 255), new Raylib_cs.Color(60, 60, 150, 255), 4 );
            DrawTextWithGlow(arenaName,windowWidth / 2 - titleWidth / 2,20, titleFontSize, Raylib_cs.Color.White, new Raylib_cs.Color(100, 100, 255, 100)  );
            string turnIndicator = isPlayerTurn ? "YOUR TURN" : $"{monster.Name}'S TURN";
            DrawTextWithGlow(turnIndicator, windowWidth / 2 - 100, 80, 30,isPlayerTurn ? Raylib_cs.Color.Green : Raylib_cs.Color.Red, new Raylib_cs.Color(100, 100, 100, 100));
            Rectangle battleLogPanel = new Rectangle(windowWidth / 2 - 400, windowHeight / 2 + 210, 800,BATTLE_LOG_HEIGHT );
            Raylib.DrawRectangleRec( battleLogPanel,new Raylib_cs.Color(20, 20, 40, 200));
            DrawPixelBorder((int)battleLogPanel.X, (int)battleLogPanel.Y, (int)battleLogPanel.Width,(int)battleLogPanel.Height, new Raylib_cs.Color(80, 80, 150, 255),  new Raylib_cs.Color(40, 40, 80, 255), 4 );
            if (battleLog.Count > 0)
            {
                int logY = (int)battleLogPanel.Y + 20;
                int lineHeight = 25;
                for (int i = Math.Max(0, battleLog.Count - 3); i < battleLog.Count; i++)
                {
                    Raylib.DrawText(battleLog[i], (int)battleLogPanel.X + 20, logY, 20,Raylib_cs.Color.White);logY += lineHeight;
                }
            }
            else
            {
                Raylib.DrawText( "Battle has begun!",(int)battleLogPanel.X + 20,(int)battleLogPanel.Y + 20, 20, Raylib_cs.Color.White);
            }           
            int buttonWidth = BATTLE_BUTTON_WIDTH;
            int buttonHeight = BATTLE_BUTTON_HEIGHT;
            int centerX = windowWidth / 2;
            int centerY = windowHeight / 2 + 460; 
            int spacing = BATTLE_BUTTON_SPACING;
            Rectangle attackButton = new Rectangle(centerX - buttonWidth - spacing / 2, centerY - buttonHeight - spacing / 2, buttonWidth, buttonHeight );
            Rectangle skillButton = new Rectangle(centerX + spacing / 2, centerY - buttonHeight - spacing / 2, buttonWidth, buttonHeight);
            Rectangle itemButton = new Rectangle(centerX - buttonWidth - spacing / 2, centerY + spacing / 2, buttonWidth, buttonHeight);
            Rectangle escapeButton = new Rectangle(centerX + spacing / 2, centerY + spacing / 2, buttonWidth, buttonHeight);
            bool attackHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), attackButton);
            bool skillHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), skillButton);
            bool itemHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), itemButton);
            bool escapeHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), escapeButton);
            DrawBattleButtons(attackButton, skillButton, itemButton, escapeButton,attackHovered, skillHovered, itemHovered, escapeHovered);
        }
        /// <summary>
        /// Draws the battle buttons with pixel art style, including attack, skill, item, and escape buttons.
        /// </summary>
        private static void DrawBattleButtons(Rectangle attackButton, Rectangle skillButton, Rectangle itemButton, Rectangle escapeButton,bool attackHovered, bool skillHovered, bool itemHovered, bool escapeHovered){
            DrawPixelArtButton( (int)attackButton.X,(int)attackButton.Y,(int)attackButton.Width,(int)attackButton.Height, "ATTACK", new Raylib_cs.Color(200, 60, 60, 255), new Raylib_cs.Color(150, 40, 40, 255), attackHovered,() =>
                {
                    int iconSize = 32;
                    int iconX = (int)attackButton.X + 20;
                    int iconY = (int)attackButton.Y + (int)attackButton.Height / 2 - iconSize / 2;
                    Raylib_cs.Color swordColor = attackHovered ? new Raylib_cs.Color(220, 200, 130, 255) : new Raylib_cs.Color(255, 230, 150, 255);
                    Raylib.DrawRectangle(iconX + 12, iconY + 20, 8, 10,attackHovered ? Raylib_cs.Color.DarkBrown : Raylib_cs.Color.Orange);
                    Raylib.DrawRectangle(iconX + 8, iconY + 18, 16, 4,attackHovered ? Raylib_cs.Color.Gray : Raylib_cs.Color.LightGray);
                    for (int i = 0; i < 18; i++)
                    {
                        int width = 12 - i / 3;
                        Raylib.DrawRectangle(iconX + 10 + i / 3,iconY + 17 - i,width,2,swordColor);
                    }
                    Raylib.DrawRectangle(iconX + 12,iconY + 2,2,14,attackHovered ? new Raylib_cs.Color(220, 220, 220, 180) : Raylib_cs.Color.White);
                }
            );
            DrawPixelArtButton((int)skillButton.X,(int)skillButton.Y,(int)skillButton.Width,(int)skillButton.Height,"SKILL",new Raylib_cs.Color(60, 60, 220, 255), new Raylib_cs.Color(40, 40, 160, 255),  skillHovered,() =>
                {
                    int iconSize = 32;
                    int iconX = (int)skillButton.X + 20;
                    int iconY = (int)skillButton.Y + (int)skillButton.Height / 2 - iconSize / 2;
                    Raylib_cs.Color magicColor = skillHovered ?new Raylib_cs.Color(80, 80, 230, 200) :    new Raylib_cs.Color(120, 120, 255, 255);   
                    Raylib.DrawRectangle(iconX + 14, iconY + 6, 4, 20,skillHovered ? Raylib_cs.Color.DarkBrown : Raylib_cs.Color.Orange);
                    Raylib.DrawRectangle(iconX + 12, iconY + 2, 8, 6, magicColor);
                    Raylib.DrawPixel(iconX + 10, iconY + 4,skillHovered ? new Raylib_cs.Color(200, 200, 255, 200) : Raylib_cs.Color.White);
                    Raylib.DrawPixel(iconX + 22, iconY + 4,skillHovered ? new Raylib_cs.Color(200, 200, 255, 200) : Raylib_cs.Color.White);
                    Raylib.DrawPixel(iconX + 16, iconY,skillHovered ? new Raylib_cs.Color(200, 200, 255, 200) : Raylib_cs.Color.White);
                    Raylib.DrawPixel(iconX + 8, iconY + 8,skillHovered ? new Raylib_cs.Color(180, 180, 255, 200) : Raylib_cs.Color.White);
                    Raylib.DrawPixel(iconX + 24, iconY + 8,skillHovered ? new Raylib_cs.Color(180, 180, 255, 200) : Raylib_cs.Color.White);
                    Raylib.DrawPixel(iconX + 5, iconY + 12,skillHovered ? new Raylib_cs.Color(100, 150, 255, 200) : Raylib_cs.Color.SkyBlue);
                    Raylib.DrawPixel(iconX + 27, iconY + 12,skillHovered ? new Raylib_cs.Color(100, 150, 255, 200) : Raylib_cs.Color.SkyBlue);
                }
            );
            DrawPixelArtButton((int)itemButton.X,(int)itemButton.Y,(int)itemButton.Width,(int)itemButton.Height,"ITEM",new Raylib_cs.Color(60, 200, 60, 255),  new Raylib_cs.Color(40, 150, 40, 255), itemHovered,() =>
                {
                    int iconSize = 32;
                    int iconX = (int)itemButton.X + 20;
                    int iconY = (int)itemButton.Y + (int)itemButton.Height / 2 - iconSize / 2;
                    Raylib_cs.Color bottleColor = itemHovered ?new Raylib_cs.Color(130, 180, 230, 200) :   new Raylib_cs.Color(170, 220, 255, 220);    
                    Raylib_cs.Color liquidColor = itemHovered ?new Raylib_cs.Color(0, 200, 80, 220) :     new Raylib_cs.Color(0, 255, 100, 255);    
                    Raylib.DrawRectangle(iconX + 14, iconY + 2, 4, 6, bottleColor);
                    Raylib.DrawRectangle(iconX + 12, iconY, 8, 2,itemHovered ? Raylib_cs.Color.Gray : Raylib_cs.Color.LightGray);
                    Raylib.DrawRectangle(iconX + 10, iconY + 8, 12, 16, bottleColor);
                    Raylib.DrawRectangle(iconX + 10, iconY + 16, 12, 8, liquidColor);
                    if (itemHovered)
                    {
                        Raylib.DrawPixel(iconX + 14, iconY + 19, new Raylib_cs.Color(200, 255, 200, 220));
                        Raylib.DrawPixel(iconX + 18, iconY + 21, new Raylib_cs.Color(200, 255, 200, 220));
                    }
                    else
                    {
                        Raylib.DrawPixel(iconX + 13, iconY + 18, Raylib_cs.Color.White);
                        Raylib.DrawPixel(iconX + 17, iconY + 20, Raylib_cs.Color.White);
                    }
                }
            );
            DrawPixelArtButton((int)escapeButton.X,(int)escapeButton.Y,(int)escapeButton.Width,(int)escapeButton.Height,"ESCAPE",new Raylib_cs.Color(200, 200, 60, 255),new Raylib_cs.Color(150, 150, 30, 255),escapeHovered,() =>
                {
                    int iconSize = 32;
                    int iconX = (int)escapeButton.X + 20;
                    int iconY = (int)escapeButton.Y + (int)escapeButton.Height / 2 - iconSize / 2;
                    Raylib_cs.Color figurineColor = escapeHovered ? new Raylib_cs.Color(220, 220, 220, 220) : new Raylib_cs.Color(255, 255, 255, 255);
                    Raylib.DrawRectangle(iconX + 16, iconY + 4, 4, 4, figurineColor);
                    Raylib.DrawRectangle(iconX + 16, iconY + 8, 4, 10, figurineColor);
                    Raylib.DrawRectangle(iconX + 14, iconY + 18, 3, 6, figurineColor);
                    Raylib.DrawRectangle(iconX + 19, iconY + 18, 3, 6, figurineColor);
                    Raylib.DrawRectangle(iconX + 12, iconY + 10, 4, 3, figurineColor);
                    Raylib.DrawRectangle(iconX + 20, iconY + 10, 4, 3, figurineColor);
                    Raylib_cs.Color speedLineColor = escapeHovered ? new Raylib_cs.Color(220, 220, 120, 180) : new Raylib_cs.Color(255, 255, 150, 200);
                    Raylib.DrawRectangle(iconX + 2, iconY + 8, 8, 2, speedLineColor);
                    Raylib.DrawRectangle(iconX + 4, iconY + 12, 6, 2, speedLineColor);
                    Raylib.DrawRectangle(iconX + 6, iconY + 16, 4, 2, speedLineColor);
                }
            );
        }
        /// <summary>
        /// Draws a pixel art style button with a specified icon and text.
        /// </summary>
        public static void DrawPixelArtButton(int x, int y, int width, int height, string text, Raylib_cs.Color mainColor, Raylib_cs.Color darkColor, bool hovered, Action drawIcon)
        {
            Raylib_cs.Color topColor = hovered ?new Raylib_cs.Color((int)Math.Min(255, mainColor.R + 40),(int)Math.Min(255, mainColor.G + 40),(int)Math.Min(255, mainColor.B + 40),255) : mainColor;
            Raylib_cs.Color bottomColor = darkColor;
            Raylib.DrawRectangle(x, y, width, height, bottomColor);
            Raylib.DrawRectangle(x, y, width, height - 4, topColor);
            Raylib.DrawRectangle(x + 2, y + 2,width - 4,2,new Raylib_cs.Color((int)Math.Min(255, topColor.R + 30),(int)Math.Min(255, topColor.G + 30),(int)Math.Min(255, topColor.B + 30),255));
            Raylib.DrawRectangle(x + width - 4, y + 4,2, height - 8,new Raylib_cs.Color((int)Math.Max(0, darkColor.R - 20),(int)Math.Max(0, darkColor.G - 20),(int)Math.Max(0, darkColor.B - 20),255));
            Raylib.DrawRectangle(x + 4,y + height - 4,width - 8,2,new Raylib_cs.Color((int)Math.Max(0, darkColor.R - 20),(int)Math.Max(0, darkColor.G - 20),(int)Math.Max(0, darkColor.B - 20),255));
            DrawPixelBorder(x, y, width, height, Raylib_cs.Color.Black, Raylib_cs.Color.Black, 2);
            Raylib.DrawText(text,x + 70,y + height / 2 - 12,24,hovered ? Raylib_cs.Color.White : new Raylib_cs.Color(240, 240, 240, 220));
            if (hovered)
            {
                Raylib.DrawRectangle(x - 4, y - 4, 8, 2, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x - 4, y - 4, 2, 8, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x + width - 4, y - 4, 8, 2, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x + width + 2, y - 4, 2, 8, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x - 4, y + height + 2, 8, 2, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x - 4, y + height - 4, 2, 8, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x + width - 4, y + height + 2, 8, 2, Raylib_cs.Color.White);
                Raylib.DrawRectangle(x + width + 2, y + height - 4, 2, 8, Raylib_cs.Color.White);
            }
            drawIcon?.Invoke();
        }
        /// <summary>
        /// Draws a pixel border around a rectangle with specified colors and thickness.
        /// </summary>
        public static void DrawPixelBorder(int x, int y, int width, int height, Raylib_cs.Color color, Raylib_cs.Color shadowColor, int thickness)
        {
            Raylib.DrawRectangle(x, y, width, thickness, color);
            Raylib.DrawRectangle(x, y, thickness, height, color);
            Raylib.DrawRectangle(x, y + height - thickness, width, thickness, shadowColor);
            Raylib.DrawRectangle(x + width - thickness, y, thickness, height, shadowColor);
            Raylib.DrawRectangle(x, y, thickness + 2, thickness + 2, color);
            Raylib.DrawRectangle(x + width - thickness - 2, y, thickness + 2, thickness + 2, color);
            Raylib.DrawRectangle(x, y + height - thickness - 2, thickness + 2, thickness + 2, shadowColor);
            Raylib.DrawRectangle(x + width - thickness - 2, y + height - thickness - 2, thickness + 2, thickness + 2, shadowColor);
        }
        /// <summary>
        /// Draws text with a glowing effect by rendering the text multiple times with an offset.
        /// </summary>
        public static void DrawTextWithGlow(string text, int x, int y, int fontSize, Raylib_cs.Color textColor, Raylib_cs.Color glowColor)
        {
            Raylib.DrawText(text, x + 1, y + 1, fontSize, glowColor);
            Raylib.DrawText(text, x - 1, y - 1, fontSize, glowColor);
            Raylib.DrawText(text, x + 1, y - 1, fontSize, glowColor);
            Raylib.DrawText(text, x - 1, y + 1, fontSize, glowColor);
            Raylib.DrawText(text, x, y, fontSize, textColor);
        }
        /// <summary>
        /// Draws text with a shadow effect by rendering the text twice, once for the shadow and once for the main text.
        /// </summary>
        public static void DrawTextWithShadow(string text, int x, int y, int fontSize, Raylib_cs.Color textColor)
        {
            Raylib.DrawText(text, x + 2, y + 2, fontSize, new Raylib_cs.Color(0, 0, 0, 150));
            Raylib.DrawText(text, x, y, fontSize, textColor);
        }
        /// <summary>
        /// Draws an enhanced bar with a bevel effect, gradient fill, and label.
        /// </summary>
        public static void DrawEnhancedBar(int x, int y, int width, int height, double percentage, Raylib_cs.Color color, string label)
        {
            Raylib.DrawRectangle(x, y, width, height, new Raylib_cs.Color(30, 30, 40, 255));
            Raylib.DrawRectangle(x + 1, y + 1, width - 2, height - 2, new Raylib_cs.Color(50, 50, 60, 255));
            int fillWidth = (int)(width * Math.Clamp(percentage, 0f, 1f));
            if (fillWidth > 0)
            {
                Raylib_cs.Color darkerColor = new Raylib_cs.Color((byte)Math.Max(0, color.R - 40),(byte)Math.Max(0, color.G - 40),(byte)Math.Max(0, color.B - 40),color.A);
                Raylib.DrawRectangleGradientV(x + 2,y + 2,fillWidth - 4,height - 4,color,darkerColor);
                Raylib.DrawRectangle(x + 2, y + 2,fillWidth - 4,(int)(height * 0.2f),new Raylib_cs.Color(255, 255, 255, 100));
            }
            Raylib.DrawRectangleLinesEx(new Rectangle(x, y, width, height),2,new Raylib_cs.Color(200, 200, 200, 255));
            DrawTextWithShadow(label, x + 10, y + (height / 2) - 10, 20, Raylib_cs.Color.White);
        }
        /// <summary>
        /// Draws the skill selection UI for the player.
        /// </summary>
        public static void DrawSkillSelectionUI(Player player,int windowWidth,int windowHeight,int selectedSkillIndex)
        {
            if (player == null) return;
            Raylib.DrawRectangle(0, 0, windowWidth, windowHeight, new Raylib_cs.Color(0, 0, 0, 120));
            int panelWidth = ITEMS_PANEL_WIDTH;
            int panelHeight = ITEMS_PANEL_HEIGHT;
            Rectangle panel = new Rectangle(windowWidth / 2 - panelWidth / 2,windowHeight / 2 - panelHeight / 2,panelWidth,panelHeight );
            Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(40, 40, 60, 240));
            Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Blue);
            Raylib.DrawText("SELECT A SKILL", (int)panel.X + 30, (int)panel.Y + 20, 40, Raylib_cs.Color.Gold);
            Rectangle closeButton = new Rectangle(panel.X + panel.Width - 60, panel.Y + 20, 40, 40);
            bool closeHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), closeButton);
            Raylib.DrawRectangleRec(closeButton, closeHovered ? new Raylib_cs.Color(200, 60, 60, 255) : new Raylib_cs.Color(60, 60, 80, 255));
            Raylib.DrawRectangleLinesEx(closeButton, 2, Raylib_cs.Color.White);
            int centerX = (int)closeButton.X + (int)closeButton.Width / 2;
            int centerY = (int)closeButton.Y + (int)closeButton.Height / 2;
            Raylib.DrawLine(centerX - 10, centerY - 10, centerX + 10, centerY + 10, Raylib_cs.Color.White);
            Raylib.DrawLine(centerX + 10, centerY - 10, centerX - 10, centerY + 10, Raylib_cs.Color.White);
            Rectangle skillsArea = new Rectangle(panel.X + 20, panel.Y + 80, panel.Width - 40, panel.Height - 100);
            Raylib.DrawRectangleRec(skillsArea, new Raylib_cs.Color(30, 30, 50, 200));
            Raylib.DrawRectangleLinesEx(skillsArea, 2, Raylib_cs.Color.LightGray);
            if (player.Skills.Count > 0)
            {                
                int skillY = (int)skillsArea.Y + 20;
                int skillButtonHeight = SKILL_BUTTON_HEIGHT;
                int skillSpacing = SKILL_SPACING; 
                for (int i = 0; i < player.Skills.Count; i++)
                {
                    Skill skill = player.Skills[i];
                    Rectangle skillButton = new Rectangle(skillsArea.X + 20,skillY + (i * skillSpacing),skillsArea.Width - 40,skillButtonHeight);
                    bool isOnCooldown = skill.Cooldown > 0;
                    bool hasEnoughMana = player.Mana >= skill.ManaCost;
                    bool isSelectable = !isOnCooldown && hasEnoughMana;
                    bool isSelected = i == selectedSkillIndex;
                    bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), skillButton);
                    Raylib_cs.Color buttonColor;
                    if (!isSelectable)
                    {
                        buttonColor = new Raylib_cs.Color(70, 70, 90, 200);
                    }
                    else if (isSelected)
                    {
                        buttonColor = Raylib_cs.Color.Gold;
                    }
                    else if (isHovered)
                    {
                        buttonColor = new Raylib_cs.Color(80, 80, 180, 255);
                    }
                    else
                    {
                        buttonColor = new Raylib_cs.Color(60, 60, 120, 255);
                    }
                    Raylib.DrawRectangleRec(skillButton, buttonColor);
                    Raylib_cs.Color borderColor;
                    float borderThickness;
                    if (isHovered || isSelected)
                    {
                        borderColor = isSelected ? Raylib_cs.Color.White : new Raylib_cs.Color(150, 150, 255, 255);
                        borderThickness = 3.0f;
                        if (isHovered)
                        {
                            Rectangle outerGlow = new Rectangle(skillButton.X - 2,skillButton.Y - 2,skillButton.Width + 4,skillButton.Height + 4);
                            Raylib.DrawRectangleLinesEx(outerGlow, 1, new Raylib_cs.Color(200, 200, 255, 150));
                        }
                    }
                    else
                    {
                        borderColor = Raylib_cs.Color.LightGray;
                        borderThickness = 2.0f;
                    }
                    Raylib.DrawRectangleLinesEx(skillButton, borderThickness, borderColor);
                    Raylib.DrawText(skill.Name, (int)skillButton.X + 20, (int)skillButton.Y + 10, 24, Raylib_cs.Color.White);
                    string manaText = "Mana: " + skill.ManaCost;
                    Raylib.DrawText(manaText, (int)skillButton.X + 20, (int)skillButton.Y + 40, 16, hasEnoughMana ? Raylib_cs.Color.Blue : Raylib_cs.Color.Red);
                    if (isOnCooldown)
                    {
                        string cooldownHeader = "Remaining Turns:";
                        string cooldownValue = skill.Cooldown.ToString();
                        Raylib.DrawText(cooldownHeader, (int)skillButton.X + 150, (int)skillButton.Y + 40, 16, Raylib_cs.Color.White);
                        Raylib.DrawText(cooldownValue, (int)skillButton.X + 275, (int)skillButton.Y + 40, 16, Raylib_cs.Color.Red);
                    }
                    string descHeader = "Description: ";
                    string desc = skill.Description;
                    if (desc.Length > 50)
                    {
                        desc = desc.Substring(0, 50) + "...";
                    }
                    Raylib.DrawText(descHeader, (int)skillButton.X + 20, (int)skillButton.Y + 60, 16, Raylib_cs.Color.White);
                    Raylib.DrawText(desc, (int)skillButton.X + 115, (int)skillButton.Y + 60, 16, Raylib_cs.Color.LightGray);
                }
            }
            else
            {
                Raylib.DrawText("No skills available.", (int)skillsArea.X + 20, (int)skillsArea.Y + 40, 30, Raylib_cs.Color.Gray);
            }
        }
        /// <summary>
        /// Draws the item selection UI for the player.
        /// </summary>
        public static void DrawItemSelectionUI(Player player,int windowWidth,int windowHeight,int inventoryScrollPosition)
        {
            if (player == null) {
                return;
            }
            Raylib.DrawRectangle(0, 0, windowWidth, windowHeight, new Raylib_cs.Color(0, 0, 0, 120));
            int panelWidth = ITEMS_PANEL_WIDTH;
            int panelHeight = ITEMS_PANEL_HEIGHT;
            Rectangle panel = new Rectangle(windowWidth / 2 - panelWidth / 2, windowHeight / 2 - panelHeight / 2, panelWidth,panelHeight);
            Raylib.DrawRectangleRec(panel, new Raylib_cs.Color(40, 40, 60, 240));
            Raylib.DrawRectangleLinesEx(panel, 4, Raylib_cs.Color.Blue);
            Raylib.DrawText("SELECT AN ITEM", (int)panel.X + 30, (int)panel.Y + 20, 40, Raylib_cs.Color.SkyBlue);
            Rectangle closeButton = new Rectangle(panel.X + panel.Width - 60, panel.Y + 20, 40, 40);
            bool closeHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), closeButton);
            Raylib.DrawRectangleRec(closeButton, closeHovered ? new Raylib_cs.Color(200, 60, 60, 255) : new Raylib_cs.Color(60, 60, 80, 255));
            Raylib.DrawRectangleLinesEx(closeButton, 2, Raylib_cs.Color.White);
            int centerX = (int)closeButton.X + (int)closeButton.Width / 2;
            int centerY = (int)closeButton.Y + (int)closeButton.Height / 2;
            Raylib.DrawLine(centerX - 10, centerY - 10, centerX + 10, centerY + 10, Raylib_cs.Color.White);
            Raylib.DrawLine(centerX + 10, centerY - 10, centerX - 10, centerY + 10, Raylib_cs.Color.White);
            List<Item> usableItems = new List<Item>();
            foreach (Item item in player.Inventory.Items)
            {
                if (item is Potion potion)
                {
                    if (potion.PotionType != PotionType.ExpBoost)
                    {
                        usableItems.Add(item);
                    }
                }
            }
            Rectangle itemsArea = new Rectangle(panel.X + 20, panel.Y + 80, panel.Width - 40, panel.Height - 100);
            Raylib.DrawRectangleRec(itemsArea, new Raylib_cs.Color(30, 30, 50, 200));
            Raylib.DrawRectangleLinesEx(itemsArea, 2, Raylib_cs.Color.LightGray);
            if (usableItems.Count > 0)
            {                
                int itemButtonHeight = ITEM_BUTTON_HEIGHT;
                int itemSpacing = ITEM_SPACING;
                int visibleItems = VISIBLE_ITEMS;
                int maxScrollPosition = Math.Max(0, usableItems.Count - visibleItems);
                inventoryScrollPosition = Math.Clamp(inventoryScrollPosition, 0, maxScrollPosition);
                if (usableItems.Count > visibleItems)
                {
                    DrawItemScrollbar(itemsArea, usableItems.Count, visibleItems, inventoryScrollPosition, maxScrollPosition);
                }
                int itemY = (int)itemsArea.Y + 20;
                for (int i = 0; i < Math.Min(visibleItems, usableItems.Count); i++)
                {
                    int itemIndex = i + inventoryScrollPosition;
                    if (itemIndex >= usableItems.Count) break;
                    Item item = usableItems[itemIndex];
                    Rectangle itemButton = new Rectangle(itemsArea.X + 20,itemY + (i * itemSpacing),itemsArea.Width - 60, itemButtonHeight);
                    DrawItemButton(item, itemButton);
                }
            }
            else
            {
                Raylib.DrawText("No usable items available.", (int)itemsArea.X + 40, (int)itemsArea.Y + 40, 30, Raylib_cs.Color.Gray);
            }
        }
        /// <summary>
        /// Draws the item scrollbar for the item selection UI.
        /// </summary>
        private static void DrawItemScrollbar(Rectangle itemsArea, int totalItems, int visibleItems, int scrollPosition, int maxScrollPosition)
        {
            Rectangle scrollTrack = new Rectangle(itemsArea.X + itemsArea.Width - 20,itemsArea.Y + 10,10,itemsArea.Height - 20);
            Raylib.DrawRectangleRec(scrollTrack, new Raylib_cs.Color(50, 50, 70, 255));
            float handleRatio = (float)visibleItems / totalItems;
            float handleHeight = Math.Max(30, scrollTrack.Height * handleRatio);
            float scrollProgress = maxScrollPosition > 0 ? (float)scrollPosition / maxScrollPosition : 0;
            float handleY = scrollTrack.Y + scrollProgress * (scrollTrack.Height - handleHeight);
            Rectangle scrollHandle = new Rectangle(scrollTrack.X,handleY,10, handleHeight );
            Raylib.DrawRectangleRec(scrollHandle, Raylib_cs.Color.LightGray);
        }
        /// <summary>
        /// Draws an item selected button for the item selection UI.
        /// </summary>
        private static void DrawItemButton(Item item, Rectangle itemButton)
        {
            bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), itemButton);
            Raylib_cs.Color buttonColor = isHovered ?new Raylib_cs.Color(80, 80, 180, 255) :new Raylib_cs.Color(60, 60, 120, 255);
            Raylib.DrawRectangleRec(itemButton, buttonColor);
            if (isHovered)
            {
                Raylib.DrawRectangleLinesEx(itemButton, 3.0f, new Raylib_cs.Color(150, 150, 255, 255));
                Rectangle outerGlow = new Rectangle(itemButton.X - 2,itemButton.Y - 2,itemButton.Width + 4,itemButton.Height + 4);
                Raylib.DrawRectangleLinesEx(outerGlow, 1, new Raylib_cs.Color(200, 200, 255, 150));
            }
            else
            {
                Raylib.DrawRectangleLinesEx(itemButton, 2.0f, Raylib_cs.Color.LightGray);
            }
            Rectangle itemIcon = new Rectangle(itemButton.X + 10,itemButton.Y + 10,60,60);
            if (item is Potion potion)
            {
                DrawPotionIcon(potion, itemIcon);
            }
            Raylib.DrawText(item.Name, (int)itemButton.X + 80, (int)itemButton.Y + 10, 24, Raylib_cs.Color.White);
            if (item.Quantity > 1)
            {
                Raylib.DrawText("Quantity: " + item.Quantity, (int)itemButton.X + 80, (int)itemButton.Y + 40, 16, Raylib_cs.Color.Yellow);
            }
            string effectText = GetItemEffectText(item);
            if (!string.IsNullOrEmpty(effectText))
            {
                Raylib.DrawText(effectText, (int)itemButton.X + 80, (int)itemButton.Y + 60, 16, Raylib_cs.Color.LightGray);
            }
        }
        /// <summary>
        /// Draws the potion icon for the item selection UI.
        /// </summary>
        private static void DrawPotionIcon(Potion potion, Rectangle itemIcon)
        {
            Raylib_cs.Color iconColor;
            switch (potion.PotionType)
            {
                case PotionType.Healing:
                    iconColor = new Raylib_cs.Color(50, 200, 50, 255);
                    break;
                case PotionType.Mana:
                    iconColor = new Raylib_cs.Color(50, 50, 200, 255);
                    break;
                case PotionType.ReduceCooldown:
                    iconColor = new Raylib_cs.Color(200, 50, 200, 255);
                    break;
                default:
                    iconColor = new Raylib_cs.Color(150, 150, 150, 255);
                    break;
            }
            Raylib.DrawRectangleRec(itemIcon, iconColor);
            try
            {
                Raylib_cs.Texture2D texture = potion.GetTexture();
                int padding = 4;
                int drawableSize = (int)itemIcon.Width - (padding * 2);
                float scaleX = (float)drawableSize / texture.Width;
                float scaleY = (float)drawableSize / texture.Height;
                float scale = Math.Min(scaleX, scaleY) * 0.8f;
                float scaledWidth = texture.Width * scale;
                float scaledHeight = texture.Height * scale;
                float xOffset = itemIcon.X + (itemIcon.Width - scaledWidth) / 2;
                float yOffset = itemIcon.Y + (itemIcon.Height - scaledHeight) / 2;
                Raylib.DrawTextureEx(texture,new Vector2(xOffset, yOffset),0.0f,scale,Raylib_cs.Color.White);
            }
            catch
            {
                Raylib.DrawText("P", (int)itemIcon.X + 20, (int)itemIcon.Y + 20, 30, Raylib_cs.Color.White);
            }
        }
        /// <summary>
        /// Gets the effect text for an item based on its type.
        /// </summary>
        private static string GetItemEffectText(Item item)
        {
            if (item is Potion potion)
            {
                switch (potion.PotionType)
                {
                    case PotionType.Healing:
                        return "Restores " + potion.HealingAmount + " HP";

                    case PotionType.Mana:
                        return "Restores " + potion.Mana + " MP";

                    case PotionType.ReduceCooldown:
                        return "Reduces all skill cooldowns by " + potion.ReducingCooldown;

                    default:
                        return "";
                }
            }
            return "";
        }
        /*        
        public static Monster? CreateMonsterFromSelection(string monsterType, int level, string prefix = "", Difficulty difficulty = Difficulty.Medium, int row = 0, int column = 0)
        {
            double baseHP = 40.0;
            double baseAttack = 5.0;
            double baseCriticalRate = 0.05;
            double baseDefense = 2.0;
            double baseSpeed = 5.0;
            double baseMana = 20.0;
            int baseExpReward = 30;
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    baseHP = 40.0;
                    baseAttack = 5.0;
                    baseCriticalRate = 0.05;
                    baseDefense = 2.0;
                    baseSpeed = 8.0;
                    baseMana = 20.0;
                    baseExpReward = 25 + level * 5;
                    break;
                case "SLIME":
                    baseHP = 40.0;
                    baseAttack = 5.0;
                    baseCriticalRate = 0.05;
                    baseDefense = 2.0;
                    baseSpeed = 3.0;
                    baseMana = 15.0;
                    baseExpReward = 20 + level * 5;
                    break;
                case "SKELETON":
                    baseHP = 40.0;
                    baseAttack = 5.0;
                    baseCriticalRate = 0.05;
                    baseDefense = 2.0;
                    baseSpeed = 5.0;
                    baseMana = 30.0;
                    baseExpReward = 30 + level * 5;
                    break;
                case "BOSS_PHASE1":
                    baseHP = 300.0;
                    baseAttack = 10.0;
                    baseCriticalRate = 0.08;
                    baseDefense = 5.0;
                    baseSpeed = 5.0;
                    baseMana = 100.0;
                    baseExpReward = 100;
                    break;
            }
            double finalHP = baseHP + (level * 10);
            double finalMaxHP = baseHP + (level * 10);
            double finalAttack = baseAttack + (level * 2);
            double finalDefense = baseDefense + level;
            double finalCritRate = baseCriticalRate + (level * 0.01);
            double finalSpeed = baseSpeed;
            double finalMana = baseMana;
            int moneyReward = 5 * level + 30;
            if (!string.IsNullOrEmpty(prefix))
            {
                switch (prefix.ToUpper())
                {
                    case "FERAL":
                        finalAttack *= 1.2;
                        finalHP *= 0.9;
                        finalMaxHP *= 0.9;
                        finalDefense *= 1.0;
                        finalCritRate += 0.0;
                        break;
                    case "DARK":
                        finalAttack *= 1.1;
                        finalHP *= 1.0;
                        finalMaxHP *= 1.0;
                        finalDefense *= 1.0;
                        finalCritRate += 0.05;
                        break;
                    case "SAVAGE":
                        finalAttack *= 1.3;
                        finalHP *= 1.0;
                        finalMaxHP *= 1.0;
                        finalDefense *= 0.8;
                        finalCritRate += 0.0;
                        break;
                    case "VENOMOUS":
                        finalAttack *= 1.1;
                        finalHP *= 1.0;
                        finalMaxHP *= 1.0;
                        finalDefense *= 1.0;
                        finalCritRate += 0.1;
                        break;
                    case "SHADOW":
                        finalAttack *= 1.0;
                        finalHP *= 1.0;
                        finalMaxHP *= 1.0;
                        finalDefense *= 0.9;
                        finalCritRate += 0.15;
                        break;
                    case "CURSED":
                        finalAttack *= 1.2;
                        finalHP *= 1.2;
                        finalMaxHP *= 1.2;
                        finalDefense *= 0.8;
                        finalCritRate += 0.0;
                        break;
                    case "ANCIENT":
                        finalAttack *= 1.0;
                        finalHP *= 1.5;
                        finalMaxHP *= 1.5;
                        finalDefense *= 1.3;
                        finalCritRate += 0.0;
                        break;
                    case "GIANT":
                        finalAttack *= 1.2;
                        finalHP *= 1.8;
                        finalMaxHP *= 1.8;
                        finalDefense *= 1.1;
                        finalCritRate += 0.0;
                        break;
                }
            }
            switch (difficulty)
            {
                case Difficulty.Easy:
                    finalHP *= 0.8;
                    finalMaxHP *= 0.8;
                    finalAttack *= 0.8;
                    break;

                case Difficulty.Hard:
                    finalHP *= 1.2;
                    finalMaxHP *= 1.2;
                    finalAttack *= 1.2;
                    break;
            }
            Monster monster;
            string fullName = !string.IsNullOrEmpty(prefix) ? $"{prefix} {monsterType}" : monsterType;
            bool isAlive = true;
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    monster = new Goblin(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward);
                    break;
                case "SLIME":
                    monster = new Slime(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward);
                    break;
                case "SKELETON":
                    monster = new Skeleton(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column, isAlive, baseExpReward);
                    break;
                case "BOSS_PHASE1":
                    monster = new Boss(fullName, finalHP, finalMaxHP, finalAttack, finalCritRate, finalDefense, finalSpeed, finalMana, moneyReward, level, row, column,
                        isAlive,
                        baseExpReward
                    );
                    break;

                default:
                    throw new ArgumentException("Unknown monster type: " + monsterType);
            }
            AssignMonsterSkills(monster, monsterType);
            return monster;
        }
        private static void AssignMonsterSkills(Monster monster, string monsterType)
        {
            switch (monsterType.ToUpper())
            {
                case "GOBLIN":
                    if (monster.Level >= 1)
                    {
                        monster.Skills.Add(new AttackSkill(1, "UpDownBleeding", "Up Down Bleeding Attack", 12, 5, 0, 0, 1, 1.2, 0.0));
                    }
                    if (monster.Level >= 3)
                    {
                        monster.Skills.Add(new AttackSkill(3, "LeftRightSpin", "Increases attack and defense power", 35, 0, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
                case "SLIME":
                    if (monster.Level >= 1)
                    {
                        monster.Skills.Add(new AttackSkill(4, "IhaveKnife", "Has a knife to kill", 0, 10, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
                case "SKELETON":
                    if (monster.Level >= 2)
                    {
                        monster.Skills.Add(new AttackSkill(6, "AheadChop ", "Move the knife ahead to attack", 15, 4, 0, 0, 1, 1.0, 0.0));
                    }
                    if (monster.Level >= 3)
                    {
                        monster.Skills.Add(new AttackSkill(7, "HeavySlash", "Increases attack and defense power", 30, 0, 0, 0, 1, 1.0, 0.0));
                    }
                    break;
            }
        }
        */
        /// <summary>
        /// Property to get or set the experience reward for defeating the monster.
        /// </summary>
        public int ExpReward
        {
            get { return _expReward; }
            set { _expReward = value; }
        }
        /// <summary>
        /// Property to get or set the drop list of items that the monster can drop when defeated.
        /// </summary>
        public List<Item> DropList
        {
            get { return _dropList; }
            set { _dropList = value; }
        }
        /// <summary>
        /// Property to get or set the money reward for defeating the monster.
        /// </summary>
        public int MoneyReward
        {
            get { return _moneyReward; }
            set { _moneyReward = value; }
        }
    }
}