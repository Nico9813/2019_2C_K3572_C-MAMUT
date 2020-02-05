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
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	public class Personaje
	{
		String mediaDir;
		public TgcMesh mesh;
		private List<Item> items;
		private List<Pieza> piezas;
		private List<Pista> pistas;
		private Iluminador iluminadorPrincipal;
		public Item itemSelecionado;
		private Linterna linterna;
		public float tiempoDesprotegido;
		public float tiempoLimiteDesprotegido = 30;
        public float tiempoVisionNocturna;
        public float tiempoLimiteVisionNocturna = 40;

        public Boolean ilumnacionActiva;
		public Boolean permisosAdmin = false;
		private Boolean objetoEquipado;
		private Boolean perdio;
		private Boolean itemSelecionadoActivo;
		private Boolean agendaActiva;
		public Boolean posicionModificada = false;
		public Boolean canoaEquipada = false;
		public Boolean visionNocturna = false;
		public Boolean visionNocturnaActivada = false;

		public int pistaActual;

		Canoa canoa = null;

		public TgcMesh meshEnMano = null;

		public void Init(TgcMesh meshPersonaje, String MediaDir, Linterna linternaInicial) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();
			mediaDir = MediaDir;
            tiempoVisionNocturna = 0;
            //var VelasMesh = loader.loadSceneFromFile(MediaDir + "velas-TgcScene.xml").Meshes[0];
            //VelasMesh.Scale = new TGCVector3(0.03f, 0.03f, 0.03f);
            //linterna.vaciarBateria();

            items = new List<Item>();
			piezas = new List<Pieza>();
			pistas = new List<Pista>();

			HUD.Instance.Init(mediaDir, this);
			HUD.Instance.HUDpersonaje = true;
			HUD.Instance.HUDpersonaje_piezas = true;
			HUD.Instance.Agenda = false;

			linterna = linternaInicial;

			agregarItem(linterna);
			//agregarItem(new Vela(VelasMesh, mediaDir + "\\2D\\imgVela.png"));
			//agregarItem(new Mapa(VelasMesh, mediaDir + "\\2D\\MapaHud.png"));

			//pistas.Add(new Pista(null,mediaDir + "\\2D\\pista_pala.png", null));
			//pistas.Add(new Pista(null, mediaDir + "\\2D\\pista_sudo.png", null));

			objetoEquipado = false;
			ilumnacionActiva = false;
			itemSelecionadoActivo = false;
			agendaActiva = false;
			perdio = false;

			HUD.Instance.seleccionarItem(linterna);
			itemSelecionado = items.ElementAt(0);
			meshEnMano = itemSelecionado.mesh;
		}

		public void Update(TgcD3dInput Input, float elapsedTime)
		{
             
			if (ilumnacionActiva == false)
				tiempoDesprotegido += elapsedTime;
			else tiempoDesprotegido = 0;
            if (ilumnacionActiva)
            {
                iluminadorPrincipal.Update(this, elapsedTime);
            }


            if (tiempoDesprotegido >= tiempoLimiteDesprotegido)
				this.perdio = true;

			if (Input.keyPressed(Key.Tab))
			{
				EquiparProximoItem();
			}

			if (Input.keyPressed(Key.N))
			{
				if (visionNocturna) {
					visionNocturnaActivada = !visionNocturnaActivada;
				}
			}
            if (visionNocturnaActivada)
            {
                tiempoDesprotegido = 0;
                tiempoVisionNocturna += elapsedTime;
            }
            if (tiempoVisionNocturna >= tiempoLimiteVisionNocturna) visionNocturnaActivada = false;
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

			if (Input.keyPressed(Key.G) && pistas.Count != 0) {
				AbrirAgenda(0);
			}



			if (agendaActiva && Input.keyPressed(Key.Space)) {
				pistaActual = (pistaActual + 1) % pistas.Count;
				HUD.Instance.seleccionarPaginaActual(pistas[pistaActual]);
			}

			if (agendaActiva && Input.keyPressed(Key.Space)){
                pistaActual = (pistaActual + 1) % pistas.Count;
				HUD.Instance.paginaActual = pistas[pistaActual];
                GameModel.sonidoNota.play(false);
            }

			HUD.Instance.Update(elapsedTime);
		}

		public void Render(float ElapsedTime, TgcD3dInput input, TGCVector3 director)
		{
			
			this.mesh.Render();
			if (meshEnMano != null) {
				meshEnMano.Position = mesh.Position + new TGCVector3(50 * -director.X, 30, 50 * -director.Z);
				meshEnMano.Render();
				if (canoaEquipada) {
					canoa.mesh.Position = mesh.Position + new TGCVector3(50 * -director.X, 30, 50 * -director.Z);
					canoa.mesh.Render();
;				}
			}
		}

		public void setVisionNoturna(bool estado) {
			visionNocturna = estado;
		}
		public bool TieneCanoa() {
			return canoa != null;
		}

		public void SetCanoa(Canoa canoa) {
			this.canoa = canoa;
		}

		public void EquiparCanoa() {
			canoaEquipada = true;
			canoa.ReiniciarPosicion(this);
		}

		public void DesequiparCanoa()
		{
			canoaEquipada = false;
		}

		public void RotarManos(float angulo) {
			
             foreach (var item in items)
             {
                if(item.mesh != null)
                    item.mesh.RotateY(angulo);
             }
                    
			
			if (canoaEquipada)
			{
				canoa.mesh.RotateY(angulo);
			}
			}
		

		public void AbrirAgenda(int indice) {
			agendaActiva = !agendaActiva;
			HUD.Instance.Agenda = !HUD.Instance.Agenda;
			HUD.Instance.seleccionarPaginaActual(pistas[indice]);
			pistaActual = indice;
			GameModel.sonidoNota.play(false);
		}

		public void RemoverItemPorNombre(String nombre) {
			Item item = items.Find(a => a.getDescripcion().Equals(nombre));
			items.Remove(item);
			HUD.Instance.removerItem(item);
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
			HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("Se agrego al inventario el item " + item.getDescripcion()));
			HUD.Instance.guardarItem(item);
		}

		public void agregarPista(Pista nuevaPista)
		{
			for (var i = 0; i < pistas.Count; i++) {
				Console.WriteLine("Pista " + i + " Descripcion: " + pistas[i].getDescripcion());
			}
			HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("Has encontrado una nueva pista, pulsa G para visualizarla"));
			this.pistas.Add(nuevaPista);
			AbrirAgenda(pistas.IndexOf(nuevaPista));
		}

		public void agregarPieza(Pieza pieza){
			if (!piezas.Contains(pieza)){
				this.piezas.Add(pieza);
				HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("Has encontrado una nueva pieza de la imagen"));
				HUD.Instance.guardarPieza(pieza);
				if (JuegoTerminado()) {
					agregarPista(new Pista(null, mediaDir + "\\2D\\pista_final.png" , null));
				}
			}
		}

		public bool JuegoTerminado() {
			return piezas.Count.Equals(9);
		}

		public bool tieneItem(String descripcionItem) {
			return items.Exists(item => item.getDescripcion().Equals(descripcionItem));
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

		public void EquiparProximoItem() {
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
			meshEnMano = itemSelecionado.mesh;
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
			return tiempoDesprotegido >= (0.1f * tiempoLimiteDesprotegido);
        }
        public void setPerdioJuego(Boolean perdioJuego)
        {
            perdio = perdioJuego;
        }
	}
}
