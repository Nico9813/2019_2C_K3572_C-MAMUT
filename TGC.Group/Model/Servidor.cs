using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	class Servidor : Colisionable
	{
		String MediaDir;
		public Servidor(TgcMesh meshActual, String mediaDir) {
			mesh = meshActual;
			MediaDir = mediaDir;
			interactuable = true;
		}

		public override string getMensajeColision()
		{
			return "Servidor DNS";
		}

		public override void serColisionado(Personaje personaje)
		{
			if (personaje.tieneItem("Pala")) {
				personaje.agregarPieza(new Pieza(1, "Pieza 1", MediaDir + "\\2D\\windows\\windows_1.png",null));
				personaje.agregarPista(new Pista(null, MediaDir + "\\2D\\pista_hacha.png", null));
			}
		}
	}
}
