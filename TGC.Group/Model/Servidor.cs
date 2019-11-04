using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
	class Servidor : Colisionable
	{
		public Servidor(TgcMesh meshActual) {
			mesh = meshActual;
		}
		public override void serColisionado(Personaje personaje)
		{
			if (personaje.tieneItem("Pala")) {

			}
		}
	}
}
