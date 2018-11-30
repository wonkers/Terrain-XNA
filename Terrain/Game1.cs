using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Terrain
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont arielFont;
        Texture2D uiMask;
        //GraphicsDevice device;
        Effect effect;
        TerrainManager terrainManager = new TerrainManager();
        SeasonController seasonController = new SeasonController();
        CameraController Camera = new CameraController();
        //InputController = new InputController();
        MapDisplay Display = new MapDisplay();

        //Matrices
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix wvp;

        private float angle = 0f;

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
            wvp = Matrix.Identity;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            arielFont = Content.Load<SpriteFont>("Ariel");
            //initialize graphicsdevice
            //device = graphics.GraphicsDevice;

            uiMask = Content.Load<Texture2D>("UIMask");

            effect = Content.Load<Effect>("effects");

            SetUpCamera();

            //load the terrain data
            terrainManager.Load(GraphicsDevice, Content);

        }

        protected override void UnloadContent()
        {

        }
        private void SetUpCamera()
        {
            //position and orientation of camera
            //viewMatrix = Matrix.CreateLookAt(new Vector3(-40, 16, 40),
            //new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            //view angle, apsect ratio, range(1-300).
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio, 1.0f, 300.0f);
        }
        protected override void Update(GameTime gameTime)
        {
            //slightly to the side view
            viewMatrix = Matrix.CreateLookAt(Camera.Position,
                new Vector3(-45, -32, -0), new Vector3(0, 1, 0));

            Display.GetVertices(terrainManager.Vertices, Camera.MapPosition);
            //Display.GetTrees(Terrain.treePositions, Camera.MapPosition);
            Display.SetupIndices();
            Display.CalculateNormals();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            /************draw 3d terrain and stuff************************/

            GraphicsDevice.BlendState = BlendState.Opaque;
            RenderTarget2D renderTarget;
            renderTarget = new RenderTarget2D(GraphicsDevice, 800, 600, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //turn off culling
            RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //set wireframe
            //rs.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rs;

            //load up the coloring effect
            Matrix worldMatrix = Matrix.CreateTranslation(-terrainManager.TerrainWidth / 2.0f, 0, terrainManager.TerrainHeight / 2.0f)
                * Matrix.CreateRotationY(angle);

            effect.CurrentTechnique = effect.Techniques[0];
           
            effect.Parameters["WorldViewProjection"].SetValue(projectionMatrix);
            effect.Parameters["View"].SetValue(viewMatrix);
            effect.Parameters["World"].SetValue(worldMatrix);
            //effect.Parameters["SeasonColour"].SetValue(SeasonController.SeasonColour);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Display.Verts, 0, Display.Verts.Length / 3, VertexPositionColorNormal.VertexDeclaration);
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Display.Verts, 0, Display.Verts.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, Display.WireFrameVertices, 0, Display.WireFrameVertices.Length,
                    Display.Indices, 0, Display.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
            }

            GraphicsDevice.SetRenderTarget(null);

            base.Draw(gameTime);
        }
    }
}
