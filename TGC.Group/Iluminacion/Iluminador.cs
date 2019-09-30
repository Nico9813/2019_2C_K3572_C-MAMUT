using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{
	abstract class Iluminador : Item
	{
		protected TGCBox lightMesh;
		protected Color colorLuz;

		public abstract override void accion(Personaje personaje);

		public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 camaraDir, TGCVector3 direccionLuz)
		{
			foreach (var mesh in meshTotales)
			{
				mesh.Effect = TGCShaders.Instance.TgcMeshSpotLightShader;
				//El Technique depende del tipo RenderType del mesh
				mesh.Technique = TGCShaders.Instance.GetTGCMeshTechnique(mesh.RenderType);
			}
			terreno.Effect = TGCShaders.Instance.TgcMeshSpotLightShader;
			terreno.Technique = "DIFFUSE_MAP";

			var desplazamiento = direccionLuz;
			desplazamiento.Multiply(-1f);
			var lightPos = camaraDir;
			lightPos.Add(desplazamiento);
			lightMesh.Position = lightPos;
			var lightDir = -direccionLuz;

			foreach (var mesh in meshTotales)
			{
				//Cargar variables shader de la luz
				mesh.Effect.SetValue("lightColor", ColorValue.FromColor(colorLuz));
				mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPos));
				mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lightPos));
				mesh.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
				mesh.Effect.SetValue("lightIntensity", 50f);
				mesh.Effect.SetValue("lightAttenuation", 0.3f);
				mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(19));
				mesh.Effect.SetValue("spotLightExponent", 7f);

				//Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
				mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(12, 12, 12)));
				mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(colorLuz));
				mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(colorLuz));
				mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(colorLuz));
				mesh.Effect.SetValue("materialSpecularExp", 9f);
			}

			//Cargar variables shader de la luz
			terreno.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
			terreno.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPos));
			terreno.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lightPos));
			terreno.Effect.SetValue("spotLightDir", TGCVector3.Vector3ToFloat3Array(lightDir));
			terreno.Effect.SetValue("lightIntensity", 50f);
			terreno.Effect.SetValue("lightAttenuation", 0.3f);
			terreno.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(19));
			terreno.Effect.SetValue("spotLightExponent", 7f);

			//Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
			terreno.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(12, 12, 12)));
			terreno.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(colorLuz));
			terreno.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(colorLuz));
			terreno.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(colorLuz));
			terreno.Effect.SetValue("materialSpecularExp", 9f);
		}
	}
}

