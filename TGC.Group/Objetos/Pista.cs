using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	class Pista : Recolectable
	{
		String imagenHUD;

		public Pista(TgcMesh _mesh, String imagen, String imagenMostrable) {
			rutaImagen = imagen;
			this.mesh = _mesh;
			imagenHUD = imagenMostrable;
		}

		public override void Agregarse(Personaje personaje)
		{
			personaje.agregarPista(this);
		}

		public override string getRutaImagen() {
			return imagenHUD;
		}

		public override string getDescripcion()
		{
			return "Pista";
		}
	}
}
