using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	abstract class Recolectable
	{
		public TgcMesh mesh;
		public String rutaImagen;

		abstract public String getDescripcion();
		abstract public String getRutaImagen();
		abstract public void Agregarse(Personaje personaje);
	}
}
