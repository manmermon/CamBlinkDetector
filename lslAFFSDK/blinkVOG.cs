/*
* Copyright 2018-2019 by Manuel Merino Monge <manmermon@dte.us.es>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lslAFFSDK
{
    class blinkVOG
    {
        public const int NONE = 0;
        public const int BLINK = 1;
        public const int WLINK = 2;
        public const int MLINK = 4 | BLINK;

        private enum EYE_STATES { Close, Open };
        private enum STATES { Wait, Blink, Wait2Blink, MultiBlinks };

        Centroid centroids;
        double FPS;

        int blinkDuration;
        STATES estado;

        int mlinkTimeThreshold;
        int mlinkSpan = 0;

        bool centroidInit = false;
        
        public blinkVOG( int nCentroids, double FPS, double mlinkTimeThres = 1.2 )
        {
            this.centroids = new Centroid( nCentroids );
            
            this.FPS = FPS;

            estado = STATES.Wait;

            this.mlinkTimeThreshold = (int)( mlinkTimeThres * FPS );
        }

        public blinkVOG( double[] centroids, double FPS, double mlinkTimeThres = 1.2 )
        {
            this.centroids = new Centroid( centroids );
            
            this.FPS = FPS;

            this.mlinkTimeThreshold = (int)(mlinkTimeThres * FPS);

            this.centroidInit = true;
        }
         
        public int[]  apply( double[] data )
        {
            if (!this.centroidInit)
            {
                double val = -1;
                for( int i = 0; i < data.Length && !this.centroidInit; i++ )
                {
                    val = data[i];
                    this.centroidInit =  val >= 0; ;
                }

                if ( this.centroidInit )
                {
                    //this.centroidInit = true;

                    int numCentrs = this.centroids.getNumCentroids();

                    double adj = ( 1.3D - 0.5D )/ (numCentrs - 1);

                    double step = 0.5;

                    for (int i = 0; i < numCentrs; i++)
                    {
                        this.centroids.setCentroid(i, data[0] * step);
                        step += adj;
                    }
                }
            }

            int[] eventos = new int[ data.Length ];

            if (this.centroidInit)
            {
                int[] Labels = this.classify(data);                

                for (int iLab = 0; iLab < Labels.Length; iLab++)
                {
                    eventos[iLab] = this.pulseVOG(Labels[iLab]);
                }
            }

            return eventos;
        }
        
        public void reset()
        {
            this.centroidInit = false;
            estado = 0;
            blinkDuration = 0;
        }

        private int[] classify( double[] data )
        {
            double adj1 = this.FPS * 7 / 12;
            double adj2 = adj1 + 1;

            int[] labels = new int[data.Length];

            int posCentroid = -1;

            for (int iData = 0; iData < data.Length; iData++)
            {
                double valData = data[iData];

                double distance = Double.PositiveInfinity;

                for (int iCentrods = 0; iCentrods < this.centroids.getNumCentroids(); iCentrods++)
                {
                    double c = this.centroids.getCentroid(iCentrods);
                    c = Math.Abs(c - valData);
                    if (c < distance)
                    {
                        posCentroid = iCentrods;
                        distance = c;
                    }
                }

                if (distance > 0)
                {
                    this.centroids.setCentroid(posCentroid, ( this.centroids.getCentroid(posCentroid) * adj1 + valData) / adj2);
                }

                labels[iData] = posCentroid;
            }

            return labels;
        }

        private int pulseVOG( int inVal)
        {
            int evento = NONE;

            /*
            switch (estado)
            {
                case EYE_STATES.Close:
                    {
                        if (inVal == 0)
                        {
                            blinkDuration += 1;
                        }
                        else
                        {
                            estado = EYE_STATES.Open;

                            evento = BLINK;
                            if (blinkDuration >= this.FPS / 2)
                            {
                                evento = WLINK;
                            }

                            blinkDuration = 0;
                        }
                        break;
                    }
                default:
                    {
                        if (inVal == 0)
                        {
                            estado = EYE_STATES.Close;
                            blinkDuration += 1;
                        }
                        break;
                    }
            }
            */

            switch (estado)
            {
                case STATES.Blink:
                    {
                        if (inVal == (int)EYE_STATES.Close )
                        {
                            blinkDuration += 1;
                        }
                        else
                        {
                            estado = STATES.Wait;
                            evento = WLINK;                            

                            if (blinkDuration < this.FPS / 2)
                            {
                                evento = BLINK;
                                estado = STATES.Wait2Blink;
                                mlinkSpan++;
                            }

                            blinkDuration = 0;
                        }
                        
                        break;
                    }
                case STATES.Wait2Blink:
                    {
                        if( inVal == (int)EYE_STATES.Open )
                        {
                            mlinkSpan++;
                            if( mlinkSpan > mlinkTimeThreshold )
                            {
                                estado = STATES.Wait;
                                mlinkSpan = 0;
                            }
                        }
                        else
                        {
                            estado = STATES.MultiBlinks;

                            mlinkSpan = 0;
                        }

                        break;
                    }
                case STATES.MultiBlinks:
                    {
                        if( inVal == (int)EYE_STATES.Open )
                        {
                            evento = MLINK;
                            estado = STATES.Wait;
                        }

                        break;
                    }
                default:
                    {
                        if (inVal == (int)EYE_STATES.Close )
                        {
                            estado = STATES.Blink;
                            blinkDuration += 1;
                        }
                        break;
                    }
            }

            return evento;
        }

        private class Centroid
        {
            private double[] centroids;

            public Centroid(int nCentroids)
            {
                this.centroids = new double[nCentroids];
            }

            public Centroid(double[] cs)
            {
                this.centroids = cs;
            }

            public int getNumCentroids()
            {
                return this.centroids.Length;
            }

            public double getCentroid(int index)
            {
                return this.centroids[index];
            }

            public void setCentroid(int index, double val)
            {
                this.centroids[index] = val;
            }
        }
    }
}
