using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	public class VisionNocturna : Item
	{
		Pista pistaAsociada;

		public VisionNocturna(string MediaDir)
		{
			var loader = new TgcSceneLoader();
			this.mesh = loader.loadSceneFromFile(MediaDir + @"nocturne-TgcScene.xml").Meshes[0];
			this.rutaImagen = MediaDir + "\\2D\\nocturneIcono.png";
			this.descripcion = "Gafas de vision nocturna";
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_gafas.png", null);
		}

		public override void accion(Personaje personaje)
		{
			personaje.removerItem(this);
			personaje.EquiparProximoItem();
			personaje.visionNocturna = true;
			personaje.agregarPista(pistaAsociada);
		}

	
		internal override void desactivar(Personaje personaje)
		{

		}
	}
}
