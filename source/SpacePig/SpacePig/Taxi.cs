using Microsoft.Xna.Framework;

namespace SpacePig
{
    public class Taxi
    {
        public const int WIDTH = 50;
        public const int HEIGHT = 25;

        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public bool Landed { get; set; }
    }
}
