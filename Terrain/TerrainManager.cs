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
        public int[] Indices { get; set; }
        public Texture2D HeightMap { get; set; }
        public Texture2D TreeMap { get; set; }

        public float[,] HeightData;
        public List<Vector3> TreePositions;

        public TerrainManager()
        {

        }
        public void Load(GraphicsDevice device, ContentManager Content)
        {
            HeightMap = Content.Load<Texture2D>("heightmap");
            TreeMap = Content.Load<Texture2D>("treemap");

            LoadHeightData(HeightMap);
            SetUpVertices();
            //LoadTreeData(TreeMap);
            SetUpIndices();
            CalculateNormals();
            //CopyToBuffers(device);
        }

        private void SetUpVertices()
        {
            Vertices = new VertexPositionColorNormal[TerrainWidth * TerrainHeight];
            for (int x = 0; x < TerrainWidth; x++)
            {
                for (int y = 0; y < TerrainHeight; y++)
                {
                    //change the divider to heighData[x, y] to reduce hill sizes
                    Vertices[x + y * TerrainWidth].Position = new Vector3(x, (HeightData[x, y]) / 3, -y);
                }
            }
        }
        private void SetUpIndices()
        {
            Indices = new int[(TerrainWidth - 1) * (TerrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < TerrainHeight - 1; y++)
            {
                for (int x = 0; x < TerrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * TerrainWidth;
                    int lowerRight = (x + 1) + y * TerrainWidth;
                    int topLeft = x + (y + 1) * TerrainWidth;
                    int topRight = (x + 1) + (y + 1) * TerrainWidth;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
            }
        }

        private void LoadTreeData(Texture2D treeMap)
        {
            Color[] treeMapColors = new Color[TerrainWidth * TerrainHeight];
            treeMap.GetData(treeMapColors);
            TreePositions = new List<Vector3>();//[TerrainWidth * TerrainHeight];
            for (int x = 0; x < TerrainWidth; x++)
                for (int y = 0; y < TerrainHeight; y++)
                    if ((treeMapColors[x + y * TerrainWidth].R / 255) == 1)
                    {
                        TreePositions.Add(new Vector3(x, Vertices[x + y * TerrainWidth].Position.Y, y));
                    }
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
                    HeightData[y, x] = (heightMapColors[x + y * TerrainWidth].G) / 24;
        }

        private void CalculateNormals()
        {
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

        private void CopyToBuffers(GraphicsDevice device)
        {
            vertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, Vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(Vertices);

            indexBuffer = new IndexBuffer(device, typeof(int), Indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(Indices);
        }
    }
    
}
