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
        protected TGCVector4[] FogatasPos;

		public abstract override void accion(Personaje personaje);
		public abstract float getDuracionRestante();
		public abstract float getDuracionTotal();
        public Color getColor()
        {
            return this.colorLuz;
        }
		public virtual void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 camaraDir, TGCVector3 direccionLuz,string ShadersDir, List<TgcMesh> meshFogatas)
		{
		/*	foreach (var mesh in meshTotales)
			{
				mesh.Effect = TGCShaders.Instance.LoadEffect(ShadersDir + "Iluminacion.fx"); ;
				
				mesh.Technique = "Fogatas";
                
			}
			//terreno.Effect = TGCShaders.Instance.TgcMeshSpotLightShader;
			//terreno.Technique = "DIFFUSE_MAP";

			var desplazamiento = direccionLuz;
			desplazamiento.Multiply(-1f);
			var lightPos = camaraDir;
			lightPos.Add(desplazamiento);
			lightMesh.Position = lightPos;
			var lightDir = -direccionLuz;
            

            foreach (var mesh in meshTotales)
			{
				//Cargar variables shader de la luz
				mesh.Effect.SetValue("lightColorFog", ColorValue.FromColor(Color.Yellow));
                mesh.Effect.SetValue("lightPosition", FogatasPos);
                mesh.Effect.SetValue("lightIntensity", 0.5f);
                mesh.Effect.SetValue("lightAttenuation", 0.5f);
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.DarkSlateGray));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.DarkSlateGray));
            }
            */
		}
	}
}

