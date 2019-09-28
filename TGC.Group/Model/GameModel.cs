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
            var loader = new TgcSceneLoader();

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Pinos = new List<TgcMesh>();
            MeshTotales = new List<TgcMesh>();
            MeshRecolectables = new List<TgcMesh>();
			//Linterna = new Linterna();

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

            quadtree = new Quadtree();
            MeshPlano = physicsExample.getPlano().toMesh("MeshPlano");
            quadtree.create(MeshTotales, MeshPlano.BoundingBox);

            

            //Vamos a utilizar la camara en 3ra persona para que siga al objeto principal a medida que se mueve

            //camaraInterna = new MamutCamara(Personaje.Position,100,100, Input); //primera persona
            //camaraInterna = new MamutCamara(Personaje.Position, 100, 300, Input);


            camaraInterna = new MamutCamara(new TGCVector3(0,0,0), 50, 50, Input);
            Camara = camaraInterna;
            
        }

		public override void Update()
        {
            PreUpdate();

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
            skyBox.Render();
           

            physicsExample.Render(ElapsedTime,camaraInterna.LookAt);
            quadtree.render(Frustum, true);
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
 