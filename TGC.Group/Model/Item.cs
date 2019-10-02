using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
	abstract class Item
	{
		public String descripcion;
		public TgcMesh mesh;

		public abstract void accion(Personaje personaje);
		public abstract void update(Personaje personaje, float elapsedTime);
        public String getDescripcion()
        {
            return this.descripcion;
        }
        public virtual String getDuracionRestante()
        {
            return " (1 uso)";
        }

		internal abstract void desactivar(Personaje personaje);
	}
}
