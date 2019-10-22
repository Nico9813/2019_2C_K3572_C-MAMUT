using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{

	class Vela : Iluminador
	{
		public float duracionVela;
		public float tiempoUsada;
		public Vela(TgcMesh mesh)
		{
			//Mesh para la luz
			this.mesh = mesh;
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.Orange;
			duracionVela = 5;
			tiempoUsada = 0;
            this.descripcion = "Vela";
        }

		public override void accion(Personaje personaje)
		{
		personaje.setIluminador(this,true);
        
        }
		internal override void desactivar(Personaje personaje)
		{
			personaje.quitarIluminacion();
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
			if (tiempoUsada + elapsedTime <= duracionVela)
			{
				tiempoUsada += elapsedTime;
			}
			else
			{
				personaje.quitarIluminacion();
				personaje.itemSelecionado = new SinLuz();
				personaje.removerItem(this);
			}
		}

		public override float getDuracionTotal()
		{
			return duracionVela;
		}

		public override float getDuracionRestante()
		{
			return duracionVela - tiempoUsada;
		}
	}
}
