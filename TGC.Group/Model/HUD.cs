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
            DrawText.drawText("Item seleccionado: " + personaje.getItemSeleccionado().getDescripcion() + "| " + personaje.getItemSeleccionado().getDuracionRestante() , 5, 80, Color.Coral);
            DrawText.drawText("Inventario: " + personaje.getDescripcionesItems(),5,100,Color.Aquamarine);
            if(personaje.perdioJuego()) DrawText.drawText("Fin del Juego", 500, 500, Color.Crimson);

        }
	}
}
