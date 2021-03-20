
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MvxFotosRadar.Wpf
{
    class Utils
    {
        public static void EscriureText(string[] metadades, string rutaImatge, string nomImatge)
        {
            string rutaSortida = @"C:\Users\Gines\Desktop\SORTIDA\";
            Image imagen = Image.FromFile(rutaImatge);
            Graphics g = Graphics.FromImage(imagen);
            using (Font myFont = new Font("Arial", 14))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,

                };


                g.DrawString(metadades[7], myFont, Brushes.Black, new Point(50, 25), format);

                imagen.Save(rutaSortida + nomImatge);

            }
                
            


        }
    }
}
