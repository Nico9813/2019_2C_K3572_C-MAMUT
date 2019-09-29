using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Examples.Camara;
using TGC.Examples.Optimization.Quadtree;
using BulletSharp;
using TGC.Examples.Physics.CubePhysic;
using TGC.Core.Shaders;
using TGC.Group.Iluminacion;
//using TGC.Examples.UserControls.Modifier;
//using TGC.Group.Iluminacion;

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

		private const float VELOCIDAD_ROTACION = 3f;
		private const float VELOCIDAD_MOVIMIENTO = 750f;

		//private Linterna Linterna;
		private TGCBox Box;
		private TgcPlane Plano;
		private Personaje Personaje;
		private TgcMesh MeshPersonaje;
		private TgcMesh PinoOriginal;

		private TgcSkyBox skyBox;

		private List<Item> Items;
		private List<Pieza> Piezas;
		private List<TgcMesh> Objetos;
		private List<TgcMesh> MeshARenderizar;
		private List<TgcMesh> MeshTotales;

		private TgcMesh MeshPlano;
		private TgcMesh MeshLago;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
		private float tamanioMapa = 10000;

		private Quadtree quadtree;
		private MamutCamara camaraInterna;
		private Linterna linterna;

		private Fisicas physicsExample;

        public override void Init()
        {
            var loader = new TgcSceneLoader();

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Items = new List<Item>();
			Piezas = new List<Pieza>();
			Objetos = new List<TgcMesh>();
			MeshARenderizar = new List<TgcMesh>();

			//Instancio el terreno (Heigthmap)
			terreno = new TgcSimpleTerrain();
			var position = TGCVector3.Empty;
			var pathTextura = MediaDir + "Textures\\Montes.jpg";
			var pathHeighmap = MediaDir + "montanias.jpg";
			currentScaleXZ = 100f;
			currentScaleY = 3f;
			terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -15, 0));
			terreno.loadTexture(pathTextura);
			terreno.AlphaBlendEnable = true;

			//Instancio el piso
			var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\Piso.jpg");
			Plano = new TgcPlane(new TGCVector3(-tamanioMapa / 2, 0, -tamanioMapa / 2), new TGCVector3(tamanioMapa, 0, tamanioMapa), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f);
			MeshPlano = Plano.toMesh("MeshPlano");
			Objetos.Add(MeshPlano);
			MeshARenderizar.Add(MeshPlano);

			//Instancio la vegetacion
			var scene = loader.loadSceneFromFile(MediaDir + @"vegetacion-TgcScene.xml");
			foreach (var Mesh in scene.Meshes)
			{
				Mesh.Scale = new TGCVector3(5, 5, 5);
				Objetos.Add(Mesh);
				MeshARenderizar.Add(Mesh);
			}

			//Instancio la Cabania
			var sceneCabania = loader.loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml");
			foreach (var Mesh in sceneCabania.Meshes) {
				Mesh.Move(-500,0,500);
				Mesh.Scale = new TGCVector3(2.5f, 2.5f, 2.5f);

				Mesh.updateBoundingBox();
				Objetos.Add(Mesh);
				MeshARenderizar.Add(Mesh);
			}

			//Instancia del personaje
			MeshPersonaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];
			Personaje = new Personaje();
			Personaje.Init(MeshPersonaje);

			//Instancia de skybox
			skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Empty;
            skyBox.Size = new TGCVector3(10000, 10000, 10000);

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, MediaDir + "cielo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, MediaDir + "cielo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, MediaDir + "cielo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, MediaDir + "cielo.jpg");

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, MediaDir + "cielo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, MediaDir + "cielo.jpg");
            skyBox.SkyEpsilon = 25f;

            skyBox.Init();

			//Instancia de motor de fisica para colisiones con mesh y terreno
            physicsExample = new Fisicas();
			physicsExample.setTerrain(terreno);
			physicsExample.setPersonaje(Personaje.mesh);
			physicsExample.setBuildings(Objetos);
            physicsExample.Init(MediaDir);

			//Instancia de linterna
			var scene4 = loader.loadSceneFromFile(MediaDir + "Canon.max-TgcScene.xml");
			var Canon = scene4.Meshes[0];
			linterna = new Linterna(Canon);
			Items.Add(linterna);
			MeshARenderizar.Add(Canon);

			//Instancia del quadTree (optimizacion)
            quadtree = new Quadtree();
            quadtree.create(MeshARenderizar, MeshPlano.BoundingBox);

			//Instancia de la camara (primera persona)
            camaraInterna = new MamutCamara(new TGCVector3(0,0,-1), 50, 50, Input);
            Camara = camaraInterna;
            
        }

		public override void Update()
        {
            PreUpdate();
			Personaje.Update(Input,ElapsedTime);
			physicsExample.Update(Input);

            if (Input.keyDown(Key.A))
            {
                camaraInterna.rotateY(-0.005f);
            }

            if (Input.keyDown(Key.D))
            {
                camaraInterna.rotateY(0.5f * 0.01f);
            }

			var colision = false;
			Item objetoColisionado = null;
			foreach (var item in Items)
			{
				var result = FastMath.Sqrt(TGCVector3.LengthSq(item.mesh.Position - Personaje.mesh.Position)) < 150;
				if (result)
				{
					colision = true;
					objetoColisionado = item;
					break;
				}
			}

			//Si hubo colision, restaurar la posicion anterior
			if (colision)
			{
				if (Input.keyPressed(Key.E)){
					MeshARenderizar.Remove(objetoColisionado.mesh);
					Personaje.agregarItem(objetoColisionado);
					quadtree.actualizarModelos(MeshARenderizar);
				}
			}



			camaraInterna.Target = physicsExample.getPersonaje().Position;

            PostUpdate();
        }

		public override void Render()
		{
			PreRender();

			skyBox.Render();

			quadtree.render(Frustum, true);

			physicsExample.Render();
			var direccionLuz =  physicsExample.getDirector();

			Personaje.Render(MeshARenderizar, terreno, camaraInterna.LookAt, direccionLuz);

			DrawText.drawText("Personaje pos: " + TGCVector3.PrintVector3(physicsExample.getPersonaje().Position), 5, 20, Color.Red);
			DrawText.drawText("Camera LookAt: " + TGCVector3.PrintVector3(camaraInterna.LookAt), 5, 40, Color.Red);
			DrawText.drawText("Modelos Renderizados" + quadtree.cantModelosRenderizados(), 5, 60, Color.GreenYellow);

			PostRender();
        }


        public override void Dispose()
        {
            /*Plano.Dispose();
			terreno.Dispose();
			Personaje.Dispose();
			PinoOriginal.Dispose();
            //Montes.DisposeAll();
            //Piso.DisposeAll();
            */
            skyBox.Dispose();
            physicsExample.Dispose();
            //terreno.Dispose();
        }
    }
}
 