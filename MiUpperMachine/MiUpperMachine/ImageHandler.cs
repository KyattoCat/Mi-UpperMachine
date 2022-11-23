using MiFramework;
using MiUpperMachine.MiImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace MiUpperMachine
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public unsafe class ImageHandler
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int* Handler(int* imagePtr, int width, int height, int useARGB);

        private Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

        private string ImageDLLPath = @"image.dll";
        private string ImageHandleMethodName = "ImageHandler";

        /// <summary>
        /// 用于配置传入dll的数组是否为ARGB格式<br/>
        /// 否则传入大津法二值化的数组
        /// </summary>
        private bool UseARGB = true;

        public ImageHandler()
        {
            // TODO: 在new的时候读取文件中的配置
            colorMap.Add(-1, Color.White);
            colorMap.Add(0, Color.Black);
            colorMap.Add(1, Color.Red);
            colorMap.Add(2, Color.Blue);
            colorMap.Add(3, Color.Green);
        }

        public void Init(string imageDllPath, string imageHandleMethodName)
        {
            ImageDLLPath = imageDllPath;
            ImageHandleMethodName = imageHandleMethodName;
        }

        public Bitmap? Handle(ImageData imageData)
        {
            int width = imageData.ImageWidth;
            int height = imageData.ImageHeight;
            // 创建dll引用
            IntPtr pDll = NativeMethods.LoadLibrary(ImageDLLPath);
            if (pDll == IntPtr.Zero)
            {
                MessageBox.Show($"未找到{ImageDLLPath}，请检查{ImageDLLPath}是否在程序目录或系统环境变量Path下");
                return null;
            }
            // 获取方法
            IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, ImageHandleMethodName);
            if (pAddressOfFunctionToCall == IntPtr.Zero)
            {
                MessageBox.Show($"未在{ImageDLLPath}中找到{ImageHandleMethodName}，请检查入口名称是否正确");
                return null;
            }
            // 获取方法委托
            Handler handler = Marshal.GetDelegateForFunctionPointer<Handler>(pAddressOfFunctionToCall);
            
            fixed (int* imagePtr = new int[width * height])
            {
                if (UseARGB)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color color = imageData.OriginalImage[y, x];
                            int A = color.A;
                            int R = color.R;
                            int G = color.G;
                            int B = color.B;
                            int colorIntValue = A << 24 | R << 16 | G << 8 | B;
                            imagePtr[y * width + x] = colorIntValue;
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            imagePtr[y * width + x] = imageData.BinImage[y, x];
                        }
                    }
                }

                int* resultPtr = null;
                try
                {
                    resultPtr = handler(imagePtr, width, height, UseARGB ? 1 : 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }

                // 运行结果映射到Bitmap
                Bitmap result = new Bitmap(width, height);
                if (UseARGB)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int colorIntValue = resultPtr[y * width + x];
                            Color color = Color.FromArgb(colorIntValue);
                            result.SetPixel(x, y, color);
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int colorIndex = resultPtr[y * width + x] - 255;
                            if (colorIndex < 0) colorIndex = -1;
                            if (colorMap.TryGetValue(colorIndex, out var color))
                            {
                                result.SetPixel(x, y, color);
                            }
                            else
                            {
                                Debug.Log($"不存在id:{colorIndex}的颜色映射");
                            }
                        }
                    }
                }
                // 释放dll引用
                NativeMethods.FreeLibrary(pDll);
                return result;
            }
        }
    }
}
