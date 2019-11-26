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

namespace TGC.Group.Objetos
{
	public abstract class Iluminador : Item
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
        public abstract void Update(Personaje personaje,float time);
		public virtual void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 camaraDir, TGCVector3 direccionLuz,string ShadersDir, List<TgcMesh> meshFogatas)
		{
		
		}
	}
}

