using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Terrain
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        SpriteFont arielFont;

        Effect effect;
        TerrainManager terrainManager = new TerrainManager();
        SeasonController seasonController = new SeasonController();
        CameraController Camera = new CameraController();
        //InputController = new InputController();
        MapDisplay Display = new MapDisplay();

        //Matrices
        Matrix viewMatrix;
        Matrix projectionMatrix;

        //Buffer stuff
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        VertexBuffer texturedBuffer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;

            graphics.ApplyChanges();
            IsMouseVisible = true;
            Window.Title = "Terrain";
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            arielFont = Content.Load<SpriteFont>("Ariel");
            
            //initialize graphicsdevice
            device = graphics.GraphicsDevice;

            effect = Content.Load<Effect>("effects");

            SetUpCamera();

            //load the terrain data
            terrainManager.Load(device, Content);
            CopyToBuffers();

        }

        protected override void UnloadContent()
        {

        }
        private void CopyToBuffers()
        {
            vertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, terrainManager.Vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(terrainManager.Vertices);
            indexBuffer = new IndexBuffer(device, typeof(System.Int16), terrainManager.Indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(terrainManager.Indices);

            texturedBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, terrainManager.textureList.Count, BufferUsage.WriteOnly);
            texturedBuffer.SetData(terrainManager.textureList.ToArray());
        }
        private void SetUpCamera()
        {
            //position and orientation of camera
            viewMatrix = Matrix.CreateLookAt(Camera.Position,
                new Vector3(16, 0, -16), new Vector3(0, 1, 0));

            //view angle, apsect ratio, range(1-300).
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                device.Viewport.AspectRatio, 1.0f, 300.0f);

       }
        protected override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.Position.X += 1.0f;
                Camera.Position.Z -= 1.0f;
                Camera.MapPosition.X += 1;
                Camera.MapPosition.Y += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.Position.X -= 1.0f;
                Camera.Position.Z += 1.0f;
                Camera.MapPosition.X -= 1;
                Camera.MapPosition.Y -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.Position.X -= 1.0f;
                Camera.Position.Z -= 1.0f;
                Camera.MapPosition.X -= 1;
                Camera.MapPosition.Y += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.Position.X += 1.0f;
                Camera.Position.Z += 1.0f;
                Camera.MapPosition.X += 1;
                Camera.MapPosition.Y -= 1;
            }
            if(Camera.MapPosition.X < 0)
            {
                Camera.MapPosition.X = 0;
                Camera.Position.X = -10;
            }
            if (Camera.MapPosition.X > 100)
            {
                Camera.MapPosition.X = 100;
                Camera.Position.X = 90;
            }
            if (Camera.MapPosition.Y < 0)
            {
                Camera.MapPosition.Y = 0;
                Camera.Position.Z = 10;
            }
            if (Camera.MapPosition.Y > 110)
            {
                Camera.MapPosition.Y = 110;
                Camera.Position.Z = -100;
            }
            seasonController.Update();
            viewMatrix = Matrix.CreateLookAt(Camera.Position,
                new Vector3(Camera.Position.X + 16, 0, Camera.Position.Z - 16), new Vector3(0, 1, 0));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.CornflowerBlue);
            /************draw 3d terrain and stuff************************/
            RasterizerState rs = new RasterizerState();
            //turn off culling
            //rs.CullMode = CullMode.None;

            //set wireframe
            //rs.FillMode = FillMode.WireFrame;
            device.RasterizerState = rs;

            //select technique
            effect.CurrentTechnique = effect.Techniques["Colored"];
            //effect.CurrentTechnique = effect.Techniques["SeasonColoredNoShading"];
            effect.CurrentTechnique = effect.Techniques["SeasonColored"];
            //effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
            //effect.CurrentTechnique = effect.Techniques["Textured"];
            //effect.CurrentTechnique = effect.Techniques["PointSprites"];


            //set effect Parameters
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            Vector4 color = new Vector4(SeasonController.SeasonColour[0], SeasonController.SeasonColour[1], SeasonController.SeasonColour[2], SeasonController.SeasonColour[3]);
            effect.Parameters["xSeasonColor"].SetValue(color);

            Vector3 light = new Vector3(1.0f, -1.0f, -1.0f);
            light.Normalize();
            effect.Parameters["xLightDirection"].SetValue(light);
            effect.Parameters["xAmbient"].SetValue((float)(Math.Abs(seasonController.Time-120))/120);// day night cycle
            effect.Parameters["xEnableLighting"].SetValue(true);

            effect.Parameters["xTexture"].SetValue(terrainManager.TerrainTextures);

            //draw triangles
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.Indices = indexBuffer;
                device.SetVertexBuffer(vertexBuffer);
                //device.SetVertexBuffer(texturedBuffer);
                int numOfPrims = 32;
                int size = terrainManager.TerrainWidth-1; //width -1.
                for(int c = 0; c < 16 * size; c+=size)
                {
                   device.DrawIndexedPrimitives(
                       PrimitiveType.TriangleList, 
                       0, 
                       (int)(Camera.MapPosition.X + c + (Camera.MapPosition.Y * size)) * 6, 
                       numOfPrims);
                }

            }

            /*effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];*/

            // foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            // {
            //pass.Apply();
            //device.SetVertexBuffer(texturedBuffer);
            //device.DrawUserPrimitives(PrimitiveType.TriangleList, terrainManager.textureList.ToArray(), 0, 2, VertexPositionNormalTexture.VertexDeclaration);
            //device.DrawPrimitives(PrimitiveType.TriangleList, 0, texturedBuffer.VertexCount / 3);
            // }


            /*spriteBatch.Begin();
            spriteBatch.DrawString(arielFont, 
                seasonController.Day + " " 
                + seasonController.CurrentMonth + " " 
                + seasonController.CurrentSeason, 
                new Vector2(0, 0), Color.White);
            spriteBatch.End();*/

            base.Draw(gameTime);
        }
    }
}
