using System;
using System.Drawing;

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
            velocitat = velocitat.Replace("Speed", "Velocitat");

            string distancia = metadades[8];
            distancia = distancia.Replace("Distance", "Distancia");

            string distanciaUnitats = metadades[12];
            distanciaUnitats = distanciaUnitats.Replace("DistanceUnits=", "");

            string localitzacio = metadades[6];
            localitzacio = localitzacio.Replace("Location=", "");

            string laserSN = metadades[18];
            laserSN = laserSN.Replace("LaserSN=", "");




            Image imagen = Image.FromFile(rutaImatge);
            Graphics g = Graphics.FromImage(imagen);
            using (Font myFont = new Font("Arial", 10,FontStyle.Bold))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,


                };

                RectangleF rectangleSuperior = new RectangleF(0, 0,1280, 25);
                RectangleF rectangleInferior = new RectangleF(0, 994, 1280, 30);
                Pen whitePen = new Pen(Color.White);
                Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
                //g.DrawRectangle(whitePen, 0, 0, 1280, 30);
                g.FillRectangle(brush, rectangleSuperior);
                g.FillRectangle(brush, rectangleInferior);

                g.DrawString(data, myFont, Brushes.Black, new Point(75, 15), format);
                g.DrawString(hora, myFont, Brushes.Black, new Point(190, 15), format);
                g.DrawString(operatorID, myFont, Brushes.Black, new Point(500, 15), format);
                g.DrawString(laserSN, myFont, Brushes.Black, new Point(900, 15), format);


                g.DrawString(limit+" "+velocitatUnitats, myFont, Brushes.Black, new Point(125, 1010), format);
                g.DrawString(velocitat + " " + velocitatUnitats, myFont, Brushes.Black, new Point(350, 1010), format);
                g.DrawString(distancia + " " + distanciaUnitats, myFont, Brushes.Black, new Point(575, 1010), format);
                g.DrawString(localitzacio, myFont, Brushes.Black, new Point(900, 1010), format);

                imagen.Save(rutaSortida + "\\"+ nomImatge);

            }
                
            


        }
    }
}
