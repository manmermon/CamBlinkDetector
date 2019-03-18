/*
* Modified by Manuel Merino Monge <manmermon@dte.us.es>
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading;
using LSL;

namespace lslAFFSDK
{
    class Program
    {
        static Affdex.Frame LoadFrameFromFile(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int numBytes = bitmap.Width * bitmap.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            int data_x = 0;
            int ptr_x = 0;
            int row_bytes = bitmap.Width * 3;

            // The bitmap requires bitmap data to be byte aligned.
            // http://stackoverflow.com/questions/20743134/converting-opencv-image-to-gdi-bitmap-doesnt-work-depends-on-image-size

            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(ptr + ptr_x, rgbValues, data_x, row_bytes);//(pixels, data_x, ptr + ptr_x, row_bytes);
                data_x += row_bytes;
                ptr_x += bmpData.Stride;
            }

            bitmap.UnlockBits(bmpData);

            return new Affdex.Frame(bitmap.Width, bitmap.Height, rgbValues, Affdex.Frame.COLOR_FORMAT.BGR);
        }

        private static List<string> findVideos(string path, string ext)
        {
            List<string> lst = new List<string>();

            DirectoryInfo dInfo = new DirectoryInfo(path);
            FileInfo[] files = dInfo.GetFiles("*." + ext);
            DirectoryInfo[] folders = dInfo.GetDirectories();

            foreach (FileInfo fInfo in files)
            {
                if (fInfo.Name.ToCharArray()[0] != '_')
                {
                    lst.Add(fInfo.FullName);
                }
            }

            foreach (DirectoryInfo infoD in folders)
            {
                lst.AddRange(findVideos(infoD.FullName, ext));
            }

            return lst;
        }

        [STAThread]
        static void Main(string[] args)
        {
            CmdOptions options = new CmdOptions();

            string[] opts = new string[args.Length + 2];

            for( int i = 0; i < args.Length; i++ )
            {
                opts[i] = args[i];
            }

            opts = new string[4 ];
            //opts = new string[ 5 ];
            opts[0] = "-i";
            opts[1] = "camera";
            opts[ 2 ] = "-d";
            opts[ 3 ] = "C:\\Program Files\\Affectiva\\AffdexSDK\\data";
            //opts[4] = "-r";

            if (CommandLine.Parser.Default.ParseArguments( opts, options))
            {
                String dataFolder = options.DataFolder;

                if (options.Input.ToLower() != "camera")
                {

                    List<string> pathVideos = findVideos( options.Input, options.Extension );

                    foreach (string pVideo in pathVideos)
                    {
                        processVideo( pVideo, options );
                    }
                }
                else
                {
                    processCamera( dataFolder, options.recordCamera );
                }
            }
        }

        static void processVideo( String pVideo, CmdOptions options )
        {
            try
            {
                Affdex.Detector detector = null;
                List<string> imgExts = new List<string> { ".bmp", ".jpg", ".gif", ".png", ".jpe" };
                List<string> vidExts = new List<string> { ".avi", ".mov", ".flv", ".webm", ".wmv", ".mp4" };

                bool isImage = imgExts.Any<string>(s => (pVideo.Contains(s) || pVideo.Contains(s.ToUpper())));

                if (isImage)
                {
                    System.Console.WriteLine("Trying to process a bitmap image..." + options.Input.ToString());
                    detector = new Affdex.PhotoDetector((uint)options.numFaces, (Affdex.FaceDetectorMode)options.faceMode);
                }
                else
                {
                    System.Console.WriteLine("Trying to process a video file..." + options.Input.ToString());
                    detector = new Affdex.VideoDetector(60F, (uint)options.numFaces, (Affdex.FaceDetectorMode)options.faceMode);
                }

                if (detector != null)
                {
                    System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                    customCulture.NumberFormat.NumberDecimalSeparator = ".";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

                    string pV = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        pV = Directory.GetParent(pV).ToString();
                    }

                    Boolean research = false;
                    StreamWriter outputFile = null;

                    string Fname = Path.Combine(@pV, "blinkValues_" + DateTime.Now.ToString("yyMMdd_hhmmss") + ".txt");
                    string fileHeader = "";

                    fileHeader += "Blink\tEye-Aspect-Rate\t";
                    fileHeader += "frame";

                    System.Console.WriteLine(fileHeader);
                    outputFile = new StreamWriter(Fname);

                    outputFile.WriteLine(fileHeader);
                    
                    ProcessVideo videoForm = new ProcessVideo(detector, 15);

                    videoForm.setOutputVideoFileLog(outputFile);

                    detector.setClassifierPath(options.DataFolder);
                    detector.setDetectAllEmotions(true);
                    detector.setDetectAllExpressions(true);
                    detector.setDetectAllEmojis(true);
                    detector.setDetectAllAppearances(true);
                    detector.start();
                    System.Console.WriteLine("Face detector mode = " + detector.getFaceDetectorMode().ToString());

                    if (isImage)
                    {
                        Affdex.Frame img = LoadFrameFromFile(options.Input);

                        ((Affdex.PhotoDetector)detector).process(img);
                    }
                    else
                    {
                        ((Affdex.VideoDetector)detector).process(options.Input);
                    }

                    videoForm.ShowDialog();
                    videoForm.Dispose();
                    //videoForm = null;

                    outputFile.Close();

                    detector.stop();
                    detector.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        static void processCamera( String trainingDataFolder, bool record )
        {
            try
            {                
                Affdex.Detector detector = null;
                
                System.Console.WriteLine("Trying to process a camera feed...");
                double FPS = 30.0D;
                uint faceNo = 1;
                Affdex.FaceDetectorMode faceLarge = Affdex.FaceDetectorMode.LARGE_FACES;
                detector = new Affdex.CameraDetector(0, FPS, FPS, faceNo, faceLarge );


                if (detector != null)
                {
                    System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                    customCulture.NumberFormat.NumberDecimalSeparator = ".";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

                    string pV = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        pV = Directory.GetParent(pV).ToString();
                    }

                    string Fname = Path.Combine(@pV, "video_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".avi");
                    ProcessVideo videoForm = new ProcessVideo(detector, FPS);
                    if (record)
                    {
                        videoForm.setOutputVideoFile(Fname);
                    }

                    detector.setClassifierPath(trainingDataFolder);
                    detector.setDetectAllEmotions(true);
                    detector.setDetectAllExpressions(true);
                    detector.setDetectAllEmojis(true);
                    detector.setDetectAllAppearances(true);
                    detector.start();
                    System.Console.WriteLine("Face detector mode = " + detector.getFaceDetectorMode().ToString());

                    videoForm.ShowDialog();
                    videoForm.Dispose();

                    /*
                    detector.stop();
                    detector.Dispose();
                    */
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
