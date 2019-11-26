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
using TGC.Group.Model;

namespace TGC.Group.Objetos
{
	public class Linterna : Iluminador
	{
		private float DuracionBateria;
		private float BateriaConsumida;

		public Linterna(TgcMesh mesh, string v)
		{
			//Mesh para la luz
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.White;
			DuracionBateria = 90;
			BateriaConsumida = 0;
			this.mesh = mesh;
            this.descripcion = "Linterna";
			this.rutaImagen = v;
		}

		public override void accion(Personaje personaje) {
            if (BateriaConsumida < DuracionBateria)
            { 
                personaje.setIluminador(this, true);
            }
            else
            Console.WriteLine("La linterna se quedo sin bateria");
		}
		internal override void desactivar(Personaje personaje)
		{
			personaje.quitarIluminacion();
		}

		public override void Update(Personaje personaje, float elapsedTime)
		{
			if (BateriaConsumida + elapsedTime <= DuracionBateria)
			{
				BateriaConsumida += elapsedTime;
			}
			else
			{
				personaje.quitarIluminacion();
			}
		}

		public void recargarBateria() {
			BateriaConsumida = 0;
		}

		public void vaciarBateria() {
			BateriaConsumida = DuracionBateria;
		}

		public override float getDuracionTotal()
		{
			return DuracionBateria;
		}

		public override float getDuracionRestante() {
			return DuracionBateria - BateriaConsumida;
		}
	}
}
