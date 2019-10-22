using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Examples.Camara;
using TGC.Core.Shaders;
using TGC.Examples.Optimization.Quadtree;
using BulletSharp;
using TGC.Examples.Physics.CubePhysic;
using TGC.Group.Iluminacion;
using TGC.Core.Interpolation;
using Device = Microsoft.DirectX.Direct3D.Device;
// using TgcViewer.Utils.Gui;
using TGC.Core.Example;
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

		//private Linterna Linterna;
		private TgcPlane Plano;
		private Personaje Personaje;
		private TgcMesh MeshPersonaje;

		private TgcSkyBox skyBox;

		private List<Item> Items;
		private List<Pieza> Piezas;
		private List<TgcMesh> Objetos;
		private List<TgcMesh> MeshARenderizar;
        private List<TgcMesh> meshFogatas;

		private List<Fogata> IluminacionEscenario;

		private List<TgcMesh> MeshTotales;
		private Boolean itemCerca = false;
        private Boolean fogataCerca = false;
        Item objetoColisionado = null;

		private TgcMesh MeshPlano;
		private TgcMesh MeshLago;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
		private float tamanioMapa = 10000;

		private Quadtree quadtree;
		private MamutCamara camaraInterna;
		private Bateria bateria;
		private Vela vela;

		private Fisicas physicsExample;

        private float giroMuerte;
        private TgcMesh monstruo;
        private TGCVector3 posPersonaje;
        int turnoIluminacion;
        TgcBoundingAxisAlignBox cabaniaBoundingBox;

		public override void Init()
        {
            var loader = new TgcSceneLoader();

            turnoIluminacion = 0;

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Items = new List<Item>();
			Piezas = new List<Pieza>();
			Objetos = new List<TgcMesh>();
			MeshARenderizar = new List<TgcMesh>();
            meshFogatas= new List<TgcMesh>();
            IluminacionEscenario = new List<Fogata>();

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
			var i = 0;
			var ultimaPos = TGCVector3.Empty;
			foreach (var Mesh in scene.Meshes)
			{
				Mesh.Scale = new TGCVector3(1.5f, 1.5f, 1.5f);
				TGCVector3 nuevaPos = Mesh.Position + ultimaPos * FastMath.Pow(-1,i);
				Mesh.Transform = TGCMatrix.Translation(nuevaPos);
				Mesh.Move(nuevaPos);
				Objetos.Add(Mesh);
				MeshARenderizar.Add(Mesh);
				i++;
				ultimaPos = Mesh.Position;
			}

			//Instancio la Cabania
			var sceneCabania = loader.loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml");
			foreach (var Mesh in sceneCabania.Meshes) {
                Mesh.Move(-500, -28, 500);
				Mesh.Scale = new TGCVector3(6f, 6f, 6f);
                
                Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);
				
				Objetos.Add(Mesh);
				MeshARenderizar.Add(Mesh);
			}
            cabaniaBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-500, -5, 520), new TGCVector3(0, 1001, 1080));
            

            //Instancia del personaje
            MeshPersonaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];
			Personaje = new Personaje();
			Personaje.Init(MeshPersonaje);
			Personaje.InitHUD(MediaDir);
			Personaje.mesh.RotateY(-FastMath.PI_HALF);
			Personaje.mesh.Transform = TGCMatrix.RotationY(-FastMath.PI_HALF);

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

			//Instancia de baterias
			var scene4 = loader.loadSceneFromFile(MediaDir + "Bateria-TgcScene.xml");
			var BateriaMesh = scene4.Meshes[0];
			BateriaMesh.Move(-3400,10,530);
			BateriaMesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
			BateriaMesh.Transform = TGCMatrix.Translation(-3400, 10, 530);
			bateria = new Bateria(BateriaMesh);
			Items.Add(bateria);
			MeshARenderizar.Add(BateriaMesh);

			//Instancia de velas
			var scene5 = loader.loadSceneFromFile(MediaDir + "velas-TgcScene.xml");
			var VelasMesh = scene5.Meshes[0];
			VelasMesh.Move(-3400, 10, 400);
			VelasMesh.Scale = new TGCVector3(0.03f, 0.03f, 0.03f);
			VelasMesh.Transform = TGCMatrix.Translation(-3400, 10, 400);
			vela = new Vela(VelasMesh);
			Items.Add(vela);
			MeshARenderizar.Add(VelasMesh);

			//Instancia de fogatas
			var scene6 = loader.loadSceneFromFile(MediaDir + "hoguera-TgcScene.xml");
			var fogataMesh = scene6.Meshes[0];
			Fogata fogata1 = new Fogata(fogataMesh.createMeshInstance("Fogata1"), new TGCVector3(100, 0, -1000));
			Fogata fogata2 = new Fogata(fogataMesh.createMeshInstance("Fogata2"), new TGCVector3(0, 0, -350));
			//Fogata fogata3 = new Fogata(Canon.createMeshInstance("Fogata3"), new TGCVector3(350, 70, 0));
			//Fogata fogata4 = new Fogata(Canon.createMeshInstance("Fogata4"), new TGCVector3(-350, 70, 0));
			IluminacionEscenario.Add(fogata1);
			IluminacionEscenario.Add(fogata2);
			//IluminacionEscenario.Add(fogata3);
			//IluminacionEscenario.Add(fogata4);
			meshFogatas.Add(fogata1.mesh);
			meshFogatas.Add(fogata2.mesh);
			//MeshARenderizar.Add(fogata3.mesh);
			//MeshARenderizar.Add(fogata4.mesh);
			foreach (var fog in IluminacionEscenario) {
				fog.mesh.Move(fog.getPosicion());
				fog.mesh.Transform = TGCMatrix.Translation(fog.getPosicion());
			}

			//Instancia de motor de fisica para colisiones con mesh y terreno
			physicsExample = new Fisicas();
			physicsExample.setTerrain(terreno);
			physicsExample.setPersonaje(Personaje.mesh);
			physicsExample.setBuildings(Objetos);
			physicsExample.Init(MediaDir);

			//Instancia del quadTree (optimizacion)
			quadtree = new Quadtree();
            quadtree.create(MeshARenderizar, MeshPlano.BoundingBox);

			//Instancia de la camara (primera persona)
            camaraInterna = new MamutCamara(new TGCVector3(0,0,-1), 50, 50, Input);
			camaraInterna.rotateY(-FastMath.PI_HALF);
			Camara = camaraInterna;

			giroMuerte = 0;
            monstruo = loader.loadSceneFromFile(MediaDir + @"monstruo-TgcScene.xml").Meshes[0];

            HUDBarras.Instance.Init(MediaDir);

        }

		public override void Update()
        {
            PreUpdate();
			Personaje.Update(Input,ElapsedTime);
			physicsExample.Update(Input,monstruo);

			itemCerca = false;
			objetoColisionado = null;

			if (Input.keyDown(Key.A))
            {
                camaraInterna.rotateY(-0.005f);
            }

            if (Input.keyDown(Key.D))
            {
                camaraInterna.rotateY(0.5f * 0.01f);
            }
            if (Personaje.perdioJuego()) 
            {

                camaraInterna.rotateY(FastMath.PI/180);
                this.giroMuerte += 1f;
                if (giroMuerte >= 180f)
                {
                    camaraInterna.RotationSpeed = 0;
                    physicsExample.strength = 0;
                    physicsExample.angle = 0;
                }
                physicsExample.personajeBody.ActivationState = ActivationState.IslandSleeping;
            }

			foreach (var item in Items)
			{
				var result = FastMath.Sqrt(TGCVector3.LengthSq(item.mesh.Position - Personaje.mesh.Position)) < 100;
				if (result)
				{
					itemCerca = true;
					objetoColisionado = item;
					break;
				}
			}

			if (itemCerca)
			{
				if (Input.keyPressed(Key.E)){
					MeshARenderizar.Remove(objetoColisionado.mesh);
                    Items.Remove(objetoColisionado);
					Personaje.agregarItem(objetoColisionado);
					quadtree.actualizarModelos(MeshARenderizar);
				}
			}
            var fogatasLejos = 0;
            foreach (var iluminador in IluminacionEscenario)
            {
                var distancia = FastMath.Sqrt(TGCVector3.LengthSq(new TGCVector3(50,0,50) + iluminador.mesh.Position - Personaje.mesh.Position));//50 en xz es porque no esta centrada la hoguera
                var dentroFogata = distancia < 300;
                var entroFogata = distancia > 300 && distancia < 310;
                if (entroFogata) Personaje.quitarIluminacion();
                if (dentroFogata)
                {
                    fogataCerca = true;
                    turnoIluminacion = IluminacionEscenario.IndexOf(iluminador)+1;
                    Personaje.ilumnacionActiva = true;
                    
                    break;
                }

                else
                {
					fogatasLejos++;
                }
                if (fogatasLejos == IluminacionEscenario.Count) turnoIluminacion = 0;
            }

            //Cabania es lugar seguro
            if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, cabaniaBoundingBox))
            {
                Personaje.tiempoDesprotegido = 0;
            }


            camaraInterna.Target = physicsExample.getPersonaje().Position;

            PostUpdate();
        }

        public override void Render()
        {
			PreRender();

			if (itemCerca) DrawText.drawText("Presionar E para agarrar [" + objetoColisionado.getDescripcion() + "]", 700, 700, Color.Red);

			skyBox.Render();

			quadtree.render(Frustum, true);

			physicsExample.Render();
			var direccionLuz = physicsExample.getDirector();
			if (turnoIluminacion != 0)
			{
				IluminacionEscenario[turnoIluminacion - 1].Render(MeshARenderizar, terreno);
			}
			if (turnoIluminacion == 0)
			{
				Personaje.getIluminadorPrincipal().Render(MeshARenderizar, terreno, camaraInterna.LookAt, direccionLuz);
			}


			foreach (TgcMesh meshFog in meshFogatas)
			{
				meshFog.Render();
			}

			Personaje.Render(ElapsedTime, Input);

			var desplazamiento = physicsExample.getDirector() * (180f);
			monstruo.Position = new TGCVector3(camaraInterna.Position.X + desplazamiento.X, camaraInterna.Position.Y - 60 + desplazamiento.Y, camaraInterna.Position.Z + desplazamiento.Z);
			monstruo.Scale = new TGCVector3(0.8f, 0.8f, 0.8f);
			monstruo.Transform = TGCMatrix.Translation(camaraInterna.Position.X, camaraInterna.Position.Y, camaraInterna.Position.Z) * TGCMatrix.Scaling(new TGCVector3(0.8f, 0.8f, 0.8f));

			if (giroMuerte >= 180)
			{
				monstruo.Render();
			}

			DrawText.drawText("Personaje pos: " + TGCVector3.PrintVector3(physicsExample.getPersonaje().Position), 5, 20, Color.Red);
			DrawText.drawText("Camera LookAt: " + TGCVector3.PrintVector3(camaraInterna.LookAt), 5, 40, Color.Red);
			DrawText.drawText("Modelos Renderizados" + quadtree.cantModelosRenderizados(), 5, 60, Color.GreenYellow);
            DrawText.drawText("Monstruo aparece en: " + (Personaje.tiempoLimiteDesprotegido - Personaje.tiempoDesprotegido).ToString(), 5, 80, Color.Gold);

            Personaje.mesh.Render();

            HUDBarras.Instance.Render();

			PostRender();
		}
		public override void Dispose()
        {
            skyBox.Dispose();
            physicsExample.Dispose();
            monstruo.Dispose();
        }
	}
}
