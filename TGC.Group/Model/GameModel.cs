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
using TGC.Group.Objetos;
using TGC.Core.Interpolation;

using TGC.Core.Example;
using TGC.Group.Sprites;
using Microsoft.DirectX;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Core.Fog;
using TGC.Core.Sound;
using Device = Microsoft.DirectX.Direct3D.Device;
using System.Windows.Forms;

namespace TGC.Group.Model
{
	//No les conviene copiarse de este tp, menos si cursaron disenio
    public class GameModel : TgcExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Boolean Pausa = false;
        private Boolean JuegoIniciado = false;

        private TgcPlane Plano;
        private Personaje Personaje;
        private TgcMesh MeshPersonaje;

        private TgcSkyBox skyBox;

		private List<Recolectable> Items;
		private List<Pieza> Piezas;
		private List<Colisionable> Objetos;
		private List<TgcMesh> MeshARenderizar;
        private List<TgcMesh> meshFogatas;
		TgcBoundingAxisAlignBox iglesiaBoundingBox;
		TgcBoundingAxisAlignBox altarFinalBoundingBox;
		List<TgcBoundingAxisAlignBox> lugaresSeguros;
		List<TgcMesh> arbolesMesh;

        private List<Fogata> IluminacionEscenario;

        private List<TgcMesh> MeshTotales;
        private Boolean fogataCerca = false;
        Recolectable objetoColisionado = null;

		private TgcMesh MeshPlano;
		private TgcMesh MeshLago;
		private Pieza piezaAsociadaLago;
		private Pista pistaAsociadaLago;
		MensajeTemporal mensajeAgua = null;
		bool piezaLagoEntregada = false;
		private TgcBoundingElipsoid lago;
		private TgcSimpleTerrain terreno;
		private float currentScaleXZ;
		private float currentScaleY;
		private float tamanioMapa = 10000;

		private Quadtree quadtree;
		private TgcFpsCamera camaraInterna;
		private Bateria bateria;
		private Vela vela;


		Bug bug;

		private Fisicas physicsExample;

        private float giroMuerte;
        private TgcMesh monstruo;
        private TgcMesh monstruoSilueta;
        private Vector4[] FogatasPos;
        TgcBoundingAxisAlignBox cabaniaBoundingBox;

        private Effect effect;
        float time = 0;
		float auxShader = 0;
        float auxShaderMonstruoAparece = 0;
		private TgcFog fog;

        private bool salirDelJuego = false;

        private TGCVector3 ultimaPosTierra = new TGCVector3(-3800, 80, 160);


        public List<Tgc3dSound> sonidos = new List<Tgc3dSound>();
        //para sonidos
        public static TgcStaticSound sonidoPisadas;
        public static TgcStaticSound sonidoAgua;
        public static TgcStaticSound sonidoNota;
        public static TgcStaticSound sonidoPickup;
		public static TgcStaticSound sonidoAmbiente;
		public static TgcStaticSound sonidoAgitacion;
		public static TgcStaticSound sonidoMonstruoAcechando;
		public static TgcStaticSound sonidoMuertePersonaje;
		bool murioPersonaje = false;
		public static TgcStaticSound sonidoMonstruoAparece;
        public static TgcStaticSound sonidoMonstruoMuere;
        public static TgcStaticSound sonidoWindowsInicia;
        public static TgcStaticSound sonidoWindowsError;
        public Tgc3dSound sonidoBug;//sonido que actualiza la posicion

        public bool terminoJuego = false;

        private bool ganoEnRealidad = false;

        public override void Init()
        {
            var loader = new TgcSceneLoader();

            effect = TGCShaders.Instance.LoadEffect(ShadersDir + "Iluminacion.fx");

            BackgroundColor = Color.Black;
            var d3dDevice = D3DDevice.Instance.Device;
            var Loader = new TgcSceneLoader();
            System.Windows.Forms.Cursor.Hide();

            Items = new List<Recolectable>();
            Piezas = new List<Pieza>();

			Objetos = new List<Colisionable>();
            MeshARenderizar = new List<TgcMesh>();
            meshFogatas = new List<TgcMesh>();
			lugaresSeguros = new List<TgcBoundingAxisAlignBox>();
			IluminacionEscenario = new List<Fogata>();
            //Instancio la vegetacion
            var scene = loader.loadSceneFromFile(MediaDir + @"Pino-TgcScene.xml");
            var PinoOriginal = scene.Meshes[0];
            List<TGCVector3> posicionesArboles = new List<TGCVector3>();
            posicionesArboles.Add(new TGCVector3(-3877, 1, -678));
            posicionesArboles.Add(new TGCVector3(-3517, 1, -1982));
            posicionesArboles.Add(new TGCVector3(-3118, 1, -1524));
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
            
            posicionesArboles.Add(new TGCVector3(-3997, 1, -1079));
            posicionesArboles.Add(new TGCVector3(-3996, 1, -1617));
            posicionesArboles.Add(new TGCVector3(-3968, 1, -1952));
            posicionesArboles.Add(new TGCVector3(-3550, 1, -1562));
            posicionesArboles.Add(new TGCVector3(-3557, 1, -1192));
            posicionesArboles.Add(new TGCVector3(-3938, 1, -1048));
            posicionesArboles.Add(new TGCVector3(-3148, 1, -268));
            posicionesArboles.Add(new TGCVector3(-4120, 1, 433));
            posicionesArboles.Add(new TGCVector3(-3136, 1, -135));
            posicionesArboles.Add(new TGCVector3(-2793, 1, -476));


            var indiceArbolDirectorio = (new Random()).Next(posicionesArboles.Count+100, posicionesArboles.Count+119);
            arbolesMesh = new List<TgcMesh>();
            Colisionable Arbol;

            for (var i = 0; i < posicionesArboles.Count; i++)
            {
                var Instance = PinoOriginal.createMeshInstance("Pino" + i);

                Arbol = new SinEfecto(Instance);
                Arbol.mesh.Move(0, 0, 0);
                Arbol.mesh.Scale += new TGCVector3(0.015f * i, 0.025f * i, 0.015f * i);
                Arbol.mesh.Move(posicionesArboles[i]);
                Arbol.mesh.Transform = TGCMatrix.Translation(posicionesArboles[i]);
                Objetos.Add(Arbol);
                MeshARenderizar.Add(Arbol.mesh);
                arbolesMesh.Add(Arbol.mesh);
            }

            for (var i = posicionesArboles.Count; i < posicionesArboles.Count + 100; i++)
            {
                var Instance = PinoOriginal.createMeshInstance("Pino" + i);
                Arbol = new SinEfecto(Instance);
                Arbol.mesh.Move(0, 0, 0);
				Arbol.mesh.Scale += new TGCVector3(0.015f * i, 0.025f * i, 0.015f * i);
				Arbol.mesh.Move(new TGCVector3(((float)Math.Pow(i, Math.PI) % 1500) + 98, 1, ((float)Math.Pow(i, Math.E) % 2300) - 1339));
                Arbol.mesh.Transform = TGCMatrix.Translation(new TGCVector3(((float)Math.Pow(i, Math.PI) % 1800) + 98, 1, ((float)Math.Pow(i, Math.E) % 2300) - 1339));
                Objetos.Add(Arbol);
                MeshARenderizar.Add(Arbol.mesh);
                arbolesMesh.Add(Arbol.mesh);
            }

			for (var i = posicionesArboles.Count + 100; i < posicionesArboles.Count + 100 + 20; i++)
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
				Arbol.mesh.Scale += new TGCVector3(0.005f * i, 0.005f * i, 0.005f * i);

				var centro = new TGCVector3(-450, 40, 2500);
				var posicionFinal = centro;
				posicionFinal.X += 450 * FastMath.Cos(FastMath.TWO_PI / 20 * i);
				posicionFinal.Z += 450 * FastMath.Sin(FastMath.TWO_PI / 20 * i);

				Arbol.mesh.Move(posicionFinal);
				Arbol.mesh.Transform = TGCMatrix.Translation(posicionFinal);
				Objetos.Add(Arbol);
				MeshARenderizar.Add(Arbol.mesh);
				arbolesMesh.Add(Arbol.mesh);
			}

			for (var i = posicionesArboles.Count + 100 + 20; i < posicionesArboles.Count + 100 + 20 + 20; i++)
			{
				var Instance = PinoOriginal.createMeshInstance("Pino" + i);

				Arbol = new SinEfecto(Instance);

				Arbol.mesh.Move(0, 0, 0);
				Arbol.mesh.Scale += new TGCVector3(0.005f * i, 0.005f * i, 0.005f * i);

				var centro = new TGCVector3(-450, 40, 2500);
				var posicionFinal = centro;
				posicionFinal.X += 700 * FastMath.Cos(FastMath.TWO_PI / 20 * i);
				posicionFinal.Z += 700 * FastMath.Sin(FastMath.TWO_PI / 20 * i);

				Arbol.mesh.Move(posicionFinal);
				Arbol.mesh.Transform = TGCMatrix.Translation(posicionFinal);
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

            //Instancio la Cabania
            var sceneCabania = loader.loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml");
            foreach (var Mesh in sceneCabania.Meshes)
            {
                Mesh.Move(-400, 20, 600);
                Mesh.Scale = new TGCVector3(3f, 3f, 3f);

                Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

                Objetos.Add(new SinEfecto(Mesh));
                MeshARenderizar.Add(Mesh);
            }

			//Instancio el altar
			var altar = loader.loadSceneFromFile(MediaDir + @"Iglesia-TgcScene.xml");
			foreach (var Mesh in altar.Meshes)
			{
				Mesh.Move(-2000, 31, 2760);


				Objetos.Add(new SinEfecto(Mesh));
				MeshARenderizar.Add(Mesh);
			}

			cabaniaBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-500, 20, 500), new TGCVector3(0, 1001, 1080));
			lugaresSeguros.Add(cabaniaBoundingBox);
			iglesiaBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-2190, 20, 2400), new TGCVector3(-1700, 500, 3200));
			lugaresSeguros.Add(iglesiaBoundingBox);

			altarFinalBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-2177, 20, 2800), new TGCVector3(-1820, 500, 3023));


			//Instancia puente
			var sceneBridge = loader.loadSceneFromFile(MediaDir + @"Bridge-TgcScene.xml");
            foreach (var Mesh in sceneBridge.Meshes)
            {
                Mesh.Move(-2600, 26, 100);
                Mesh.Scale = new TGCVector3(5.1f, .50f, 1.6f);

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

            //Instancia del personaje
            MeshPersonaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];
			MeshPersonaje.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);
			MeshPersonaje.Position = new TGCVector3(-4000, 30, 532);

			var LinternaMesh = loader.loadSceneFromFile(MediaDir + "linterna-TgcScene.xml").Meshes[0];
			Linterna linterna = new Linterna(LinternaMesh, MediaDir + "\\2D\\imgLinterna.png");
			linterna.mesh.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
			linterna.mesh.RotateZ(-FastMath.PI_HALF);

			Personaje = new Personaje();
			Personaje.Init(MeshPersonaje, MediaDir, linterna);
            Personaje.mesh.RotateY(-FastMath.PI_HALF);
            Personaje.mesh.Transform = TGCMatrix.RotationY(-FastMath.PI_HALF);

			//Instancia de Bug
			bug = new Bug(MediaDir);
			Objetos.Add(bug);
			MeshARenderizar.Add(bug.mesh);
			MeshARenderizar.Add(bug.meshMounstroMiniatura);

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

            String rutaImagen;

			//Instancia de baterias
			instanciarBateria(new TGCVector3(3400, 10, 530));

			//Instancia de velas
			instanciarVela(new TGCVector3(-280, 54, 729));
			instanciarVela(new TGCVector3(-2077, 48, 2758));
			instanciarVela(new TGCVector3(-1694, 42, -2988));

			//Instancia de pista
			instanciarPista(new TGCVector3(-300, 55, 741));

			//Instancia de Piezas
			instanciarPieza(4, new TGCVector3(-1665, 65, -2971));
			instanciarPieza(5, new TGCVector3(-3460, 40, -3402));
            instanciarVela(new TGCVector3(-3470, 40, -3985));
            instanciarVela(new TGCVector3(-353, 40, 3120));
            instanciarBateria(new TGCVector3(1207, 40, -575));
			instanciarPieza(6, new TGCVector3(2654, 40, -3043));
			instanciarPieza(7, new TGCVector3(4082, 40, 1301));
			instanciarPieza(8, new TGCVector3(-4004, 40, 2830));

			//Instancia de las gafas
			var Gafas = new VisionNocturna(MediaDir);

			Gafas.mesh.Move(new TGCVector3(-1650, 65, -2950));
			Gafas.mesh.Scale = new TGCVector3(0.02f, 0.02f, 0.02f);
			Gafas.mesh.Transform = TGCMatrix.Translation(-1650, 65, -2950);

			Items.Add(Gafas);
			MeshARenderizar.Add(Gafas.mesh);

			//Instancia de la canoa
			var CanoaMesh = loader.loadSceneFromFile(MediaDir + "Canoa-TgcScene.xml").Meshes[0];
			
			rutaImagen = MediaDir + "\\2D\\canoa.png";
			var Canoa = new Canoa(CanoaMesh, rutaImagen, MediaDir);

			Canoa.mesh.Move(-2970, 40, 450);
			Canoa.mesh.Transform = TGCMatrix.Translation(-2970, 40, 450);

			Items.Add(Canoa);
			MeshARenderizar.Add(Canoa.mesh);

			//Instancia de Mesa
			var MesaMesh = loader.loadSceneFromFile(MediaDir + "MesaRedonda-TgcScene.xml").Meshes[0];
			MesaMesh.Move(-250, 20, 741);
			MesaMesh.Transform = TGCMatrix.Translation(-300, 30, 741);
			Objetos.Add(new SinEfecto(MesaMesh));
			MeshARenderizar.Add(MesaMesh);

			//Instancia del Servidor
			var Servidor = new Servidor(MediaDir);
			Servidor.mesh.Move(500, 25, 500);
			Servidor.mesh.Transform = TGCMatrix.Translation(500, 25, 500);
			Objetos.Add(Servidor);
			MeshARenderizar.Add(Servidor.mesh);

			//Instancia de Pala
			var PalaMesh = loader.loadSceneFromFile(MediaDir + "Pala-TgcScene.xml").Meshes[0];
			rutaImagen = MediaDir + "\\2D\\pala.png";
			var Pala = new Herramienta("Pala", PalaMesh, rutaImagen);
			

			Pala.mesh.Move(-220, 100, 680);
			Pala.mesh.RotateX(FastMath.PI);
			Pala.mesh.RotateZ(FastMath.PI_HALF);
			Pala.mesh.RotateY(FastMath.PI_HALF);
			Pala.mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
			Pala.mesh.Transform = TGCMatrix.Translation(-250, 100, 700);
			
			Items.Add(Pala);
			MeshARenderizar.Add(Pala.mesh);

			//Instancia de fogatas
			var scene6 = loader.loadSceneFromFile(MediaDir + "hoguera-TgcScene.xml");
            var fogataMesh = scene6.Meshes[0];
            Fogata fogata1 = new Fogata(fogataMesh.createMeshInstance("Fogata1"), new TGCVector3(-3600, 25, -3450));
            Fogata fogata2 = new Fogata(fogataMesh.createMeshInstance("Fogata2"), new TGCVector3(-4100, 20, 2900));
            Fogata fogata3 = new Fogata(fogataMesh.createMeshInstance("Fogata3"), new TGCVector3(-1665, 40, -2971));
            //Fogata fogata4 = new Fogata(Canon.createMeshInstance("Fogata4"), new TGCVector3(-350, 70, 0));
            IluminacionEscenario.Add(fogata1);
            IluminacionEscenario.Add(fogata2);
            IluminacionEscenario.Add(fogata3);
            //IluminacionEscenario.Add(fogata4);
            meshFogatas.Add(fogata1.mesh);
            meshFogatas.Add(fogata2.mesh);
			meshFogatas.Add(fogata3.mesh);
            //MeshARenderizar.Add(fogata4.mesh);
            
            int auxF = 0;
            FogatasPos = new Vector4[meshFogatas.Count];
            foreach (var fog in IluminacionEscenario) {
				fog.mesh.Move(fog.getPosicion());
				fog.mesh.Transform = TGCMatrix.Translation(fog.getPosicion());
                FogatasPos[auxF] = new Vector4 (fog.getPosicion().X+50, fog.getPosicion().Y + 55, fog.getPosicion().Z+50,1) ; //+50 en xz porque no esta centrada la hoguera -55 en y porque no se ilumina el piso sino
                auxF++;
			}

            //Instancia de motor de fisica para colisiones con mesh y terreno
            physicsExample = new Fisicas();
			physicsExample.setTerrain(terreno);
			physicsExample.setPersonaje(Personaje);
			physicsExample.setBuildings(Objetos.ConvertAll(objeto => objeto.mesh));
			physicsExample.Init(MediaDir);

			//Instancia del quadTree (optimizacion)
			quadtree = new Quadtree();
            quadtree.create(MeshARenderizar, MeshPlano.BoundingBox);

			//Instancia de la camara (primera persona)
            camaraInterna = new TgcFpsCamera(Personaje.mesh.Position, Input);
			//camaraInterna.rotateY(-FastMath.PI_HALF);
			Camara = camaraInterna;

			giroMuerte = 0;
            monstruo = loader.loadSceneFromFile(MediaDir + @"monstruo-TgcScene.xml").Meshes[0];
            monstruoSilueta = loader.loadSceneFromFile(MediaDir + @"monstruo-TgcScene.xml").Meshes[0];
            monstruoSilueta.Effect = effect;
            monstruoSilueta.Technique = "Silueta";
			monstruoSilueta.RotateY(FastMath.PI);

			fog = new TgcFog();
            fog.StartDistance = 1000f;
            fog.EndDistance = 1200f;

            SonidosInit();
        }

		public override void Update()
        {
			PreUpdate();


			if (!JuegoIniciado)
			{
				HUD.Instance.MainMenu = true;
                Personaje.mesh.Position = new TGCVector3(-4000, 50, 532);

                camaraInterna.UpdateCamera(this.ElapsedTime,this.Personaje.mesh.Position,-physicsExample.getDirector());
				Camara = camaraInterna;
				if (Input.keyPressed(Key.F)){
					JuegoIniciado = true;
					HUD.Instance.MainMenu = false;
					var rutaImagen = MediaDir + "\\2D\\texto_inicial.png";
					Personaje.agregarPista(new Pista(null,rutaImagen,null));
				}
			}
			else {
				if (giroMuerte > 180)
				{
					auxShader += ElapsedTime;
				}

				time += ElapsedTime;

				if (Pausa) ElapsedTime = 0;

				camaraInterna.UpdateCamera(this.ElapsedTime, this.Personaje.mesh.Position, -physicsExample.getDirector());
				Camara = camaraInterna;

				if (Personaje.posicionModificada)
				{
					physicsExample.setPersonaje(Personaje);
					physicsExample.Init(MediaDir);

					Personaje.posicionModificada = false;
				}

				if (!Pausa)
				{
					physicsExample.Update(Input, monstruo,monstruoSilueta);
					Personaje.Update(Input, ElapsedTime);
                    camaraInterna.UpdateCamera(this.ElapsedTime, this.Personaje.mesh.Position,- physicsExample.getDirector());
                 
				}

				objetoColisionado = null;

				if (Input.keyPressed(Key.Escape))
				{
					Pausa = !Pausa;
					HUD.Instance.MenuControles = !HUD.Instance.MenuControles;
					HUD.Instance.HUDpersonaje = !HUD.Instance.HUDpersonaje;
					HUD.Instance.HUDpersonaje_piezas = !HUD.Instance.HUDpersonaje_piezas;
					HUD.Instance.Mensaje = false;
				}

				if (Personaje.perdioJuego() && giroMuerte <= 180)
				{

					physicsExample.rotar((float)(FastMath.PI));
					giroMuerte += 1.8f;
					camaraInterna.RotationSpeed = 0;
					physicsExample.strength = 0;
					physicsExample.angle = 0;

					physicsExample.personajeBody.ActivationState = ActivationState.IslandSleeping;
				}

				var itemCerca = false;

				foreach (var item in Items)
				{
					var result = FastMath.Sqrt(TGCVector3.LengthSq(item.mesh.Position - Personaje.mesh.Position)) < 100;
					if (result)
					{
						itemCerca = true;
						HUD.Instance.MensajeRecolectable = item;
						if (Input.keyPressed(Key.E))
						{
							item.mesh.Technique = "Spotlight";
							MeshARenderizar.Remove(item.mesh);
							Items.Remove(item);
							Personaje.agregarRecolectable(item);
							quadtree.actualizarModelos(MeshARenderizar);
                            sonidoPickup.play(false);
						}
						break;
					}
				}

				if (Input.keyPressed(Key.K))
				{
					physicsExample.ModoCreativo = true;
				}

				HUD.Instance.Mensaje = itemCerca;

				var objetoCerca = false;

				foreach (var objeto in Objetos)
				{
					var result = FastMath.Sqrt(TGCVector3.LengthSq(objeto.mesh.Position - Personaje.mesh.Position)) < 150;
					if (result)
					{
						objetoCerca = true;
						HUD.Instance.Colisionado = objeto;
						if (Input.keyPressed(Key.E))
						{
							objeto.serColisionado(Personaje);
						}
						break;
					}
				}

				HUD.Instance.MensajeColisionable = objetoCerca;

				var fogatasLejos = 0;
				foreach (var iluminador in IluminacionEscenario)
				{
					var distancia = FastMath.Sqrt(TGCVector3.LengthSq(new TGCVector3(50, 0, 50) + iluminador.mesh.Position - Personaje.mesh.Position));//50 en xz es porque no esta centrada la hoguera
					var dentroFogata = distancia < 300;

					if (dentroFogata)
					{
						fogataCerca = true;

						Personaje.tiempoDesprotegido = 0;

						break;
					}

					else
					{
						fogatasLejos++;
					}

				}

				this.effect.SetValue("time", time);
				this.effect.SetValue("aux", auxShader);

				//Cabania es lugar seguro


                foreach(var lugarSeguro in lugaresSeguros)
                {
                    if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, lugarSeguro))
                    {
                        Personaje.tiempoDesprotegido = 0;
                    }

                }
			       


				if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, altarFinalBoundingBox))
				{
					if (Personaje.JuegoTerminado()) {
						HUD.Instance.MensajeExtra = true;
						HUD.Instance.MensajeExtraContenido = ganoEnRealidad ? "Iniciando windows..." : "Press [F] for linux";

                        if(ganoEnRealidad && auxShader > 5)
                        {
                            foreach(Key key in Enum.GetValues(typeof(Key)))
                            {
                                if (Input.keyPressed(key))
                                {
                                    salirDelJuego = true;
                                }
                            }
                        }

                        if (Input.keyPressed(Key.F))
                        {
                            
                            sonidoWindowsInicia.play(false);
                            ganoEnRealidad = true;
                            murioPersonaje = true;
                            monstruo.Render();
                            Personaje.setPerdioJuego(true);
                           

                        }
                        if (auxShader > 4 && auxShader<5 && ganoEnRealidad)
                        {
                            HUD.Instance.PantallaAzul = true;
                            sonidoWindowsError.play(false);
                        }

                    }
				}
				else {
					HUD.Instance.MensajeExtra = false;
				}
				

				if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, Plano.BoundingBox))
				{
					Personaje.getItems().ForEach(item => Console.WriteLine(item.getDescripcion()));
					if (Personaje.TieneCanoa())
					{
						Personaje.EquiparCanoa();
					}
					else
					{
						if (mensajeAgua == null || mensajeAgua.tiempoCumplido())
						{
							mensajeAgua = new MensajeTemporal("El agua esta muy fria para nadar");
							HUD.Instance.mensajesTemporales.Add(mensajeAgua);
						}
						physicsExample.personajeBody.ActivationState = ActivationState.ActiveTag;
						physicsExample.personajeBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
						var direccionATierra = (Personaje.mesh.Position - ultimaPosTierra);
						direccionATierra.Normalize();
						physicsExample.personajeBody.ApplyCentralImpulse(-20 * direccionATierra.ToBulletVector3());
					}
					sonidoAgua.play(false);
				}
				else {
					Personaje.DesequiparCanoa();
				}

                if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, new TgcBoundingAxisAlignBox(new TGCVector3(-2520, 0,179), new TGCVector3(-2435, 100,276))))//Dependiendo si pasa el puente o no se guarda ultima posicion en tierra
                    ultimaPosTierra = new TGCVector3(-3800, 80, 160);
                if(TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, new TgcBoundingAxisAlignBox(new TGCVector3(-1125, 0, 179), new TGCVector3(-900, 100, 276))))
                    ultimaPosTierra = new TGCVector3(1000, 80, 1200);

				if (Personaje.estaEnPeligro() && !Personaje.perdioJuego())
				{
					physicsExample.rotar((float)(0.1 * (Math.Cos(40 * time))));
				}
                
                quadtree.actualizarModelos(MeshARenderizar);
			}

            SonidosUpdate();
			var desplazamientoSilueta = -2000 * physicsExample.getDirector() * (FastMath.Min(0.65f, 0.35f + (Personaje.tiempoLimiteDesprotegido - Personaje.tiempoDesprotegido) / Personaje.tiempoLimiteDesprotegido));
			monstruoSilueta.Position = Personaje.mesh.Position + new TGCVector3(0, 20, 0);
            
            monstruoSilueta.Move(desplazamientoSilueta);
            monstruoSilueta.Scale = new TGCVector3(0.7f, 0.7f, 0.7f);
            monstruoSilueta.Transform = TGCMatrix.Translation(camaraInterna.Position.X, camaraInterna.Position.Y, camaraInterna.Position.Z) * TGCMatrix.Scaling(new TGCVector3(0.7f, 0.7f, 0.7f));

            auxShaderMonstruoAparece = FastMath.Min(0.5f, auxShaderMonstruoAparece + (float) auxShader/150);

            PostUpdate();
        }

        public override void Render()
        {
			PreRender();

          

			foreach (var mesh in MeshARenderizar) mesh.UpdateMeshTransform();

			skyBox.Render();

			var direccionLuz = physicsExample.getDirector();

            var desplazamientoLuz = direccionLuz;
            desplazamientoLuz.Multiply(-0.2f);
            var lightPos = camaraInterna.LookAt;
            lightPos.Add(desplazamientoLuz);
           
            var lightDir = -direccionLuz;
            
            effect.SetValue("ColorFog", fog.Color.ToArgb());
       
            effect.SetValue("StartFogDistance", fog.StartDistance);
            effect.SetValue("EndFogDistance", fog.EndDistance);
			//effect.SetValue("Density", fog.Density);

			if (Personaje.estaEnPeligro() && !Personaje.perdioJuego())
			{
                
                foreach (var mesh in MeshARenderizar)
				{
					mesh.Effect = effect;
					mesh.Technique = "Sepia";
				}

				monstruo.Effect = effect;
				monstruo.Technique = "Sepia";

				terreno.Effect = effect;
				terreno.Technique = "Sepia";

				foreach (var mesh in meshFogatas)
				{
					mesh.Effect = effect;
					mesh.Technique = "Sepia";

				}
				foreach (var mesh in skyBox.Faces)
				{
					mesh.Effect = effect;
					mesh.Technique = "Sepia";
				}
				foreach (var item in Items)
				{
					item.mesh.Technique = "Item";
				}
			}
			else
			{
				String tecnicaActual = (Personaje.visionNocturnaActivada) ? "VisionNocturna" : "Spotlight";
				String tecnicaItemActual = (Personaje.visionNocturnaActivada) ? "VisionNocturnaItems" : "Item";
                String tecnicaArbolActual = (Personaje.visionNocturnaActivada) ? "VisionNocturnaAB" : "SpotlightAB";
                String tecnicaAguaActual = (Personaje.visionNocturnaActivada) ? "VisionNocturnaAgua" : "Agua";

                foreach (var mesh in MeshARenderizar)
				{
					mesh.Effect = effect;
					mesh.Technique = tecnicaActual;

				}

				terreno.Effect = effect;
				terreno.Technique = tecnicaActual;

				foreach (var mesh in meshFogatas)
				{
					mesh.Effect = effect;
					mesh.Technique = tecnicaActual;

				}
				foreach (var mesh in skyBox.Faces)
				{
					mesh.Effect = effect;
					mesh.Technique = tecnicaActual;
				}
                foreach(var mesh in arbolesMesh)
                {
                    mesh.Technique = tecnicaArbolActual;
                }
				foreach (var item in Items)
				{
					item.mesh.Technique = tecnicaItemActual;
				}

				MeshPlano.Technique = tecnicaAguaActual;

			}

			foreach (var mesh in MeshARenderizar)
            {
				//mesh.Effect = effect;
				//mesh.Technique = "Spotlight";
				//Cargar variables shader de la luz FOGATA
				mesh.Effect.SetValue("lightColorFog", ColorValue.FromColor(Color.White));
				mesh.Effect.SetValue("lightPositionFog", FogatasPos);

				mesh.Effect.SetValue("lightIntensityFog", 40f);
				mesh.Effect.SetValue("lightAttenuationFog", 0.65f);
				mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(0, 1, 2)));
				//mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
				mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.FromArgb(255, 244, 191)));

				//Cargo variables Shader Linterna/Vela "SpotLight"
				mesh.Effect.SetValue("lightColorPj", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
				//Cargar variables shader de la luz
				mesh.Effect.SetValue("lightPositionPj", TGCVector3.Vector3ToFloat4Array(lightPos));
				mesh.Effect.SetValue("eyePositionPj", TGCVector3.Vector3ToFloat4Array(Camara.Position));
				mesh.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
				mesh.Effect.SetValue("lightIntensityPj", 500f);
				mesh.Effect.SetValue("lightAttenuationPj", 0.3f);
				mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(3));
				mesh.Effect.SetValue("spotLightExponent", 80f);

				//Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
				mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
				mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
				mesh.Effect.SetValue("materialSpecularExp", 9f);
                mesh.Effect.SetValue("auxMonstruoAparece", auxShaderMonstruoAparece);
			}

			physicsExample.Render();

			foreach (TgcMesh meshFog in meshFogatas)
			{
				meshFog.Render();
			}

			var desplazamiento = physicsExample.getDirector() * (180f);
            
            if (Personaje.estaEnPeligro()&&!Personaje.perdioJuego())
            {
                monstruoSilueta.Render();
            }

			if (giroMuerte == 0)
			{
				monstruo.Position = new TGCVector3(camaraInterna.Position.X + desplazamiento.X, camaraInterna.Position.Y - 60 + desplazamiento.Y, camaraInterna.Position.Z + desplazamiento.Z);
				monstruo.Scale = new TGCVector3(0.8f, 0.8f, 0.8f);
				monstruo.Transform = TGCMatrix.Translation(camaraInterna.Position.X, camaraInterna.Position.Y, camaraInterna.Position.Z) * TGCMatrix.Scaling(new TGCVector3(0.8f, 0.8f, 0.8f));
			}
            
           
			if (giroMuerte > 175)
			{

                monstruo.Effect = effect;
                monstruo.Technique = ganoEnRealidad ? "NoMeQuieroIr" : "MonstruoAparece";
               
				monstruo.Render();
				HUD.Instance.Perdio = true;
				
			}

			DrawText.drawText("Modelos Renderizados" + quadtree.cantModelosRenderizados(), 5, 20, Color.GreenYellow);
            DrawText.drawText("Monstruo aparece en: " + ((Personaje.tiempoLimiteDesprotegido - Personaje.tiempoDesprotegido > 0) ? (Personaje.tiempoLimiteDesprotegido - Personaje.tiempoDesprotegido).ToString() : "GG"), 5, 40, Color.Gold);
            
            Personaje.Render(ElapsedTime, Input, physicsExample.getDirector());

			quadtree.render(Frustum, true);

			bug.Update(time);

            PostRender();
            if (salirDelJuego)
            {
                Application.Exit(); 
                System.Windows.Forms.Application.ExitThread();
                System.Environment.Exit(0);
            }
        }
		public override void Dispose()
        {
            skyBox.Dispose();
            physicsExample.Dispose();
            monstruo.Dispose();
            
        }
        public void SonidosInit()
        {
            DirectSound.InitializeD3DDevice(new System.Windows.Forms.Control());
            //Hacer que el Listener del sonido 3D siga al personaje
            DirectSound.ListenerTracking = Personaje.mesh;
            


            foreach (var mesh in meshFogatas)
            {
                Tgc3dSound sonidoFogata;
                sonidoFogata = new Tgc3dSound(MediaDir + "Sound\\fogata.wav", mesh.Position, DirectSound.DsDevice);

                sonidoFogata.MinDistance = 60f;
                sonidos.Add(sonidoFogata);
            }

            sonidoBug = new Tgc3dSound(MediaDir + "Sound\\bug.wav", bug.mesh.Position, DirectSound.DsDevice);

            sonidoBug.MinDistance = 75f;
            sonidos.Add(sonidoBug);

            sonidoPisadas = new TgcStaticSound();
            sonidoPisadas.loadSound(MediaDir + "Sound\\pisadas.wav", DirectSound.DsDevice);

            sonidoAgua = new TgcStaticSound();
            sonidoAgua.loadSound(MediaDir + "Sound\\agua.wav", DirectSound.DsDevice);

            sonidoNota = new TgcStaticSound();
            sonidoNota.loadSound(MediaDir + "Sound\\notas.wav", DirectSound.DsDevice);

            sonidoPickup = new TgcStaticSound();
            sonidoPickup.loadSound(MediaDir + "Sound\\pickup.wav", DirectSound.DsDevice);

			sonidoAmbiente = new TgcStaticSound();
			sonidoAmbiente.loadSound(MediaDir + "Sound\\ambiente.wav", DirectSound.DsDevice);

			sonidoAgitacion = new TgcStaticSound();
			sonidoAgitacion.loadSound(MediaDir + "Sound\\agitacion.wav", DirectSound.DsDevice);

			sonidoMonstruoAcechando = new TgcStaticSound();
			sonidoMonstruoAcechando.loadSound(MediaDir + "Sound\\monstruoAcechando.wav", DirectSound.DsDevice);

			sonidoMuertePersonaje = new TgcStaticSound();
			sonidoMuertePersonaje.loadSound(MediaDir + "Sound\\muertePersonaje.wav", DirectSound.DsDevice);

			sonidoMonstruoAparece = new TgcStaticSound();
			sonidoMonstruoAparece.loadSound(MediaDir + "Sound\\monstruoAparece.wav", DirectSound.DsDevice);

            sonidoMonstruoMuere = new TgcStaticSound();
            sonidoMonstruoMuere.loadSound(MediaDir + "Sound\\monstruoAparece.wav", DirectSound.DsDevice);

            sonidoWindowsInicia = new TgcStaticSound();
            sonidoWindowsInicia.loadSound(MediaDir + "Sound\\windowsInicia.wav", DirectSound.DsDevice);

            sonidoWindowsError = new TgcStaticSound();
            sonidoWindowsError.loadSound(MediaDir + "Sound\\error.wav", DirectSound.DsDevice);

            foreach (var s in sonidos)
            {
                s.play(true);
            }
        }


        public void SonidosUpdate()
        {
            sonidoBug.Position = bug.mesh.Position;


            if (physicsExample.personajeBody.ActivationState == ActivationState.ActiveTag)
            {
                sonidoPisadas.play(true);
            }
            else
            {
                sonidoPisadas.stop();
            }

			if (JuegoIniciado)
			{
				sonidoAmbiente.play(true);
				if (Personaje.estaEnPeligro() && !murioPersonaje)
				{
					sonidoAgitacion.play(true);
					if ((Personaje.tiempoDesprotegido / 10) % 1 < 0.2f)
						sonidoMonstruoAcechando.play(false);
				}

				else
					sonidoAgitacion.stop();
				if (giroMuerte > 179 && !murioPersonaje && !ganoEnRealidad)
				{
                    
					sonidoMonstruoAparece.play(false);
					sonidoMuertePersonaje.play(false);
					murioPersonaje = true;
				}
			}
		}

		public void instanciarVela(TGCVector3 pos)
		{
			var scene5 = (new TgcSceneLoader()).loadSceneFromFile(MediaDir + "velas-TgcScene.xml");
			var VelasMesh = scene5.Meshes[0];
			VelasMesh.Move(pos);
			VelasMesh.Scale = new TGCVector3(0.03f, 0.03f, 0.03f);
			VelasMesh.Transform = TGCMatrix.Translation(pos);
			string rutaImagen = MediaDir + "\\2D\\imgVela.png";
			vela = new Vela(VelasMesh, rutaImagen);
			Items.Add(vela);
			MeshARenderizar.Add(VelasMesh);
		}

		public void instanciarPieza(int numero_pieza, TGCVector3 pos)
		{
			var pieza = new Pieza(numero_pieza, "Pieza " + numero_pieza, 
				MediaDir + "\\2D\\windows\\windows_" + numero_pieza + ".png", MediaDir + "\\windows-TgcScene.xml");

			pieza.mesh.Move(pos);
			pieza.mesh.Transform = TGCMatrix.Translation(pos);
			pieza.mesh.Scale = new TGCVector3(0.02f, 0.02f, 0.02f);

			Items.Add(pieza);
			MeshARenderizar.Add(pieza.mesh);
		}


		public void instanciarPista(TGCVector3 pos)
		{
			var PistaMesh = (new TgcSceneLoader()).loadSceneFromFile(MediaDir + "pista-TgcScene.xml").Meshes[0];
			PistaMesh.Move(pos);
			PistaMesh.Transform = TGCMatrix.Translation(pos);
			PistaMesh.Scale = new TGCVector3(0.25f, 0.25f, 0.25f);
			var rutaImagen = MediaDir + "\\2D\\pista_pala.png";
			var rutaMostrable = MediaDir + "\\2D\\EspacioPistaHUD.png";
			var pista = new Pista(PistaMesh, rutaImagen, rutaMostrable);
			Items.Add(pista);
			MeshARenderizar.Add(pista.mesh);
		}

		public void instanciarBateria(TGCVector3 pos)
		{
			var scene4 = (new TgcSceneLoader()).loadSceneFromFile(MediaDir + "Bateria-TgcScene.xml");
			var BateriaMesh = scene4.Meshes[0];
			BateriaMesh.Move(pos);
			BateriaMesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
			BateriaMesh.Transform = TGCMatrix.Translation(pos);
			var rutaImagen = MediaDir + "\\2D\\imgBateria.png";
			bateria = new Bateria(BateriaMesh, rutaImagen);
			Items.Add(bateria);
			MeshARenderizar.Add(BateriaMesh);
		}

		

	}


    }

