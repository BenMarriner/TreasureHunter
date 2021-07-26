using SplashKitSDK;

namespace TreasureHunter
{
    abstract class Renderer
    {
        public Renderer()
        {
        }

        // Draw Level in the window
        private void DrawLevel(Level level)
        {
            for (int column = 0; column < level.Tiles.GetLength(0); column++)
            {
                for (int row = 0; row < level.Tiles.GetLength(1); row++)
                {
                    Tile currentTile = level.Tiles[column, row];
                    Vector2D currentWindowLocation = ProgramMath.TileToWindowPosition(column, row);
                    
                    SplashKit.DrawBitmap(currentTile.BitmapName, currentWindowLocation.X, currentWindowLocation.Y);
                }
            }
        }

        // Draws game in the level. The base function for this just renders the level.
        public virtual void DrawGame(Level level)
        {
            this.DrawLevel(level);
        }
    }

    class EditorRenderer : Renderer
    {
        private Editor.Elements _elementsToDraw;
        public EditorRenderer() : base()
        {
        }

        // Draw level and editor elements
        public void DrawGame(Level level, Editor.Elements elementsToDraw, TileSelector tileSelector)
        {
            // Set which elements to draw
            this._elementsToDraw = elementsToDraw;
            // Draw level
            base.DrawGame(level);
            // Draw Tile Grid
            this.DrawTileGrid();

            // Highlight spawnable tiles
            if (this._elementsToDraw.SpawnableTiles > 0)
            {
                this.HighlightSpawnableTiles(level.GetSpawnableTiles(this._elementsToDraw.SpawnableTiles));
            }

            // Spawnpoints
            if (elementsToDraw.Spawnpoints && level.GetPlayerSpawnPoint() != null)
            {
                SplashKit.DrawBitmap
                    ("PlayerSpawn",
                    ProgramMath.TileToWindowCoordinate(level.GetPlayerSpawnPoint().Location.X, ProgramMath.Coordinate.X),
                    ProgramMath.TileToWindowCoordinate(level.GetPlayerSpawnPoint().Location.Y, ProgramMath.Coordinate.Y));
            }

            foreach (Tile spawnpoint in level.GetEnemySpawnPoints())
            {
                SplashKit.DrawBitmap
                    ("EnemySpawn",
                    ProgramMath.TileToWindowCoordinate(spawnpoint.Location.X, ProgramMath.Coordinate.X),
                    ProgramMath.TileToWindowCoordinate(spawnpoint.Location.Y, ProgramMath.Coordinate.Y));
            }

            foreach (Tile spawnpoint in level.GetCoinSpawnPoints())
            {
                SplashKit.DrawBitmap
                    ("Coin",
                    ProgramMath.TileToWindowCoordinate(spawnpoint.Location.X, ProgramMath.Coordinate.X),
                    ProgramMath.TileToWindowCoordinate(spawnpoint.Location.Y, ProgramMath.Coordinate.Y));
            }

            // Tile selector
            if (elementsToDraw.TileSelector)
            {
                SplashKit.DrawBitmap
                    ("EditorCrosshair",
                    tileSelector.SelectedTileWindowLocation.X,
                    tileSelector.SelectedTileWindowLocation.Y);
            }

            // Draw Tile Coordinates
            this.DrawTileCoordinates(level.Tiles);
        }

        // Highlight tiles that can be set as spawnpoints. The mode parameter switches between which tiles can be set as player, enemy or coin spawnpoints
        public void HighlightSpawnableTiles(Tile[] spawnableTiles)
        {
            foreach (Tile tile in spawnableTiles)
            {
                switch (this._elementsToDraw.SpawnableTiles)
                {
                    // Highlight player-spawnable tiles, mark enemy and coin spawnpoints as unspawnable
                    case 1:
                        switch (tile.SpawnpointState)
                        {
                            case 0:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 1:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 2:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 3:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                        }
                        break;
                    // Highlight enemy-spawnable tiles, mark player and coin spawnpoints as unspawnable
                    case 2:
                        switch (tile.SpawnpointState)
                        {
                            case 0:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 1:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 2:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 3:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                        }
                        break;
                    // Highlight coin-spawnable tiles, mark player and enemy spawnpoints as unspawnable
                    case 3:
                        switch (tile.SpawnpointState)
                        {
                            case 0:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 1:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 2:
                                SplashKit.DrawBitmap
                                    ("UnspawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                            case 3:
                                SplashKit.DrawBitmap
                                    ("SpawnableTileHighlight",
                                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X),
                                    ProgramMath.TileToWindowCoordinate(tile.Location.Y, ProgramMath.Coordinate.Y));
                                break;
                        }
                        break;
                }
            }
        }

        // Draw grid lines to mark the tiles
        public void DrawTileGrid()
        {
            Line line = new Line();

            for (int column = 1; column < GlobalSettings.LevelDimensions.X; column++)
            {
                line.StartPoint.X = ProgramMath.TileToWindowCoordinate(0, ProgramMath.Coordinate.X);
                line.StartPoint.Y = ProgramMath.TileToWindowCoordinate(column, ProgramMath.Coordinate.Y);
                line.EndPoint.X = ProgramMath.TileToWindowCoordinate(20, ProgramMath.Coordinate.X);
                line.EndPoint.Y = ProgramMath.TileToWindowCoordinate(column, ProgramMath.Coordinate.Y);

                SplashKit.DrawLine(Color.Black, line);
                for (int row = 1; row < GlobalSettings.LevelDimensions.Y; row++)
                {
                    line.StartPoint.X = ProgramMath.TileToWindowCoordinate(column, ProgramMath.Coordinate.X);
                    line.StartPoint.Y = ProgramMath.TileToWindowCoordinate(0, ProgramMath.Coordinate.Y);
                    line.EndPoint.X = ProgramMath.TileToWindowCoordinate(column, ProgramMath.Coordinate.X);
                    line.EndPoint.Y = ProgramMath.TileToWindowCoordinate(20, ProgramMath.Coordinate.Y);

                    SplashKit.DrawLine(Color.Black, line);
                }
            }
        }

        // Draw each tile's coordinates in the level
        public void DrawTileCoordinates(Tile[,] tiles)
        {
            foreach (Tile tile in tiles)
            {
                SplashKit.DrawText
                    /* Text to draw: */
                    ("(" + tile.Location.X + "," + tile.Location.Y + ")", 
                    /* Font and Font Colour: */
                    Color.Black, GlobalSettings.Neon, 
                    /* Font Size: */
                    20, 
                    /* Coordinates to draw at: */
                    ProgramMath.TileToWindowCoordinate(tile.Location.X, ProgramMath.Coordinate.X), 
                    ProgramMath.TileToWindowCoordinate(tile.Location.Y,ProgramMath.Coordinate.Y));
            }
        }
    }
    
    class GameRenderer : Renderer
    {
        public GameRenderer() : base()
        {
        }

        public override void DrawGame(Level level)
        {
            // Draw level
            base.DrawGame(level);

            // Draw player
            SplashKit.DrawSprite(level.Player.Sprite);

            // Draw enemies
            foreach (Enemy enemy in level.Enemies)
            {
                SplashKit.DrawSprite(enemy.Sprite);
            }

            // Draw coins
            foreach (Coin coin in level.Coins)
            {
                SplashKit.DrawSprite(coin.Sprite);
            }
        }
    }
}
