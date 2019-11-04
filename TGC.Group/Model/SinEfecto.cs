using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
	class SinEfecto : Colisionable
	{

		public SinEfecto(TgcMesh meshPlano)
		{
			this.mesh = meshPlano;
		}

		public override void serColisionado(Personaje personaje) {
			
		}
	}
}
