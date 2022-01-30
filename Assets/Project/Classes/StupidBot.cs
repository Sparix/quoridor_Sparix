using System;

namespace Project.Classes {
    public class StupidBot : Bot {
        protected override Action GetNextMove() {
            return () => TryMovePawn(Pawn.GetPossibleDirections()[0]);
        }
    }
}