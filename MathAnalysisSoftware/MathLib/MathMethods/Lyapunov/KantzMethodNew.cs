using MathLib.DrawEngine;
using System;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov
{
    public class KantzMethodNew1 : LyapunovMethod {

        private int DimMin { get; set; }
        private int DimMax { get; set; }
        private int Tau { get; set; }
        private int MaxIterations { get; set; }
        private int Window { get; set; }        //'theiler window' (0)
        private int Epscount { get; set; }      //number of length scales to use (5)
        private double Epsmin = 1e-3;//{ get; set; }     
        private double Epsmax = 1e-2;//{ get; set; }     

        private const int BOX = 128;
        private const int ibox = BOX - 1;
        
        long[,] box = new long[BOX, BOX];

        public DataSeries Slope;


        static int IM = 100;
        static int II = 100000000;
        static int IFUM = 200;
        static int NX = 50000;



        //-------------------------------------------
        long nmax, nmin, id, m, ifu, nfmin, ncmin;
        double eps;
        //double[] y;
        double[] s;

        public KantzMethodNew1(double[] timeSeries, int dimMin, int dimMax, int tau, int maxIterations, int window, double scaleMin, double scaleMax, int epscount)
            :base(timeSeries) {

            /*
             * DimMin = dimMin;
            DimMax = dimMax;
            Tau = tau;
            MaxIterations = maxIterations;
            Window = window;

            if (scaleMin != 0) {
                eps0set = true;
                Epsmin = scaleMin;
            }

            if (scaleMax != 0) {
                eps1set = true;
                Epsmax = scaleMax;
            }

            Epscount = epscount;
            */
            nmax = 190;
            nmin = 10;
            id = 0;
            m = 2;
            nfmin = 0;
            ncmin = 0;
            ifu = 190;
            eps = 1e-3;

            Slope = new DataSeries();
        }


        static long[] jh, jpntr, nlist;
        static double[] sh;


        public override void Calculate() {
            long i, nc, nf, np, n, nn, nfound;
            jh = new long[IM * IM + 1];
            jpntr = new long[NX];
            nlist = new long[NX];
            sh = new double[IFUM];
            s = new double[ifu];

            if (nmax > NX || ifu > IFUM)
                throw new Exception("Make NX/IFUM larger.");

            _base(nmax - ifu, timeSeries, id, m, jh, jpntr, eps);
            for (i = 0; i < ifu; i++)
                s[i] = 0;
            for (nc = 0, n = (m - 1) * id; n < nmax - ifu && nc < ncmin; n++)
            {
                for (i = 0; i < ifu; i++)
                    sh[i] = 0;                        /* reference points */
                nfound = neigh(nmax - ifu, timeSeries, n, nmax, id, m, jh, jpntr, eps, nlist);
                for (nf = 0, nn = 0; nn < nfound; nn++)
                {
                    np = nlist[nn];
                    if (Math.Abs(n - np) > nmin)
                    {
                        nf++;                                           /* average distances */
                        for (i = 0; i < ifu; i++)
                            sh[i] += Math.Abs((double)(timeSeries[n + i] - timeSeries[np + i]));
                    }
                }
                if (nf >= nfmin)
                {                    /* enough neighbours closer $\epsilon$ */
                    nc++;                             /* average log of averaged distances */
                    for (i = 0; i < ifu; i++)
                        s[i] += Math.Log(sh[i] / nf);
                }
            }
            /*!*/
            Log.AppendFormat("tried {0} reference points, nc={1}\n", n, nc);
            if (nc != 0)
                for (i = 0; i < ifu; i++)
                    Slope.AddDataPoint(i, s[i] / nc);

        }


        void _base(long nmax, double[] y, long id, long m, long[] jh,  long[] jpntr, double eps)
        {
          long i, n;

          for(i=0; i<IM*IM+1; i++) jh[i]=0;
          for(n=(m-1)*id; n<nmax; n++) 
            jh[Index(y[n] / eps, y[n - (m - 1) * id] / eps)]++;  
          for(i=1; i<IM*IM+1; i++) jh[i]+=jh[i - 1];           /* accumulate histogram */
          for(n=(m-1)*id; n<nmax; n++){                  /* fill list of ``pointers" */
            i= Index(y[n]/eps, y[n - (m - 1) * id]/eps);
                jpntr[jh[i]--]=n;
          }
        }

        long neigh(long nmax, double[] y, long n, long nlast, long id,  long m, long[] jh, long[] jpntr, double eps, long[] nlist)
        {
            long jj, kk, i, j, k, jk, ip, np, nfound;

            jj = (long)(y[n] / eps);
            kk = (long)(y[n - (m - 1) * id] / eps);
            for (nfound = 0, j = jj - 1; j <= jj + 1; j++)
            {            /* scan neighbouring boxes */
                for (k = kk - 1; k <= kk + 1; k++)
                {
                    jk = Index(j, k);
                    for (ip = jh[jk + 1]; ip > jh[jk]; ip--)
                    {            /* this is in time order */
                        np = jpntr[ip];
                        if (np >= nlast) break;
                        for (i = 0; i < m; i++)
                            if (Math.Abs((double)(y[n - i * id] - y[np - i * id])) >= eps) break;
                        if (i == m) nlist[nfound++] = np;              /* make list of neighbours */
                    }
                }
            }
            return nfound;
        }


        long Index(double a, double b)
        {
            long la = (long)a, lb = (long)b;
            return IM * ((la + II) % IM) + (lb + II) % IM;
        }


        public override string GetInfoShort() {
            return "Done";
        }

        
        public override string GetInfoFull() {
            StringBuilder fullInfo = new StringBuilder();

            fullInfo.AppendFormat("Min Embedding dimension: {0}\n", DimMin)
                .AppendFormat("Max Embedding dimension: {0}\n", DimMax)
                .AppendFormat("Delay: {0}\n", Tau)
                .AppendFormat("Max Iterations: {0}\n", MaxIterations)
                .AppendFormat("Window around the reference point which should be omitted: {0}\n", Window)
                .AppendFormat(CultureInfo.InvariantCulture, "Min scale: {0:F5}\n", Epsmin)
                .AppendFormat(CultureInfo.InvariantCulture, "Max scale: {0:F5}\n\n", Epsmax)
                .Append(Log.ToString());
            return fullInfo.ToString();
        }


        public void SetSlope(string index) {
            //SlopesList.TryGetValue(index, out slope);
        }

    }
}
