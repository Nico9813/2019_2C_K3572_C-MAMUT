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
        public RigidBody personajeBody;
        private TGCVector3 fowardback;
        private TGCVector3 leftright;

        private TgcSimpleTerrain terreno;

        private float currentScaleXZ;
        private float currentScaleY;

        private TGCBox lightMesh;
        private TGCVector3 director;

        public float strength;
        public float angle;

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

        public void setTerrain(TgcSimpleTerrain terreno)
        {
            this.terreno = terreno;
        }

        public TGCVector3 getBodyPos()
        {
            return new TGCVector3(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y, personajeBody.CenterOfMassPosition.Z);
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

            strength = 5f;
            angle = 0.5f;

            foreach (var mesh in meshes)
            {
                var childTriangleMesh = construirTriangleMeshShape(mesh);
                var buildingbody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, mesh.Position, mesh.Scale);


                //buildingbody.Translate(mesh.Position.ToBulletVector3());
                dynamicsWorld.AddRigidBody(buildingbody);

            }

            //Se crea un plano ya que esta escena tiene problemas
            //con la definición de triangulos para el suelo
            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 10);
            floorShape.LocalScaling = new TGCVector3().ToBulletVector3();
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            floorBody = new RigidBody(floorInfo);
            floorBody.Friction = 0.5f;
            floorBody.RollingFriction = 1;
            floorBody.Restitution = 1f;
            floorBody.UserObject = "floorBody";
            dynamicsWorld.AddRigidBody(floorBody);

            var meshRigidBody = BulletRigidBodyFactory.Instance.CreateSurfaceFromHeighMap(terreno.getData());
            dynamicsWorld.AddRigidBody(meshRigidBody);

            var loader = new TgcSceneLoader();

            //Se crea el cuerpo rígido de la caja, en la definicio de CreateBox el ultimo parametro representa si se quiere o no
            //calcular el momento de inercia del cuerpo. No calcularlo lo que va a hacer es que la caja que representa el personaje
            //no rote cuando colicione contra el mundo.
            personajeBody = BulletRigidBodyFactory.Instance.CreateCapsule(10, 10, new TGCVector3(-4000, 50, 532) /*personaje.Position*/,  2.55f, false);

            personajeBody.Gravity = new TGCVector3(0, -100, 0).ToBulletVector3();
            personajeBody.SetDamping(0.3f, 0f);
            personajeBody.Restitution = 0.1f;
            personajeBody.Friction = 1;
            personajeBody.ActivationState = ActivationState.IslandSleeping;

            dynamicsWorld.AddRigidBody(personajeBody);

            director = new TGCVector3(-1, 0, 0);
        }

        public void Update(TgcD3dInput input, TgcMesh monstruo)
        {

            dynamicsWorld.StepSimulation(1 / 60f, 100);


            #region Comportamiento

            if (input.keyDown(Key.W))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                personajeBody.ActivationState = ActivationState.ActiveTag;
                personajeBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                personajeBody.ApplyCentralImpulse(-strength * director.ToBulletVector3());
            }
            if (input.keyUp(Key.W))
            {
                personajeBody.ActivationState = ActivationState.IslandSleeping;
            }
            if (input.keyUp(Key.S))
            {
                personajeBody.ActivationState = ActivationState.IslandSleeping;
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
                monstruo.RotateY(-angle * 0.01f);
            }

            if (input.keyDown(Key.D))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.01f));
                personaje.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(angle * 0.01f) * new TGCMatrix(personajeBody.InterpolationWorldTransform);
                personajeBody.WorldTransform = personaje.Transform.ToBsMatrix;
                personaje.RotateY(angle * 0.01f);
                monstruo.RotateY(angle * 0.01f);
            }


            #endregion Comportamiento
        }

        public void Render()
        {
            //Se hace el transform a la posicion que devuelve el el Rigid Body del personaje
            personaje.Position = new TGCVector3(personajeBody.CenterOfMassPosition.X, personajeBody.CenterOfMassPosition.Y - 27, personajeBody.CenterOfMassPosition.Z);//-27 para que no toque el piso pero casi (para que toque el piso -30)
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

        public TGCVector3 getDirector()
        {
            return this.director;
        }

        public TGCVector3 getPositionPersonaje()
        {
            return personaje.Position;
        }

        private BvhTriangleMeshShape construirTriangleMeshShape(TgcMesh mesh)
        {
            var vertexCoords = mesh.getVertexPositions();

            TriangleMesh triangleMesh = new TriangleMesh();
            for (int i = 0; i < vertexCoords.Length; i = i + 3)
            {
                triangleMesh.AddTriangle(vertexCoords[i].ToBulletVector3(), vertexCoords[i + 1].ToBulletVector3(), vertexCoords[i + 2].ToBulletVector3());
            }
            return new BvhTriangleMeshShape(triangleMesh, false);
        }

        private RigidBody construirRigidBodyDeTriangleMeshShape(BvhTriangleMeshShape triangleMeshShape)
        {
            var transformationMatrix = TGCMatrix.RotationYawPitchRoll(0, 0, 0).ToBsMatrix;
            DefaultMotionState motionState = new DefaultMotionState(transformationMatrix);

            var boxLocalInertia = triangleMeshShape.CalculateLocalInertia(0);
            var bodyInfo = new RigidBodyConstructionInfo(0, motionState, triangleMeshShape, boxLocalInertia);
            var rigidBody = new RigidBody(bodyInfo);
            rigidBody.Friction = 0.4f;
            rigidBody.RollingFriction = 1;
            rigidBody.Restitution = 1f;

            return rigidBody;
        }

        private RigidBody construirRigidBodyDeChildTriangleMeshShape(BvhTriangleMeshShape triangleMeshShape, TGCVector3 posicion, TGCVector3 escalado)
        {
            var transformationMatrix = TGCMatrix.RotationYawPitchRoll(0, 0, 0);
            transformationMatrix.Origin = posicion;
            DefaultMotionState motionState = new DefaultMotionState(transformationMatrix.ToBsMatrix);

            var bulletShape = new ScaledBvhTriangleMeshShape(triangleMeshShape, escalado.ToBulletVector3());
            var boxLocalInertia = bulletShape.CalculateLocalInertia(0);

            var bodyInfo = new RigidBodyConstructionInfo(0, motionState, bulletShape, boxLocalInertia);
            var rigidBody = new RigidBody(bodyInfo);
            rigidBody.Friction = 0.4f;
            rigidBody.RollingFriction = 1;
            rigidBody.Restitution = 1f;

            return rigidBody;
        }

    }
}


