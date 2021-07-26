using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Transactions;

namespace TreasureHunter
{
    class Level
    {
        // Name of the level
        private String _levelName;
        // 2D array of all the tiles the level consists of
        private Tile[,] _tiles;
        // Size of the level. Measured in number of tiles
        private Vector2D _levelDimensions;
        // The player
        private Player _player;
        // The enemies
        private Enemy[] _enemies;
        // The coins
        private Coin[] _coins;

        // Constructor for starting a new level in the editor
        public Level(String levelName, double levelDimensionX, double levelDimensionY)
        {
            this._levelName = levelName;
            this._levelDimensions.X = levelDimensionX;
            this._levelDimensions.Y = levelDimensionY;
            GlobalSettings.LevelDimensions = this._levelDimensions;

            // Initialise new 2D array of tiles
            this._tiles = new Tile[Convert.ToInt32(this._levelDimensions.X), Convert.ToInt32(this._levelDimensions.Y)];

            // Every new level starts with the outside tiles as walls. It is impossible to change these to any other tile... Unless you're savvy with a text editor...
            for (int column = 0; column < this._levelDimensions.X; column++)
            {
                for (int row = 0; row < this._levelDimensions.Y; row++)
                {
                    if (this.IsOuterWallTile(column, row))
                    {
                        this._tiles[column, row] = new Wall(column, row);
                    }
                    else
                    {
                        this._tiles[column, row] = new Floor(column, row);
                    }
                }
            }
        }

        // Constructor for converting level based on JSON data
        public Level(String levelName, double levelDimensionX, double levelDimensionY, List<List<double>> tileIDs, List<List<double>> spawnpointStates)
        {
            this._levelName = levelName;
            this._levelDimensions.X = levelDimensionX;
            this._levelDimensions.Y = levelDimensionY;
            GlobalSettings.LevelDimensions = this._levelDimensions;

            // Initialise new 2D array of tiles
            this._tiles = new Tile[Convert.ToInt32(this._levelDimensions.X), Convert.ToInt32(this._levelDimensions.Y)];

            // Set up tiles from converted json data
            for (int column = 0; column < this._levelDimensions.X; column++)
            {
                for (int row = 0; row < this._levelDimensions.Y; row++)
                {
                    switch (tileIDs[column][row])
                    {
                        case 0:
                            this._tiles[column, row] = new Wall(column, row);
                            
                            break;
                        case 1:
                            this._tiles[column, row] = new Floor(column, row);
                            break;
                    }
                    this._tiles[column, row].SpawnpointState = Convert.ToInt32(spawnpointStates[column][row]);
                }
            }

            // If the program is in game mode, then spawn the characters and items
            if (GlobalSettings.ProgramMode == 1)
            {
                // Spawn the player
                this._player = new Player(this.GetPlayerSpawnPoint());

                // Spawn the enemies
                this._enemies = new Enemy[this.GetEnemySpawnPoints().Length];
                for (int i = 0; i < this.GetEnemySpawnPoints().Length; i++)
                {
                    this._enemies[i] = new Enemy(this.GetEnemySpawnPoints()[i]);
                }

                // Spawn the coins
                this._coins = new Coin[this.GetCoinSpawnPoints().Length];
                for (int i = 0; i < this.GetCoinSpawnPoints().Length; i++)
                {
                    this._coins[i] = new Coin(this.GetCoinSpawnPoints()[i]);
                }
            }
        }

        public Tile[,] Tiles
        {
            get { return this._tiles; }
        }

        public String LevelName
        { 
            get { return this._levelName; }
        }

        public Vector2D LevelDimensions
        {
            get
            {
                Vector2D vector;
                vector.X = this._levelDimensions.X;
                vector.Y = this._levelDimensions.Y;
                return vector;
            }
        }

        public Player Player
        { 
            get { return this._player; }
        }

        public Enemy[] Enemies
        {
            get { return this._enemies; }
        }

        public Coin[] Coins
        {
            get { return this._coins; }
        }

        // Update the level. This is only called by the game class because the characters require their update procedures to be called.
        // For the editor this is unncessary as there are no characters. Just their spawns.
        public void Update()
        {
            // Update player and enemies
            Vector2D currentTileLocation = ProgramMath.WindowPositionToTile(this._player.WindowLocation);
            this._player.Update(this.GetTile(currentTileLocation), this.GetSurroundingTiles(this._player));
            foreach (Enemy enemy in this._enemies)
            {
                currentTileLocation = ProgramMath.WindowPositionToTile(enemy.WindowLocation);
                enemy.Update(this.GetTile(currentTileLocation), this.GetSurroundingTiles(enemy));
            }
        }

        // Check if the inputted tile is an outer wall tile. This is used to prevent changes being made to outer walls
        private bool IsOuterWallTile(Tile tile)
        {
            return tile.Location.X == 0 || tile.Location.X == this._levelDimensions.X || tile.Location.Y == 0 || tile.Location.Y == this._levelDimensions.Y;
        }

        private bool IsOuterWallTile(int column, int row)
        {
            return column == 0 || column == this._levelDimensions.X - 1 || row == 0 || row == this._levelDimensions.Y - 1;
        }

        private bool IsOuterWallTile(Vector2D location)
        {
            return IsOuterWallTile(Convert.ToInt32(location.X), Convert.ToInt32(location.Y));
        }

        // Check if the tile being retrieved is out of bounds
        private bool RequestedTileOutOfBounds(int column, int row)
        {
            if (column < 0 || column >= GlobalSettings.LevelDimensions.X || row < 0 || row >= GlobalSettings.LevelDimensions.Y)
                return true;
            else return false;
        }

        // Get tile at specified coordinates
        public Tile GetTile(int column, int row)
        {
            if (!RequestedTileOutOfBounds(column, row))
                return this._tiles[column, row];
            else return null;
        }

        public Tile GetTile(Vector2D location)
        {
            return this.GetTile(Convert.ToInt32(location.X), Convert.ToInt32(location.Y));
        }

        // Convert a tile to a different type at the specified location
        private void SetTile(int column, int row, Tile tile)
        {
            this._tiles[column, row] = tile;
        }

        private void SetTile(Vector2D location, Tile tile)
        {
            this.SetTile(Convert.ToInt32(location.X), Convert.ToInt32(location.Y), tile);
        }

        // Place the tile in the level
        public void PlaceInLevel(Vector2D location, Tile tile)
        {
            if (!this.IsOuterWallTile(tile) && !(this.GetTile(location).GetType() == tile.GetType()))
            {
                this.SetTile(location, tile);
            }
        }

        // Returns an array of all spawnable tiles. The spawnableTilesFilter parameter lets us specify if we want spawnable tiles for the player, enemies, or coins
        public Tile[] GetSpawnableTiles(int spawnableTilesFilter)
        {
            List<Tile> spawnableTiles = new List<Tile>();

            if (spawnableTilesFilter != 0)
            {
                foreach (Tile tile in this._tiles)
                {
                    switch (spawnableTilesFilter)
                    {
                        case 1:
                            if (tile.IsSpawnableForPlayer)
                            {
                                spawnableTiles.Add(tile);
                            }
                            break;
                        case 2:
                            if (tile.IsSpawnableForEnemy)
                            {
                                spawnableTiles.Add(tile);
                            }
                            break;
                        case 3:
                            if (tile.IsSpawnableForCoin)
                            {
                                spawnableTiles.Add(tile);
                            }
                            break;
                    }
                }
            }
            return spawnableTiles.ToArray();
        }

        // Returns the tile that is set as the player's spawnpoint
        public Tile GetPlayerSpawnPoint()
        {
            foreach (Tile tile in this._tiles)
            {
                if (tile.SpawnpointState == 1)
                    return tile;
            }
            return null;
        }

        // Returns an array of tiles that are set as the enemies' spawnpoints
        public Tile[] GetEnemySpawnPoints()
        {
            List<Tile> tiles = new List<Tile>();

            foreach (Tile tile in this._tiles)
            {
                if (tile.SpawnpointState == 2)
                {
                    tiles.Add(tile);
                }
            }

            return tiles.ToArray();
        }

        // Returns an array of tiles that are set as the coins' spawnpoints
        public Tile[] GetCoinSpawnPoints()
        {
            List<Tile> tiles = new List<Tile>();

            foreach (Tile tile in this._tiles)
            {
                if (tile.SpawnpointState == 3)
                {
                    tiles.Add(tile);
                }
            }

            return tiles.ToArray();
        }

        // Set the tile that will be the spawnpoint for the player
        public void SetPlayerSpawnPoint(Tile tile)
        {
            if (tile.IsSpawnableForPlayer && tile.SpawnpointState != 2 && tile.SpawnpointState != 3)
            {
                switch (tile.SpawnpointState)
                {
                    case 0:  
                        if (this.GetPlayerSpawnPoint() != null)
                        {
                            this.GetPlayerSpawnPoint().SpawnpointState = 0;
                        }
                        tile.SpawnpointState = 1;
                        break;
                    case 1:
                        tile.SpawnpointState = 0;
                        break;
                }
            }
        }

        // Add or remove an ememy spawnpoint
        public void SetEnemySpawnPoint(Tile tile)
        {
            if (tile.IsSpawnableForEnemy && tile.SpawnpointState != 1 && tile.SpawnpointState != 3)
            {
                switch (tile.SpawnpointState)
                {
                    case 0:
                        tile.SpawnpointState = 2;
                        break;
                    case 2:
                        tile.SpawnpointState = 0;
                        break;
                }
            }
        }

        // Add or remove a coin spawnpoint
        public void SetCoinSpawnPoint(Tile tile)
        {
            if (tile.IsSpawnableForCoin && tile.SpawnpointState != 1 && tile.SpawnpointState != 2)
            {
                switch (tile.SpawnpointState)
                {
                    case 0:
                        tile.SpawnpointState = 3;
                        break;
                    case 3:
                        tile.SpawnpointState = 0;
                        break;
                }
            }
        }

        // Returns an array of the tiles surrounding a character
        private Tile[] GetSurroundingTiles(Character character)
        {
            List<Tile> surroundingTiles = new List<Tile>();
            Vector2D currentTileLocation = new Vector2D();
            
            for (int column = -1; column <= 1; column++)
            {
                for (int row = -1; row <= 1; row++)
                {
                    // Don't add the middle tile as this is the tile the character is standing on
                    if (!(column == 0 && row == 0))
                    {
                        /* At the moment, the game will crash because it always expects there to be 8 tiles surrounding a character
                        However this will not be the case if the character is touching the edge of the screen.
                        Under normal circumstances during gameplay this would be impossible anyway since the outer tiles
                        surrounding the level are always wall tiles. A character could never surpass them.
                        This was the most recent problem I ran into before starting the report. */
                        currentTileLocation.X = character.CurrentTile.Location.X + column;
                        currentTileLocation.Y = character.CurrentTile.Location.Y + row;
                        if (this.GetTile(currentTileLocation) != null)
                        {
                            surroundingTiles.Add(this.GetTile(currentTileLocation));
                        }
                    }
                }
            }

            return surroundingTiles.ToArray();
        }
    }

    abstract class Tile
    {
        // A number that uniquely identifies the type of tile. 0 = Wall, 1 = Floor
        protected int _tileID;
        // Name of the bitmap the renderer has to draw to display this tile
        protected String _bitmapName;
        // Location of the tile in the level
        private Vector2D _location;
        // Can this type of tile be spawned on
        private bool _isSpawnable;
        // Can this type of tile be spawned on for the player
        protected bool _isSpawnableForPlayer;
        // Can this type of tile be spawned on for enemies
        protected bool _isSpawnableForEnemy;
        // Can this type of tile be spawned on for coins
        protected bool _isSpawnableForCoin;
        // What current will be spawned on this tile. 0 = Nothing, 1 = Player, 2 = Enemy, 3 = Coin
        protected int _spawnpointState;

        // Constructors
        public Tile(int tileLocationX, int tileLocationY)
        {
            this._location.X = tileLocationX;
            this._location.Y = tileLocationY;
            this._isSpawnable = this._isSpawnableForPlayer || this._isSpawnableForEnemy || this._isSpawnableForCoin;
            this._spawnpointState = 0;
        }

        public Tile(Vector2D tileLocation)
        {
            this._location.X = tileLocation.X;
            this._location.Y = tileLocation.Y;
            this._isSpawnable = this._isSpawnableForPlayer || this._isSpawnableForEnemy || this._isSpawnableForCoin;
            this._spawnpointState = 0;
        }

        public int TileID
        { 
            get { return this._tileID; }
        }

        public String BitmapName
        {
            get { return this._bitmapName; }
        }

        public Vector2D Location
        { 
            get { return this._location; }
        }
        
        public bool IsSpawnable
        {
            get { return this._isSpawnable; }
        }

        public bool IsSpawnableForPlayer
        {
            get { return this._isSpawnableForPlayer; }
        }
        public bool IsSpawnableForEnemy
        {
            get { return this._isSpawnableForEnemy; }
        }

        public bool IsSpawnableForCoin
        {
            get { return this._isSpawnableForCoin; }
        }
        
        public int SpawnpointState
        { 
            get { return this._spawnpointState; }
            set { this._spawnpointState = value; }
        }
    }

    class Wall : Tile
    {
        public Wall(int tileLocationX, int tileLocationY) : base(tileLocationX, tileLocationY)
        {
            this._tileID = 0;
            // Set bitmap
            this._bitmapName = "TestWall";
            this._isSpawnableForPlayer = false;
            this._isSpawnableForEnemy = false;
            this._isSpawnableForCoin = false;
        }

        public Wall(Vector2D tileLocation) : base(tileLocation)
        {
            this._tileID = 0;
            // Set bitmap
            this._bitmapName = "TestWall";
            this._isSpawnableForPlayer = false;
            this._isSpawnableForEnemy = false;
            this._isSpawnableForCoin = false;
        }
    }

    class Floor : Tile
    {
        public Floor(int tileLocationX, int tileLocationY) : base(tileLocationX, tileLocationY)
        {
            this._tileID = 1;
            // Set bitmap
            this._bitmapName = "TestFloor";
            this._isSpawnableForPlayer = true;
            this._isSpawnableForEnemy = true;
            this._isSpawnableForCoin = true;
        }

        public Floor(Vector2D tileLocation) : base(tileLocation)
        {
            this._tileID = 1;
            // Set bitmap
            this._bitmapName = "TestFloor";
            this._isSpawnableForPlayer = true;
            this._isSpawnableForEnemy = true;
            this._isSpawnableForCoin = true;
        }
    }
}
