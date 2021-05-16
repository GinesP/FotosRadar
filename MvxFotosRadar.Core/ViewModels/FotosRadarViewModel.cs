using BBCSharp;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using MvxFotosRadar.Core.Models;
using MvxFotosRadar;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace MvxFotosRadar.Core.ViewModels
{
    public class FotosRadarViewModel : MvxViewModel
    {
        public FotosRadarViewModel()
        {
            ProcessarImatgesCommand = new MvxCommand(ProcessarImatges);
        }
        private ObservableCollection<FotoModel> _fotos = new ObservableCollection<FotoModel>();

        public IMvxCommand ProcessarImatgesCommand { get; set; }

        public ObservableCollection<FotoModel> Fotos
        {
            get { return _fotos; }
            set { SetProperty(ref _fotos, value); }
        }

        private string _client;

        public string Client
        {
            get { return _client; }
            set { SetProperty(ref _client, value); }
        }

        private string _expedient;

        public string Expedient
        {
            get { return _expedient; }
            set { SetProperty(ref _expedient, value); }
        }

        private string _arxiu_foto;

        public string Arxiu_foto
        {
            get { return _arxiu_foto; }
            set { SetProperty(ref _arxiu_foto, value); }
        }


        public void ObtenerRutaEntradaImagenes(string ruta)
        {
            Console.WriteLine(ruta);
        }

        private string _rutaImatges;

        public string RutaImatges
        {
            get { return _rutaImatges; }
            set { SetProperty(ref _rutaImatges, value);
                RaisePropertyChanged(() => RutaImatges);
                Fotos.Clear();
                AddFotos();
            }
            
        }

        private int contador;

        public int Contador
        {
            get { return contador; }
            set
            {
                SetProperty(ref contador, value);
                RaisePropertyChanged(() => Contador);
                //contador = value;
                //NotifyOfPropertyChange(() => Contador);
            }
        }

        private string pbVisible = "Hidden";

        public string PBVisible
        {
            get { return pbVisible; }
            set
            {
                SetProperty(ref pbVisible, value);
                RaisePropertyChanged(() => PBVisible);
                //pbVisible = value;
                //NotifyOfPropertyChange(() => PBVisible);
            }
        }

        private string missatgeFet = "Hidden";

        public string MissatgeFet
        {
            get { return missatgeFet; }
            set
            {
                SetProperty(ref missatgeFet, value);
                RaisePropertyChanged(() => MissatgeFet);
                //pbVisible = value;
                //NotifyOfPropertyChange(() => PBVisible);
            }
        }



        private bool pbIndeterminate = false;

        public bool PBIndeterminate
        {
            get { return pbIndeterminate; }
            set
            {
                SetProperty(ref pbIndeterminate, value);
                RaisePropertyChanged(() => PBIndeterminate);
                //pbIndeterminate = value;
                //NotifyOfPropertyChange(() => PBIndeterminate);
            }
        }

        private int _totalImatges;


        public int TotalImatges
        {
            get { return _totalImatges; }
            set
            {
                SetProperty(ref _totalImatges, value);
                RaisePropertyChanged(() => TotalImatges);
                //_totalImatges = value;
                //NotifyOfPropertyChange(() => totalMarcadores);
            }
        }

        Dictionary<string, string> d;
        string result;
        string[] separationString = { ".." };
        string[] metadades = null;


        public void AddFotos()
        {
            string[] arxiusDirectori = Directory.GetFiles(_rutaImatges);
            string nom;
            foreach(string nomArxiu in arxiusDirectori)
            {
                nom = Path.GetFileName(nomArxiu);

                FotoModel f = new FotoModel
                {
                    Client = "003",
                    Expedient = "",
                    Arxiu_foto = nom
                };

                Fotos.Add(f);
            }

            TotalImatges = arxiusDirectori.Length;
            
        }

        



        //PROCESAR FOTOS
        public void ProcessarImatges()
        {
            PBVisible = "Visible";
            MissatgeFet = "Hidden";
            BackgroundWorker workerProcessarImatges = new BackgroundWorker();
            workerProcessarImatges.DoWork += worker_DoWork;
            workerProcessarImatges.ProgressChanged += worker_ProgressChanged;
            workerProcessarImatges.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerProcessarImatges.WorkerReportsProgress = true;
            workerProcessarImatges.RunWorkerAsync();

            //A partir de aqui al worker
           
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            string pathSortida = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"SORTIDA");

            Contador = 1;
            
            bool existeix = Directory.Exists(pathSortida);
            if (!existeix)
                Directory.CreateDirectory(pathSortida);

            //Buidar la carpeta
            DirectoryInfo di = new DirectoryInfo(pathSortida);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            using (ExifToolWrapper extw = new ExifToolWrapper())
            {
                extw.Start();

                string linea;



                // Append text to an existing file named "WriteLines.txt".
                string temporal = Path.Combine(Path.GetDirectoryName(RutaImatges), "index.txt");
                string pathIndex=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SORTIDA");

                using (StreamWriter outputFile = new StreamWriter(pathIndex+"\\index.txt", true))
                {
                    

                    foreach (FotoModel foto in Fotos)
                    {
                        try
                        {
                            linea = foto.Client + foto.Expedient + foto.Arxiu_foto;
                            outputFile.WriteLine(linea);
                            d = extw.FetchExifFrom(_rutaImatges + "\\" + foto.Arxiu_foto);
                            d.TryGetValue("Comment", out result);
                            metadades = result.Split(separationString, System.StringSplitOptions.RemoveEmptyEntries);

                            Utils.EscriureText(metadades, _rutaImatges + "\\" + foto.Arxiu_foto, foto.Arxiu_foto);
                            Contador++;
                        } catch(Exception ex)
                        {
                            continue;
                        }

                    }
                }
            }

            

        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //MensajeActualizacion = $"Comprobando {e.ProgressPercentage} de {totalMarcadores}";
            
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Código que se ejecuta cuando termina el segundo plano
            //ReloadBookmarks();
            //MensajeActualizacion = "";
            PBVisible = "Hidden";
            MissatgeFet = "Visible";

        }

        //GENERAR INDEX.TXT
        //OBRIR CARPETADE SORTIDA %HOMEPATH%/DESKTOP/FOTOS_RADAR




    }
}
