using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{
	class Fogata : Iluminador
	{

		TGCVector3 posicion;
		public Fogata(TgcMesh mesh,TGCVector3 posicion)
		{
			this.mesh = mesh;
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.DarkOrange;
			this.posicion = posicion;
		}

		public override void accion(Personaje personaje)
		{
			throw new NotImplementedException();
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
			throw new NotImplementedException();
		}
		/*public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, string ShadersDir, List<TgcMesh> meshFogatas) {
			base.Render(meshTotales, terreno, new TGCVector3 (posicion.X +50 , posicion.Y +150, posicion.Z + 50), new TGCVector3(0, 1, 0),ShadersDir,meshFogatas); //50 en xz es porque no esta centrada la hoguera
		}
        */
		internal TGCVector3 getPosicion()
		{
			return posicion;
		}

		internal override void desactivar(Personaje personaje)
		{
			throw new NotImplementedException();
		}

		public override float getDuracionRestante()
		{
			throw new NotImplementedException();
		}

		public override float getDuracionTotal()
		{
			throw new NotImplementedException();
		}
	}
}
