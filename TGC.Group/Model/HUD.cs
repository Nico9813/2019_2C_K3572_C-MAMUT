using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Text;
using TgcViewer.Utils.Gui;

namespace TGC.Group.Model
{
	class HUD
	{
        public TgcText2D DrawText = new TgcText2D();
		public Personaje personaje;
		private DXGui gui;

		public const int ID_ITEM = 1;


		public void Init(Personaje _personaje,String MediaDir) {
			personaje = _personaje;

			gui = new DXGui();
			// levanto el GUI
			gui.Create(MediaDir);

			// menu principal
			gui.InitDialog(true);
			int W = D3DDevice.Instance.Width;
			int H = D3DDevice.Instance.Height;
			int x0 = 70;
			int y0 = 10;
			int dy = 120;
			int dy2 = dy;
			int dx = 400;

			GUIItem item = gui.InsertImage("transformers\\10.png", x0, y0, MediaDir);
			item.image_centrada = false;
			y0 += dy;
			gui.InsertItem(new static_text(gui, "SCOUT", x0, y0, 400, 25));
		}

        public void Render2(Personaje personaje) {
			DrawText.drawText("Item seleccionado: " + personaje.getItemSeleccionado().getDescripcion() + "| " + personaje.getItemSeleccionado().getDuracionRestante() , 5, 100, Color.Coral);
			DrawText.drawText("TAB para cambiar entre los items", 5, 120, Color.Coral);
			DrawText.drawText("F para activar/desactivar item", 5, 140, Color.Coral);
			DrawText.drawText("Inventario: " + personaje.getDescripcionesItems(),5,180,Color.Aquamarine);
			DrawText.drawText("Piezas: " + personaje.getDescripcionPiezas(), 5, 200, Color.Aquamarine);

			if(!personaje.ilumnacionActiva) DrawText.drawText("Tiempo restante: " + (personaje.tiempoLimiteDesprotegido - personaje.tiempoDesprotegido).ToString(), 5, 220, Color.Cyan);
			if (personaje.perdioJuego()) DrawText.drawText("Fin del Juego", 500, 500, Color.Crimson);
        }

		public void Render(float ElapsedTime, TgcD3dInput Input)
		{
			gui_render(ElapsedTime, Input);
		}

		public void gui_render(float elapsedTime, TgcD3dInput Input)
		{
			GuiMessage msg = gui.Update(elapsedTime, Input);

			switch (msg.message)
			{
				case MessageType.WM_COMMAND:
					switch (msg.id)
					{
						case ID_ITEM:
							break;
						default:
							break;
					}
					break;

				default:
					break;
			}
			gui.Render();
		}
	}
}
