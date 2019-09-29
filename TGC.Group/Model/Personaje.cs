using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Iluminacion;

namespace TGC.Group.Model
{
	class Personaje
	{
		public TgcMesh mesh;
		private List<Item> items;
		private List<Pieza> piezas;
		private Iluminador iluminadorPrincipal;

		private Boolean ilumnacionActiva;

		public void Init(TgcMesh meshPersonaje) {
			mesh = meshPersonaje;
			iluminadorPrincipal = new SinLuz();
			ilumnacionActiva = false;
		}

		public void Update(TgcD3dInput Input)
		{
			if (Input.keyPressed(Key.F))
			{
				if (ilumnacionActiva)
				{
					iluminadorPrincipal = new SinLuz();
					ilumnacionActiva = false;
				}
				else
				{
					iluminadorPrincipal = new Linterna();
					ilumnacionActiva = true;
				}
			}
			if (Input.keyPressed(Key.G))
			{
				if (ilumnacionActiva)
				{
					iluminadorPrincipal = new SinLuz();
					ilumnacionActiva = false;
				}
				else
				{
					iluminadorPrincipal = new Vela();
					ilumnacionActiva = true;
				}
			}
		}

		public void Render(List<TgcMesh> meshTotales, TgcSimpleTerrain terreno, TGCVector3 lookAt, TGCVector3 direccionLuz)
		{
			iluminadorPrincipal.Render(meshTotales, terreno, lookAt, direccionLuz);
		}

		public void setIluminador(Iluminador iluminador)
		{
			iluminadorPrincipal = iluminador;
		}

		internal void agregarItem(object objetoColisiano)
		{
			this.items.Add(objetoColisiano);
		}
	}
}
