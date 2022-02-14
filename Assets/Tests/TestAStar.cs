using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project.Classes;
using Project.Classes.Field;
using Project.Classes.Pathfinding;
using UnityEngine;

namespace Tests {
    public class TestAStar {
        [Test]
        public void FindPathTest1() {
            var field = new Field(3, 3);
            var correctPath = new[] {new Point(2, 2), new Point(0, 2)};
            var path = AStar<FieldSpace>.FindPath(field.FieldSpaces, new Point(4, 2), new Point(0, 2),
                Point.ManhattanLengthFloat);
            Console.WriteLine(path.ToString());
            Assert.AreEqual(correctPath, path);
        }

        [Test]
        public void FindPathTest2() {
            var field = new Field(3, 3);
            Assert.IsTrue(field.TrySetWall(new Wall(1, 1, Wall.Type.Horizontal)));
            var correctPathLen = 6;
            var path = AStar<FieldSpace>.FindPath(field.FieldSpaces, new Point(0, 0), new Point(4, 0),
                Point.ManhattanLengthFloat);
            Assert.IsTrue(path.Count == correctPathLen);
        }

        [Test]
        public void FindPathTest3() {
            var field = new Field(5, 5);
            Assert.IsTrue(field.TrySetWall(new Wall(1, 1, Wall.Type.Horizontal)));
            Assert.IsTrue(field.TrySetWall(new Wall(1, 5, Wall.Type.Vertical)));
            Assert.IsTrue(field.TrySetWall(new Wall(3, 3, Wall.Type.Horizontal)));
            Assert.IsTrue(field.TrySetWall(new Wall(5, 1, Wall.Type.Horizontal)));
            var correctPathLen = 10;
            var path = AStar<FieldSpace>.FindPath(field.FieldSpaces, new Point(0, 0), new Point(8, 4), Point.ManhattanLengthFloat);
            Assert.IsTrue(path.Count == correctPathLen);
        }
    }
}