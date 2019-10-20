using System;
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
	class SinLuz : Iluminador
	{
		public SinLuz()
		{
			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.FromArgb(3, 1, 4);
			descripcion = "";
		}
		public override void accion(Model.Personaje personaje)
		{
			personaje.setIluminador(this);
		}

		public override void update(Personaje personaje, float elapsedTime)
		{

		}
        public override string getDuracionRestante()
        {
            return "";
        }

		internal override void desactivar(Personaje personaje)
		{
		}
	}
}
