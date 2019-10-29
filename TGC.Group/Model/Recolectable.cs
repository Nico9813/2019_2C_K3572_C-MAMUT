using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
	abstract class Recolectable
	{
		public TgcMesh mesh;
		public String rutaImagen;

		abstract public String getDescripcion();
		public String getRutaImagen()
		{
			return rutaImagen;
		}
		abstract public void Agregarse(Personaje personaje);
	}
}
