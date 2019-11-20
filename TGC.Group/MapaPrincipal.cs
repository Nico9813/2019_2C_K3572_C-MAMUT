using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Examples.Physics.CubePhysic;
using TGC.Group.Model;
using TGC.Group.Objetos;

namespace TGC.Group
{
	public class MapaPrincipal : MapaJuego
	{
		public String MediaDir;
		public String ShadersDir;

		private TgcPlane Plano;
		private TgcMesh MeshPlano;
		private TgcMesh MeshLago;
		private Pieza piezaAsociadaLago;
		private Pista pistaAsociadaLago;
		private MensajeTemporal mensajeAgua = null;
		bool piezaLagoEntregada = false;
		private TgcBoundingElipsoid lago;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
		private float tamanioMapa = 10000;

		private TGCVector3 ultimaPosTierra = new TGCVector3(-3800, 80, 160);

		private TgcSkyBox skyBox;

		TgcBoundingAxisAlignBox cabaniaBoundingBox;

		private List<Recolectable> Items;
		private List<Pieza> Piezas;
		private List<Colisionable> Objetos;
		private List<TgcMesh> meshFogatas;
		private List<Fogata> IluminacionEscenario;
		private List<TgcMesh> arbolesMesh;
		private Vector4[] FogatasPos;

		private List<TgcMesh> MeshARenderizar;

		public Personaje personaje;

		private Bateria bateria;
		private Vela vela;
		private Bug bug;

		public Tgc3dSound sonidoBug;

		private Boolean fogataCerca = false;

		public static TgcStaticSound sonidoAgua;

		private Fisicas physics;

		

		public MapaPrincipal(string mediaDir, string shadersDir)
		{
			this.MediaDir = mediaDir;
			this.ShadersDir = shadersDir;
		}

		public void setPersonaje(Personaje personaje) {
			this.personaje = personaje;
		}

		public void setMotoFisicas(Fisicas motor)
		{
			this.physics = motor;
		}

		public void Init() {

			var loader = new TgcSceneLoader();

			Items = new List<Recolectable>();
			Piezas = new List<Pieza>();
			Objetos = new List<Colisionable>();
			MeshARenderizar = new List<TgcMesh>();
			meshFogatas = new List<TgcMesh>();
			IluminacionEscenario = new List<Fogata>();

			

			//Instancia de skybox
			skyBox = new TgcSkyBox();
			skyBox.Center = TGCVector3.Empty;
			skyBox.Size = new TGCVector3(9000, 9000, 9000);

			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, MediaDir + "cielo.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, MediaDir + "cielo.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, MediaDir + "cielo.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, MediaDir + "cielo.jpg");

			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, MediaDir + "cielo.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, MediaDir + "cielo.jpg");
			skyBox.SkyEpsilon = 25f;

			skyBox.Init();

			//Instancio la vegetacion
			var scene = loader.loadSceneFromFile(MediaDir + @"Pino-TgcScene.xml");
			var PinoOriginal = scene.Meshes[0];
			List<TGCVector3> posicionesArboles = new List<TGCVector3>();

			posicionesArboles.Add(new TGCVector3(1, 1, 1));
			posicionesArboles.Add(new TGCVector3(-3442, 1, -2736));
			posicionesArboles.Add(new TGCVector3(-3689, 1, -3039));
			posicionesArboles.Add(new TGCVector3(-3799, 1, -2719));
			posicionesArboles.Add(new TGCVector3(-3417, 1, -2480));
			posicionesArboles.Add(new TGCVector3(-2917, 1, -2433));
			posicionesArboles.Add(new TGCVector3(-3668, 1, -2025));
			posicionesArboles.Add(new TGCVector3(-3362, 1, -2009));
			posicionesArboles.Add(new TGCVector3(-3451, 1, -3786));
			posicionesArboles.Add(new TGCVector3(-4037, 1, -2329));
			posicionesArboles.Add(new TGCVector3(-2885, 1, -1826));
			posicionesArboles.Add(new TGCVector3(-4123, 1, -1581));
			posicionesArboles.Add(new TGCVector3(-3289, 1, -909));
			posicionesArboles.Add(new TGCVector3(-4261, 1, -435));
			posicionesArboles.Add(new TGCVector3(-2883, 1, -655));
			posicionesArboles.Add(new TGCVector3(-3352, 1, -1761));
			posicionesArboles.Add(new TGCVector3(-3244, 1, -2394));
			posicionesArboles.Add(new TGCVector3(-3978, 1, -2572));
			posicionesArboles.Add(new TGCVector3(-3517, 1, -1982));
			posicionesArboles.Add(new TGCVector3(-3118, 1, -1524));
			posicionesArboles.Add(new TGCVector3(-3349, 1, -980));
			posicionesArboles.Add(new TGCVector3(-4110, 1, -407));
			posicionesArboles.Add(new TGCVector3(-3304, 1, -1774));
			posicionesArboles.Add(new TGCVector3(-3139, 1, -1269));
			posicionesArboles.Add(new TGCVector3(-2140, 1, -562));
			posicionesArboles.Add(new TGCVector3(-4094, 1, -145));
			posicionesArboles.Add(new TGCVector3(-3103, 1, -1337));
			posicionesArboles.Add(new TGCVector3(-2896, 1, -1087));
			posicionesArboles.Add(new TGCVector3(-2529, 1, 10));
			posicionesArboles.Add(new TGCVector3(-3917, 1, 772));
			posicionesArboles.Add(new TGCVector3(746, 1, 157));
			posicionesArboles.Add(new TGCVector3(951, 1, 637));
			posicionesArboles.Add(new TGCVector3(1361, 1, 404));
			posicionesArboles.Add(new TGCVector3(1361, 1, 440));
			posicionesArboles.Add(new TGCVector3(-3877, 1, -678));
			posicionesArboles.Add(new TGCVector3(-3997, 1, -1079));
			posicionesArboles.Add(new TGCVector3(-3996, 1, -1617));
			posicionesArboles.Add(new TGCVector3(-3701, 1, -1505));
			posicionesArboles.Add(new TGCVector3(-3761, 1, -1069));
			posicionesArboles.Add(new TGCVector3(-3968, 1, -1952));
			posicionesArboles.Add(new TGCVector3(-3550, 1, -1562));
			posicionesArboles.Add(new TGCVector3(-3557, 1, -1192));
			posicionesArboles.Add(new TGCVector3(-3938, 1, -1048));
			posicionesArboles.Add(new TGCVector3(-3148, 1, -268));
			posicionesArboles.Add(new TGCVector3(-4120, 1, 433));
			posicionesArboles.Add(new TGCVector3(-3136, 1, -135));
			posicionesArboles.Add(new TGCVector3(-2793, 1, -476));


			var indiceArbolDirectorio = (new Random()).Next(posicionesArboles.Count, posicionesArboles.Count + 100);
			arbolesMesh = new List<TgcMesh>();
			Colisionable Arbol;

			for (var i = 0; i < posicionesArboles.Count; i++)
			{
				var Instance = PinoOriginal.createMeshInstance("Pino" + i);
				Arbol = new SinEfecto(Instance);
				Arbol.mesh.Move(0, 0, 0);
				Arbol.mesh.Scale = new TGCVector3(0.05f * i, 0.05f * i, 0.05f * i);
				Arbol.mesh.Move(posicionesArboles[i]);
				Arbol.mesh.Transform = TGCMatrix.Translation(posicionesArboles[i]);
				Objetos.Add(Arbol);
				MeshARenderizar.Add(Arbol.mesh);
				arbolesMesh.Add(Arbol.mesh);

			}

			for (var i = posicionesArboles.Count; i < posicionesArboles.Count + 100; i++)
			{
				var Instance = PinoOriginal.createMeshInstance("Pino" + i);
				if (i == indiceArbolDirectorio)
				{
					Arbol = new ArbolDirectorio(MediaDir);
				}
				else
				{
					Arbol = new SinEfecto(Instance);
				}

				Arbol.mesh.Move(0, 0, 0);
				Arbol.mesh.Scale = new TGCVector3(0.01f * i, 0.01f * i, 0.01f * i);
				Arbol.mesh.Move(new TGCVector3(((float)Math.Pow(i, Math.PI) % 2066) + 98, 1, ((float)Math.Pow(i, Math.E) % 3136) - 1339));
				Arbol.mesh.Transform = TGCMatrix.Translation(new TGCVector3(((float)Math.Pow(i, Math.PI) % 2066) + 98, 1, ((float)Math.Pow(i, Math.E) % 3136) - 1339));
				Objetos.Add(Arbol);
				MeshARenderizar.Add(Arbol.mesh);
				arbolesMesh.Add(Arbol.mesh);
			}
			foreach (var mesh in arbolesMesh)
			{
				mesh.AlphaBlendEnable = true;
			}

			//Instancio el terreno (Heigthmap)
			terreno = new TgcSimpleTerrain();
			var pathTextura = MediaDir + "Textures\\mapa1.jpg";
			var pathHeighmap = MediaDir + "mapa1.jpg";
			currentScaleXZ = 100f;
			currentScaleY = 3f;
			terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -30, 0));
			terreno.loadTexture(pathTextura);

			//Instancio el piso
			var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\water2.jpg");
			Plano = new TgcPlane(new TGCVector3(-tamanioMapa / 2, 0, -tamanioMapa / 2), new TGCVector3(tamanioMapa, 0, tamanioMapa), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f);
			MeshPlano = Plano.toMesh("MeshPlano");
			Objetos.Add(new SinEfecto(MeshPlano));
			MeshARenderizar.Add(MeshPlano);
			piezaAsociadaLago = new Pieza(2, "Pieza 2", MediaDir + "\\2D\\windows\\windows_2.png", null);
			pistaAsociadaLago = new Pista(null, MediaDir + "\\2D\\pista_hacha.png", null);

			//Instancio la Cabania
			var sceneCabania = loader.loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml");
			foreach (var Mesh in sceneCabania.Meshes)
			{
				Mesh.Move(-500, 20, 500);
				Mesh.Scale = new TGCVector3(4.5f, 4.5f, 4.5f);

				Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

				Objetos.Add(new SinEfecto(Mesh));
				MeshARenderizar.Add(Mesh);
			}
			cabaniaBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-500, 20, 500), new TGCVector3(0, 1001, 1080));

			var sceneBridge = loader.loadSceneFromFile(MediaDir + @"Bridge-TgcScene.xml");
			foreach (var Mesh in sceneBridge.Meshes)
			{
				Mesh.Move(-2561, 12, 159);
				Mesh.Scale = new TGCVector3(4.5f, .75f, 1.35f);

				Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

				Objetos.Add(new SinEfecto(Mesh));
				MeshARenderizar.Add(Mesh);
			}

			var sceneCanoa = loader.loadSceneFromFile(MediaDir + @"Canoa-TgcScene.xml");
			foreach (var Mesh in sceneCanoa.Meshes)
			{
				Mesh.Move(-482, 20, -3110);
				Mesh.Scale = new TGCVector3(1.5f, 1.5f, 1.5f);

				Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

				Objetos.Add(new SinEfecto(Mesh));
				MeshARenderizar.Add(Mesh);
			}

			//Cabania es lugar seguro

			if (TgcCollisionUtils.testAABBAABB(personaje.mesh.BoundingBox, cabaniaBoundingBox))
			{
				personaje.tiempoDesprotegido = 0;
			}

		}

		public void Update(float time, TgcD3dInput Input)
		{
			//Iluminacion de fogatas

			var fogatasLejos = 0;
			foreach (var iluminador in IluminacionEscenario)
			{
				var distancia = FastMath.Sqrt(TGCVector3.LengthSq(new TGCVector3(50, 0, 50) + iluminador.mesh.Position - personaje.mesh.Position));//50 en xz es porque no esta centrada la hoguera
				var dentroFogata = distancia < 300;

				if (dentroFogata)
				{
					fogataCerca = true;

					personaje.tiempoDesprotegido = 0;

					break;
				}
				else
				{
					fogatasLejos++;
				}

			}

			//Colision con el agua

			if (TgcCollisionUtils.testAABBAABB(personaje.mesh.BoundingBox, new TgcBoundingAxisAlignBox(new TGCVector3(-2520, 0, 179), new TGCVector3(-2435, 100, 276))))//Dependiendo si pasa el puente o no se guarda ultima posicion en tierra
				ultimaPosTierra = new TGCVector3(-3800, 80, 160);
			if (TgcCollisionUtils.testAABBAABB(personaje.mesh.BoundingBox, new TgcBoundingAxisAlignBox(new TGCVector3(-1125, 0, 179), new TGCVector3(-900, 100, 276))))
				ultimaPosTierra = new TGCVector3(1000, 80, 1200);

			if (TgcCollisionUtils.testAABBAABB(personaje.mesh.BoundingBox, Plano.BoundingBox))
			{
				if (personaje.tieneItem("SUDO"))
				{
					if (!piezaLagoEntregada)
					{
						personaje.agregarPieza(piezaAsociadaLago);
						personaje.agregarPista(pistaAsociadaLago);
						piezaLagoEntregada = true;
					}
				}
				else
				{
					if (mensajeAgua == null || mensajeAgua.tiempoCumplido())
					{
						mensajeAgua = new MensajeTemporal("No tienes permiso para nadar");
						HUD.Instance.mensajesTemporales.Add(mensajeAgua);
					}
					physics.personajeBody.ActivationState = ActivationState.ActiveTag;
					physics.personajeBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
					var direccionATierra = (personaje.mesh.Position - ultimaPosTierra);
					direccionATierra.Normalize();
					physics.personajeBody.ApplyCentralImpulse(-20 * direccionATierra.ToBulletVector3());
				}
				sonidoAgua.play(false);
			}

			
		}

		public void Render()
		{
			skyBox.Render();


		}

		public void Dispose()
		{
			skyBox.Dispose();
		}

		public override List<TgcMesh> getMeshRender()
		{
			return MeshARenderizar;
		}

		public TgcSimpleTerrain getTerreno()
		{
			return terreno;
		}

		public TgcMesh getMeshPlano()
		{
			return MeshPlano;
		}

		public List<Colisionable> getObjetos()
		{
			return Objetos;
		}

		public List<Recolectable> getItems()
		{
			return Items;
		}

		public List<TgcMesh> getFogatas() {
			return meshFogatas;
		}
	}
}
