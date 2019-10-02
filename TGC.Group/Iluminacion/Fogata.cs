﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model;

namespace TGC.Group.Iluminacion
{
	class Fogata : Iluminador
	{

		TGCVector3 posicion;
		public Fogata(TgcMesh mesh,TGCVector3 posicion)
		{
			this.mesh = mesh;
			lightMesh = TGCBox.fromSize(new TGCVector3(0.1f, 0.1f, 0.1f), Color.Red);
			colorLuz = Color.White;
			this.posicion = posicion;
		}

		public override void accion(Personaje personaje)
		{
			throw new NotImplementedException();
		}

		public override void update(Personaje personaje, float elapsedTime)
		{
			throw new NotImplementedException();
		}
		public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno) {
			base.Render(meshTotales, terreno, posicion, new TGCVector3(0, 1, 0));
		}

		internal TGCVector3 getPosicion()
		{
			return posicion;
		}

		internal override void desactivar(Personaje personaje)
		{
			throw new NotImplementedException();
		}
	}
}
