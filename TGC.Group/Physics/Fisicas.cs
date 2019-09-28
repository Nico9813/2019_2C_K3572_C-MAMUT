using BulletSharp;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.BulletPhysics;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Example;
using TGC.Core.Terrain;
using TGC.Core.Shaders;

namespace TGC.Examples.Physics.CubePhysic
{
    public class Fisicas
    {
        //config mundo Fisico
        private DiscreteDynamicsWorld dynamicsWorld;

        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;

        private List<TgcMesh> meshes = new List<TgcMesh>();
        
        private RigidBody floorBody;

        private TgcMesh personaje;
        private RigidBody personajeBody;
        private TGCVector3 fowardback;
        private TGCVector3 leftright;
        private TGCVector3 director;

        private TgcPlane Plano;
        private TgcMesh PlanoMesh;

        private TgcSimpleTerrain terreno;

        private float currentScaleXZ;
        private float currentScaleY;

        private TgcMesh MeshPlano;
        private float tamanioMapa = 10000;

        private TGCBox lightMesh;

        public void setPersonaje(TgcMesh personaje)
        {
            this.personaje = personaje;
        }

        public TgcMesh getPersonaje()
        {
            return this.personaje;
        }

        public void setBuildings(List<TgcMesh> meshes)
        {
            this.meshes = meshes;
        }

        public TGCVector3 getBodyPos()
        {
            return new TGCVector3(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y, personajeBody.CenterOfMassPosition.Z);
        }
        public TGCVector3 getDirector()
        {
            return this.director;
        }
        public TgcPlane getPlano()
        {
            return this.Plano;
        }

        public void Init(string MediaDir)
        {
            #region Configuracion Basica de World

            //Creamos el mundo fisico por defecto.
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase(); //AxisSweep3(new BsVector3(-5000f, -5000f, -5000f), new BsVector3(5000f, 5000f, 5000f), 8192);
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = new TGCVector3(0, -100f, 0).ToBulletVector3();

            #endregion Configuracion Basica de World

            foreach (var mesh in meshes)
            {
                var buildingbody = BulletRigidBodyFactory.Instance.CreateRigidBodyFromTgcMesh(mesh);
                buildingbody.Translate(mesh.Position.ToBulletVector3());
                dynamicsWorld.AddRigidBody(buildingbody);
            }

            //Se crea un plano ya que esta escena tiene problemas
            //con la definición de triangulos para el suelo
            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 10);
            floorShape.LocalScaling = new TGCVector3().ToBulletVector3();
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            floorBody = new RigidBody(floorInfo);
            floorBody.Friction = 1;
            floorBody.RollingFriction = 1;
            floorBody.Restitution = 1f;
            floorBody.UserObject = "floorBody";
            dynamicsWorld.AddRigidBody(floorBody);

            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\Piso.jpg");
            Plano = new TgcPlane(new TGCVector3(-tamanioMapa / 2, 0, -tamanioMapa / 2), new TGCVector3(tamanioMapa, 0, tamanioMapa), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f);
            PlanoMesh = Plano.toMesh("PlanoMesh");
            meshes.Add(PlanoMesh);
            var PlanoBody = BulletRigidBodyFactory.Instance.CreateRigidBodyFromTgcMesh(PlanoMesh);
            dynamicsWorld.AddRigidBody(PlanoBody);
            
            //TERRENO(HEIGHMAP)
            terreno = new TgcSimpleTerrain();
            var position = TGCVector3.Empty;
            
            var pathTextura = MediaDir + "Textures\\Montes.jpg";
            var pathHeighmap = MediaDir + "montanias.jpg";
            currentScaleXZ = 100f;
            currentScaleY = 3f;
            terreno.loadHeightmap(pathHeighmap, currentScaleXZ, currentScaleY, new TGCVector3(0, -15, 0));
            terreno.loadTexture(pathTextura);

            terreno.AlphaBlendEnable = true;

            var meshRigidBody = BulletRigidBodyFactory.Instance.CreateSurfaceFromHeighMap(terreno.getData());
            dynamicsWorld.AddRigidBody(meshRigidBody);

            var loader = new TgcSceneLoader();
            ///Se crea una caja para que haga las veces del personaje dentro del modelo físico
            TgcTexture texture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + @"\Textures\lavafloor.jpg");
            TGCBox boxMesh1 = TGCBox.fromSize(new TGCVector3(20, 20, 20), texture);
            boxMesh1.Position = new TGCVector3(0, 10, 0);
            personaje = boxMesh1.ToMesh("box");
            boxMesh1.Dispose();

            //Se crea el cuerpo rígido de la caja, en la definicio de CreateBox el ultimo parametro representa si se quiere o no
            //calcular el momento de inercia del cuerpo. No calcularlo lo que va a hacer es que la caja que representa el personaje
            //no rote cuando colicione contra el mundo.
            personajeBody = BulletRigidBodyFactory.Instance.CreateBox(new TGCVector3(20, 17, 20), 10, new TGCVector3(100,50,-1000) /*personaje.Position*/, 0, 0, 0, 0.55f, false);
            personajeBody.Restitution = 0;
            personajeBody.Gravity = new TGCVector3(0, -100, 0).ToBulletVector3();
            dynamicsWorld.AddRigidBody(personajeBody);

            //Se carga el modelo del personaje
            personaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];

            director = new TGCVector3(0, 0, 1);
            
            //Mesh para la luz
            lightMesh = TGCBox.fromSize(new TGCVector3(10, 10, 10), Color.Red);

            
            

            
        }

        public void Update(TgcD3dInput input)
        {
            var strength = 12.30f;
            dynamicsWorld.StepSimulation(1 / 60f, 100);
            var angle = 0.5f;

            #region Comportamiento

            if (input.keyDown(Key.W))
            {
                
                //Activa el comportamiento de la simulacion fisica para la capsula
                personajeBody.ActivationState = ActivationState.ActiveTag;
                personajeBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                personajeBody.ApplyCentralImpulse(-strength * director.ToBulletVector3());
            }

            if (input.keyDown(Key.S))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                personajeBody.ActivationState = ActivationState.ActiveTag;
                personajeBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                personajeBody.ApplyCentralImpulse(strength * director.ToBulletVector3());
            }

            if (input.keyDown(Key.A))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.01f));
                personaje.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(-angle * 0.01f) * new TGCMatrix(personajeBody.InterpolationWorldTransform);
                personajeBody.WorldTransform = personaje.Transform.ToBsMatrix;
                personaje.RotateY(-angle * 0.01f);
            }

            if (input.keyDown(Key.D))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.01f));
                personaje.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(angle * 0.01f) * new TGCMatrix(personajeBody.InterpolationWorldTransform);
                personajeBody.WorldTransform = personaje.Transform.ToBsMatrix;
                personaje.RotateY(angle * 0.01f);
            }

            #endregion Comportamiento
        }

        public void Render(float time,TGCVector3 lookAtCamera)
        {
            foreach (var mesh in meshes)
            {
                mesh.Effect = TGCShaders.Instance.TgcMeshSpotLightShader; 
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = TGCShaders.Instance.GetTGCMeshTechnique(mesh.RenderType);
            }
            terreno.Effect = TGCShaders.Instance.TgcMeshSpotLightShader;
            terreno.Technique = "DIFFUSE_MAP";

            var desplazamiento = this.getDirector();
            desplazamiento.Multiply(-350f);
            var lightPos = lookAtCamera;
            lightPos.Add(desplazamiento);
            lightMesh.Position = lightPos;
            var lightDir = this.getDirector();
            

            foreach (var mesh in meshes)
            {

                //Cargar variables shader de la luz
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPos));
                mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lookAtCamera));
                mesh.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
                mesh.Effect.SetValue("lightIntensity", 35f);
                mesh.Effect.SetValue("lightAttenuation", 0.3f);
                mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(39));
                mesh.Effect.SetValue("spotLightExponent", 7f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(12, 12, 12)));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 9f);

            }
            //Cargar variables shader de la luz
            terreno.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            terreno.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPos));
            terreno.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lookAtCamera));
            terreno.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
            terreno.Effect.SetValue("lightIntensity", 35f);
            terreno.Effect.SetValue("lightAttenuation", 0.3f);
            terreno.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(39));
            terreno.Effect.SetValue("spotLightExponent", 7f);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            terreno.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(12, 12, 12)));
            terreno.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            terreno.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            terreno.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            terreno.Effect.SetValue("materialSpecularExp", 9f);
            //Hacemos render de la escena.
            foreach (var mesh in meshes) mesh.Render();

            //Se hace el transform a la posicion que devuelve el el Rigid Body del personaje
            personaje.Position = new TGCVector3(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y -27, personajeBody.CenterOfMassPosition.Z);//-27 para que no toque el piso pero casi (para que toque el piso -30)
            personaje.Transform = TGCMatrix.Translation(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y, personajeBody.CenterOfMassPosition.Z);
            
            personaje.Render();
            
            terreno.Render();
        }

        public void Dispose()
        {
            //Se hace dispose del modelo fisico.
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
            personajeBody.Dispose();
            floorBody.Dispose();

            //Dispose de Meshes
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Dispose();
            }

            personaje.Dispose();
           
            terreno.Dispose();

        }

        public TGCVector3 getPositionPersonaje()
        {
            return personaje.Position;
        }
    }
}

