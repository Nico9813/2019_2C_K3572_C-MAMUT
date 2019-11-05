using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	class Pieza : Recolectable
	{
		public String descripcion;
		public int numeroPieza;

		public Pieza(int np, String descrip, String imagen, String rutaMesh) {
			numeroPieza = np;
			descripcion = descrip;
			rutaImagen = imagen;
			if (rutaMesh != null) {
				mesh = (new TgcSceneLoader()).loadSceneFromFile(rutaMesh).Meshes[0];
			}
			
		}
		public override String getRutaImagen()
		{
			return this.rutaImagen;
		}
		public override String getDescripcion() { return descripcion; }
		public override void Agregarse(Personaje personaje) {
			personaje.agregarPieza(this);
		}
	}
}
