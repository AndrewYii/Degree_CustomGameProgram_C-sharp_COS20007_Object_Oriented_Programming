using System;
using System.IO;
using Raylib_cs;
using System.Diagnostics;

namespace DistinctionTask
{
    class Program
    {
        /// <summary>
        /// Main entry point for the Dungeon Adventure RPG game.
        /// </summary>
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Raylib.InitWindow(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, "Dungeon Adventure RPG");
            Raylib.SetExitKey(KeyboardKey.Null);
            Raylib.SetTargetFPS(60);
            Game game = new Game();
            Directory.CreateDirectory("log");
            stopwatch.Stop();
            Console.WriteLine("Initialisation took:" + stopwatch.ElapsedMilliseconds + "ms");
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