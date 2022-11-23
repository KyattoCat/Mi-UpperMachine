using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MiUpperMachine.MiImage
{
    /// <summary>
    /// 仅储存一张图片的数据<br/>
    /// 包括长宽，文件名，源bitmap，RGB(Color)二维数组，int灰度二维数组...
    /// </summary>
    public class ImageData
    {
        private readonly string fileName;
        private readonly int imageWidth;
        private readonly int imageHeight;
        private readonly Bitmap originalBitmap;

        private Color[,] originalImage;
        private int[,] grayImage;
        private int[,] binImage;
        /// <summary>
        /// 构造函数中已经调用了Bitmap转Color数组
        /// </summary>
        /// <param name="originalBitmap"></param>
        /// <param name="fileName"></param>
        public ImageData(Bitmap originalBitmap, string fileName)
        {
            this.originalBitmap = originalBitmap;
            this.imageWidth = originalBitmap.Width;
            this.imageHeight = originalBitmap.Height;
            this.fileName = fileName;

            originalImage = new Color[imageHeight, imageWidth];
            grayImage = new int[imageHeight, imageWidth];
            binImage = new int[imageHeight, imageWidth];

            //bitmap转color二维
            MiIMG.BitmapToColor2(this);
            //color二维转灰度
            MiIMG.RGBToGray(this);
            MiIMG.GrayToBin(this);
        }

        public Color[,] OriginalImage { get => originalImage; set => originalImage = value; }
        public int[,] GrayImage { get => grayImage; set => grayImage = value; }
        public int[,] BinImage { get => binImage; set => binImage = value; }
        public Bitmap OriginalBitmap => originalBitmap;
        public string FileName => fileName;
        public int ImageWidth => imageWidth;
        public int ImageHeight => imageHeight;

        public Bitmap GrayBitmap => MiIMG.GrayToBitmap(this);
        public Bitmap BinBitmap => MiIMG.BinToBitmap(this);
        public Bitmap ColorBitmap => MiIMG.Color2ToBitmap(OriginalImage);

        /// <summary>
        /// 根据源bitmap重置三个数组
        /// </summary>
        public void Reset()
        {
            //bitmap转color二维
            MiIMG.BitmapToColor2(this);
            //color二维转灰度
            MiIMG.RGBToGray(this);
            MiIMG.GrayToBin(this);
        }
    }
    /// <summary>
    /// 静态类 处理图像<br/>
    /// 例如彩色转灰度，二值化等
    /// </summary>
    public static class MiIMG
    {
        /// <summary>
        /// 面向ImageData<br/>
        /// 根据data中color数组生成gray数组<br/>
        /// 切记先调用BitmapToColor2生成color数组
        /// </summary>
        /// <param name="imageData"></param>
        public static void RGBToGray(ImageData imageData)
        {
            for (int i = 0; i < imageData.ImageHeight; i++)
            {
                for (int j = 0; j < imageData.ImageWidth; j++)
                {
                    imageData.GrayImage[i, j] = (
                          imageData.OriginalBitmap.GetPixel(j, i).R * 19595
                        + imageData.OriginalBitmap.GetPixel(j, i).G * 38469
                        + imageData.OriginalBitmap.GetPixel(j, i).B * 7472) >> 16;
                }
            }
        }
        public static void GrayToBin(ImageData imageData)
        {
            int threshold = OSTU(imageData.GrayImage);
            for (int i = 0; i < imageData.ImageHeight; i++)
            {
                for (int j = 0; j < imageData.ImageWidth; j++)
                {
                    if (imageData.GrayImage[i, j] > threshold)
                    {
                        imageData.BinImage[i, j] = 255;
                    }
                    else
                    {
                        imageData.BinImage[i, j] = 0;
                    }
                }
            }
        }
        /// <summary>
        /// https://blog.csdn.net/bearmomo/article/details/77944860
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int OSTU(int[,] image)
        {
            int width = image.GetUpperBound(1) + 1;
            int height = image.GetUpperBound(0) + 1;
            int x = 0, y = 0;
            int[] pixelCount = new int[256];
            float[] pixelPro = new float[256];
            int i, j, pixelSum = width * height, threshold = 0;
            //初始化
            for (i = 0; i < 256; i++)
            {
                pixelCount[i] = 0;
                pixelPro[i] = 0;
            }
            //统计灰度级中每个像素在整幅图像中的个数
            for (i = y; i < height; i++)
            {
                for (j = x; j < width; j++)
                {
                    pixelCount[image[i, j]]++;
                }
            }
            //计算每个像素在整幅图像中的比例
            for (i = 0; i < 256; i++)
            {
                pixelPro[i] = (float)(pixelCount[i]) / (float)(pixelSum);
            }


            //经典ostu算法,得到前景和背景的分割
            //遍历灰度级[0,255],计算出方差最大的灰度值,为最佳阈值
            float w0, w1, u0tmp, u1tmp, u0, u1, u, deltaTmp, deltaMax = 0;
            for (i = 0; i < 256; i++)
            {
                w0 = w1 = u0tmp = u1tmp = u0 = u1 = u = deltaTmp = 0;


                for (j = 0; j < 256; j++)
                {
                    if (j <= i) //背景部分
                    {
                        //以i为阈值分类，第一类总的概率
                        w0 += pixelPro[j];
                        u0tmp += j * pixelPro[j];
                    }
                    else       //前景部分
                    {
                        //以i为阈值分类，第二类总的概率
                        w1 += pixelPro[j];
                        u1tmp += j * pixelPro[j];
                    }
                }


                u0 = u0tmp / w0;        //第一类的平均灰度
                u1 = u1tmp / w1;        //第二类的平均灰度
                u = u0tmp + u1tmp;      //整幅图像的平均灰度
                                        //计算类间方差
                deltaTmp = w0 * (u0 - u) * (u0 - u) + w1 * (u1 - u) * (u1 - u);
                //找出最大类间方差以及对应的阈值
                if (deltaTmp > deltaMax)
                {
                    deltaMax = deltaTmp;
                    threshold = i;
                }
            }
            //返回最佳阈值;
            return threshold;
        }
        /// <summary>
        /// 面向ImageData<br/>
        /// 根据data中源bitmap生成color数组
        /// </summary>
        /// <param name="imageData"></param>
        public static void BitmapToColor2(ImageData imageData)
        {
            for (int i = 0; i < imageData.ImageHeight; i++)
            {
                for (int j = 0; j < imageData.ImageWidth; j++)
                {
                    imageData.OriginalImage[i, j] = imageData.OriginalBitmap.GetPixel(j, i);
                }
            }
        }
        public static Bitmap Color2ToBitmap(Color[,] array)
        {
            int height = array.GetUpperBound(0) + 1, width = array.GetUpperBound(1) + 1;
            Bitmap bitmap = new Bitmap(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, i, array[i, j]);
                }
            }
            return bitmap;
        }
        public static Bitmap GrayToBitmap(ImageData imageData)
        {
            Bitmap bitmap = new Bitmap(imageData.ImageWidth, imageData.ImageHeight);
            for (int i = 0; i < imageData.ImageHeight; i++)
            {
                for (int j = 0; j < imageData.ImageWidth; j++)
                {
                    bitmap.SetPixel(j, i, Color.FromArgb(imageData.GrayImage[i, j], imageData.GrayImage[i, j], imageData.GrayImage[i, j]));
                }
            }
            return bitmap;
        }
        public static Bitmap BinToBitmap(ImageData imageData)
        {
            Bitmap bitmap = new Bitmap(imageData.ImageWidth, imageData.ImageHeight);
            for (int i = 0; i < imageData.ImageHeight; i++)
            {
                for (int j = 0; j < imageData.ImageWidth; j++)
                {
                    if (imageData.BinImage[i, j] == 0)
                    {
                        bitmap.SetPixel(j, i, Color.Black);
                    }
                    else
                    {
                        bitmap.SetPixel(j, i, Color.White);
                    }
                }
            }
            return bitmap;
        }
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
