using System;
using System.IO;
using Raylib_cs;

namespace DistinctionTask
{
    class Program
    {
        /// <summary>
        /// Main entry point for the Dungeon Adventure RPG game.
        /// </summary>
        static void Main(string[] args)
        {
            Raylib.InitWindow(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, "Dungeon Adventure RPG");
            Raylib.SetExitKey(KeyboardKey.Null);
            Raylib.SetTargetFPS(60);
            Game game = new Game();
            Directory.CreateDirectory("log");
            while (!Raylib.WindowShouldClose())
            {
                game.HandleInput();
                game.Update();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib_cs.Color.Black);
                game.DrawCurrentState();
                Raylib.EndDrawing();
            }
            game.Cleanup();
            Raylib.CloseWindow();
        }
    }
}