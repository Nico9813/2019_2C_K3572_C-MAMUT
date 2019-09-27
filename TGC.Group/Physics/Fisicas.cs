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

        private TgcSimpleTerrain terreno;

        private float currentScaleXZ;
        private float currentScaleY;

        private TgcMesh MeshPlano;
        private float tamanioMapa = 5000;

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

            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\Montes.jpg");
            Plano = new TgcPlane(new TGCVector3(-tamanioMapa / 2, 0, -tamanioMapa / 2), new TGCVector3(5000, 0, 5000), TgcPlane.Orientations.XZplane, pisoTexture, 50f, 50f);

            
            //TERRENO(HEIGHMAP)
            terreno = new TgcSimpleTerrain();
            var position = TGCVector3.Empty;
            terreno = new TgcSimpleTerrain();
            var pathTextura = MediaDir + "Textures\\Montes.jpg";
            var pathHeighmap = MediaDir + "montanias.jpg";
            currentScaleXZ = 50f;
            currentScaleY = 1.5f;
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
            personajeBody = BulletRigidBodyFactory.Instance.CreateBox(new TGCVector3(20, 17, 20), 10, new TGCVector3(100,0,-1000) /*personaje.Position*/, 0, 0, 0, 0.55f, false);
            personajeBody.Restitution = 0;
            personajeBody.Gravity = new TGCVector3(0, -100, 0).ToBulletVector3();
            dynamicsWorld.AddRigidBody(personajeBody);

            //Se carga el modelo del personaje
            personaje = loader.loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];


            
            director = new TGCVector3(0, 0, 1);


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

        public void Render(float time)
        {
            //Hacemos render de la escena.
            foreach (var mesh in meshes) mesh.Render();

            //Se hace el transform a la posicion que devuelve el el Rigid Body del personaje
            personaje.Position = new TGCVector3(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y -27, personajeBody.CenterOfMassPosition.Z);//-27 para que no toque el piso pero casi (para que toque el piso -30)
            personaje.Transform = TGCMatrix.Translation(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y, personajeBody.CenterOfMassPosition.Z);
            
            personaje.Render();
            Plano.Render();
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
            Plano.Dispose();
            terreno.Dispose();

        }

        public TGCVector3 getPositionPersonaje()
        {
            return personaje.Position;
        }
    }
}

