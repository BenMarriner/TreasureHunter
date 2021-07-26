using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace TreasureHunter
{
    abstract class ProgramWindow
    {
        // Level the window is being opened with
        protected Level _level;
        // Editor window
        protected Window _window;
        // Name of the window. This is supposed to display the name of the level being played or edited and then either " - Level Editor" or " - Treasure Hunter"
        protected String _windowName;
        
        // Constructor
        public ProgramWindow(Level level)
        {
            this._level = level;
        }

        // Opens a new editor window
        public virtual void WindowOpen()
        {
            this._window = SplashKit.OpenWindow(this._windowName, 960, 960);
            Input.AddListener(this);
        }
        // Close the editor window
        public virtual void WindowClose()
        {
            Input.RemoveListener(this);
            SplashKit.CloseWindow(this._windowName);
        }
        // Update window
        public abstract void Update();
    }

    class Editor : ProgramWindow, IEditorKeyEvents
    {
        // Renderer to use for the editor
        private EditorRenderer _editorRenderer;
        // Keep track of the currently selected tile
        private TileSelector _tileSelector;
        // ID of tile to be placed
        private int _placementMode;
        // Elements to draw and not draw. Is passed onto the renderer
        private Elements _elements;
        // Is the editor in testing mode? This is asking if the player has pressed T to test their map in the game
        private bool _testModeEnabled;

        // Initialise editor with new level
        public Editor(String levelName) : base(new Level(levelName, 20, 20))
        {
            this._editorRenderer = new EditorRenderer();
            this._tileSelector = new TileSelector();
            this._placementMode = 0;

            this._elements.SpawnableTiles = 0;
            this._elements.TileSelector = true;
            this._elements.Spawnpoints = true;
        }

        // Initialise editor with existing level
        public Editor(Level level) : base(level)
        {
            this._editorRenderer = new EditorRenderer();
            this._tileSelector = new TileSelector();
            this._placementMode = 0;

            this._elements.SpawnableTiles = 0;
            this._elements.TileSelector = true;
            this._elements.Spawnpoints = true;
        }

        // Used to keep track of the elements the renderer should be told to draw and not draw
        public struct Elements
        {
            public int SpawnableTiles;
            public bool TileSelector;
            public bool Spawnpoints;
        }

        // Open Editor Window
        public override void WindowOpen()
        {
            GlobalSettings.ProgramMode = 0;
            this._windowName = this._level.LevelName + " - Level Editor";
            base.WindowOpen();
        }

        // Update Editor Window
        public override void Update()
        {
            do
            {
                // Render the game
                this._editorRenderer.DrawGame(this._level, this._elements, this._tileSelector);

                // Process keyboard and mouse inputs
                Input.ProcessInput();

                // Refresh the window to show new informations
                SplashKit.RefreshWindow(this._window, 60);
            }
            while (!SplashKit.QuitRequested());
        }

        // Key Events
        // Arrows will move tile selector crosshair around the level
        public void UpArrowPressed()
        {
            this._tileSelector.DecrementTileCoordinate(ProgramMath.Coordinate.Y);
        }

        public void DownArrowPressed()
        {
            this._tileSelector.IncrementTileCoordinate(ProgramMath.Coordinate.Y);
        }

        public void LeftArrowPressed()
        {
            this._tileSelector.DecrementTileCoordinate(ProgramMath.Coordinate.X);
            
        }

        public void RightArrowPressed()
        {
            this._tileSelector.IncrementTileCoordinate(ProgramMath.Coordinate.X);
        }

        // Places a tile or a spawn in the level
        public void SpacebarPressed()
        {
            switch (this._placementMode)
            {
                case 0:
                    this._level.PlaceInLevel(this._tileSelector.SelectedTileLocation, new Wall(this._tileSelector.SelectedTileLocation));
                    break;
                case 1:
                    this._level.PlaceInLevel(this._tileSelector.SelectedTileLocation, new Floor(this._tileSelector.SelectedTileLocation));
                    break;
                case 2:
                    this._level.SetPlayerSpawnPoint(this._level.GetTile(this._tileSelector.SelectedTileLocation));
                    break;
                case 3:
                    this._level.SetEnemySpawnPoint(this._level.GetTile(this._tileSelector.SelectedTileLocation));
                    break;
                case 4:
                    this._level.SetCoinSpawnPoint(this._level.GetTile(this._tileSelector.SelectedTileLocation));
                    break;
            }
        }

        // Number keys will change the current placement mode so that different tiles and spawns can be placed
        public void OneKeyPressed()
        {
            this._placementMode = 0;
            this._elements.SpawnableTiles = 0;
        }

        public void TwoKeyPressed()
        {
            this._placementMode = 1;
            this._elements.SpawnableTiles = 0;
        }

        public void ThreeKeyPressed()
        {
            this._placementMode = 2;
            this._elements.SpawnableTiles = 1;
        }

        public void FourKeyPressed()
        {
            this._placementMode = 3;
            this._elements.SpawnableTiles = 2;
        }

        public void FiveKeyPressed()
        {
            this._placementMode = 4;
            this._elements.SpawnableTiles = 3;
        }

        // Executes save procedure
        public void SKeyPressed()
        {
            FileManagement.SaveLevel(this._level);
            SplashKit.DisplayDialog("Save Complete", "Your map saved successfully", GlobalSettings.Neon, 30);
        }

        // Puts the editor in test mode
        public void TKeyPressed()
        {
            if (!this._testModeEnabled)
            {
                this._testModeEnabled = true;
            }
        }
    }

    class Game : ProgramWindow, IGameKeyEvents
    {
        // Renderer
        private GameRenderer _gameRenderer;
        
        public Game(Level level) : base(level)
        {
            // Initialise renderer
            this._gameRenderer = new GameRenderer();
            
        }

        public override void WindowOpen()
        {
            GlobalSettings.ProgramMode = 1;
            this._windowName = this._level.LevelName + " - Treasure Hunter";
            base.WindowOpen();
        }

        public override void Update()
        {
            do
            {
                // Update level information
                this._level.Update();

                // Draw the game
                this._gameRenderer.DrawGame(this._level);
                // Process keyboard and mouse input
                Input.ProcessInput(); 

                // Refresh the window to show new frame
                SplashKit.RefreshWindow(this._window, 60);
            }
            while (!SplashKit.QuitRequested());
        }

        public void UpArrowDown()
        {
            this._level.Player.Move(ProgramMath.Direction.Up);
        }

        public void DownArrowDown()
        {
            this._level.Player.Move(ProgramMath.Direction.Down);
        }

        public void LeftArrowDown()
        {
            this._level.Player.Move(ProgramMath.Direction.Left);
        }

        public void RightArrowDown()
        {
            this._level.Player.Move(ProgramMath.Direction.Right);
        }

        public void SpacebarPressed()
        {

        }

        public void RKeyPressed()
        {

        }
    }

    class TileSelector
    {
        // Location of the tile the crosshair is currently on
        private Vector2D _selectedTileLocation;

        // Constructor
        public TileSelector()
        {
            this._selectedTileLocation.X = 10;
            this._selectedTileLocation.Y = 10;
        }

        public Vector2D SelectedTileLocation
        { 
            get { return this._selectedTileLocation; }
        }

        // Property of the selected tile location converted to a location in window space
        public Vector2D SelectedTileWindowLocation
        { 
            get { return ProgramMath.TileToWindowPosition(this.SelectedTileLocation); }
        }

        // Move the crosshair one tile down or right
        public void IncrementTileCoordinate(ProgramMath.Coordinate coordinate)
        {
            switch (coordinate)
            {
                case ProgramMath.Coordinate.X:
                    SetSelectedTileCoordinate(coordinate, this._selectedTileLocation.X + 1);
                    break;
                case ProgramMath.Coordinate.Y:
                    SetSelectedTileCoordinate(coordinate, this._selectedTileLocation.Y + 1);
                    break;
            }
        }

        // Move the crosshair one tile up or left
        public void DecrementTileCoordinate(ProgramMath.Coordinate coordinate)
        {
            switch (coordinate)
            {
                case ProgramMath.Coordinate.X:
                    SetSelectedTileCoordinate(coordinate, this._selectedTileLocation.X - 1);
                    break;
                case ProgramMath.Coordinate.Y:
                    SetSelectedTileCoordinate(coordinate, this._selectedTileLocation.Y - 1);
                    break;
            }
        }

        // Set the crosshair location on a new tile
        public void SetSelectedTileCoordinate(ProgramMath.Coordinate coordinate, double value)
        {
            switch (coordinate)
            {
                case ProgramMath.Coordinate.X:
                    this._selectedTileLocation.X = ValidateTileCoordinate(value);
                    break;
                case ProgramMath.Coordinate.Y:
                    this._selectedTileLocation.Y = ValidateTileCoordinate(value);
                    break;
            }
        }

        // Keeps the crosshair location within the level
        private double ValidateTileCoordinate(double coordinate)
        {   
            if (coordinate < 0)
            {
                coordinate = 0;
            }
            else if (coordinate > 19)
            {
                coordinate = 19;
            }

            return coordinate;
        }
    }
}
