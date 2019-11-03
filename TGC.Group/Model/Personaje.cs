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
		String mediaDir;
		public TgcMesh mesh;
		private List<Item> items;
		private List<Pieza> piezas;
		private Iluminador iluminadorPrincipal;
		public Item itemSelecionado;
		private Linterna linterna;
		public float tiempoDesprotegido;
		public float tiempoLimiteDesprotegido = 20;
		//private HUD HUD;
		public Boolean ilumnacionActiva;
		private Boolean objetoEquipado;
        private Boolean perdio = false;
		private Boolean itemSelecionadoActivo;
		private TgcMesh meshEnMano = null;

		public void Init(TgcMesh meshPersonaje, String MediaDir) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();
			mediaDir = MediaDir;

			var loader = new TgcSceneLoader();
			var scene5 = loader.loadSceneFromFile(MediaDir + "velas-TgcScene.xml");
			var VelasMesh = scene5.Meshes[0];
			VelasMesh.Scale = new TGCVector3(0.03f, 0.03f, 0.03f);

			//linterna.vaciarBateria();
			items = new List<Item>();
			piezas = new List<Pieza>();
			linterna = new Linterna(VelasMesh, mediaDir + "\\2D\\imgLinterna.png");

			HUD.Instance.Init(mediaDir, this);
			HUD.Instance.HUDpersonaje = true;
			HUD.Instance.HUDpersonaje_piezas = true;

			

			agregarItem(linterna);
			agregarItem(new Vela(VelasMesh, mediaDir + "\\2D\\imgVela.png"));
			agregarItem(new Mapa(VelasMesh, mediaDir + "\\2D\\MapaHud.png"));
			HUD.Instance.seleccionarItem(linterna);

			objetoEquipado = false;
			ilumnacionActiva = false;
			itemSelecionadoActivo = false;

			itemSelecionado = items.ElementAt(0);
		}

		public void Update(TgcD3dInput Input,float elapsedTime)
		{
            if (ilumnacionActiva == false)
                tiempoDesprotegido += elapsedTime;
            else tiempoDesprotegido = 0;

            if (tiempoDesprotegido >= tiempoLimiteDesprotegido)
                this.perdio = true;

			if (Input.keyPressed(Key.Tab))
			{
				objetoEquipado = false;
				var index = items.IndexOf(itemSelecionado);
				var indiceObjeto = (index + 1) % items.Count;
				Item itemViejo = itemSelecionado;
				itemSelecionado = items.ElementAtOrDefault(indiceObjeto);
				meshEnMano = itemSelecionado.mesh;
				HUD.Instance.seleccionarItem(itemSelecionado);
				if (itemSelecionadoActivo)
				{
					itemViejo.desactivar(this);
					objetoEquipado = true;
					itemSelecionadoActivo = false;
				}
			}

			if (Input.keyPressed(Key.R))
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

			HUD.Instance.Update();
		}

		public void Render(float ElapsedTime, TgcD3dInput input)
		{
			HUD.Instance.Render();
			if (meshEnMano != null) {
				
				meshEnMano.Move(new TGCVector3(-3500, 50, 555));
				meshEnMano.Transform = TGCMatrix.Translation(new TGCVector3(-3500, 50, 555));
				meshEnMano.Render();
			}
		}

		public void equiparMeshEnMano(TgcMesh mesh) {
			meshEnMano = mesh;
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

		internal void agregarRecolectable(Recolectable objetoColisiano)
		{
			objetoColisiano.Agregarse(this);
		}

		public void agregarItem(Item item) {
			this.items.Add(item);
			HUD.Instance.guardarItem(item);
		}

		public void agregarPieza(Pieza pieza){
			this.piezas.Add(pieza);
			HUD.Instance.guardarPieza(pieza);
		}

		internal void removerItem(Item item)
		{
			this.items.Remove(item);
			HUD.Instance.removerItem(item);
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
        public Boolean estaEnPeligro()
        {
            return tiempoDesprotegido >= (0.8f * tiempoLimiteDesprotegido);
        }
	}
}
