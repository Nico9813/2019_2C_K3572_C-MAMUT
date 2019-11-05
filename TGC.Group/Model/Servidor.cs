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
		Pieza piezaAsociada;
		Pista pistaAsociada;
		bool usado;
		public Servidor(String MediaDir) {
			piezaAsociada = new Pieza(0, "Pieza 0", MediaDir + "\\2D\\windows\\windows_0.png", null);
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_hacha.png", null);
			mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + @"servidor-TgcScene.xml").Meshes[0];
			usado = false;
			interactuable = true;
		}

		public override string getMensajeColision()
		{
			return "Servidor DNS";
		}

		public override void serColisionado(Personaje personaje)
		{
			if (!usado)
			{
				if (personaje.tieneItem("Pala"))
				{
					personaje.agregarPieza(piezaAsociada);
					personaje.agregarPista(pistaAsociada);
					usado = true;
				}
				else
				{
					HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("Parece que necesito algo mas para que esto funcione"));
				}
			}
			else
			{
				HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("Esto no sirve, ya lo use antes"));
			}
		}
	}
}
