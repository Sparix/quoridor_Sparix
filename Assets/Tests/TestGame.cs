using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Project.Classes;
using UnityEngine;

namespace Tests {
    public static class UnityTestUtils {
        public static void RunAsyncMethodSync(this Func<Task> asyncFunc) {
            Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }
    }

    public class TestGame {
        [Test]
        public void Test() {
            UnityTestUtils.RunAsyncMethodSync(GameTest);
        }

        [Test]
        public async Task GameTest() {
            var p1 = new LocalPlayer();
            var p2 = new StupidBot();
            var game = new Game(5, 5, new List<Player>(new Player[] {p1, p2}));
            game.StartGame();
            Assert.IsTrue(game.CurrentPlayer == p1);
            Assert.AreEqual(new Point(0, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            game.Tick();
            game.Tick();
            Assert.IsTrue(game.CurrentPlayer == p1);
            Assert.IsTrue(p1.myTurn);
            Assert.AreEqual(new Point(0, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            Assert.IsTrue(p1.TryMovePawn(new Point(2, 4)));
            Assert.AreEqual(new Point(2, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            game.Tick();


            // Assert.IsTrue(game.CurrentPlayer == p2);
            // Assert.IsTrue(p2.myTurn);
            while (p2.myTurn) {
                await Task.Yield();
            }

            // Assert.AreEqual(new Point(2, 4), p1.Pawn.Pos);
            // Assert.AreEqual(new Point(8, 2), p2.Pawn.Pos);
            var flag = false;
            game.GameFinished += player => { flag = player == p1; };
            Assert.IsTrue(p1.TryMovePawn(new Point(4, 4)));
            game.Tick();
            while (p2.myTurn) {
                await Task.Yield();
            }

            Assert.IsTrue(p1.TryMovePawn(new Point(6, 4)));
            game.Tick();
            while (p2.myTurn) {
                await Task.Yield();
            }

            Assert.IsTrue(p1.TryMovePawn(new Point(8, 4)));
            Assert.IsTrue(flag);
        }

        [Test]
        public void GameTest1() {
            var p1 = new LocalPlayer();
            var p2 = new StupidBot();
            var game = new Game(5, 5, new List<Player>(new Player[] {p1, p2}));
            game.StartGame();
            Assert.IsTrue(game.CurrentPlayer == p1);
            Assert.AreEqual(new Point(0, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            game.Tick();
            game.Tick();
            Assert.IsTrue(game.CurrentPlayer == p1);
            Assert.IsTrue(p1.myTurn);
            Assert.AreEqual(new Point(0, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            Assert.IsTrue(p1.TryMovePawn(new Point(2, 4)));
            Assert.AreEqual(new Point(2, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 4), p2.Pawn.Pos);
            game.Tick();
            Assert.IsTrue(game.CurrentPlayer == p2);
            Assert.IsTrue(p2.myTurn);

            Assert.AreEqual(new Point(2, 4), p1.Pawn.Pos);
            Assert.AreEqual(new Point(8, 2), p2.Pawn.Pos);
            // var flag = false;
            // game.GameFinished += player => { flag = player == p1; };
            // Assert.IsTrue(p1.TryMovePawn(new Point(4, 4)));
            // game.Tick();
            // Assert.IsTrue(p1.TryMovePawn(new Point(6, 4)));
            // game.Tick();
            // Assert.IsTrue(p1.TryMovePawn(new Point(8, 4)));
            // Assert.IsTrue(flag);
        }
    }
}