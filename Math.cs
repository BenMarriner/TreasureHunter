using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;

namespace TreasureHunter
{
    static class ProgramMath
    {
        // Dimensions
        public enum Coordinate
        { 
            X,
            Y
        }

        public enum Direction
        { 
            Up,
            Down,
            Left,
            Right
        }

        // Converts Vector2D to Point2D
        public static Point2D Vector2DToPoint2D(Vector2D vector)
        {
            Point2D point;

            point.X = vector.X;
            point.Y = vector.Y;

            return point;
        }

        // Compares two vectors, returns true if they are the same
        public static bool EqualVectors(Vector2D v1, Vector2D v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            return false;
        }

        // Compares one vector against multiple vectors. the allEqual parameter determines the condition on which the function returns true
        // allEqual = true: All vectors must be the same
        // allEqual = false: Two vectors must be the same
        public static bool EqualVectors(bool allEqual, Vector2D v1, Vector2D[] vectors)
        {
            if (vectors.Length == 0)
            {
                return false;
            }

            foreach (Vector2D v2 in vectors)
            {
                if (allEqual)
                {
                    if (!EqualVectors(v1, v2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (EqualVectors(v1, v2))
                    {
                        return true;
                    }
                } 
            }
            return true;
        }

        // Converts tile coordinates to locations in window space. Mainly used in the renderer classes
        public static Vector2D TileToWindowPosition(double tileCoordinateX, double tileCoordinateY)
        {
            Vector2D newCoordinates;

            newCoordinates.X = GlobalSettings.BitmapResolution * tileCoordinateX;
            newCoordinates.Y = GlobalSettings.BitmapResolution * tileCoordinateY;

            return newCoordinates;
        }

        public static Vector2D TileToWindowPosition(Vector2D tileCoordinates)
        {
            return TileToWindowPosition(tileCoordinates.X, tileCoordinates.Y);
        }

        // Converts a single tile coordinate to a location in window space. Mainly used in the renderer classes
        public static double TileToWindowCoordinate(double tileCoordinate, Coordinate coordinate)
        {
            switch (coordinate)
            {
                case Coordinate.X:
                    return GlobalSettings.RendererWindowLocation.X + GlobalSettings.BitmapResolution * tileCoordinate;
                case Coordinate.Y:
                    return GlobalSettings.RendererWindowLocation.Y + GlobalSettings.BitmapResolution * tileCoordinate;
            }
            return GlobalSettings.BitmapResolution + 48 * tileCoordinate;
        }

        public static double TileToWindowCoordinate(int tileCoordinate, Coordinate coordinate)
        {
            return TileToWindowCoordinate(Convert.ToDouble(tileCoordinate), coordinate);
        }

        // Converts a location in window space to a tile coordinate. Mainly used to find out what tile a character is on
        public static Vector2D WindowPositionToTile(Vector2D windowLocation)
        {
            Vector2D newCoordinates;

            newCoordinates.X = Math.Floor(windowLocation.X / GlobalSettings.BitmapResolution);
            newCoordinates.Y = Math.Floor(windowLocation.Y / GlobalSettings.BitmapResolution);

            return newCoordinates;
        }
    }
}