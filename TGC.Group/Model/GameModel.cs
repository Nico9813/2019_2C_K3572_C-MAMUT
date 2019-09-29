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

		private List<TgcMesh> Pinos;

		private List<TgcMesh> MeshTotales;
		private List<TgcMesh> MeshRecolectables;

		private TgcMesh MeshPlano;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
		private float tamanioMapa = 10000;

		//private TGCVector3 vectorPruebas;

		private Quadtree quadtree;
		private MamutCamara camaraInterna;

		private Linterna linterna;

		private Fisicas physicsExample;

        private TgcScene scene;

       
        public override void Init()
        {
            var loader = new TgcSceneLoader();

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Pinos = new List<TgcMesh>();
            MeshTotales = new List<TgcMesh>();
            MeshRecolectables = new List<TgcMesh>();

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
			MeshTotales.Add(MeshPlano);

			//Instancio los arboles y los pongo en su posicion inicial
			var scene3 = loader.loadSceneFromFile(MediaDir + "Pino-TgcScene.xml");
            PinoOriginal = scene3.Meshes[0];

			for (var i = 0; i < 4; i++)
            {
                var instance = PinoOriginal.createMeshInstance(PinoOriginal.Name + i);
                Pinos.Add(instance);
                MeshTotales.Add(Pinos[i]);
            }

            Pinos[0].Move(150, 0, 150);
            Pinos[1].Move(-150, 0, 150);
            Pinos[2].Move(150, 0, -150);
            Pinos[3].Move(-150, 0, -150);

            Pinos[0].Transform = TGCMatrix.Translation(150, 0, 150);
            Pinos[1].Transform = TGCMatrix.Translation(-150, 0, 150);
            Pinos[2].Transform = TGCMatrix.Translation(150, 0, -150);
            Pinos[3].Transform = TGCMatrix.Translation(-150, 0, -150);

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
			physicsExample.setBuildings(MeshTotales);
            physicsExample.Init(MediaDir);

			//Instancia de linterna
			linterna = new Linterna();

			//Instancia del quadTree (optimizacion)
            quadtree = new Quadtree();
            quadtree.create(MeshTotales, MeshPlano.BoundingBox);

			//Instancia de la camara (primera persona)
            camaraInterna = new MamutCamara(new TGCVector3(0,0,0), 50, 50, Input);
            Camara = camaraInterna;
            
        }

		public override void Update()
        {
            PreUpdate();
			Personaje.Update(Input);
            physicsExample.Update(Input);

            if (Input.keyDown(Key.A))
            {
                camaraInterna.rotateY(-0.005f);
            }

            if (Input.keyDown(Key.D))
            {
                camaraInterna.rotateY(0.5f * 0.01f);
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

			Personaje.Render(MeshTotales, terreno, camaraInterna.LookAt, direccionLuz);

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
 