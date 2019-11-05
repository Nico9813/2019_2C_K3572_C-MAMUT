using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	class Bateria : Item
	{
        public Bateria(TgcMesh mesh, string rutaImagen) {
            this.mesh = mesh;
            this.descripcion = "Bateria";
			this.rutaImagen = rutaImagen;
		}

		public override void accion(Personaje personaje)
		{
			personaje.agregarBateria();
			personaje.EquiparProximoItem();
			personaje.removerItem(this);
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
		}

		internal override void desactivar(Personaje personaje)
		{
		}
	}
}
