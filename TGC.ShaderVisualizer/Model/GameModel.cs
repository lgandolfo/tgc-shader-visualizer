using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace TGC.Group.Model
{

    public class GameModel : TGCExample
    {
        private VertexBuffer fullScreenQuad;
        private Effect effect;
        private HotReloadEffect hotReloadEffect;
        private TgcTexture texture;

        private float time;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        public override void Init()
        {
            var device = D3DDevice.Instance.Device;

            // Creo una textura, genero sus mipmaps
            texture = TgcTexture.createTexture(MediaDir + "Textures//perlin1.png");
            texture.D3dTexture.GenerateMipSubLevels();

            // Creo un vertex buffer, en este caso un full screen quad
            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            fullScreenQuad = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            fullScreenQuad.SetData(vertices, 0, LockFlags.None);

            // Cargo mi efecto, que contiene mis tecnicas (shaders)
            effect = TGCShaders.Instance.LoadEffect(ShadersDir + "Example.fx");
            hotReloadEffect = new HotReloadEffect(effect, ShadersDir + "Example.fx", 0.5f);

            time = 0f;

            // Da igual la camara, nuestros vertices no se van
            // a multiplicar por ninguna matriz
            var cameraPosition = TGCVector3.Empty;
            var lookAt = TGCVector3.Empty;
            Camera.SetCamera(cameraPosition, lookAt);

            FixedTickEnable = true;
            TimeBetweenUpdates = 1f/40f;
        }

        public override void Update()
        {
            PreUpdate();

            try
            {
                hotReloadEffect.Update(ElapsedTime);
                effect = hotReloadEffect.Effect;
                effect.Technique = "Default";
                effect.SetValue("time", time);
                effect.SetValue("textureExample", texture.D3dTexture);
                effect.SetValue("screenWidth", D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth);
                effect.SetValue("screenHeight", D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Input.keyDown(Key.A))
                D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;
            else
                D3DDevice.Instance.Device.RenderState.FillMode = FillMode.Solid;


            time += ElapsedTime;


            PostUpdate();
        }
        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            if (effect != null && !effect.Disposed)
            {
                var device = D3DDevice.Instance.Device;

                PreRender();
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, fullScreenQuad, 0);
                effect.Begin(FX.None);
                effect.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                effect.EndPass();
                effect.End();
                PostRender();

            }
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose del quad
            fullScreenQuad.Dispose();
            //Dispose del mesh.
            effect.Dispose();
        }
    }
}