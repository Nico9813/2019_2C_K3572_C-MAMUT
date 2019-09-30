using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Interpolation;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Textures;
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
		private Linterna linterna;
		private float tiempoDesprotegido;
		private HUD HUD;
		private Boolean ilumnacionActiva;

		public void Init(TgcMesh meshPersonaje) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();

			HUD = new HUD();
			linterna = new Linterna(null);
			linterna.vaciarBateria();
			items = new List<Item>();
			piezas = new List<Pieza>();

			items.Add(new SinLuz());
			items.Add(linterna);
			itemSelecionado = items.ElementAt(0);

			items.Add(new Vela());
			ilumnacionActiva = false;
		}

		public void Update(TgcD3dInput Input,float elapsedTime)
		{
			if (ilumnacionActiva == false)
				tiempoDesprotegido += elapsedTime;

			if(tiempoDesprotegido >= 5)
				Console.WriteLine("Fin del juego");

			if (Input.keyPressed(Key.Tab))
			{
				var index = items.IndexOf(itemSelecionado);
				itemSelecionado = items.ElementAtOrDefault((index + 1) % items.Count);
				itemSelecionado.accion(this);
			}
			itemSelecionado.update(this, elapsedTime);

		}

		public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 lookAt, TGCVector3 direccionLuz)
		{
			HUD.Render(this);
			iluminadorPrincipal.Render(meshTotales, terreno, lookAt, direccionLuz);
		}

		internal void quitarIluminacion()
		{
			setIluminador(new SinLuz());
			ilumnacionActiva = false;
		}

		public void setIluminador(Iluminador iluminador)
		{
			iluminadorPrincipal = iluminador;
			ilumnacionActiva = true;
		}

		internal void agregarItem(Item objetoColisiano)
		{
			this.items.Add(objetoColisiano);
		}

		internal void removerItem(Item item)
		{
			this.items.Remove(item);
		}

		internal void agregarBateria()
		{
			linterna.recargarBateria();
		}
	}
}
