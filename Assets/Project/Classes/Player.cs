namespace Project.Classes {
    public abstract class Player {
        public Pawn Pawn { get; set; }

        public Player(Pawn pawn = null) {
            Pawn = pawn;
        }
    }
}