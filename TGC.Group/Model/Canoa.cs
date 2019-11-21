using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	class Canoa : Item
	{
		TGCVector3 rotacion_inicial;
		public Canoa(TgcMesh mesh, string rutaImagen) {
			this.mesh = mesh;
			this.rutaImagen = rutaImagen;
			this.descripcion = "Canoa";
		}

		public override void Agregarse(Personaje personaje)
		{
			this.mesh.RotateY(FastMath.QUARTER_PI);
			rotacion_inicial = mesh.Rotation;
			personaje.agregarItem(this);
		}

		public void ReiniciarPosicion(Personaje personaje) {
			mesh.Rotation = personaje.mesh.Rotation;
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
