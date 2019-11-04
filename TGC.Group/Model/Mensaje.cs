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
		int duraccionMaxima = 5;

		public MensajeTemporal(string _contenido) {
			contenido = _contenido;
		}
	}
}
