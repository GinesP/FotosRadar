using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MvxFotosRadar.Core
{
    public static class Utils
    {
        public static void EscriureText(string[] metadades, string rutaImatge, string nomImatge)
        {
            
            string rutaSortida = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SORTIDA");

            //Normalitzar dades 
            string data = metadades[2];
            data = data.Replace("Date=", "");

            string hora = metadades[3];
            hora = hora.Replace("Time=", "");

            string operatorID = metadades[5];
            operatorID = operatorID.Replace("OperatorID=", "");

            string limit = metadades[11];
            limit = limit.Replace("SpeedLimit", "Límit");

            string velocitatUnitats = metadades[13];
            velocitatUnitats = velocitatUnitats.Replace("SpeedUnits=", "");

            string velocitat = metadades[7];
            velocitat = velocitat.Replace("Speed", "Vel");

            string distancia = metadades[8];
            distancia = distancia.Replace("Distance", "Dist");

            string distanciaUnitats = metadades[12];
            distanciaUnitats = distanciaUnitats.Replace("DistanceUnits=", "");

            string localitzacio = metadades[6];
            localitzacio = localitzacio.Replace("Location=", "");

            string laserSN = metadades[18];
            laserSN = laserSN.Replace("LaserSN=", "");




            Image imagen = Image.FromFile(rutaImatge);
            Graphics g = Graphics.FromImage(imagen);

            int ample =  imagen.Width;
            int alt = imagen.Height;

            
            using (Font myFont = new Font("Arial", 10,FontStyle.Bold))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,


                };

                var format2 = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,


                };

                RectangleF rectangleSuperior = new RectangleF(0, 0,ample, 25);
                RectangleF rectangleInferior = new RectangleF(0, alt-30, 1280, 30);
                Pen whitePen = new Pen(Color.White);
                Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

                //TODO: MeasureString

                SizeF stringSize = new SizeF();

                using(Graphics gr = Graphics.FromHwnd(IntPtr.Zero))
                {
                    stringSize = gr.MeasureString(localitzacio, myFont);
                }
                

                //g.DrawRectangle(whitePen, 0, 0, 1280, 30);
                g.FillRectangle(brush, rectangleSuperior);
                g.FillRectangle(brush, rectangleInferior);

                g.DrawString(data, myFont, Brushes.Black, new Point(75, 15), format);
                g.DrawString(hora, myFont, Brushes.Black, new Point(190, 15), format);
                g.DrawString(operatorID, myFont, Brushes.Black, new Point(500, 15), format);
                g.DrawString(laserSN, myFont, Brushes.Black, new Point(Convert.ToInt32(ample / 1.42), 15), format);


                g.DrawString(limit+" "+velocitatUnitats, myFont, Brushes.Black, new Point(75, alt-15), format);
                g.DrawString(velocitat + " " + velocitatUnitats, myFont, Brushes.Black, new Point(225, alt - 15), format);
                g.DrawString(distancia + " " + distanciaUnitats, myFont, Brushes.Black, new Point(355, alt - 15), format);
                
                g.DrawString(localitzacio, myFont, Brushes.Black, new Point( (Convert.ToInt32( ample-stringSize.Width)) , alt - 15), format2);
                /*g.DrawString(localitzacio, myFont, Brushes.Black, , format);*/

                imagen.Save(rutaSortida + "\\"+ nomImatge);



            }
        }

       

    }
}
