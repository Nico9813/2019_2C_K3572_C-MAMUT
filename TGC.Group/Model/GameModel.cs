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
using Device = Microsoft.DirectX.Direct3D.Device;
using TGC.Core.Example;
using TGC.Group.Sprites;
using Microsoft.DirectX;
using Effect = Microsoft.DirectX.Direct3D.Effect;


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

		private Boolean Pausa = false;

		private TgcPlane Plano;
		private Personaje Personaje;
		private TgcMesh MeshPersonaje;

		private TgcSkyBox skyBox;

		private List<Recolectable> Items;
		private List<Pieza> Piezas;
		private List<TgcMesh> Objetos;
		private List<TgcMesh> MeshARenderizar;
        private List<TgcMesh> meshFogatas;

		private List<Fogata> IluminacionEscenario;

		private List<TgcMesh> MeshTotales;
		private Boolean itemCerca = false;
        private Boolean fogataCerca = false;
        Recolectable objetoColisionado = null;

		private TgcMesh MeshPlano;
		private TgcMesh MeshLago;
		private TgcBoundingElipsoid lago;
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
        private Vector4[] FogatasPos;
        TgcBoundingAxisAlignBox cabaniaBoundingBox;

        private Effect effect;
        float time = 0;

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
            Objetos = new List<TgcMesh>();
            MeshARenderizar = new List<TgcMesh>();
            meshFogatas = new List<TgcMesh>();
            IluminacionEscenario = new List<Fogata>();

            //Instancio el terreno (Heigthmap)
            terreno = new TgcSimpleTerrain();
            var pathTextura = MediaDir + "Textures\\mapa1.jpg";
            var pathHeighmap = MediaDir + "mapa1.jpg";
            currentScaleXZ = 100f;
            currentScaleY = 3f;
            terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -30, 0));
            terreno.loadTexture(pathTextura);
            terreno.AlphaBlendEnable = true;

            //Instancio el piso
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\water2.jpg");
            Plano = new TgcPlane(new TGCVector3(-tamanioMapa / 2, 0, -tamanioMapa / 2), new TGCVector3(tamanioMapa, 0, tamanioMapa), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f);
            MeshPlano = Plano.toMesh("MeshPlano");
            Objetos.Add(MeshPlano);
			MeshARenderizar.Add(MeshPlano);

			//Instancio la vegetacion
			var scene = loader.loadSceneFromFile(MediaDir + @"Vegetacion-TgcScene.xml");
			List<TGCVector3> posicionesArboles = new List<TGCVector3>();

			posicionesArboles.Add(new TGCVector3(1,1,1));
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


            for (var i = 0; i< scene.Meshes.Count && i<posicionesArboles.Count; i++)
            {
                scene.Meshes[i].Scale = new TGCVector3(1.5f, 1.5f, 1.5f);
				scene.Meshes[i].Move(posicionesArboles[i]);
				scene.Meshes[i].Transform = TGCMatrix.Translation(posicionesArboles[i]);
				Objetos.Add(scene.Meshes[i]);
                MeshARenderizar.Add(scene.Meshes[i]);
				i++;
            }

            //Instancio la Cabania
            var sceneCabania = loader.loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml");
            foreach (var Mesh in sceneCabania.Meshes)
            {
                Mesh.Move(-500, 20, 500);
                Mesh.Scale = new TGCVector3(4.5f, 4.5f, 4.5f);

                Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

                Objetos.Add(Mesh);
                MeshARenderizar.Add(Mesh);
            }
            cabaniaBoundingBox = new TgcBoundingAxisAlignBox(new TGCVector3(-500, 20, 500), new TGCVector3(0, 1001, 1080));

            var sceneBridge = loader.loadSceneFromFile(MediaDir + @"Bridge-TgcScene.xml");
            foreach (var Mesh in sceneBridge.Meshes)
            {
                Mesh.Move(-2561, 12, 159);
                Mesh.Scale = new TGCVector3(4.5f, .75f, 1.35f);

                Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

                Objetos.Add(Mesh);
                MeshARenderizar.Add(Mesh);
            }


            var sceneCanoa = loader.loadSceneFromFile(MediaDir + @"Canoa-TgcScene.xml");
            foreach (var Mesh in sceneCanoa.Meshes)
            {
                Mesh.Move(-482, 20, -3110);
                Mesh.Scale = new TGCVector3(1.5f, 1.5f, 1.5f);

                Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale);

                Objetos.Add(Mesh);
                MeshARenderizar.Add(Mesh);
            }

            //Instancia del personaje
            MeshPersonaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];
            Personaje = new Personaje();
            Personaje.Init(MeshPersonaje, MediaDir);
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

            String rutaImagen;



            //Instancia de baterias
            var scene4 = loader.loadSceneFromFile(MediaDir + "Bateria-TgcScene.xml");
            var BateriaMesh = scene4.Meshes[0];
            BateriaMesh.Move(-3400, 10, 530);
            BateriaMesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
            BateriaMesh.Transform = TGCMatrix.Translation(-3400, 10, 530);
            rutaImagen = MediaDir + "\\2D\\imgBateria.png";
            bateria = new Bateria(BateriaMesh, rutaImagen);
            Items.Add(bateria);
            MeshARenderizar.Add(BateriaMesh);

			//Instancio piezas
			for (var j = 0; j < 9; j++)
			{
				rutaImagen = MediaDir + "\\2D\\windows\\windows_" + (j+1) + ".png";
				var pieza = new Pieza(j, "Pieza" + j, rutaImagen, MediaDir + "Bateria-TgcScene.xml");
				Items.Add(pieza);
				Personaje.agregarPieza(pieza);
			}

			//Instancia de velas
			var scene5 = loader.loadSceneFromFile(MediaDir + "velas-TgcScene.xml");
            var VelasMesh = scene5.Meshes[0];
            VelasMesh.Move(-3400, 10, 400);
            VelasMesh.Scale = new TGCVector3(0.03f, 0.03f, 0.03f);
            VelasMesh.Transform = TGCMatrix.Translation(-3400, 10, 400);
            rutaImagen = MediaDir + "\\2D\\imgVela.png";
            vela = new Vela(VelasMesh, rutaImagen);
            Items.Add(vela);
            MeshARenderizar.Add(VelasMesh);

            //Instancia de fogatas
            var scene6 = loader.loadSceneFromFile(MediaDir + "hoguera-TgcScene.xml");
            var fogataMesh = scene6.Meshes[0];
            Fogata fogata1 = new Fogata(fogataMesh.createMeshInstance("Fogata1"), new TGCVector3(-3500, 25, -3200));
            Fogata fogata2 = new Fogata(fogataMesh.createMeshInstance("Fogata2"), new TGCVector3(-4100, 20, 2900));
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
        }

		public override void Update()
        {
			PreUpdate();
            time += ElapsedTime;

			if (Pausa) ElapsedTime = 0;

			if (!Pausa) {
				physicsExample.Update(Input, monstruo);
				Personaje.Update(Input, ElapsedTime);

				if (Input.keyDown(Key.A))
				{
					camaraInterna.rotateY(-0.005f);
				}

				if (Input.keyDown(Key.D))
				{
					camaraInterna.rotateY(0.5f * 0.01f);
				}
			}

			itemCerca = false;
			objetoColisionado = null;

			if (Input.keyPressed(Key.Escape))
			{
				Pausa = !Pausa;
				HUD.Instance.MenuControles = !HUD.Instance.MenuControles;
				HUD.Instance.HUDpersonaje = !HUD.Instance.HUDpersonaje;
				HUD.Instance.HUDpersonaje_piezas = !HUD.Instance.HUDpersonaje_piezas;
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
					Personaje.agregarRecolectable(objetoColisionado);
					quadtree.actualizarModelos(MeshARenderizar);
				}
			}
            var fogatasLejos = 0;
            foreach (var iluminador in IluminacionEscenario)
            {
                var distancia = FastMath.Sqrt(TGCVector3.LengthSq(new TGCVector3(50,0,50) + iluminador.mesh.Position - Personaje.mesh.Position));//50 en xz es porque no esta centrada la hoguera
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

            //Cabania es lugar seguro
			
            if (TgcCollisionUtils.testAABBAABB(Personaje.mesh.BoundingBox, cabaniaBoundingBox))
            {
                Personaje.tiempoDesprotegido = 0;
            }

			camaraInterna.Target = physicsExample.getPersonaje().Position;
            if (Personaje.estaEnPeligro())
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
            }
            else 
            {
                foreach (var mesh in MeshARenderizar)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "Spotlight";

                }

                terreno.Effect = effect;
                terreno.Technique = "Spotlight";
            }

            PostUpdate();
        }

        public override void Render()
        {
			PreRender();

			if (itemCerca)
			{
				HUD.Instance.Mensaje = true;
				HUD.Instance.MensajeRecolectable = objetoColisionado;
			}
			else {
				HUD.Instance.Mensaje = false;
			}


			skyBox.Render();

			var direccionLuz = physicsExample.getDirector();

            var desplazamientoLuz = direccionLuz;
            desplazamientoLuz.Multiply(-1f);
            var lightPos = camaraInterna.LookAt;
            lightPos.Add(desplazamientoLuz);
           
            var lightDir = -direccionLuz;

            foreach (var mesh in MeshARenderizar)
            {
                //Cargar variables shader de la luz FOGATA
                mesh.Effect.SetValue("lightColorFog", ColorValue.FromColor(Color.FromArgb(255, 244, 191)));
                mesh.Effect.SetValue("lightPositionFog", FogatasPos);
                
                mesh.Effect.SetValue("lightIntensityFog", 500f);
                mesh.Effect.SetValue("lightAttenuationFog", 0.65f);
				mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(1,2,3)));
				//mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
				mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.Yellow));

                //Cargo variables Shader Linterna/Vela "SpotLight"
                mesh.Effect.SetValue("lightColorPj", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
                //Cargar variables shader de la luz
                mesh.Effect.SetValue("lightPositionPj", TGCVector3.Vector3ToFloat4Array(lightPos));
                mesh.Effect.SetValue("eyePositionPj", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                mesh.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
                mesh.Effect.SetValue("lightIntensityPj", 150f);
                mesh.Effect.SetValue("lightAttenuationPj", 0.3f);
                mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(3));
                mesh.Effect.SetValue("spotLightExponent", 35f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
                mesh.Effect.SetValue("materialSpecularExp", 9f);
            }

			//Cargar variables shader de la luz FOGATA
			terreno.Effect.SetValue("lightColorFog", ColorValue.FromColor(Color.FromArgb(255, 244, 191)));
			terreno.Effect.SetValue("lightPositionFog", FogatasPos);

			terreno.Effect.SetValue("lightIntensityFog", 500f);
			terreno.Effect.SetValue("lightAttenuationFog", 0.65f);
			terreno.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(1, 2, 3)));
			//mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
			terreno.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.Yellow));

			//Cargo variables Shader Linterna/Vela "SpotLight"
			terreno.Effect.SetValue("lightColorPj", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
			//Cargar variables shader de la luz
			terreno.Effect.SetValue("lightPositionPj", TGCVector3.Vector3ToFloat4Array(lightPos));
			terreno.Effect.SetValue("eyePositionPj", TGCVector3.Vector3ToFloat4Array(Camara.Position));
			terreno.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
			terreno.Effect.SetValue("lightIntensityPj", 150f);
			terreno.Effect.SetValue("lightAttenuationPj", 0.3f);
			terreno.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(3));
			terreno.Effect.SetValue("spotLightExponent", 35f);

			//Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
			terreno.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
			terreno.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Personaje.getIluminadorPrincipal().getColor()));
			terreno.Effect.SetValue("materialSpecularExp", 9f);

			quadtree.render(Frustum, true);

			physicsExample.Render();

			foreach (TgcMesh meshFog in meshFogatas)
			{
				meshFog.Render();
			}

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

			Personaje.Render(ElapsedTime, Input);

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
