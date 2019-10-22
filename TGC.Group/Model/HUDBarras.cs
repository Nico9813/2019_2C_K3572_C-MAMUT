using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Sprites;
using TGC.Core.Mathematica;
using TGC.Core.Direct3D;
using System.Drawing;
using TGC.Core.Text;
using TGC.Core.Direct3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    class HUDBarras
    {
        private CustomSprite BarraBateria;
        private CustomSprite RellenoBateria;
        private Drawer2D drawer;


        private readonly static HUDBarras _instance = new HUDBarras();

        private HUDBarras()
        {
        }

        public static HUDBarras Instance
        {
            get
            {
                return _instance;
            }
        }


        public void Init(String MediaDir)
        {
            var width = D3DDevice.Instance.Width;
            var height = D3DDevice.Instance.Height;
            drawer = new Drawer2D();

            BarraBateria = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "\\2D\\BarraBateria.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(width * 0.25f, height * 0.25f),
                Color = Color.Red,
                
            };

            RellenoBateria = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "\\2D\\Bateria.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(width * 0.25f, height * 0.25f),
                //Scaling = new TGCVector2(0.5f,0.5f),
            };





        }

        public void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(RellenoBateria);
            drawer.DrawSprite(BarraBateria);
           
            drawer.EndDrawSprite();

        }




    }
}
