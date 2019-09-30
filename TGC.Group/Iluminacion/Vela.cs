﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{

	class Vela : Iluminador
	{
		public float duracionVela;
		public float tiempoUsada;
		public Vela()
		{
			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.Orange;
			duracionVela = 5;
			tiempoUsada = 0;
		}

		public override void accion(Personaje personaje)
		{
			personaje.setIluminador(this);
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
				personaje.removerItem(this);
			}
		}
	}
}
