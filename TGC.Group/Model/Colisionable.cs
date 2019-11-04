using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
	abstract class Colisionable
	{
		public TgcMesh mesh;
		abstract public void serColisionado(Personaje personaje);
	}
}
