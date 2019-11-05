using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	class Herramienta : Item
	{
		public Herramienta(String nombre, TgcMesh _mesh, String media) {
			descripcion = nombre;
			mesh = _mesh;
			rutaImagen = media;
		}

		public override void accion(Personaje personaje)
		{
			
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
			
		}

		internal override void desactivar(Personaje personaje)
		{
			
		}
	}
}
