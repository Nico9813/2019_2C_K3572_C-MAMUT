using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;

namespace TGC.Group.Iluminacion
{

	class Vela : Iluminador
	{
		public Vela()
		{
			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.Orange;
		}

		public override void accion(Model.Personaje personaje, float elapsedTime)
		{
			personaje.setIluminador(this);
		}
	}
}
