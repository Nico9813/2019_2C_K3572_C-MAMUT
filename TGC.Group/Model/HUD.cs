using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Sprites;

namespace TGC.Group.Model
{
	class HUD
	{
		private CustomSprite BarraBateria;
		private CustomSprite RellenoBateria;
		private Drawer2D drawer;

		private Personaje personaje;

		private readonly static HUD instance = new HUD();

		private HUD(){}

		public static HUD Instance
		{
			get
			{
				return instance;
			}
		}


		public void Init(String MediaDir, Personaje _personaje)
		{
			personaje = _personaje;
			var width = D3DDevice.Instance.Width;
			var height = D3DDevice.Instance.Height;
			drawer = new Drawer2D();

			BarraBateria = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\BarraBateria.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.02f, height * 0.85f),
				Scaling = new TGCVector2(0.5f, 0.5f),
				Color = Color.Red,

			};

			RellenoBateria = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\Bateria.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.02f, height * 0.85f),
				Scaling = new TGCVector2(0.5f,0.5f),
			};
		}
		public void Update()
		{
			float bateriaRestante = personaje.getIluminadorPrincipal().getDuracionRestante();
			RellenoBateria.Scaling = new TGCVector2(bateriaRestante * 0.5f / personaje.getIluminadorPrincipal().getDuracionTotal(), 1* 0.5f);
		}

		public void Render()
		{
			drawer.BeginDrawSprite();
			if (personaje.ilumnacionActiva) {
				drawer.DrawSprite(RellenoBateria);
				drawer.DrawSprite(BarraBateria);
			}
			drawer.EndDrawSprite();
		}

	}
}