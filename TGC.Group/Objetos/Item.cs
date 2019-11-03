using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	abstract class Item : Recolectable
	{
		public String descripcion;

		public abstract void accion(Personaje personaje);
		public abstract void update(Personaje personaje, float elapsedTime);
        public override String getDescripcion()
        {
            return this.descripcion;
        }

		public override void Agregarse(Personaje personaje)
		{
			personaje.agregarItem(this);
		}

		internal abstract void desactivar(Personaje personaje);
	}
}
