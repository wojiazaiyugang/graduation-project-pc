using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 毕设
{
    public partial class Form1 : Form
    {
        static int K_MEANS_CLUSTERING_NUM = 8;
        static int K_MEANS_CLUSTERING_TURN = 3;
        static int GROUP_WIDTH = 4;

        int[] KMeansClusteringValueR = new int[K_MEANS_CLUSTERING_NUM], KMeansClusteringValueG = new int[K_MEANS_CLUSTERING_NUM], KMeansClusteringValueB = new int[K_MEANS_CLUSTERING_NUM];//k聚类算法第i簇RGB值
        int[] KMeansPointsNum = new int[K_MEANS_CLUSTERING_NUM], KMeansTotalR = new int[K_MEANS_CLUSTERING_NUM], KMeansTotalG = new int[K_MEANS_CLUSTERING_NUM], KMeansTotalB = new int[K_MEANS_CLUSTERING_NUM];

        Bitmap result;
        public Form1()
        {
           
            InitializeComponent();
            Bitmap bitmap = new Bitmap("F:\\beihang university\\final\\毕设\\1.bmp");
            kMeansCluster(bitmap,bitmap.Width);
            result = new Bitmap(bitmap.Width + GROUP_WIDTH, bitmap.Height + GROUP_WIDTH);
          
            diffusion(bitmap);
            drawEdge(((bitmap.Width / GROUP_WIDTH) / 2) + 1, bitmap.Width + GROUP_WIDTH);
            pictureBox1.Image = result;

            result.Save("F:\\beihang university\\final\\毕设\\result.bmp");

            Bitmap bitmap2 = new Bitmap(1000, 500);
            for (int i=0;i<K_MEANS_CLUSTERING_NUM;i++)
                for (int j=i*30;j<30+i*30;j++)
                    for (int k=0;k<30;k++)
                    {
                        bitmap2.SetPixel(j,k, Color.FromArgb(KMeansClusteringValueR[i], KMeansClusteringValueG[i], KMeansClusteringValueB[i]));
                    }
            pictureBox2.Image = bitmap2;

        }

        private void drawEdge(int number,int width)
        {
            //draw a collun
            for (int i = 0; i < GROUP_WIDTH; i++)
                for (int j = 0; j < width; j++)
                    if ((j / GROUP_WIDTH) % 2 == 0)
                        result.SetPixel(i, j, Color.Black);
                    else
                        result.SetPixel(i, j, Color.White);
            //draw a row
            for (int i = 0; i < width; i++)
                for (int j = 0; j < GROUP_WIDTH; j++)
                    if ((i / GROUP_WIDTH) % 2 == 0)
                        result.SetPixel(i, j, Color.Black);
                    else
                        result.SetPixel(i, j, Color.White);
            //draw encoding modules
            for (int i = GROUP_WIDTH; i < width; i++)
                for (int j = GROUP_WIDTH; j < 2*GROUP_WIDTH; j++)
                    if ((i / GROUP_WIDTH) % 3 == 0)
                        result.SetPixel(j,i, Color.Black);
                    else
                        result.SetPixel(j,i, Color.White);
        }

        private double colourDistance(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            double rmean = (r1 + r2) / 2;
            double r = r1 - r2;
            double g = g1 - g2;
            double b = b1 - b2;
            return Math.Sqrt(((rmean / 256) + 2) * r * r + 4 * g * g + (((255 - rmean) / 256) + 2) * b * b);
        }
        private double distEuc(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            return (Math.Sqrt((r1-r2)*(r1-r2)+(g1-g2)*(g1-g2)+(b1-b2)*(b1-b2)));
        }
        private void kMeansCluster(Bitmap bitmap,int Width)
        {
            for (int i = 0;i<K_MEANS_CLUSTERING_NUM;i++)
            {
                int temp = (255 / K_MEANS_CLUSTERING_NUM) * i;
                KMeansClusteringValueR[i] = temp;
                KMeansClusteringValueG[i] = temp;
                KMeansClusteringValueB[i] = temp;
            }
            for (int i = 1; i <= K_MEANS_CLUSTERING_TURN; i++)
            {
                for (int j = 0;j<K_MEANS_CLUSTERING_NUM;j++)
                {
                    KMeansPointsNum[j] = 0;
                    KMeansTotalR[j] = 0;
                    KMeansTotalG[j] = 0;
                    KMeansTotalB[j] = 0;
                }
                for (int j = 0; j < Width; j++)
                {
                    for (int k = 0;k<Width; k++)
                    {

                        double MinDistance = -1;
                        int x=0;
                        for (int m =0;m<K_MEANS_CLUSTERING_NUM;m++)
                        {
                            if (distEuc(bitmap.GetPixel(j,k).R, bitmap.GetPixel(j, k).G, bitmap.GetPixel(j, k).B, KMeansClusteringValueR[m], KMeansClusteringValueG[m], KMeansClusteringValueB[m])<MinDistance || MinDistance == -1)
                            {
                                MinDistance = distEuc(bitmap.GetPixel(j, k).R, bitmap.GetPixel(j, k).G, bitmap.GetPixel(j, k).B, KMeansClusteringValueR[m], KMeansClusteringValueG[m], KMeansClusteringValueB[m]);
                                x = m;
                            }          
                        }
                        //Console.WriteLine("x:  "+x);
                        KMeansPointsNum[x] = KMeansPointsNum[x] + 1;
                        KMeansTotalR[x] = KMeansTotalR[x] + bitmap.GetPixel(j, k).R;
                        KMeansTotalG[x] = KMeansTotalG[x] + bitmap.GetPixel(j, k).G;
                        KMeansTotalB[x] = KMeansTotalB[x] + bitmap.GetPixel(j, k).B;
                    }
                }
                for (int j =0;j<K_MEANS_CLUSTERING_NUM;j++)
                {
                    if (KMeansPointsNum[j]>0)
                    {
                        KMeansClusteringValueR[j] = KMeansTotalR[j] / KMeansPointsNum[j];
                        KMeansClusteringValueG[j] = KMeansTotalG[j] / KMeansPointsNum[j];
                        KMeansClusteringValueB[j] = KMeansTotalB[j] / KMeansPointsNum[j];
                    }
                }
            }
        }

        private void diffusion(Bitmap bitmap)
        {
            int[,] ValueR = new int[(bitmap.Width/ GROUP_WIDTH)+1, (bitmap.Width  / GROUP_WIDTH)+1], ValueG = new int[(bitmap.Width  / GROUP_WIDTH)+1 , (bitmap.Width  / GROUP_WIDTH)+1], ValueB = new int[(bitmap.Width  / GROUP_WIDTH)+1, (bitmap.Width  / GROUP_WIDTH)+1] ;//第ij组的rgb值
            int AveR, AveG, AveB,ErrR,ErrG,ErrB;
            int x = 0;
            Double MinDistance;
            for (int i=0;i<bitmap.Width/ GROUP_WIDTH; i++)
                for (int j=0;j<bitmap.Width/ GROUP_WIDTH; j++)
                {
                    AveR = 0;
                    AveG = 0;
                    AveB = 0;
                    MinDistance = -1;
                    for (int m1 = i* GROUP_WIDTH; m1<i* GROUP_WIDTH + GROUP_WIDTH; m1++)
                        for (int m2 = j* GROUP_WIDTH; m2<j* GROUP_WIDTH + GROUP_WIDTH; m2++)
                        {
                            AveR = AveR + bitmap.GetPixel(m1, m2).R;
                            AveG = AveG + bitmap.GetPixel(m1, m2).G;
                            AveB = AveB + bitmap.GetPixel(m1, m2).B;
                        }
                    AveR = AveR / (GROUP_WIDTH* GROUP_WIDTH);
                    AveG = AveG / (GROUP_WIDTH * GROUP_WIDTH);
                    AveB = AveB / (GROUP_WIDTH * GROUP_WIDTH);
                    ValueR[i, j] = ValueR[i, j] + AveR;
                    ValueG[i, j] = ValueG[i, j] + AveG;
                    ValueB[i, j] = ValueB[i, j] + AveB;
                    
                    for (int k=0;k<K_MEANS_CLUSTERING_NUM;k++)
                    {
                         if (distEuc(ValueR[i,j],ValueG[i,j],ValueB[i,j],KMeansClusteringValueR[k],KMeansClusteringValueG[k],KMeansClusteringValueB[k])< MinDistance || MinDistance == -1)
                        {
                             MinDistance = distEuc(ValueR[i, j], ValueG[i, j], ValueB[i,j], KMeansClusteringValueR[k], KMeansClusteringValueG[k], KMeansClusteringValueB[k]);
                            x = k;
                        }
                    }

                    for (int m1 = i * GROUP_WIDTH; m1 < i * GROUP_WIDTH + GROUP_WIDTH; m1++)
                        for (int m2 = j * GROUP_WIDTH; m2 < j * GROUP_WIDTH + GROUP_WIDTH; m2++)
                        {
                            //bitmap.SetPixel(m1, m2, Color.FromArgb(KMeansClusteringValueR[x], KMeansClusteringValueG[x], KMeansClusteringValueB[x]));
                            result.SetPixel(m1+GROUP_WIDTH, m2+GROUP_WIDTH, Color.FromArgb(KMeansClusteringValueR[x], KMeansClusteringValueG[x], KMeansClusteringValueB[x]));
                        }
                    ErrR = KMeansClusteringValueR[x] - ValueR[i, j];
                    ErrG = KMeansClusteringValueG[x] - ValueG[i, j];
                    ErrB = KMeansClusteringValueB[x] - ValueB[i, j];
                    ValueR[i + 1, j] = ValueR[i + 1, j] + (int)(ErrR * 0.375);
                    ValueG[i + 1, j] = ValueG[i + 1, j] + (int)(ErrG * 0.375);
                    ValueB[i + 1, j] = ValueB[i + 1, j] + (int)(ErrB * 0.375);

                    ValueR[i, j + 1] = ValueR[i ,j + 1] + (int)(ErrR * 0.375);
                    ValueG[i, j + 1] = ValueG[i, j + 1] + (int)(ErrG * 0.375);
                    ValueB[i, j + 1] = ValueB[i, j + 1] + (int)(ErrB * 0.375);

                    ValueR[i + 1, j + 1] = ValueR[i + 1, j + 1] + (int)(ErrR * 0.25);
                    ValueG[i + 1, j + 1] = ValueG[i + 1, j + 1] + (int)(ErrG * 0.25);
                    ValueB[i + 1, j + 1] = ValueB[i + 1, j + 1] + (int)(ErrB * 0.25);
                }
        }
    }
}
