using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terrain
{
    class MapDisplay
    {
        public static int size = 15;

        public VertexPositionColorNormal[] Vertices { get; set; }
        public VertexPositionColorNormal[] WaterVertices { get; set; }
        public VertexPositionColorNormal[] WireFrameVertices { get; set; }

        public VertexPositionColorNormal[] Verts { get; set; }

        public Color[] ImageData = new Color[240 * 480];

        public int[] Indices { get; set; }
        public List<Vector3> Trees;
        public Color[] GetImageData(Color[] colorData, int Value)
        {
            Color[] color = new Color[1];
            color[0] = colorData[Value];
            return color;
        }

        public void GetTrees(List<Vector3> trees, Vector2 MapPointer)
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
                    

        }
        public void GetVertices(VertexPositionColorNormal[] map, Vector2 MapPointer)
        {
            Vertices = new VertexPositionColorNormal[size * size];
            WaterVertices = new VertexPositionColorNormal[size * size];
            WireFrameVertices = new VertexPositionColorNormal[size * size];
            Verts = new VertexPositionColorNormal[((size-1) * (size-1)) * 24];
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
                }
            }

            int counter = 0;
            for(int x = 1; x < size-1; x+=2)
            {
                for (int y = 1; y < size-1; y += 2)
                {
                    int topLeft = (x - 1) + (y + 1) * size;
                    int middleLeft = (x - 1) + y * size;
                    int lowerLeft = (x - 1) + (y - 1) * size;

                    int topMiddle = x + (y + 1) * size;
                    int middleMiddle = x + y * size;
                    int lowerMiddle = x + (y - 1) * size;

                    int topRight = (x + 1) + (y + 1) * size;
                    int middleRight = (x + 1) + y * size;
                    int lowerRight = (x + 1) + (y - 1) * size;

                    //lowerMiddleleft
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[lowerMiddle].Position;
                    Verts[counter++].Position = Vertices[lowerLeft].Position;
                    WaterSand(counter - 3);
                    //set1(Vertices[lowerMiddle].Position.Y,
                    //    Vertices[lowerLeft].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //lowerleft
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[lowerLeft].Position;
                    Verts[counter++].Position = Vertices[middleLeft].Position;
                    WaterSand(counter - 3);
                    //set2(Vertices[topLeft].Position.Y,
                    //    Vertices[lowerLeft].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //lowerright
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[middleRight].Position;
                    Verts[counter++].Position = Vertices[lowerRight].Position;
                    WaterSand(counter - 3);
                    //set4(Vertices[middleRight].Position.Y,
                    //    Vertices[lowerRight].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //lowermiddleright
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[lowerRight].Position;
                    Verts[counter++].Position = Vertices[lowerMiddle].Position;
                    WaterSand(counter - 3);
                    //set3(Vertices[lowerMiddle].Position.Y,
                    //    Vertices[lowerRight].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //topleft
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[middleLeft].Position;
                    Verts[counter++].Position = Vertices[topLeft].Position;
                    WaterSand(counter - 3);
                    //set3(Vertices[middleLeft].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    Vertices[topLeft].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //topmiddleleft
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[topLeft].Position;
                    Verts[counter++].Position = Vertices[topMiddle].Position;
                    WaterSand(counter - 3);
                    //set4(Vertices[topMiddle].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    Vertices[topLeft].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //topright
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[topMiddle].Position;
                    Verts[counter++].Position = Vertices[topRight].Position;
                    WaterSand(counter - 3);
                    //set2(Vertices[topMiddle].Position.Y,
                    //    Vertices[middleMiddle].Position.Y,
                    //    Vertices[topRight].Position.Y,
                    //    counter);
                    SetNormal(counter - 3);

                    //topmiddleright
                    Verts[counter++].Position = Vertices[middleMiddle].Position;
                    Verts[counter++].Position = Vertices[topRight].Position;
                    Verts[counter++].Position = Vertices[middleRight].Position;
                    WaterSand(counter - 3);
                    //set1(Vertices[middleRight].Position.Y, 
                    //    Vertices[middleMiddle].Position.Y, 
                    //    Vertices[topRight].Position.Y, 
                    //    counter);
                    SetNormal(counter - 3);
                }
            }
        }
        private void SetNormal(int index)
        {
            Vector3 a, b, c, Normal;
            a = Verts[index].Position - Verts[index + 2].Position;
            b = Verts[index + 1].Position - Verts[index].Position;
            c = Verts[index + 1].Position - Verts[index + 2].Position;

            Normal = Vector3.Cross(a, c);
            
            for(int i = index; i < index+3; i++)
            {
                Verts[i].Normal += Normal;
            }
            for (int i = index; i < index+3; i++)
            {
                Verts[i].Normal.Normalize();
            }
        }
        private void set1(float a, float b, float c, int counterPosition)
        {
            float difference = 0.0f;

            if (a == b && a == c && b == c)
            {

            }
            else if (a > b || a > c)
            {
                //no shading, maybe highlighting?
                difference -= 0.3f;
            }
            else
            {
                if (b > c) difference = b - a;
                else difference = c - a;
            }

            SetColor(difference, counterPosition);

        }
        private void set2(float a, float b, float c, int counterPosition)
        {
            float difference = 0.0f;
            if(a == b && a == c && b == c)
            {

            }
            else if (a < b || a < c)
            {
                //no shading, maybe highlighting?
                difference -= 0.3f;
            }
            else
            {
                if (c == a) difference = 0.3f;
                if (a > c) difference = a - b;
                if (c > b) difference = c - b;
            }
            SetColor(difference, counterPosition);
        }
        private void set3(float a, float b, float c, int counterPosition)
        {
            float difference = 0.0f;
            if (a == b && a == c && b == c)
            {

            }
            else if (b > a || b > c)
            {
                //no shading, maybe highlighting?
                difference -= 0.3f;
            }
            else
            {
                if (b > a) difference = c - a;
                else difference = c - b;
            }
            SetColor(difference, counterPosition);
        }
        private void set4(float a, float b, float c, int counterPosition)
        {
            float difference = 0.0f;
            if (a == b && a == c && b == c)
            {

            }
            else if (c < a || c < b)
            {
                //no shading, maybe highlighting?
                difference -= 0.3f;
            }
            else
            {
                if (a > b) difference = c - b;
                else difference = c - a;
            }
            SetColor(difference, counterPosition);
        }
        private void SetColor(float difference, int counterPosition)
        {
            difference *= 96;
            for (int i = counterPosition-3; i < counterPosition; i++)
            {
                Verts[i].Color = new Color(
                    Verts[i].Color.R - (int)difference,
                    Verts[i].Color.G - (int)difference,
                    Verts[i].Color.B - (int)difference,
                    Verts[i].Color.A);

                if (Verts[i].Color.R < 0) Verts[i].Color.R = 0;
                if (Verts[i].Color.G < 0) Verts[i].Color.G = 0;
                if (Verts[i].Color.B < 0) Verts[i].Color.B = 0;
            }
        }
        private void WaterSand(int position)
        {
            int waterCount = 0;
            for(int x = position; x < position+3; x++)
            {
                if(Verts[x].Position.Y == 0)
                {
                    waterCount++;
                }
            }
            if(waterCount == 3)
            {
                for (int x = position; x < position + 3; x++)
                {
                    Verts[x].Color = Color.Blue;
                }
            }
            else if(waterCount > 0)
            {
                for (int x = position; x < position + 3; x++)
                {
                    Verts[x].Color = Color.SandyBrown;
                }
            }
            else
            {
                //check that it has any vertices < 0.5;
                int sandCount = 0;
                for (int x = position; x < position + 3; x++)
                {
                    if(Verts[x].Position.Y < 0.5f)
                    {
                        sandCount++;
                    }
                }

                
                for (int x = position; x < position + 3; x++)
                {
                    if (sandCount > 0)
                    {
                        Verts[x].Color = Color.SandyBrown;
                    }
                    else
                    {
                        Verts[x].Color = new Color(SeasonController.SeasonColour[0],
                            SeasonController.SeasonColour[1] ,
                            SeasonController.SeasonColour[2] ,
                            SeasonController.SeasonColour[3] );
                    }
                }
            }
        }
        public void SetupIndices()
        {
            Indices = new int[(size - 1) * (size - 1) * 24];
            int counter = 0;
            for (int y = 0; y < size - 1; y+=2)
            {
                for (int x = 0; x < size - 1; x+=2)
                {
                    //int lowerLeft = x + y * size;
                    //int lowerRight = (x + 1) + y * size;
                    //int topLeft = x + (y + 1) * size;
                    //int topRight = (x + 1) + (y + 1) * size;
                    int topLeft = x + (y + 2) * size;
                    int middleLeft = x + (y + 1) * size;
                    int lowerLeft = x + y * size;

                    int topMiddle = (x + 1) + (y + 2) * size;
                    int middleMiddle = (x + 1) + (y + 1) * size;
                    int lowerMiddle = (x + 1) + y * size;

                    int topRight = (x + 2) + (y + 2) * size;
                    int middleRight = (x + 2) + (y + 1) * size;
                    int lowerRight = (x + 2) + y * size;

                    //, topleft, middleleft, bottom left
                    //  topmiddle, middlemiddle, bottommiddle
                    //  topright, middleright, bottomright

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = lowerMiddle;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = lowerLeft;
                    Indices[counter++] = middleLeft;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = middleRight;
                    Indices[counter++] = lowerRight;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerMiddle;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = middleLeft;
                    Indices[counter++] = topLeft;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = topLeft;
                    Indices[counter++] = topMiddle;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = topMiddle;
                    Indices[counter++] = topRight;

                    Indices[counter++] = middleMiddle;
                    Indices[counter++] = topRight;
                    Indices[counter++] = middleRight;

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
