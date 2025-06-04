using System;
using NUnit.Framework;
using Raylib_cs;

namespace DistinctionTask
{
    [TestFixture]
    public class PolymorphismTest
    {
        [Test]
        public void TestArcherPolymorphism()
        {
            //Setup
            Raylib.SetConfigFlags(ConfigFlags.HiddenWindow);
            Raylib.InitWindow(100, 100, "UnitTestHiddenWindow");
            Game game = new Game();
            Player archer = PlayerFactory.CreatePlayerFromSelection("Archer", "Archer");
            Item sword = Item.CreateWeaponFromData(WeaponType.Sword, 1, 1, "Sword", "A sharp sword", 100);
            Item bow = Item.CreateWeaponFromData(WeaponType.Bow, 1, 1, "Bow", "A long-range bow", 100);
            Item axe = Item.CreateWeaponFromData(WeaponType.Axe, 1, 1, "Axe", "A heavy axe", 100);
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Blank);
            //Check(basic stats and skills)
            Assert.AreEqual(archer.Skills.Count, 0);
            Assert.AreEqual(archer.GetBaseHP(), 90);
            Assert.AreEqual(archer.GetBaseDamage(), 15);
            Assert.AreEqual(archer.GetBaseSpeed(), 12);
            Assert.AreEqual(archer.GetBaseDefense(), 4);
            Assert.AreEqual(archer.GetBaseCriticalRate(), 0.12);
            //Execute(Level up 4 times become level 5)
            for (int i = 0; i < 4; i++)
            {
                archer.LevelUp();
            }
            //Check(learnt skills and improved stats)
            Assert.AreEqual(archer.GetBaseHP(), 90 + 4 * 8);
            Assert.AreEqual(archer.GetBaseDamage(), 15 + 4 * 5);
            Assert.AreEqual(archer.GetBaseSpeed(), 12 + 4 * 3);
            Assert.AreEqual(archer.GetBaseDefense(), 4 + 4 * 2);
            Assert.AreEqual(archer.GetBaseCriticalRate(), 0.12 + 4 * 0.025);
            Assert.AreEqual(archer.Skills.Count, 1);
            //Execute && Check(Archer will get extra bonus for bow)
            Assert.AreEqual(archer.GetClassWeaponBonus((Weapon)bow), (4.0, 0.15));
            Assert.AreEqual(archer.GetClassWeaponBonus((Weapon)sword), (0, 0));
            Assert.AreEqual(archer.GetClassWeaponBonus((Weapon)axe), (0, 0));
            if (!Raylib.WindowShouldClose())
            {
                Raylib.CloseWindow();
            }
        }
        [Test]
        public void TestKnightPolymorphism()
        {
            //Setup
            Player knight = PlayerFactory.CreatePlayerFromSelection("Knight", "Knight");
            Item sword = Item.CreateWeaponFromData(WeaponType.Sword, 1, 1, "Sword", "A sharp sword", 100);
            Item bow = Item.CreateWeaponFromData(WeaponType.Bow, 1, 1, "Bow", "A long-range bow", 100);
            Item axe = Item.CreateWeaponFromData(WeaponType.Axe, 1, 1, "Axe", "A heavy axe", 100);
            //Check(basic stats and skills)
            Assert.AreEqual(knight.Skills.Count, 0);
            Assert.AreEqual(knight.GetBaseHP(), 120);
            Assert.AreEqual(knight.GetBaseDamage(), 12);
            Assert.AreEqual(knight.GetBaseSpeed(), 10);
            Assert.AreEqual(knight.GetBaseDefense(), 8);
            Assert.AreEqual(knight.GetBaseCriticalRate(), 0.05);
            //Execute(Level up 4 times become level 5)
            for (int i = 0; i < 4; i++)
            {
                knight.LevelUp();
            }
            //Check(learnt skills and improved stats)
            Assert.AreEqual(knight.GetBaseHP(), 120 + 4 * 10);
            Assert.AreEqual(knight.GetBaseDamage(), 12 + 4 * 5);
            Assert.AreEqual(knight.GetBaseSpeed(), 10 + 4 * 2);
            Assert.AreEqual(knight.GetBaseDefense(), 8 + 4 * 3);
            Assert.AreEqual(knight.GetBaseCriticalRate(), 0.05 + 4 * 0.02);
            Assert.AreEqual(knight.Skills.Count, 3);
            //Execute && Check(Knight will get extra bonus for sword)
            Assert.AreEqual(knight.GetClassWeaponBonus((Weapon)bow), (0, 0));
            Assert.AreEqual(knight.GetClassWeaponBonus((Weapon)sword), (5.0, 0.05));
            Assert.AreEqual(knight.GetClassWeaponBonus((Weapon)axe), (0, 0));

        }
        [Test]
        public void TestAxemanPolymorphism()
        {
            Player axeman = PlayerFactory.CreatePlayerFromSelection("Axeman", "Axeman");
            Item sword = Item.CreateWeaponFromData(WeaponType.Sword, 1, 1, "Sword", "A sharp sword", 100);
            Item bow = Item.CreateWeaponFromData(WeaponType.Bow, 1, 1, "Bow", "A long-range bow", 100);
            Item axe = Item.CreateWeaponFromData(WeaponType.Axe, 1, 1, "Axe", "A heavy axe", 100);
            //Check(basic stats and skills)
            Assert.AreEqual(axeman.Skills.Count, 0);
            Assert.AreEqual(axeman.GetBaseHP(), 130);
            Assert.AreEqual(axeman.GetBaseDamage(), 18);
            Assert.AreEqual(axeman.GetBaseSpeed(), 8);
            Assert.AreEqual(axeman.GetBaseDefense(), 9);
            Assert.AreEqual(axeman.GetBaseCriticalRate(), 0.07);
            //Execute(Level up 4 times become level 5)
            for (int i = 0; i < 4; i++)
            {
                axeman.LevelUp();
            }
            //Check(learnt skills and improved stats)
            Assert.AreEqual(axeman.GetBaseHP(), 130 + 4 * 12);
            Assert.AreEqual(axeman.GetBaseDamage(), 18 + 4 * 6);
            Assert.AreEqual(axeman.GetBaseSpeed(), 8 + 4 * 2);
            Assert.AreEqual(axeman.GetBaseDefense(), 9 + 4 * 4);
            Assert.AreEqual(axeman.GetBaseCriticalRate(), 0.07 + 4 * 0.01);
            Assert.AreEqual(axeman.Skills.Count, 2);
            //Execute && Check(Axeman will get extra bonus for axe)
            Assert.AreEqual(axeman.Skills.Count, 2);
            Assert.AreEqual(axeman.GetClassWeaponBonus((Weapon)bow), (0, 0));
            Assert.AreEqual(axeman.GetClassWeaponBonus((Weapon)axe), (10.0, 0.02));
            Assert.AreEqual(axeman.GetClassWeaponBonus((Weapon)sword), (0, 0));
        }
    }
}
