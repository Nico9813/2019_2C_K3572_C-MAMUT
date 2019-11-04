using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
	class MensajeTemporal
	{
		string contenido;
		float duracionMaxima = 5;
		float duracionConsumida;

		public MensajeTemporal(string _contenido) {
			contenido = _contenido;
			duracionConsumida = 0;
		}

		public String getContenido() { return contenido;  }

		public void Update(float elapsedTime) {
			duracionConsumida += elapsedTime;
		}

		public bool tiempoCumplido() { return duracionConsumida >= duracionMaxima; }
	}
}
