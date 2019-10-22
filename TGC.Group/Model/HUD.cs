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
	class EspacioObjeto
	{
		public Boolean libre;
		public Item itemGuardado;
		public CustomSprite spriteEspacioInventario;
		public CustomSprite spriteItem;

		public EspacioObjeto(CustomSprite espacio) {
			libre = true;
			spriteEspacioInventario = espacio;
		}
		public void asociarObjeto(Item item) {
			itemGuardado = item;
			spriteItem = new CustomSprite
			{
				Bitmap = new CustomBitmap(item.getRutaImagen(), D3DDevice.Instance.Device),
				Position = spriteEspacioInventario.Position,
			};
			libre = false;
		}
		public void desasociarObjeto() {
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

	class HUD
	{
		private CustomSprite BarraBateria;
		private CustomSprite RellenoBateria;
		private List<EspacioObjeto> espaciosInventario;
		private int indiceSelccionado;
		int MAXIMO_ITEMS = 8;
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
				Color = Color.DarkMagenta,

			};

			RellenoBateria = new CustomSprite
			{
				Bitmap = new CustomBitmap(MediaDir + "\\2D\\Bateria.png", D3DDevice.Instance.Device),
				Position = new TGCVector2(width * 0.045f, height * 0.85f),
				Scaling = new TGCVector2(0.5f, 0.5f),
				Color = Color.DarkGoldenrod,
			};

			espaciosInventario = new List<EspacioObjeto>();
			CustomSprite spriteActual;

			float y0 = height * 0.10f;
			float dy = 60;


			for (int i = 0; i < MAXIMO_ITEMS; i++) {
				y0 = y0 + dy;
				spriteActual = new CustomSprite
				{
					Bitmap = new CustomBitmap(MediaDir + "\\2D\\EspacioObjeto.png", D3DDevice.Instance.Device),
					Position = new TGCVector2(0, y0),
				};
				espaciosInventario.Add(new EspacioObjeto(spriteActual));
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
			if (personaje.ilumnacionActiva) {
				drawer.DrawSprite(RellenoBateria);
				drawer.DrawSprite(BarraBateria);
			}
			foreach(EspacioObjeto espacio in espaciosInventario) {
				drawer.DrawSprite(espacio.spriteEspacioInventario);
				if (!espacio.libre) {
					drawer.DrawSprite(espacio.spriteItem);
				}
			}
			drawer.EndDrawSprite();
		}

	}
}