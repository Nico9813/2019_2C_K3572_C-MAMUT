using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Objetos;

namespace TGC.Group.Model
{
	class Bug : Colisionable
	{
		TGCVector3 posicionInicial;
		TGCVector3 posicionInicialMounstro;
		Pieza piezaAsociada;
		Pista pistaAsociada;
		bool usado;

		public TgcMesh meshMounstroMiniatura;
		public Bug(String MediaDir) {
			meshMounstroMiniatura = new TgcSceneLoader().loadSceneFromFile(MediaDir + @"monstruo-TgcScene.xml").Meshes[0];
			mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + @"Buggy-TgcScene.xml").Meshes[0];

			piezaAsociada = new Pieza(2, "Pieza 2", MediaDir + "\\2D\\windows\\windows_2.png", null);
			pistaAsociada = new Pista(null, MediaDir + "\\2D\\pista_sudo.png", null);

			mesh.Position = new TGCVector3(-450, 40, 2500);
			mesh.Transform = TGCMatrix.Translation(-450, 40, 2500);
			meshMounstroMiniatura.Position = new TGCVector3(-440, 40, 2400);
			meshMounstroMiniatura.Transform = TGCMatrix.Translation(-440, 40, 2400);
			meshMounstroMiniatura.RotateY(FastMath.PI);
			meshMounstroMiniatura.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
			posicionInicial = mesh.Position;
			posicionInicialMounstro = meshMounstroMiniatura.Position;

			interactuable = true;
			usado = false;
		}
		public override string getMensajeColision()
		{
			return "Este auto no esta funcionando bien";
		}

		public override void serColisionado(Personaje personaje)
		{
			if (!usado)
			{
				if (personaje.tieneItem("Red"))
				{
					personaje.agregarPieza(piezaAsociada);
					personaje.agregarPista(pistaAsociada);
					personaje.permisosAdmin = true;
					personaje.RemoverItemPorNombre("Red");
					usado = true;
				}
				else
				{
					HUD.Instance.mensajesTemporales.Add(new MensajeTemporal("No parece irse"));
				}
			}
		}

		public void Update(float elapsedTime) {
			meshMounstroMiniatura.Position = new TGCVector3(posicionInicialMounstro.X + 100f * FastMath.Cos(elapsedTime), posicionInicialMounstro.Y, posicionInicialMounstro.Z + 200f * FastMath.Sin(elapsedTime));
			mesh.Position = new TGCVector3(posicionInicial.X + 100f * FastMath.Cos(elapsedTime), posicionInicial.Y, posicionInicial.Z + 200f * FastMath.Sin(elapsedTime));
			mesh.RotateX(FastMath.PI * elapsedTime);
		}
	}
}
