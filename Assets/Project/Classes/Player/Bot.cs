using System;
using System.Threading.Tasks;
using Project.Classes.Field;

namespace Project.Classes.Player {
    public abstract class Bot : Player {
        public Bot(Pawn pawn = null) : base(pawn) { }
        
        public sealed override Task MakeMove() {
            GetNextMove()();
            return base.MakeMove();
        }

        protected abstract Action GetNextMove();
    }
}