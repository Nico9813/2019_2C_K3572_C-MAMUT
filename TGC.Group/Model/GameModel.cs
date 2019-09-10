using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;

namespace TGC.Group.Model
{

    public class GameModel : TgcExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

		private const float VELOCIDAD_ROTACION = 1f;
		private const float VELOCIDAD_MOVIMIENTO = 5f;

		private TGCBox Box;
		private TgcPlane Piso;
		private TgcScene Scene;
		private TgcFpsCamera CamaraPrincipal;

		public override void Init()
        {
			BackgroundColor = Color.Black;
			var d3dDevice = D3DDevice.Instance.Device;
			var Loader = new TgcSceneLoader();
			

			Scene = Loader.loadSceneFromFile(MediaDir + "UltimaChance.xml");

			foreach (var mesh in Scene.Meshes) {
				mesh.Move(TGCVector3.Empty);
			}

			//Box = TGCBox.fromSize(new TGCVector3(0, 5, 0), new TGCVector3(5,5,5) ,Color.Yellow);
			//Box.AutoTransformEnable = true;

			CamaraPrincipal = new TgcFpsCamera(TGCVector3.Empty, 200f, 200f, Input);
			CamaraPrincipal.SetCamera(TGCVector3.Empty, TGCVector3.Empty);
			//CamaraPrincipal = new TgcThirdPersonCamera(Box.Position, 10f, 10f);
			Camara = CamaraPrincipal;
			
        }

        public override void Update()
        {
            PreUpdate();
			/*
			var velocidadCaminar = VELOCIDAD_MOVIMIENTO * ElapsedTime;
			var velocidadRotar = VELOCIDAD_ROTACION * ElapsedTime;

			//Calcular proxima posicion de personaje segun Input
			var moving = false;
			var rotating = false;
			var rotate = 0f;
			var movement = TGCVector3.Empty;

			//Adelante
			if (Input.keyDown(Key.W))
			{
				movement.Z = velocidadCaminar;
				moving = true;
			}

			//Atras
			if (Input.keyDown(Key.S))
			{
				movement.Z = -velocidadCaminar;
				moving = true;
			}

			//Derecha
			if (Input.keyDown(Key.D))
			{
				rotate = velocidadRotar;
				rotating = true;
			}

			//Izquierda
			if (Input.keyDown(Key.A))
			{
				rotate = -velocidadRotar;
				rotating = true;
			}
			//Salto
			if (Input.keyDown(Key.Space))
			{
				movement.Y = velocidadCaminar;
				moving = true;
			}
			//Agachar
			if (Input.keyDown(Key.LeftControl))
			{
				movement.Y = -velocidadCaminar;
				moving = true;
			}
			//Si hubo desplazamiento
			if (moving)
			{
				//Aplicar movimiento, internamente suma valores a la posicion actual del mesh.
				Box.Move(movement);
			}
			if (rotating)
			{
				//Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
				var rotAngle = Geometry.DegreeToRadian(rotate);
				Box.RotateY(rotAngle);
				CamaraPrincipal.rotateY(rotAngle);
			}
			CamaraPrincipal.Target = Box.Position;
			*/
			PostUpdate();
        }

        public override void Render()
        {
           
            PreRender();

			Scene.RenderAll();
			/*
			foreach (var mesh in Scene.Meshes)
			{
				mesh.BoundingBox.Render();
			}
			*/
			//Box.Render();

			PostRender();
        }


        public override void Dispose()
        {
			//Dispose de la caja.
			Scene.DisposeAll();
            //Box.Dispose();
			//Piso.Dispose();
        }
    }
}