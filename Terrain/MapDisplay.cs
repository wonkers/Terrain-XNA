using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Terrain
{
    class MapDisplay
    {
        public static short size = 16;

        public VertexPositionColorNormal[] Vertices { get; set; }
        public VertexPositionColorNormal[] WaterVertices { get; set; }
        public VertexPositionColorNormal[] WireFrameVertices { get; set; }

        //public Color[] ImageData = new Color[240 * 480];

        public short[] Indices { get; set; }
        //public List<Vector3> Trees;

        /*public Color[] GetImageData(Color[] colorData, int Value)
        {
            Color[] color = new Color[1];
            color[0] = colorData[Value];
            return color;
        }*/

       /* public void GetTrees(List<Vector3> trees, Vector2 MapPointer)
        {
            Trees = new List<Vector3>();
            foreach(Vector3 tree in trees)
            {
                if(tree.X < size + (int)MapPointer.Y-2 &&
                    tree.X > (int)MapPointer.Y)
                {
                    if(tree.Z < (size + (int)MapPointer.X) &&
                        tree.Z > (int)MapPointer.X)
                    {
                        float f = ((tree.Z - MapPointer.X) + (tree.X - MapPointer.Y) * size) - 1;
                        float h = Vertices[(int)f].Position.Y;
                        Trees.Add(new Vector3(tree.Z-MapPointer.X, h, -(tree.X-MapPointer.Y)));
                        
                    }
                }
            }
                    

        }*/
        public void GetVertices(VertexPositionColorNormal[] map, Vector2 MapPointer)
        {
            Vertices = new VertexPositionColorNormal[size * size];
            WaterVertices = new VertexPositionColorNormal[size * size];
            WireFrameVertices = new VertexPositionColorNormal[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vertices[x + y * size].Position = new Vector3(x, 0, -y);
                    Vertices[x + y * size].Position.Y = map[(x + (int)MapPointer.X) + (y + (int)MapPointer.Y) * 128].Position.Y;
                    WaterVertices[x + y * size].Position = new Vector3(x, 0.001f, -y);

                    WireFrameVertices[x + y * size].Position = Vertices[x + y * size].Position;
                    WireFrameVertices[x + y * size].Position.Y += 0.01f;
                    WireFrameVertices[x + y * size].Color = Color.Black;

                    Vertices[x + y * size].Color = Color.ForestGreen;
                    WaterSand(x + y * size);
                    SetNormal(x + y * size);
                }
            }

        }
        private void SetNormal(int index)
        {
            Vector3 a, b, c, Normal;
            try
            {
                a = Vertices[index].Position - Vertices[index + 2].Position;
                b = Vertices[index + 1].Position - Vertices[index].Position;
                c = Vertices[index + 1].Position - Vertices[index + 2].Position;

                Normal = Vector3.Cross(a, c);

                for (int i = index; i < index + 3; i++)
                {
                    Vertices[i].Normal += Normal;
                }
                for (int i = index; i < index + 3; i++)
                {
                    Vertices[i].Normal.Normalize();
                }
            }
            catch(System.Exception e)
            {
                //out of bounds
            }
        }

        private void WaterSand(int position)
        {
            if (Vertices[position].Position.Y == 0)
                 Vertices[position].Color = Color.Blue;
             else if (Vertices[position].Position.Y < 1)
                 Vertices[position].Color = Color.SandyBrown;
             else
                 Vertices[position].Color = new Color(SeasonController.SeasonColour[0],
                                 SeasonController.SeasonColour[1],
                                 SeasonController.SeasonColour[2],
                                 SeasonController.SeasonColour[3]);

        }
        public void SetupIndices()
        {
            Indices = new short[(size - 1) * (size - 1) * 6];
            int counter = 0;
            for (short y = 0; y < size - 1; y++)
            {
                for (short x = 0; x < size - 1; x++)
                {
                    short lowerLeft = (short)(x + y * size);
                    short lowerRight = (short)((x + 1) + y * size);
                    short topLeft = (short)(x + (y + 1) * size);
                    short topRight = (short)((x + 1) + (y + 1) * size);

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;
                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
            }
        }
        
        public void CalculateNormals()
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = new Vector3(0, 0, 0.1f);
                WaterVertices[i].Normal = new Vector3(0, 0, 0.1f);
                //VerticesPC[i].Normal = new Vector3(0, 0, 1);
            }

            for (int i = 0; i < Indices.Length / 3; i++)
            {
                int index1 = Indices[i * 3];
                int index2 = Indices[i * 3 + 1];
                int index3 = Indices[i * 3 + 2];

                Vector3 side1 = Vertices[index1].Position - Vertices[index3].Position;
                Vector3 side2 = Vertices[index1].Position - Vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                Vertices[index1].Normal += normal;
                Vertices[index2].Normal += normal;
                Vertices[index3].Normal += normal;
            }

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal.Normalize();
                WaterVertices[i].Normal.Normalize();
            }
        }
    }
}
