﻿/*
* Copyright by Manuel Merino Monge <manmermon@dte.us.es>
* Based on Affective project: csharp-sample-apps
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using LSL;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace lslAFFSDK
{
    public partial class VideoPanel : Panel, Affdex.ProcessStatusListener, Affdex.ImageListener, Affdex.FaceListener
    {
        private Bitmap img { get; set; }
        private Dictionary<int, Affdex.Face> faces { get; set; }
        private Affdex.Detector detector { get; set; }
        private ReaderWriterLock rwLock { get; set; }

        private float process_last_timestamp = -1.0f;
        private float process_fps = -1.0f;

        int frameNo = 0;

        int blinkNo = 0;
        int WlinkNo = 0;
        int mlinkNo = 0;

        int EYE_LEFT_OUTER = 16;
        int EYE_LEFT_INNER = 17;
        int EYE_LEFT_UP = 30;
        int EYE_LEFT_BOTTOM = 31;

        int EYE_RIGHT_OUTER = 18;
        int EYE_RIGHT_INNER = 19;
        int EYE_RIGHT_UP = 32;
        int EYE_RIGHT_BOTTOM = 33;

        float blink = 0.0F;

        Affdex.FeaturePoint[] eyeLeft = new Affdex.FeaturePoint[4];
        Affdex.FeaturePoint[] eyeRight = new Affdex.FeaturePoint[4];

        double ear = double.NaN;

        liblsl.StreamInfo info;
        liblsl.StreamOutlet outlet;

        int nChannels = 3;//11;
        double[] data = new double[3];//double[ 11 ];

        //private Emgu.CV.Capture capture = null;
        //Image<Bgr, Byte> bmp;
        VideoWriter vWritter;
        bool saveWebCam = false;
        //bool opened = false;
        string videoPath;

        StreamWriter outputFile;

        double framePerSeconds;

        blinkVOG vogBlink;
        Stopwatch timeFromLastEvent;

        private bool start = false;

        private Form owner;

        private ExcCommander cmdWlink;
        private ExcCommander cmdMultiBlink;

        public VideoPanel( Form owner ): base( )
        {
            System.Console.WriteLine("Starting Interface...");

            this.owner = owner;

            rwLock = new ReaderWriterLock();
            this.DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            this.setFPS(30);
        }
        
        public void setOutputVideoFileLog( StreamWriter outFile )
        {
            outputFile = outFile;
        }

        public void setOutputFileVideo( string vPath )
        {
            this.videoPath = vPath;
            this.saveWebCam = true;
        }
        
        public void setDetector( Affdex.Detector detector )
        {
            this.detector = detector;

            detector.setImageListener( this );
            detector.setProcessStatusListener( this );
        }

        public void setFPS( double FPS )
        {
            framePerSeconds = FPS / 2;
            
            
        }
        
        public void setStart( ExcCommander excWlink, ExcCommander excMultiBlink)
        {
            this.start = !start;

            if( !this.start )
            {
                this.stop();
            }
            else
            {
                this.initBlinkVOG( this.framePerSeconds );

                this.info = new liblsl.StreamInfo("Affectiva-Blinks", "VOG", nChannels, framePerSeconds, liblsl.channel_format_t.cf_double64, "TAIS022-manmermon-AFF-Blinks");
                this.info.desc().append_child_value("channelIDs", "blink, EAR, frame");//, left outer eye, left inner eye, left upper eye, left bottom eye, right outer eye, right inner eye, right upper eye, right bottom eye");
                this.outlet = new liblsl.StreamOutlet(info);

                this.cmdWlink = excWlink;
                this.cmdMultiBlink = excMultiBlink;
            }
        }
        
        private void stop()
        {
            blinkNo = 0;
            WlinkNo = 0;
            mlinkNo = 0;
            frameNo = 0;

            process_last_timestamp = -1.0f;
            process_fps = -1.0f;

            blink = 0.0F;

            ear = double.NaN;

            saveWebCam = false;
            
            if (this.vWritter != null)
            {
                this.vWritter.Dispose();
            }
            
            if (this.outputFile != null)
            {
                this.outputFile.Close();
            }
            
            this.rwLock.ReleaseLock();
            
            
            //this.Close();
            /*
            if (this.outlet != null)
            {
                this.outlet.close_stream();
            }
            */
            this.outlet = null;
            this.info = null;

            this.cmdMultiBlink = null;
            this.cmdWlink = null;
        }

        private void initBlinkVOG(double FPS)
        {
            this.vogBlink = new blinkVOG(2, FPS);
            this.timeFromLastEvent = Stopwatch.StartNew();
        }

        public void onImageCapture(Affdex.Frame frame)
        {
            frame.Dispose();
        }

        public void onFaceFound(float timestamp, int faceid)
        {
        }

        public void onFaceLost(float timestamp, int faceId)
        {
            System.Console.WriteLine(timestamp);
        }

        public void onImageResults(Dictionary<int, Affdex.Face> faces, Affdex.Frame frame)
        {
            process_fps = 1.0f / (frame.getTimestamp() - process_last_timestamp);
            process_last_timestamp = frame.getTimestamp();

            //Console.WriteLine(process_fps);

            try
            {
                byte[] pixels = frame.getBGRByteArray();
                this.img = new Bitmap(frame.getWidth(), frame.getHeight(), PixelFormat.Format24bppRgb);

                if (this.saveWebCam && this.start )
                {
                    this.saveWebCam = false;
                    //this.vWritter = new VideoWriter( this.videoPath, CvInvoke.CV_FOURCC('X', 'V', 'I', 'D'), (int)framePerSeconds, this.img.Width, this.img.Height, true);
                    this.vWritter = new VideoWriter(this.videoPath, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), (int)framePerSeconds, this.img.Width, this.img.Height, true);
                    //this.vWritter = new VideoWriter(this.videoPath, CvInvoke.CV_FOURCC('M', 'P', '4', '2'), (int)framePerSeconds, this.img.Width, this.img.Height, true);
                }

                var bounds = new Rectangle(0, 0, frame.getWidth(), frame.getHeight());
                BitmapData bmpData = img.LockBits(bounds, ImageLockMode.WriteOnly, img.PixelFormat);
                IntPtr ptr = bmpData.Scan0;

                int data_x = 0;
                int ptr_x = 0;
                int row_bytes = frame.getWidth() * 3;

                // The bitmap requires bitmap data to be byte aligned.
                // http://stackoverflow.com/questions/20743134/converting-opencv-image-to-gdi-bitmap-doesnt-work-depends-on-image-size

                for (int y = 0; y < frame.getHeight(); y++)
                {
                    Marshal.Copy(pixels, data_x, ptr + ptr_x, row_bytes);
                    data_x += row_bytes;
                    ptr_x += bmpData.Stride;
                }
                img.UnlockBits(bmpData);

                if (this.vWritter != null && this.start )
                {
                    //Bitmap bmp = img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
                    //this.vWritter.WriteFrame(new Image<Bgr, byte>( bmp ));

                    this.vWritter.WriteFrame(new Image<Bgr, byte>(this.img));
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }

            this.faces = faces;
            //rwLock.ReleaseWriterLock();

            this.Invalidate();
            frame.Dispose();
        }

        private void DrawResults(Graphics g, Dictionary<int, Affdex.Face> faces)
        {
            Pen whitePen = new Pen(Color.White, 2);
            Pen blackPen = new Pen(Color.Black, 2f );
            Brush whiteBrush = new SolidBrush(Color.White);
            Font aFont = new Font(FontFamily.GenericSerif, 15, FontStyle.Bold);
            float radius = 3;
            int spacing = 10;
            int left_margin = 30;

            if (this.start)
            {
                if (faces.Values.Count < 1)
                {
                    this.vogBlink.reset();

                    data[0] = blinkVOG.NONE;
                    data[1] = -1;
                    data[2] = frameNo;

                    if (this.outlet.have_consumers())
                    {
                        outlet.push_sample(data);
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, Affdex.Face> pair in faces)
                    {
                        Affdex.Face face = pair.Value;

                        int i = 0;

                        foreach (Affdex.FeaturePoint fp in face.FeaturePoints)
                        {
                            if (i == EYE_LEFT_OUTER)
                            {
                                eyeLeft[0] = fp;
                            }
                            else if (i == EYE_LEFT_INNER)
                            {
                                eyeLeft[1] = fp;
                            }
                            else if (i == EYE_RIGHT_OUTER)
                            {
                                eyeRight[0] = fp;
                            }
                            else if (i == EYE_RIGHT_INNER)
                            {
                                eyeRight[1] = fp;
                            }

                            if (i == EYE_LEFT_UP)
                            {
                                eyeLeft[2] = fp;
                            }
                            else if (i == EYE_LEFT_BOTTOM)
                            {
                                eyeLeft[3] = fp;
                            }
                            else if (i == EYE_RIGHT_UP)
                            {
                                eyeRight[2] = fp;
                            }
                            else if (i == EYE_RIGHT_BOTTOM)
                            {
                                eyeRight[3] = fp;
                            }

                            g.DrawCircle(whitePen, fp.X, fp.Y, radius);
                            i++;
                        }

                        Affdex.FeaturePoint tl = minPoint(face.FeaturePoints);
                        Affdex.FeaturePoint br = maxPoint(face.FeaturePoints);

                        double leftW = dist(eyeLeft[0], eyeLeft[1]);
                        double rightW = dist(eyeRight[0], eyeRight[1]);
                        double leftH = dist(eyeLeft[2], eyeLeft[3]);
                        double rightH = dist(eyeRight[2], eyeRight[3]);

                        ear = ((leftH + rightH) / 2) / ((leftW + rightW) / 2);

                        data[0] = 0;
                        data[1] = ear;
                        data[2] = frameNo;
                        //data[3] = eyeLeft[0]; data[4] = eyeLeft[1]; data[5] = eyeLeft[2]; data[6] = eyeLeft[3];
                        //data[7] = eyeRight[0]; data[8] = eyeRight[1]; data[9] = eyeRight[2]; data[10] = eyeRight[3];


                        int padding = (int)(tl.Y * .75D);

                        int[] evento = this.vogBlink.apply(new double[] { ear });

                        data[0] = evento[0];

                        if (data[0] == blinkVOG.MLINK)
                        {
                            mlinkNo++;

                            if (this.cmdMultiBlink != null)
                            {
                                this.cmdMultiBlink.execute();
                            }
                        }
                        else if (data[0] == blinkVOG.WLINK)
                        {
                            WlinkNo++;
                            if (this.cmdWlink != null)
                            {
                                this.cmdWlink.execute();
                            }
                        }

                        data[0] *= 100;

                        /*
                        if (ear < thres)
                        {
                            blink = 1F;
                            initBlink = true;
                        }

                        if (initBlink && ear >= thres)
                        {
                            blink = 0F;
                            initBlink = false;
                            blinkNo++;

                            data[0] = 1.0D;                          
                        }
                        */

                        if (this.outlet.have_consumers())
                        {
                            outlet.push_sample(data);
                        }

                        /*
                        String c = String.Format("{0}: {1}", "No. of Blink", blinkNo);
                        g.DrawString(c, aFont, redPen.Brush, new PointF(br.X, padding += spacing));
                        */

                        String c = String.Format("{0}: {1}", "No. Multi-Blink", mlinkNo);
                        GraphicsPath p = new GraphicsPath();
                        int fontSize = 25;

                        p.AddString(
                                    c,             // text to draw
                                    FontFamily.GenericSansSerif,  // or any other font family
                                    (int)FontStyle.Bold,      // font style (bold, italic, etc.)
                                    g.DpiY * fontSize / 72,       // em size
                                    new Point( 10, 60),              // location where to draw text
                                    new StringFormat());          // set options here (e.g. center alignment)
                        g.FillPath(whiteBrush, p);
                        g.DrawPath(blackPen, p);
                        p.Reset();
                        //g.DrawString(c, aFont, blackPen.Brush, new PointF( 10, 60 ));
                        //g.DrawString(c, aFont, whitePen.Brush, new PointF( 10, 60 ));

                        c = String.Format("{0}: {1}", "No. Wlink", WlinkNo);
                        p.AddString(
                                    c,             // text to draw
                                    FontFamily.GenericSansSerif,  // or any other font family
                                    (int)FontStyle.Bold,      // font style (bold, italic, etc.)
                                    g.DpiY * fontSize / 72,       // em size
                                    new Point( 10, 100),              // location where to draw text
                                    new StringFormat());          // set options here (e.g. center alignment)                        
                        g.FillPath(whiteBrush, p);
                        g.DrawPath(blackPen, p);
                        p.Reset();
                        //g.DrawString(c, aFont, blackPen.Brush, new PointF( 10, 80 ));
                        //g.DrawString(c, aFont, whitePen.Brush, new PointF( 10, 80 ));

                        c = String.Format("{0}: {1:0.000}", "Time:", this.timeFromLastEvent.ElapsedMilliseconds / 1000F);
                        if (data[0] == blinkVOG.MLINK * 100 || data[0] == blinkVOG.WLINK * 100)
                        {

                            timeFromLastEvent = Stopwatch.StartNew();

                        }
                        p.AddString(
                                    c,             // text to draw
                                    FontFamily.GenericSansSerif,  // or any other font family
                                    (int)FontStyle.Bold,      // font style (bold, italic, etc.)
                                    g.DpiY * fontSize / 72,       // em size
                                    new Point( 10, 140),              // location where to draw text
                                    new StringFormat());          // set options here (e.g. center alignment)                        
                        g.FillPath(whiteBrush, p);
                        g.DrawPath(blackPen, p);
                        p.Reset();

                        //g.DrawString(c, aFont, blackPen.Brush, new PointF( 10, 100 ));
                        //g.DrawString(c, aFont, whitePen.Brush, new PointF( 10, 100 ));

                        if (outputFile != null)
                        {
                            outputFile.Write(blink + "\t" + ear + "\t" + frameNo + "\n");
                        }
                    }
                }
            }
        }

        public void onProcessingException(Affdex.AffdexException A_0)
        {
            System.Console.WriteLine("Encountered an exception while processing {0}", A_0.ToString());
        }

        public void onProcessingFinished()
        {
            System.Console.WriteLine("Processing finished successfully");
            
            this.owner.Close();
        }

        Affdex.FeaturePoint minPoint(Affdex.FeaturePoint[] points)
        {
            Affdex.FeaturePoint ret = points[0];
            foreach (Affdex.FeaturePoint point in points)
            {
                if (point.X < ret.X) ret.X = point.X;
                if (point.Y < ret.Y) ret.Y = point.Y;
            }
            return ret;
        }

        Affdex.FeaturePoint maxPoint(Affdex.FeaturePoint[] points)
        {
            Affdex.FeaturePoint ret = points[0];
            foreach (Affdex.FeaturePoint point in points)
            {
                if (point.X > ret.X) ret.X = point.X;
                if (point.Y > ret.Y) ret.Y = point.Y;
            }
            return ret;
        }

        [HandleProcessCorruptedStateExceptions]
        protected override void OnPaint(PaintEventArgs e)
        {
            frameNo++;

            try
            {
                // rwLock.AcquireReaderLock(Timeout.Infinite);
                if (img != null)
                {
                    this.Width = img.Width;
                    this.Height = img.Height;
                    e.Graphics.DrawImage((Image)img, new Point(0, 0));
                }

                if (faces != null)
                {
                    DrawResults(e.Graphics, faces);
                }
                else if( this.start )
                {
                    this.vogBlink.reset();

                    data[0] = blinkVOG.NONE;
                    data[1] = -1;
                    data[2] = frameNo;

                    if (this.outlet.have_consumers())
                    {
                        outlet.push_sample(data);
                    }
                }


                e.Graphics.Flush();
            }
            catch (System.AccessViolationException exp)
            {
                System.Console.WriteLine("Encountered AccessViolationException.");
            }
            catch (Exception ex1)
            {

            }
        }

        private void ProcessVideo_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private double dist(Affdex.FeaturePoint fp1, Affdex.FeaturePoint fp2)
        {
            double res = 0;

            double dfX = fp1.X - fp2.X;
            double dfY = fp1.Y - fp2.Y;

            res = Math.Sqrt(dfX * dfX + dfY * dfY);

            return res;
        }

        public void ProcessVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            detector.stop();
            detector.Dispose();

            this.stop();
        }
    }
}

public static class GraphicsExtensions
{
    public static void DrawCircle(this Graphics g, Pen pen,
                                  float centerX, float centerY, float radius)
    {
        g.DrawEllipse(pen, centerX - radius, centerY - radius,
                      radius + radius, radius + radius);
    }
}
