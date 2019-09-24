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

		private TGCBox Box;
		private TgcPlane Plano;
		private TgcMesh Personaje;
		private TgcMesh PinoOriginal;

		private TgcSkyBox skyBox;

		private List<TgcMesh> Pinos;

		private List<TgcMesh> MeshTotales;
		private List<TgcMesh> MeshRecolectables;

		private TgcMesh MeshPlano;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
        private float tamanioMapa = 5000;

        //private TGCVector3 vectorPruebas;

		private Quadtree quadtree;
		private MamutCamara camaraInterna;

        private Fisicas physicsExample;

        private TgcScene scene; 



        public override void Init()
        {

            /*
            BackgroundColor = Color.Black;
			var d3dDevice = D3DDevice.Instance.Device;
			var Loader = new TgcSceneLoader();
			System.Windows.Forms.Cursor.Hide();

			Pinos = new List<TgcMesh>();
			MeshTotales = new List<TgcMesh>();
			MeshRecolectables = new List<TgcMesh>();

			var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\Montes.jpg");
			Plano = new TgcPlane(TGCVector3.Empty, new TGCVector3(5000, 0, 5000), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f); 

			var scene2 = Loader.loadSceneFromFile(MediaDir + "Buggy-TgcScene.xml");
			Personaje = scene2.Meshes[0];
			Personaje.Scale = new TGCVector3(0.7f, 0.7f, 0.7f);
			var scene3 = Loader.loadSceneFromFile(MediaDir + "Pino-TgcScene.xml");
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

			MeshPlano = Plano.toMesh("Plano");
			MeshPlano.Move(-tamanioMapa/2, 0, -tamanioMapa/2);
			MeshPlano.Transform = TGCMatrix.Translation(-tamanioMapa/2, 0, -tamanioMapa/2);

			terreno = new TgcSimpleTerrain();
			var pathTextura = MediaDir + "Textures\\Montes.jpg";
			var pathHeighmap = MediaDir + "montanias.jpg";
			currentScaleXZ = 50f;
			currentScaleY = 1.5f;
			terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -10, 0));
			terreno.loadTexture(pathTextura);
            terreno.AlphaBlendEnable = true;

			quadtree = new Quadtree();
			quadtree.create(MeshTotales, MeshPlano.BoundingBox);
			//quadtree.createDebugQuadtreeMeshes();
            
            
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

            terreno = new TgcSimpleTerrain();
            var position = TGCVector3.Empty;
            terreno = new TgcSimpleTerrain();
            var pathTextura = MediaDir + "Textures\\Montes.jpg";
            var pathHeighmap = MediaDir + "montanias.jpg";
            currentScaleXZ = 50f;
            currentScaleY = 1.5f;
            terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -10, 0));
            terreno.loadTexture(pathTextura);
            terreno.AlphaBlendEnable = true;
            */

            var loader = new TgcSceneLoader();

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Pinos = new List<TgcMesh>();
            MeshTotales = new List<TgcMesh>();
            MeshRecolectables = new List<TgcMesh>();

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


            physicsExample = new Fisicas();
            physicsExample.setBuildings(MeshTotales);
            physicsExample.Init(MediaDir);




            //Vamos a utilizar la camara en 3ra persona para que siga al objeto principal a medida que se mueve
                                 
            //camaraInterna = new MamutCamara(Personaje.Position,100,100, Input); //primera persona
            //camaraInterna = new MamutCamara(Personaje.Position, 100, 300, Input);


            camaraInterna = new MamutCamara(new TGCVector3(0,0,0), 100, 300, Input);
            Camara = camaraInterna;


            
        }

		public override void Update()
        {
            PreUpdate();
            /*
			var input = Input;
			var movement = TGCVector3.Empty;
			var rotation = 0f;

			if (input.keyDown(Key.Left) || input.keyDown(Key.A))
			{
				rotation = -1f;
			}
			else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
			{
				rotation = 1f;
			}

			if (input.keyDown(Key.Up) || input.keyDown(Key.W))
			{
				movement.Z = -1;
			}
			else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
			{
				movement.Z = 1;
			}

			var originalPos = Personaje.Position;

			movement *= VELOCIDAD_MOVIMIENTO * ElapsedTime;
			rotation = rotation * VELOCIDAD_ROTACION * ElapsedTime;

			
			if (FastMath.Abs(Personaje.Position.X + movement.X) <= 2500 || FastMath.Abs(Personaje.Position.Z + movement.Z) <= 2500) {
				Personaje.RotateY(rotation);
				Personaje.MoveOrientedY(movement.Z);
			}
			
			var collisionFound = false;
			TgcMesh meshColisionado = null;

			foreach (var mesh in MeshTotales)
			{
				var mainMeshBoundingBox = Personaje.BoundingBox;
				var sceneMeshBoundingBox = mesh.BoundingBox;

				var collisionResult = TgcCollisionUtils.classifyBoxBox(mainMeshBoundingBox, sceneMeshBoundingBox);

				if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
				{
					collisionFound = true;
					meshColisionado = mesh;
					break;
				}
               
            }
            TgcBoundingAxisAlignBox limiteMapa = new TgcBoundingAxisAlignBox(new TGCVector3(-tamanioMapa/2, 0, -tamanioMapa/2), new TGCVector3(tamanioMapa / 2, 100, tamanioMapa / 2));
            var collisionResult2 = TgcCollisionUtils.classifyBoxBox(Personaje.BoundingBox, limiteMapa);
            if (collisionResult2 == TgcCollisionUtils.BoxBoxResult.Afuera)
            {
                collisionFound = true;
            }


            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            if (collisionFound)
			{
				Personaje.Position = originalPos;
				if (Input.keyPressed(Key.F) && MeshRecolectables.Contains(meshColisionado)) 
				{
					MeshTotales.Remove(meshColisionado);
					meshColisionado.Dispose();
				}
			}

			


			camaraInterna.Target = Personaje.Position;
			camaraInterna.rotateY(rotation);
            */
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
            DrawText.drawText("Personaje pos: " + TGCVector3.PrintVector3(physicsExample.getPersonaje().Position), 5, 20, Color.Red);
            DrawText.drawText("Camera LookAt: " + TGCVector3.PrintVector3(camaraInterna.LookAt), 5, 40, Color.Red);
            /*
            terreno.Render();
			MeshPlano.Render();
			skyBox.Render();
			

			Personaje.Render();
			quadtree.render(Frustum, true);
            */
            skyBox.Render();
            physicsExample.Render(ElapsedTime);

            //terreno.Render();
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
 