using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	class Mapa : Item
	{
		public Mapa(TgcMesh meshMapa, String imagenHUD) {
			this.descripcion = "Mapa";
			this.rutaImagen = imagenHUD;
			this.mesh = meshMapa;
		}
		public override void accion(Personaje personaje)
		{
			HUD.Instance.MapaPersonaje = true;
		}

		public override void update(Personaje personaje, float elapsedTime){}

		internal override void desactivar(Personaje personaje)
		{
			HUD.Instance.MapaPersonaje = false;
		}
	}
}
