﻿using System;
using System.Collections.Generic;
using System.Linq;
using BlockType = Project.Classes.FieldSpace.BlockType;

namespace Project.Classes {
    public class Pawn {
        public static int NumOfCreatedPawns; // todo nullify when restart

        public enum Priority {
            First = 0,
            Second = 1,
            Third = 2,
            Fourth = 3
        }

        public Point Pos { get; private set; }

        public Player Owner { get; }
        public Field Field { get; }

        public Priority PawnPriority { get; }

        public Predicate<Point> WinnerCondition { get; private set; }
        public int Y => Pos.Y;
        public int X => Pos.X;

        public bool IsWinner => WinnerCondition(Pos);

        private List<Point> _possibleDirs = new List<Point>();
        private bool _possibleDirsUpdated;

        public List<Point> PossibleDirections =>
            _possibleDirsUpdated ? new List<Point>(_possibleDirs) : GetPossibleDirections();

        public Pawn(Point pos, Player owner, Field field, Predicate<Point> winCondition) {
            Pos = pos;
            Owner = owner;
            Field = field;
            PawnPriority = (Priority) NumOfCreatedPawns;
            WinnerCondition = winCondition;
            NumOfCreatedPawns += 1;
        }

        ~Pawn() {
            NumOfCreatedPawns -= 1; // todo test is it works
        }

        public bool TryMove(Point newPos) {
            if (!PossibleDirections.Contains(newPos)) return false;
            Pos = newPos;
            _possibleDirsUpdated = false;
            return true;
        }

        public List<Point> GetPossibleDirections() {
            _possibleDirs.Clear();
            var other = GetOther;
            LookLeft(other);
            LookRight(other);
            LookUp(other);
            LookDown(other);
            _possibleDirsUpdated = true;
            return _possibleDirs;
        }


        private void LookLeft(List<Pawn> other) {
            if (X == 0 || Field.FieldSpaces[Y, X - 1].Type == BlockType.Wall) {
                return;
            }

            if ( /*Field.FieldSpaces[Y, X - 2].Type == BlockType.Platform && */!IsPosTaken(other, Y, X - 2)) {
                _possibleDirs.Add(new Point(Y, X - 2));
                return;
            }

            if (X >= 4 && Field.FieldSpaces[Y, X - 3].Type != BlockType.Wall && !IsPosTaken(other, Y, X - 4)) {
                _possibleDirs.Add(new Point(Y, X - 4));
                return;
            }

            if ( /*Y >= 2 && */
                Field.FieldSpaces[Y - 1, X - 2].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X - 2)) {
                _possibleDirs.Add(new Point(Y - 2, X - 2));
            }

            if ( /*Y <= Field.FieldSpaces.GetLength(0) - 3 && */
                Field.FieldSpaces[Y + 1, X - 2].Type != BlockType.Wall && !IsPosTaken(other, Y + 2, X - 2)) {
                _possibleDirs.Add(new Point(Y + 2, X - 2));
            }
        }


        private void LookRight(List<Pawn> other) {
            var xLen = Field.FieldSpaces.GetLength(1);
            if (X == xLen - 1 || Field.FieldSpaces[Y, X + 1].Type == BlockType.Wall) {
                return;
            }

            if ( /*Field.FieldSpaces[Y, X + 2].Type == BlockType.Platform && */!IsPosTaken(other, Y, X + 2)) {
                _possibleDirs.Add(new Point(Y, X + 2));
                return;
            }

            if (X <= xLen - 5 && Field.FieldSpaces[Y, X + 3].Type != BlockType.Wall && !IsPosTaken(other, Y, X + 4)) {
                _possibleDirs.Add(new Point(Y, X + 4));
                return;
            }

            if ( /*Y >= 2 && */
                Field.FieldSpaces[Y - 1, X - 2].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X - 2)) {
                _possibleDirs.Add(new Point(Y - 2, X - 2));
            }

            if ( /*Y <= Field.FieldSpaces.GetLength(0) - 3 && */
                Field.FieldSpaces[Y + 1, X - 2].Type != BlockType.Wall && !IsPosTaken(other, Y + 2, X - 2)) {
                _possibleDirs.Add(new Point(Y + 2, X - 2));
            }
        }

        private void LookUp(List<Pawn> other) {
            if (Y == 0 || Field.FieldSpaces[Y - 1, X].Type == BlockType.Wall) {
                return;
            }

            if ( /*Field.FieldSpaces[Y - 2, X].Type == BlockType.Platform && */!IsPosTaken(other, Y - 2, X)) {
                _possibleDirs.Add(new Point(Y, X - 2));
                return;
            }

            if (Y >= 4 && Field.FieldSpaces[Y - 3, X].Type != BlockType.Wall && !IsPosTaken(other, Y - 4, X)) {
                _possibleDirs.Add(new Point(Y - 4, X));
                return;
            }

            if ( /*X >= 2 && */
                Field.FieldSpaces[Y - 2, X - 1].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X - 2)) {
                _possibleDirs.Add(new Point(Y - 2, X - 2));
            }

            if ( /*X <= Field.FieldSpaces.GetLength(1) - 3 && */
                Field.FieldSpaces[Y - 2, X + 1].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X + 2)) {
                _possibleDirs.Add(new Point(Y - 2, X + 2));
            }
        }

        private void LookDown(List<Pawn> other) {
            var yLen = Field.FieldSpaces.GetLength(0);
            if (Y == yLen - 1 || Field.FieldSpaces[Y + 1, X].Type == BlockType.Wall) {
                return;
            }

            if ( /*Field.FieldSpaces[Y + 2, X].Type == BlockType.Platform && */!IsPosTaken(other, Y + 2, X)) {
                _possibleDirs.Add(new Point(Y + 2, X));
                return;
            }

            if (Y <= yLen - 5 && Field.FieldSpaces[Y + 3, X].Type != BlockType.Wall && !IsPosTaken(other, Y + 4, X)) {
                _possibleDirs.Add(new Point(Y + 4, X));
                return;
            }

            if ( /*X >= 2 && */
                Field.FieldSpaces[Y - 2, X - 1].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X - 2)) {
                _possibleDirs.Add(new Point(Y - 2, X - 2));
            }

            if ( /*X <= Field.FieldSpaces.GetLength(1) - 3 && */
                Field.FieldSpaces[Y - 2, X + 1].Type != BlockType.Wall && !IsPosTaken(other, Y - 2, X + 2)) {
                _possibleDirs.Add(new Point(Y - 2, X + 2));
            }
        }

        private List<Pawn> GetOther => Field.GetOtherPawns(this);

        private bool IsPosTaken(IEnumerable<Pawn> pawns, int y, int x) {
            return pawns.Any(pawn => pawn.Y == y && pawn.X == x);
        }
    }
}