using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using Device = Microsoft.DirectX.Direct3D.Device;

namespace TGC.Examples.Camara
{
    /// <summary>
    ///     Camara en primera persona personalizada para niveles de Quake 3.
    ///     Evita utilizar senos y cosenos
    ///     Autor: Martin Giachetti
    /// </summary>
    public class MamutCamara : TgcCamera
    {
		private TGCVector3 position;
		/// <summary>
		///  Se mantiene la matriz rotacion para no hacer este calculo cada vez.
		/// </summary>
		private TGCMatrix cameraRotation;

		/// <summary>
		///  Direction view se calcula a partir de donde se quiere ver con la camara inicialmente. por defecto se ve en -Z.
		/// </summary>
		private TGCVector3 directionView;

		//No hace falta la base ya que siempre es la misma, la base se arma segun las rotaciones de esto costados y updown.
		private float leftrightRot;

		/// <summary>
		///
		/// </summary>
		private float updownRot;

		private TgcD3dInput Input { get; }
		public float RotationSpeed { get; set; }
		/// <summary>
		///     Crear una nueva camara
		/// </summary>
		public MamutCamara(TgcD3dInput input)
		{
			this.Input = input;
			this.directionView = new TGCVector3(0, 0, 100);
			this.RotationSpeed = 1f;
			this.leftrightRot = FastMath.PI_HALF;
			this.updownRot = -FastMath.PI / 10.0f;
			this.cameraRotation = TGCMatrix.RotationX(updownRot) * TGCMatrix.RotationY(leftrightRot);
			resetValues();
		}

		public MamutCamara(TGCVector3 target, float offsetHeight, float offsetForward, TgcD3dInput input) 
			: this(input)
		{
			Target = target;
			OffsetHeight = offsetHeight;
			OffsetForward = offsetForward;
		}

		public MamutCamara(TGCVector3 target, TGCVector3 targetDisplacement, float offsetHeight, float offsetForward, TgcD3dInput input)
			: this(input)
		{
			Target = target;
			TargetDisplacement = targetDisplacement;
			OffsetHeight = offsetHeight;
			OffsetForward = offsetForward;
		}

		/// <summary>
		///     Desplazamiento en altura de la camara respecto del target
		/// </summary>
		public float OffsetHeight { get; set; }

		/// <summary>
		///     Desplazamiento hacia adelante o atras de la camara repecto del target.
		///     Para que sea hacia atras tiene que ser negativo.
		/// </summary>
		public float OffsetForward { get; set; }

		/// <summary>
		///     Desplazamiento final que se le hace al target para acomodar la camara en un cierto
		///     rincon de la pantalla
		/// </summary>
		public TGCVector3 TargetDisplacement { get; set; }

		/// <summary>
		///     Rotacion absoluta en Y de la camara
		/// </summary>
		public float RotationY { get; set; }

		/// <summary>
		///     Objetivo al cual la camara tiene que apuntar
		/// </summary>
		public TGCVector3 Target { get; set; }

		public override void UpdateCamera(float elapsedTime)
		{
			TGCVector3 targetCenter;
			CalculatePositionTarget(out position, out targetCenter);

			/*
			if (Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
			{
				leftrightRot -= -Input.XposRelative * RotationSpeed;
				updownRot -= Input.YposRelative * RotationSpeed;
				
				targetCenter.X += leftrightRot;
				targetCenter.Y += updownRot;
				position.X -= leftrightRot;
				position.Y -= updownRot;

			}
			*/

			SetCamera(position, targetCenter + new TGCVector3(0,50,0));

		}

		public void resetValues()
		{
			OffsetHeight = 20;
			OffsetForward = -120;
			RotationY = 0;
			TargetDisplacement = TGCVector3.Empty;
			Target = TGCVector3.Empty;
			position = TGCVector3.Empty;
		}

		public void setTargetOffsets(TGCVector3 target, float offsetHeight, float offsetForward)
		{
			Target = target;
			OffsetHeight = offsetHeight;
			OffsetForward = offsetForward;
		}

		public void CalculatePositionTarget(out TGCVector3 pos, out TGCVector3 targetCenter)
		{
			targetCenter = TGCVector3.Add(Target, TargetDisplacement);
			var m = TGCMatrix.Translation(0, OffsetHeight, OffsetForward) * TGCMatrix.RotationY(RotationY) * TGCMatrix.Translation(targetCenter);

			pos = new TGCVector3(m.M41, m.M42, m.M43);
		}
		public void rotateY(float angle)
		{
			RotationY += angle;
		}
	}
}