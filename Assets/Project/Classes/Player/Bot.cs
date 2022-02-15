﻿using System;
using System.Threading.Tasks;
using Project.Classes.Field;

namespace Project.Classes.Player {
    public abstract class Bot : Player {
        public Bot(Pawn pawn = null) : base(pawn) { }
        
        public sealed override async Task MakeMove() {
            await Task.Delay(100);
            GetNextMove()();
            await base.MakeMove();
        }

        protected abstract Action GetNextMove();
    }
}