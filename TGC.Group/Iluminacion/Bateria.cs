using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{
	class Bateria : Item
	{
        public Bateria(TgcMesh mesh) {
            this.mesh = mesh;
            this.descripcion = "Bateria";
		}

		public override void accion(Personaje personaje)
		{
			personaje.agregarBateria();
			personaje.removerItem(this);
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
		}
	}
}
