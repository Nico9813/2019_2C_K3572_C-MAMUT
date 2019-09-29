using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Examples.Camara;
using TGC.Examples.Example;
using TGC.Examples.UserControls.Modifier;

namespace TGC.Group.Iluminacion
{
	class Linterna : Iluminador
	{
		private int DuracionBateria;
		private int Bateria;

		public Linterna(TgcMesh mesh)
		{
			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.White;
			this.mesh = mesh;
		}

		public override void accion(Model.Personaje personaje, float elapsedTime) {
			personaje.setIluminador(this);
		}
	}
}
