﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	public class Canoa : Item
	{
		TGCVector3 rotacion_inicial;
		Pieza piezaAsociada;
		Pista pistaAsociada;
		Item mapa;

		public Canoa(TgcMesh mesh, string rutaImagen,string MediaDir) {
			this.mesh = mesh;
			this.rutaImagen = rutaImagen;
			this.descripcion = "Canoa";
			piezaAsociada = new Pieza(3, "Pieza 3", MediaDir + "\\2D\\windows\\windows_3.png", null);
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_canoa.png", null);
			mapa = new Mapa(null, MediaDir + "\\2D\\MapaHud.png");
		}

		public override void Agregarse(Personaje personaje)
		{
			this.mesh.RotateY(FastMath.QUARTER_PI);
			rotacion_inicial = mesh.Rotation;
			personaje.agregarItem(this);
		}

		public void ReiniciarPosicion(Personaje personaje) {
			mesh.Rotation = personaje.mesh.Rotation;
		}

		public override void accion(Personaje personaje)
		{
			if (personaje.permisosAdmin){
				personaje.removerItem(this);
				personaje.SetCanoa(this);
				personaje.agregarItem(mapa);
				personaje.agregarPista(pistaAsociada);
				personaje.agregarPieza(piezaAsociada);
			} else {
				HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("No tienes permisos para usar una canoa"));
			}
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
			
		}

		internal override void desactivar(Personaje personaje)
		{
			
		}
	}
}
