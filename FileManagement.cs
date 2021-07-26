using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace TreasureHunter
{
    static class FileManagement
    {
        // Level save procedure
        public static void SaveLevel(Level level)
        {
            Json levelJson = SplashKit.CreateJson();
            Json levelTiles = SplashKit.CreateJson();
            Json levelMetadata = SplashKit.CreateJson();
            Json levelSpawns = SplashKit.CreateJson();

            // TileIDs
            for (int column = 0; column < level.LevelDimensions.X; column++)
            {
                List<double> currentColumn = new List<double>();
                for (int row = 0; row < level.LevelDimensions.Y; row++)                
                {
                    currentColumn.Add(level.Tiles[column, row].TileID);
                }
                levelTiles.AddArray("Column" + column, currentColumn);
            }

            // Spawnpoint States
            for (int column = 0; column < level.LevelDimensions.X; column++)
            {
                List<double> currentColumn = new List<double>();
                for (int row = 0; row < level.LevelDimensions.Y; row++)
                {
                    currentColumn.Add(level.Tiles[column, row].SpawnpointState);
                }
                levelSpawns.AddArray("Column" + column, currentColumn);
            }

            // Metadata
            levelMetadata.AddString("Name", level.LevelName);
            levelMetadata.AddString("Asset Type", "Level");
            levelMetadata.AddNumber("LevelDimensionX", GlobalSettings.LevelDimensions.X);
            levelMetadata.AddNumber("LevelDimensionY", GlobalSettings.LevelDimensions.Y);

            // Assemble final JSON file
            levelJson.AddObject("Metadata", levelMetadata);
            levelJson.AddObject("Tiles", levelTiles);
            levelJson.AddObject("Spawns", levelSpawns);

            // Write to disk
            Json.ToFile(levelJson, level.LevelName + ".json");
            Json.FreeAll();
        }

        // Level loading procedure
        public static Level LoadLevel(String filename)
        {
            Json levelJson = Json.FromFile(filename);
            // Temporary level object that converts the JSON data to level class format
            Level level;

            // Check that the contents of the JSON are compatible with the program
            if (!ValidJSONStructure(levelJson) || levelJson == null)
            {
                return null;
            }

            String levelName = levelJson.ReadObject("Metadata").ReadString("Name");
            int levelDimensionX = Convert.ToInt32(levelJson.ReadObject("Metadata").ReadNumber("LevelDimensionX"));
            int levelDimensionY = Convert.ToInt32(levelJson.ReadObject("Metadata").ReadNumber("LevelDimensionY"));
            List<List<double>> tileIDs = new List<List<double>>();
            List<List<double>> spawnpointStates = new List<List<double>>();

            // Tile IDs
            for (int column = 0; column < levelDimensionX; column++)
            {
                List<double> currentColumn = new List<double>();
                levelJson.ReadObject("Tiles").ReadArray("Column" + column, ref currentColumn);
                tileIDs.Add(currentColumn);                
            }

            // Spawnpoint States
            for (int column = 0; column < levelDimensionX; column++)
            {
                List<double> currentColumn = new List<double>();
                levelJson.ReadObject("Spawns").ReadArray("Column" + column, ref currentColumn);
                spawnpointStates.Add(currentColumn);
            }

            // Initialise converter level
            level = new Level(levelName, levelDimensionX, levelDimensionY, tileIDs, spawnpointStates);

            return level;
        }

        // Check the file structure of the JSON file. Returns false if the structure is not compatible with the program
        private static bool ValidJSONStructure(Json levelJson)
        {
            return levelJson.HasKey("Metadata") && levelJson.HasKey("Tiles") && levelJson.HasKey("Spawns");
        }

        // Returns an array of the file names of the levels contained in the json directory
        public static String[] GetLevelList()
        {
            DirectoryInfo directory = new DirectoryInfo(SplashKit.PathToResources(ResourceKind.JsonResource));
            FileInfo[] jsonFiles = directory.GetFiles("*.json", SearchOption.TopDirectoryOnly);
            List<String> levelFiles = new List<String>();

            foreach (FileInfo file in jsonFiles)
            {
                Json currentFile = SplashKit.JsonFromFile(file.Name);
                if (currentFile.HasKey("Metadata"))
                {
                    if (currentFile.ReadObject("Metadata").HasKey("Asset Type"))
                    {
                        if (currentFile.ReadObject("Metadata").ReadString("Asset Type") == "Level")
                        {
                            levelFiles.Add(file.Name);
                        }
                    }
                }
            }

            return levelFiles.ToArray();
        }

        // Load assets used by the game (bitmaps, sprites, etc)
        public static void LoadResources()
        {
            SplashKit.LoadResourceBundle("Resources", "resources.txt");
        }
    }
}
