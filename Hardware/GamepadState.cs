namespace gPadX.Hardware {
    class GamepadState {
        public const int DEFAULT_AXIS_VALUE = 128;

        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public long LX { get; set; } = DEFAULT_AXIS_VALUE;
        public long LY { get; set; } = DEFAULT_AXIS_VALUE;

        public long RX { get; set; } = DEFAULT_AXIS_VALUE;
        public long RY { get; set; } = DEFAULT_AXIS_VALUE;

        public bool A { get; set; }
        public bool B { get; set; }
        public bool X { get; set; }
        public bool Y { get; set; }

        public bool Start { get; set; }
        public bool Select { get; set; }

        public bool L1 { get; set; }
        public bool L2 { get; set; }
        public bool L3 { get; set; }

        public bool R1 { get; set; }
        public bool R2 { get; set; }
        public bool R3 { get; set; }

        public bool IsDefault {
            get { 
                if (Up || Down || Left || Right) { return false; }
                if (LX != DEFAULT_AXIS_VALUE || LY != DEFAULT_AXIS_VALUE || RX != DEFAULT_AXIS_VALUE || RY != DEFAULT_AXIS_VALUE) { return false; }
                if (A || B || X || Y) { return false; }
                if (Start || Select) { return false; }
                if (L1 || L2 || L3 || R1 || R2 || R3) { return false; }

                return true;
            }
        }

        public override bool Equals(object obj) {
            if (obj is GamepadState state) {
                if (Up != state.Up) { return false; }
                if (Down != state.Down) { return false; }
                if (Left != state.Left) { return false; }
                if (Right != state.Right) { return false; }

                if (LX != state.LX) { return false; }
                if (LY != state.LY) { return false; }
                if (RX != state.RX) { return false; }
                if (RY != state.RY) { return false; }

                if (A != state.A) { return false; }
                if (B != state.B) { return false; }
                if (X != state.X) { return false; }
                if (Y != state.Y) { return false; }

                if (Start != state.Start) { return false; }
                if (Select != state.Select) { return false; }

                if (L1 != state.L1) { return false; }
                if (L2 != state.L2) { return false; }
                if (L3 != state.L3) { return false; }
                if (R1 != state.R1) { return false; }
                if (R2 != state.R2) { return false; }
                if (R3 != state.R3) { return false; }

                return true;
            } else {
                return base.Equals(obj);
            }
        }
    }
}
