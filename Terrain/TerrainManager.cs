using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Terrain
{
    class TerrainManager
    {
        public VertexBuffer vertexBuffer { get; set; }
        public IndexBuffer indexBuffer { get; set; }
        public int TerrainWidth { get; set; }
        public int TerrainHeight { get; set; }
        public VertexPositionColorNormal[] Vertices { get; set; }
        public short[] Indices { get; set; }
        public Texture2D HeightMap { get; set; }

        //textured stuff
        public Texture2D TerrainTextures { get; set; }
        public List<VertexPositionNormalTexture> textureList { get; set; }

        //height stuff
        public float[,] HeightData;


        public TerrainManager()
        {
            textureList = new List<VertexPositionNormalTexture>();
        }
        public void Load(GraphicsDevice device, ContentManager Content)
        {
            HeightMap = Content.Load<Texture2D>("heightmap"); //128 * 128
            //HeightMap = Content.Load<Texture2D>("heightmap2"); //32 * 32
            TerrainTextures = Content.Load<Texture2D>("terrainTextures");

            LoadHeightData(HeightMap);
            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
        }
        private int LowerOrHigher(int x, int y)
        {
            return 0;
        }
        private void SetUpVertices()
        {
            Vertices = new VertexPositionColorNormal[TerrainWidth * TerrainHeight];

            int numberOfTextures = 3;
            int divider = 2;
            for (int y = 0; y < TerrainHeight; y++)
            {
                for (int x= 0; x < TerrainWidth; x++)
                {
                    //change the divider to heighData[x, y] to reduce hill sizes
                    Vertices[x + y * TerrainWidth].Position = new Vector3(x, (HeightData[x, y]) / divider, -y);
                    WaterSand(x + y * TerrainWidth);
                  
                    if (x != 0 && x != TerrainWidth - 1 && y != TerrainHeight-1)
                    {
                        /********************FIRST TRIANGLE*************************************************/
                        if (HeightData[x, y] == 0 && HeightData[x, y + 1] == 0 && HeightData[x + 1, y] == 0)
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y), new Vector3(0,1,0), new Vector2(1f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y+1] / divider, -y-1), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x+1, HeightData[x+1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 1)));
                        }
                        else if(HeightData[x, y] == 0 || HeightData[x, y + 1] == 0 || HeightData[x + 1, y] == 0)
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y+1] / divider, -y-1), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x+1, HeightData[x+1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(3f / numberOfTextures, 1)));
                        }
                        else
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(0f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y+1] / divider, -y-1), new Vector3(0, 1, 0), new Vector2(0f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x+1, HeightData[x+1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 1)));
                        }


                        /*************************SECOND TRIANGLE**********************************************/
                        if ((HeightData[x, y + 1] == 0 && HeightData[x + 1, y + 1] == 0 && HeightData[x + 1, y] == 0))
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x+1, HeightData[x+1, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x+1, HeightData[x+1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 0)));
                        }
                        else if((HeightData[x, y + 1] == 0 || HeightData[x + 1, y + 1] == 0 || HeightData[x + 1, y] == 0))
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x + 1, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(3f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x + 1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(3f / numberOfTextures, 0)));
                        }
                        else
                        {
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(0f / numberOfTextures, 0)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x + 1, y + 1] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 1)));
                            textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x + 1, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 0)));
                        }
                        /**************************************************************************************/
                        
                    }
                    else
                    {
                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 0)));
                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 1)));
                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 1)));

                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x, HeightData[x, y] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(1f / numberOfTextures, 0)));
                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x, y] / divider, -y - 1), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 1)));
                        textureList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, HeightData[x, y] / divider, -y), new Vector3(0, 1, 0), new Vector2(2f / numberOfTextures, 0)));
                    }
              
                }
            }
        }
        private void SetUpIndices()
        {
            Indices = new short[(TerrainWidth - 1) * (TerrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < TerrainHeight - 1; y++)
            {
                for (int x = 0; x < TerrainWidth - 1; x++)
                {
                    short lowerLeft = (short)(x + y * TerrainWidth);
                    short lowerRight = (short)((x + 1) + y * TerrainWidth);
                    short topLeft = (short)(x + (y + 1) * TerrainWidth);
                    short topRight = (short)((x + 1) + (y + 1) * TerrainWidth);

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
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

        

        private void LoadHeightData(Texture2D heightMap)
        {
            TerrainWidth = heightMap.Width;
            TerrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[TerrainWidth * TerrainHeight];
            heightMap.GetData(heightMapColors);

            HeightData = new float[TerrainWidth, TerrainHeight];
            for (int x = 0; x < TerrainWidth; x++)
                for (int y = 0; y < TerrainHeight; y++)
                    HeightData[y, x] = (heightMapColors[x + y * TerrainWidth].G) / 64;
        }

        private void CalculateNormals()
        {
            /*int length = Vertices.Length - 3;
            for (int i = 0; i < length; i++)
            {
                
                Vector3 a, b, c, Normal;
                a = Vertices[i].Position - Vertices[i + 2].Position;
                b = Vertices[i + 1].Position - Vertices[i].Position;
                c = Vertices[i + 1].Position - Vertices[i + 2].Position;

                Normal = Vector3.Cross(a, c);

                for (int it = i; i < i + 3; i++) 
                    Vertices[i].Normal += Normal;
                for (int it = i; i < i + 3; i++)
                    Vertices[i].Normal.Normalize();
            }*/
            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i].Normal = new Vector3(0, 0, 0);

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
                Vertices[i].Normal.Normalize();

            
        }


    }
    
}
