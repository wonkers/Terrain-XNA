using Microsoft.Xna.Framework;

namespace Terrain
{
    public class CameraController
    {
        public Vector3 Position;
        public Vector2 MapPosition;
        public CameraController()
        {
            ////diamond view
            //Position = new Vector3(-68.5f, 8, 68.5f);

            //slightly to the side view
            Position = new Vector3(-10, 10, 10);

            MapPosition = new Vector2(0, 0);
        }
    }
}
