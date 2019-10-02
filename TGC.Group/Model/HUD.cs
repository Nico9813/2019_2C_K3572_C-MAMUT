using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Text;

namespace TGC.Group.Model
{
	class HUD
	{
        public TgcText2D DrawText = new TgcText2D();

        

        public void Render(Personaje personaje) {
			DrawText.drawText("Item seleccionado: " + personaje.getItemSeleccionado().getDescripcion() + "| " + personaje.getItemSeleccionado().getDuracionRestante() , 5, 100, Color.Coral);
			DrawText.drawText("TAB para cambiar entre los items", 5, 120, Color.Coral);
			DrawText.drawText("F para activar/desactivar item", 5, 140, Color.Coral);
			DrawText.drawText("Inventario: " + personaje.getDescripcionesItems(),5,180,Color.Aquamarine);
			DrawText.drawText("Piezas: " + personaje.getDescripcionPiezas(), 5, 200, Color.Aquamarine);

			if(!personaje.ilumnacionActiva) DrawText.drawText("Tiempo restante: " + (personaje.tiempoLimiteDesprotegido - personaje.tiempoDesprotegido).ToString(), 5, 220, Color.Cyan);
			if (personaje.perdioJuego()) DrawText.drawText("Fin del Juego", 500, 500, Color.Crimson);
        }
	}
}
