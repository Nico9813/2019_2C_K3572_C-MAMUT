using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Examples.Camara;
using TGC.Examples.Example;
using TGC.Examples.UserControls.Modifier;

namespace TGC.Group.Iluminacion
{
	class Linterna
	{

		private TGCBooleanModifier lightEnableModifier;
		private TGCVertex3fModifier lightPosModifier;
		private TGCVertex3fModifier lightDirModifier;
		private TGCColorModifier lightColorModifier;
		private TGCFloatModifier lightIntensityModifier;
		private TGCFloatModifier lightAttenuationModifier;
		private TGCFloatModifier specularExModifier;
		private TGCFloatModifier spotAngleModifier;
		private TGCFloatModifier spotExponentModifier;
		private TGCColorModifier mEmissiveModifier;
		private TGCColorModifier mAmbientModifier;
		private TGCColorModifier mDiffuseModifier;
		private TGCColorModifier mSpecularModifier;

		private TGCBox lightMesh;
		private Modificadores modificadores;
		public void Init()
		{
			

			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(10, 10, 10), Color.Red);
			modificadores = new Modificadores();

			//Modifiers de la luz
			lightEnableModifier = modificadores.AddBoolean("lightEnable", "lightEnable", true);
			lightPosModifier = modificadores.AddVertex3f("lightPos", new TGCVector3(-200, -100, -200), new TGCVector3(200, 200, 300), new TGCVector3(-60, 90, 175));
			lightDirModifier = modificadores.AddVertex3f("lightDir", new TGCVector3(-1, -1, -1), TGCVector3.One, new TGCVector3(-0.05f, 0, 0));
			lightColorModifier = modificadores.AddColor("lightColor", Color.White);
			lightIntensityModifier = modificadores.AddFloat("lightIntensity", 0, 150, 35);
			lightAttenuationModifier = modificadores.AddFloat("lightAttenuation", 0.1f, 2, 0.3f);
			specularExModifier = modificadores.AddFloat("specularEx", 0, 20, 9f);
			spotAngleModifier = modificadores.AddFloat("spotAngle", 0, 180, 39f);
			spotExponentModifier = modificadores.AddFloat("spotExponent", 0, 20, 7f);

			//Modifiers de material
			mEmissiveModifier = modificadores.AddColor("mEmissive", Color.Black);
			mAmbientModifier = modificadores.AddColor("mAmbient", Color.White);
			mDiffuseModifier = modificadores.AddColor("mDiffuse", Color.White);
			mSpecularModifier = modificadores.AddColor("mSpecular", Color.White);
		}

		public void Update()
		{
			throw new NotImplementedException();
		}

		public void Render(List<TgcMesh> meshTotales,MamutCamara camara)
		{
			var lightEnable = true;
			Effect currentShader;
			if (lightEnable)
			{
				//Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con SpotLight
				currentShader = TGCShaders.Instance.TgcMeshSpotLightShader;
			}
			else
			{
				//Sin luz: Restaurar shader default
				currentShader = TGCShaders.Instance.TgcMeshShader;
			}

			//Aplicar a cada mesh el shader actual
			foreach (var mesh in meshTotales)
			{
				mesh.Effect = currentShader;
				//El Technique depende del tipo RenderType del mesh
				mesh.Technique = TGCShaders.Instance.GetTGCMeshTechnique(mesh.RenderType);
			}

			//Actualzar posicion de la luz
			var lightPos = lightPosModifier.Value;
			lightMesh.Position = lightPos;

			//Normalizar direccion de la luz
			var lightDir = lightDirModifier.Value;
			lightDir.Normalize();

			//Renderizar meshes
			foreach (var mesh in meshTotales)
			{
				if (lightEnable)
				{
					//Cargar variables shader de la luz
					mesh.Effect.SetValue("lightColor", ColorValue.FromColor(lightColorModifier.Value));
					mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPos));
					mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(camara.Position));
					mesh.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
					mesh.Effect.SetValue("lightIntensity", lightIntensityModifier.Value);
					mesh.Effect.SetValue("lightAttenuation", lightAttenuationModifier.Value);
					mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(spotAngleModifier.Value));
					mesh.Effect.SetValue("spotLightExponent", spotExponentModifier.Value);

					//Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
					mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(mEmissiveModifier.Value));
					mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(mAmbientModifier.Value));
					mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(mDiffuseModifier.Value));
					mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(mSpecularModifier.Value));
					mesh.Effect.SetValue("materialSpecularExp", specularExModifier.Value);
				}

				//Renderizar modelo
				mesh.Render();
			}

			//Renderizar mesh de luz
			lightMesh.Render();
		}

		public void Dispose()
		{
		}
	}
}
