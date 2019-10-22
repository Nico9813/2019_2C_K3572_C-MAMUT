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
		public Item itemSelecionado;
		private Linterna linterna;
		public float tiempoDesprotegido;
		public float tiempoLimiteDesprotegido = 150;
		private HUD HUD;
		public Boolean ilumnacionActiva;
		private Boolean objetoEquipado;
        private Boolean perdio = false;
		private Boolean itemSelecionadoActivo;
		private Boolean inicio = false;

		public void Init(TgcMesh meshPersonaje) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();

			linterna = new Linterna(null);
			//linterna.vaciarBateria();
			items = new List<Item>();
			piezas = new List<Pieza>();


			items.Add(linterna);
			items.Add(new Vela(null));
			itemSelecionado = items.ElementAt(0);

			objetoEquipado = false;
			ilumnacionActiva = false;
			itemSelecionadoActivo = false;
		}

		public void InitHUD(String media) {
			HUD = new HUD();
			HUD.Init(this,media);
		}

		public void Update(TgcD3dInput Input,float elapsedTime)
		{
			if (ilumnacionActiva == false)
				tiempoDesprotegido += elapsedTime;

            if (tiempoDesprotegido >= tiempoLimiteDesprotegido)
                this.perdio = true;

			if (Input.keyPressed(Key.Tab))
			{
				objetoEquipado = false;
				var index = items.IndexOf(itemSelecionado);
				itemSelecionado = items.ElementAtOrDefault((index + 1) % items.Count);
				if (itemSelecionadoActivo)
				{
					itemSelecionado.desactivar(this);
					objetoEquipado = true;
					itemSelecionadoActivo = false;
				}
			}

			if (Input.keyPressed(Key.F))
			{
				if (itemSelecionadoActivo)
				{
					itemSelecionado.desactivar(this);
					objetoEquipado = true;
					itemSelecionadoActivo = false;
				}
				else
				{
					itemSelecionado.accion(this);
					objetoEquipado = true;
					itemSelecionadoActivo = true;
				}
			}

			if (objetoEquipado && itemSelecionadoActivo)
				itemSelecionado.update(this, elapsedTime);

		}

		public void Render(float ElapsedTime, TgcD3dInput input)
		{
			HUD.Render(ElapsedTime, input);
		}

		internal void quitarIluminacion()
		{
			var sinLuz = new SinLuz();
			setIluminador(sinLuz,false);
			ilumnacionActiva = false;
			itemSelecionadoActivo = false;
			
		}

		public void setIluminador(Iluminador iluminador, Boolean iluminacionAct)
		{
			iluminadorPrincipal = iluminador;
			ilumnacionActiva = iluminacionAct;
		}
        public Iluminador getIluminadorPrincipal()
        {
            return this.iluminadorPrincipal;
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
        public List<Item> getItems()
        {
            return this.items;
        }
        public Item getItemSeleccionado()
        {
            return this.itemSelecionado;
        }
        public String getDescripcionesItems()
        {
            String descripciones = "";
            foreach (var item in items)
            {
                descripciones = descripciones + item.getDescripcion() + "|";
            }
            return descripciones.TrimStart('|').TrimEnd('|');
        }

		public String getDescripcionPiezas()
		{
			String descripciones = "";
			foreach (var item in piezas)
			{
				descripciones = descripciones + item.getDescripcion() + "|";
			}
			return descripciones.TrimStart('|').TrimEnd('|');
		}
		public Boolean perdioJuego()
		{
			return this.perdio;
		}
	}
}
