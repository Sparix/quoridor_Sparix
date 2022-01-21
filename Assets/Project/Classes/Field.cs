﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BlockType = Project.Classes.FieldSpace.BlockType;

namespace Project.Classes {
    public struct Point {
        public int Y { get; }
        public int X { get; }

        public Point(int y, int x) {
            Y = y;
            X = x;
        }

        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.Y + p2.Y, p1.X + p2.X);
        }

        public static Point operator -(Point p1, Point p2) {
            return new Point(p1.Y - p2.Y, p1.X - p2.X);
        }

        public static int ManhattanLength(Point p1, Point p2) {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        public static float ManhattanLengthFloat(Point p1, Point p2) {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }

    public class Field : ICloneable {
        public int XSize { get; }

        public int YSize { get; }

        public FieldSpace[,] FieldSpaces { get; }
        private List<Pawn> _pawns = new List<Pawn>();
        public List<Pawn> Pawns => new List<Pawn>((Pawn[]) _pawns.ToArray().Clone());

        public event Action OnFieldSizeChanged;

        private Point[] relativePointsHor = {new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0)};
        private Point[] relativePointsVer = {new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)};

        public Field(int xSize, int ySize, params Pawn[] pawns) {
            if (xSize <= 1 || ySize <= 1) {
                throw new Exception("Field sizes too small");
            }

            XSize = xSize;
            YSize = ySize;
            _pawns = pawns.ToList();

            var n = ySize * 2 - 1;
            var m = xSize * 2 - 1;

            FieldSpaces = new FieldSpace[n, m];
            for (var y = 0; y < n; y++) {
                for (var x = 0; x < m; x++) {
                    FieldSpaces[y, x] = y.IsEven() && x.IsEven()
                        ? new FieldSpace(BlockType.Platform)
                        : new FieldSpace();
                }
            }

            OnFieldSizeChanged?.Invoke();
        }

        public Field(Vector2Int sizes, params Pawn[] pawns) : this(sizes.x, sizes.y, pawns) { }

        private Field(FieldSpace[,] fieldSpaces, params Pawn[] pawns) {
            YSize = fieldSpaces.GetLength(0);
            XSize = fieldSpaces.GetLength(1);
            FieldSpaces = fieldSpaces;
            Pawns.AddRange(pawns);
        }

        public bool TryAddPawn(Pawn pawn) {
            // todo
            _pawns.Add(pawn);
            return true;
        }

        public void RemovePawn(Pawn pawn) {
            _pawns.Remove(pawn);
        }

        public List<Pawn> GetOtherPawns(Pawn pawn) {
            return Pawns.Where(p => p != pawn).ToList();
        }


        public bool CanSetWall(Wall wall) {
            return CheckWallPosition(wall) && IsThereWayToFinish(wall);
        }

        private bool CheckWallPosition(Wall wall) {
            if (FieldSpaces[wall.Y, wall.X].Type != BlockType.Empty) {
                return false;
            }

            if (wall.X < 1 ||
                wall.Y < 1 ||
                wall.X >= FieldSpaces.GetLength(1) - 1 ||
                wall.Y >= FieldSpaces.GetLength(0) - 1) {
                return false;
            }

            var relativePoints = wall.WallType switch {
                Wall.Type.Horizontal => relativePointsHor,
                Wall.Type.Vertical => relativePointsVer,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (FieldSpaces[wall.Y + relativePoints[0].Y, wall.X + relativePoints[0].X].Type != BlockType.Empty ||
                FieldSpaces[wall.Y + relativePoints[1].Y, wall.X + relativePoints[1].X].Type != BlockType.Empty ||
                FieldSpaces[wall.Y + relativePoints[2].Y, wall.X + relativePoints[2].X].Type == BlockType.Platform ||
                FieldSpaces[wall.Y + relativePoints[3].Y, wall.X + relativePoints[3].X].Type == BlockType.Platform) {
                return false;
            }

            return true;
        }

        private bool IsThereWayToFinish(Wall wall) {
            var fieldWithNewWall = new Field(FieldSpaces.DeepCopy());
            fieldWithNewWall.SetWall(wall);
            return AStarQuoridor.IsTherePaths(fieldWithNewWall.FieldSpaces, Pawns, Point.ManhattanLengthFloat);
        }


        public bool TrySetWall(Wall wall) {
            if (!CanSetWall(wall)) return false;
            SetWall(wall);
            return true;
        }

        private void SetWall(Wall wall) {
            FieldSpaces[wall.Y, wall.X].Type = BlockType.Wall;
            switch (wall.WallType) {
                case Wall.Type.Horizontal:
                    FieldSpaces[wall.Y, wall.X - 1].Type = BlockType.Wall;
                    FieldSpaces[wall.Y, wall.X + 1].Type = BlockType.Wall;
                    break;
                case Wall.Type.Vertical:
                    FieldSpaces[wall.Y - 1, wall.X].Type = BlockType.Wall;
                    FieldSpaces[wall.Y + 1, wall.X].Type = BlockType.Wall;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Clear() {
            var n = YSize * 2 - 1;
            var m = XSize * 2 - 1;
            for (var y = 0; y < n; y++) {
                for (var x = 0; x < m; x++) {
                    if (FieldSpaces[y, x] == null) {
                        FieldSpaces[y, x] = y.IsEven() && x.IsEven()
                            ? new FieldSpace(BlockType.Platform)
                            : new FieldSpace();
                    }
                    else {
                        FieldSpaces[y, x].Type = y.IsEven() && x.IsEven()
                            ? BlockType.Platform
                            : BlockType.Empty;
                    }
                }
            }
        }

        public override bool Equals(object obj) {
            if (!(obj is Field field)) {
                return false;
            }

            if (XSize != field.XSize || YSize != field.YSize) {
                return false;
            }

            var n = field.FieldSpaces.GetLength(0);
            var m = field.FieldSpaces.GetLength(0);
            if (FieldSpaces.GetLength(0) != n || FieldSpaces.GetLength(1) != m) {
                return false;
            }

            for (var i = 0; i < n; i++) {
                for (var j = 0; j < m; j++) {
                    if (!field.FieldSpaces[i, j].Equals(FieldSpaces[i, j])) {
                        return false;
                    }
                }
            }

            return true;
        }

        public object Clone() {
            var fieldSpaces = (FieldSpace[,]) FieldSpaces.Clone();
            var pawns = (Pawn[]) Pawns.ToArray().Clone();
            return new Field(fieldSpaces, pawns);
        }
    }
}