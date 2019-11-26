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
	class ArbolDirectorio : Colisionable
	{
		Pieza piezaAsociada;
		Pista pistaAsociada;
		Item itemAsociado;
		bool usado;
		public ArbolDirectorio(String MediaDir) {
			piezaAsociada = new Pieza(1, "Pieza 1", MediaDir + "\\2D\\windows\\windows_1.png", null);
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_bug.png", null);
			itemAsociado = new Herramienta("Red", null, MediaDir + "\\2D\\redHud.png");
			mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + @"Palmera2-TgcScene.xml").Meshes[0];
			mesh.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
			mesh.setColor(System.Drawing.Color.Yellow);
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
				if (personaje.tieneItem("Hacha"))
				{
					personaje.agregarPieza(piezaAsociada);
					personaje.agregarPista(pistaAsociada);
					personaje.agregarItem(itemAsociado);
					personaje.RemoverItemPorNombre("Hacha");
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
