using System;
using System.Threading.Tasks;

namespace Project.Classes {
    public abstract class Bot : Player {
        public Bot(Pawn pawn = null) : base(pawn) { }
        
        public sealed override Task MakeMove() {
            GetNextMove()();
            return base.MakeMove();
        }

        protected abstract Action GetNextMove();
    }
}