using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	class ArbolDirectorio : Colisionable
	{
		Pieza piezaAsociada;
		Pista pistaAsociada;
		bool usado;
		public ArbolDirectorio(String MediaDir) {
			piezaAsociada = new Pieza(1, "Pieza 1", MediaDir + "\\2D\\windows\\windows_1.png", null);
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_sudo.png", null);
			mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + @"cabania-TgcScene.xml").Meshes[0];
			usado = false;
			interactuable = true;
		}

		public override string getMensajeColision()
		{
			return "Este arbol no parece igual a los demas";
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
