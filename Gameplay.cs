using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace TreasureHunter
{
    abstract class Character
    {
        // Sprite to draw for character
        protected Sprite _sprite;
        // Tile the character will spawn on
        protected Tile _spawnTile;
        // Current tile the character is standing on
        protected Tile _currentTile;
        // Tiles that currently surround the character
        protected Tile[] _surroundingTiles;
        // Location of character in window space
        protected Vector2D _windowLocation;
        // Rate in which character moves. Measured in units per tick
        protected int _moveSpeed;

        // Constructor. Spawns character on a tile
        public Character(Tile spawnTile)
        {
            this._spawnTile = spawnTile;
            this._currentTile = this._spawnTile;
            this._windowLocation = ProgramMath.TileToWindowPosition(this._spawnTile.Location);
        }

        // Properties
        public Sprite Sprite
        {
            get { return this._sprite; } 
        }

        public Tile SpawnTile 
        { 
            get { return this._spawnTile; } 
        }

        public Tile CurrentTile
        { 
            get { return this._currentTile; }
        }

        public Tile[] SurroundingTiles
        {
            get { return this._surroundingTiles; }
        }

        public Vector2D WindowLocation
        {
            get { return this._windowLocation; }
        }

        public int MoveSpeed 
        {
            get { return this._moveSpeed; } 
            set { this._moveSpeed = value; }
        }

        // Move character
        public virtual void Move(ProgramMath.Direction direction)
        {

            switch (direction)
            {
                case ProgramMath.Direction.Up:
                    this._windowLocation.Y -= this._moveSpeed;
                    break;
                case ProgramMath.Direction.Down:
                    this._windowLocation.Y += this._moveSpeed;
                    break;
                case ProgramMath.Direction.Left:
                    this._windowLocation.X -= this._moveSpeed;
                    break;
                case ProgramMath.Direction.Right:
                    this._windowLocation.X += this._moveSpeed;
                    break;
            }
        }

        // Update the character. Intended to be called as part of the game's update method
        public virtual void Update(Tile currentTile, Tile[] surroundingTiles)
        {
            this._sprite.Position = ProgramMath.Vector2DToPoint2D(this._windowLocation);
            this._sprite.Update();
            this._currentTile = currentTile;
            this._surroundingTiles = surroundingTiles;
        }
    }

    class Player : Character
    {
        // Number of lives the player has. Zero means game over
        private int _lives;

        public Player(Tile spawnTile) : base(spawnTile)
        {
            this._sprite = SplashKit.CreateSprite(SplashKit.BitmapNamed("Player"));
            this._lives = 3;
            this._moveSpeed = 5;
        }

        // Properties
        public int Lives 
        { 
            get { return this._lives; }
        }

        public override void Update(Tile currentTile, Tile[] surroundingTiles)
        {
            base.Update(currentTile, surroundingTiles);
            Console.WriteLine(this._sprite.Position.X + ", " + this._sprite.Position.Y);
        }
    }
    class Enemy : Character
    { 
        public Enemy(Tile spawnTile) : base(spawnTile)
        {
            this._sprite = SplashKit.CreateSprite(SplashKit.BitmapNamed("Enemy"));
            this._sprite.Update();
            this._moveSpeed = 7;
        }
    }

    class Coin
    {
        // Coin sprite
        private Sprite _sprite;
        // Tile the coin will spawn on
        private Tile _spawnTile;

        // Constructor
        public Coin(Tile spawnTile)
        {
            this._spawnTile = spawnTile;
            this._sprite = SplashKit.CreateSprite(SplashKit.BitmapNamed("Coin"));
            this._sprite.Position = ProgramMath.Vector2DToPoint2D(ProgramMath.TileToWindowPosition(this._spawnTile.Location));
        }

        public Sprite Sprite
        {
            get { return this._sprite; }
        }

        public Tile SpawnTile
        {
            get { return this._spawnTile; }
        }
    }
}
