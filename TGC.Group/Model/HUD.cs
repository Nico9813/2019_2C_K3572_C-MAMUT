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
		private List<EspacioObjeto> espaciosInventario;
		private List<EspacioObjeto> espaciosPiezas;
		private int indiceSelccionado;
		int MAXIMO_ITEMS = 8;
		int MAXIMO_PIEZAS = 9;
		private Drawer2D drawer;
		private Personaje personaje;
		private CustomSprite MenuControlesSprite;
		private CustomSprite MapaPersonajeSprite;

		public bool MenuControles = false;
		public bool MenuPausa = false;
		public bool HUDpersonaje = false;
		public bool HUDpersonaje_piezas = false;
		public bool MapaPersonaje = false;

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
				Color = Color.DarkMagenta,

			};

			RellenoBateria = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\Bateria.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.045f, height * 0.85f),
				Scaling = new TGCVector2(0.5f, 0.5f),
				Color = Color.DarkGoldenrod,
			};

			MenuControlesSprite = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\menuControles.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.32f, height * 0.20f),
				Scaling = new TGCVector2(0.6f, 0.6f),
			};

			MapaPersonajeSprite = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\mapa.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.32f, height * 0.20f),
				Scaling = new TGCVector2(0.6f, 0.6f),
			};



			espaciosInventario = new List<EspacioObjeto>();
			espaciosPiezas = new List<EspacioObjeto>();
			CustomSprite spriteActual;

			float y0 = height * 0.10f;
			float x0 = 0;
			float dy = 60;
			float dx = 0;

			for (int i = 0; i < MAXIMO_ITEMS; i++) {
				y0 = y0 + dy;
				spriteActual = new CustomSprite
				{
					Bitmap = new CustomBitmap(MediaDir + "\\2D\\EspacioObjeto.png", D3DDevice.Instance.Device),
					Position = new TGCVector2(0, y0),
				};
				espaciosInventario.Add(new EspacioObjeto(spriteActual));
			}

			y0 = height * 0.05f;
			x0 =  width * 0.80f;
			dy = 80;
			dx = 80;

			for (int j = 0; j < 3; j++) {
				for (int i = 0; i < MAXIMO_PIEZAS / 3; i++)
				{
				x0 = x0 + dx;
					spriteActual = new CustomSprite
					{
						Bitmap = new CustomBitmap(MediaDir + "\\2D\\EspacioObjeto.png", D3DDevice.Instance.Device),
						Position = new TGCVector2(x0, y0),
					};
					espaciosPiezas.Add(new EspacioObjeto(spriteActual));
				}
				y0 = y0 + dy;
				x0 = width * 0.80f;
			}
		}

		public void guardarItem(Item item) {
			EspacioObjeto espacioInventarioActual;
			try
			{
				espacioInventarioActual = espaciosInventario.Find(espacio => espacio.libre);
				espacioInventarioActual.asociarObjeto(item);
			}
			catch (ArgumentNullException e) {
				//INVENTARIO LLENO
			}
		}

		public void guardarPieza(Pieza pieza)
		{
			EspacioObjeto espacioInventarioActual;
			espacioInventarioActual = espaciosPiezas.ElementAt(pieza.numeroPieza);
			espacioInventarioActual.asociarObjeto(pieza);
		}

		public void removerItem(Item item) {
			EspacioObjeto espacioInventarioActual;
			espacioInventarioActual = espaciosInventario.ElementAt(indiceSelccionado);
			espacioInventarioActual.desasociarObjeto();
		}

		public void seleccionarItem(int indiceItem)
		{
			espaciosInventario.ForEach(espacio => espacio.deseleccionar());
			indiceSelccionado = indiceItem;
			espaciosInventario.FindAll(espacioInventario => !espacioInventario.libre).ElementAt(indiceItem).seleccionar();
		}

		public void Update()
		{
			float bateriaRestante = personaje.getIluminadorPrincipal().getDuracionRestante();
			RellenoBateria.Scaling = new TGCVector2(bateriaRestante * 0.5f / personaje.getIluminadorPrincipal().getDuracionTotal(), 1* 0.5f);
		}

		public void Render()
		{
			drawer.BeginDrawSprite();

			if (HUDpersonaje) {
				if (personaje.ilumnacionActiva)
				{
					drawer.DrawSprite(RellenoBateria);
					drawer.DrawSprite(BarraBateria);
				}
				foreach (EspacioObjeto espacio in espaciosInventario)
				{
					drawer.DrawSprite(espacio.spriteEspacioInventario);
					if (!espacio.libre)
					{
						drawer.DrawSprite(espacio.spriteItem);
					}
				}
			}

			if (HUDpersonaje_piezas)
			{
				if (personaje.ilumnacionActiva)
				{
					drawer.DrawSprite(RellenoBateria);
					drawer.DrawSprite(BarraBateria);
				}
				foreach (EspacioObjeto espacio in espaciosPiezas)
				{
					drawer.DrawSprite(espacio.spriteEspacioInventario);
					if (!espacio.libre)
					{
						drawer.DrawSprite(espacio.spriteItem);
					}
				}
			}

			if (MapaPersonaje) {
				drawer.DrawSprite(MapaPersonajeSprite);
			}

			if (MenuControles) {
				drawer.DrawSprite(MenuControlesSprite);
			}

			if (MenuPausa) {

			}

			drawer.EndDrawSprite();
		}

	}

	class EspacioObjeto
	{
		public Boolean libre;
		public Recolectable itemGuardado;
		public CustomSprite spriteEspacioInventario;
		public CustomSprite spriteItem;

		public EspacioObjeto(CustomSprite espacio)
		{
			libre = true;
			spriteEspacioInventario = espacio;
		}
		public void asociarObjeto(Recolectable item)
		{
			itemGuardado = item;
			spriteItem = new CustomSprite
			{
				Bitmap = new CustomBitmap(item.getRutaImagen(), D3DDevice.Instance.Device),
				Position = spriteEspacioInventario.Position,
			};
			libre = false;
		}
		public void desasociarObjeto()
		{
			libre = true;
			spriteEspacioInventario.Color = Color.White;
		}

		public void seleccionar()
		{
			spriteEspacioInventario.Color = Color.Red;
		}

		public void deseleccionar()
		{
			spriteEspacioInventario.Color = Color.White;
		}
	}
}