using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SplashKitSDK;

namespace TreasureHunter
{
    // This class keeps track of any data that needs to be globally accessible
    static class GlobalSettings
    {
        // Mode the program is in 0 = Editor, 1 = Game
        public static int ProgramMode;
        // Location of renderer in the window
        public static Vector2D RendererWindowLocation = new Vector2D();
        // Dimensions of the current level
        public static Vector2D LevelDimensions = new Vector2D();
        // Resolution of all bitmaps and sprites in the game
        public const int BitmapResolution = 48;
        // Neon font
        public static readonly Font Neon;
        
        // Some attributes need to be initialised at runtime
        static GlobalSettings()
        {
            Neon = SplashKit.LoadFont("Neon", "neon.ttf");
            RendererWindowLocation.X = 0;
            RendererWindowLocation.Y = 0;
        }
    }
}