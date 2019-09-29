using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Iluminacion;

namespace TGC.Group.Model
{
	class Personaje
	{
		public TgcMesh mesh;
		private List<Item> items;
		private List<Pieza> piezas;
		private Iluminador iluminadorPrincipal;
		private Item itemSelecionado;

		private Boolean ilumnacionActiva;

		public void Init(TgcMesh meshPersonaje) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();

			items = new List<Item>();
			piezas = new List<Pieza>();
			items.Add(new SinLuz());
			itemSelecionado = items.ElementAt(0);

			items.Add(new Vela());
			ilumnacionActiva = false;
		}

		public void Update(TgcD3dInput Input,float elapsedTime)
		{

			if (Input.keyPressed(Key.Tab))
			{
				var index = items.IndexOf(itemSelecionado);
				itemSelecionado = items.ElementAtOrDefault((index + 1) % items.Count);
				itemSelecionado.accion(this, elapsedTime);
			}

		}

		public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 lookAt, TGCVector3 direccionLuz)
		{
			iluminadorPrincipal.Render(meshTotales, terreno, lookAt, direccionLuz);
		}

		public void setIluminador(Iluminador iluminador)
		{
			iluminadorPrincipal = iluminador;
		}

		internal void agregarItem(Item objetoColisiano)
		{
			this.items.Add(objetoColisiano);
		}
	}
}
