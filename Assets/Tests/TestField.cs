using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project.Classes;
using UnityEngine;
using Type = Project.Classes.FieldSpace.BlockType;

namespace Tests {
    public class TestField {
        private readonly Vector2Int _defaultFieldSizes = new Vector2Int(9, 9);

        private readonly FieldSpace[,] _correct3X3FieldSpaces = {
            {
                new FieldSpace(Type.Platform), new FieldSpace(Type.Empty), new FieldSpace(Type.Platform),
                new FieldSpace(Type.Empty), new FieldSpace(Type.Platform)
            }, {
                new FieldSpace(Type.Empty), new FieldSpace(Type.Empty), new FieldSpace(Type.Empty),
                new FieldSpace(Type.Empty), new FieldSpace(Type.Empty)
            }, {
                new FieldSpace(Type.Platform), new FieldSpace(Type.Empty), new FieldSpace(Type.Platform),
                new FieldSpace(Type.Empty), new FieldSpace(Type.Platform)
            }, {
                new FieldSpace(Type.Empty), new FieldSpace(Type.Empty), new FieldSpace(Type.Empty),
                new FieldSpace(Type.Empty), new FieldSpace(Type.Empty)
            }, {
                new FieldSpace(Type.Platform), new FieldSpace(Type.Empty), new FieldSpace(Type.Platform),
                new FieldSpace(Type.Empty), new FieldSpace(Type.Platform)
            }
        };

        [Test]
        public void FieldCreationTest() {
            var field = new Field(3, 3);
            Assert.AreEqual(_correct3X3FieldSpaces, field.FieldSpaces);
        }

        [Test]
        public void IncorrectWallsTest() {
            var field = new Field(3, 3);
            var testWalls = new List<Wall> {
                new Wall(0, 1, Wall.Type.Horizontal),
                new Wall(0, 1, Wall.Type.Vertical),
                new Wall(4, 1, Wall.Type.Horizontal),
                new Wall(4, 1, Wall.Type.Vertical),
                new Wall(1, 0, Wall.Type.Horizontal),
                new Wall(1, 0, Wall.Type.Vertical),
                new Wall(1, 4, Wall.Type.Horizontal),
                new Wall(1, 4, Wall.Type.Vertical)
            };
            testWalls.ForEach(wall => IncorrectWallTest(wall, field, _correct3X3FieldSpaces));
        }

        [Test]
        public void SetWallsTest() {
            var field = new Field(3, 3);
            var correct3X3FieldSpaces = _correct3X3FieldSpaces.DeepCopy();
            var testWall = new Wall(1, 1, Wall.Type.Horizontal);
            correct3X3FieldSpaces[1, 0].Type =
                correct3X3FieldSpaces[1, 1].Type =
                    correct3X3FieldSpaces[1, 2].Type = Type.Wall;
            CorrectWallTest(testWall, field, correct3X3FieldSpaces);

            var testWal2 = new Wall(1, 3, Wall.Type.Horizontal);
            IncorrectWallTest(testWal2, field, correct3X3FieldSpaces);

            var testWal3 = new Wall(1, 3, Wall.Type.Vertical);
            correct3X3FieldSpaces[0, 3].Type =
                correct3X3FieldSpaces[1, 3].Type =
                    correct3X3FieldSpaces[2, 3].Type = Type.Wall;
            CorrectWallTest(testWal3, field, correct3X3FieldSpaces);

            var testWal4 = new Wall(3, 1, Wall.Type.Vertical);
            correct3X3FieldSpaces[2, 1].Type =
                correct3X3FieldSpaces[3, 1].Type =
                    correct3X3FieldSpaces[4, 1].Type = Type.Wall;
            CorrectWallTest(testWal4, field, correct3X3FieldSpaces);

            var testWal5 = new Wall(3, 3, Wall.Type.Horizontal);
            correct3X3FieldSpaces[3, 2].Type =
                correct3X3FieldSpaces[3, 3].Type =
                    correct3X3FieldSpaces[3, 4].Type = Type.Wall;
            CorrectWallTest(testWal5, field, correct3X3FieldSpaces);
        }

        [Test]
        public void WallThatBlocksFinishTest() {
            var field = new Field(3, 3);
            var pawn1 = new Pawn(new Point(0, 2), new LocalPlayer(), field, p => p.Y == field.FieldSpaces.GetLength(0) - 1);
            var pawn2 = new Pawn(new Point(4, 2), new LocalPlayer(), field, p => p.Y == 0);
            field.TryAddPawn(pawn1);
            field.TryAddPawn(pawn2);
            Assert.IsTrue(field.TrySetWall(new Wall(1, 1, Wall.Type.Horizontal)));
            Assert.IsFalse(field.TrySetWall(new Wall(1, 3, Wall.Type.Vertical)));
        }


        private void IncorrectWallTest(Wall testWall, Field actualField, FieldSpace[,] correctFieldSpaces) {
            Assert.IsFalse(actualField.CanSetWall(testWall));
            Assert.IsFalse(actualField.TrySetWall(testWall));
            Assert.AreEqual(correctFieldSpaces, actualField.FieldSpaces);
        }


        private void CorrectWallTest(Wall testWall, Field actualField, FieldSpace[,] correctFieldSpaces) {
            Assert.IsTrue(actualField.CanSetWall(testWall));
            Assert.IsTrue(actualField.TrySetWall(testWall));
            Assert.AreEqual(correctFieldSpaces, actualField.FieldSpaces);
        }

        [Test]
        public void ClearTest() {
            var field = new Field(3, 3);
            var correct3X3FieldSpaces = _correct3X3FieldSpaces.DeepCopy();
            var testWall = new Wall(1, 1, Wall.Type.Horizontal);
            correct3X3FieldSpaces[1, 0].Type =
                correct3X3FieldSpaces[1, 1].Type =
                    correct3X3FieldSpaces[1, 2].Type = Type.Wall;
            CorrectWallTest(testWall, field, correct3X3FieldSpaces);
            field.Clear();
            Assert.AreEqual(_correct3X3FieldSpaces, field.FieldSpaces);
        }

        [Test]
        public void TestPawnMove1() {
            var field = new Field(3, 3);
            var pawn1 = new Pawn(new Point(0, 2), new LocalPlayer(), field, p => p.Y == field.FieldSpaces.GetLength(0) - 1);
            var pawn2 = new Pawn(new Point(4, 2), new LocalPlayer(), field, p => p.Y == 0);
            field.TryAddPawn(pawn1);
            field.TryAddPawn(pawn2);
            Assert.IsTrue(pawn1.TryMove(new Point(2, 2)));
            Assert.IsTrue(pawn2.TryMove(new Point(0, 2)));
        }

        [Test]
        public void TestPawnMove2() {
            var field = new Field(3, 3);
            var pawn1 = new Pawn(new Point(0, 2), new LocalPlayer(), field, p => p.Y == field.FieldSpaces.GetLength(0) - 1);
            var pawn2 = new Pawn(new Point(4, 2), new LocalPlayer(), field, p => p.Y == 0);
            field.TryAddPawn(pawn1);
            field.TryAddPawn(pawn2);
            Assert.IsTrue(pawn1.TryMove(new Point(2, 2)));
            Assert.IsTrue(field.TrySetWall(new Wall(1, 1, Wall.Type.Horizontal)));
            Assert.IsFalse(pawn2.TryMove(new Point(0, 2)));
            Assert.IsTrue(pawn2.TryMove(new Point(2, 0)));
            Assert.IsFalse(pawn1.TryMove(new Point(0, 2)));
        }
    }
}