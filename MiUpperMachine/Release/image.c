//?------------------------------------------------------------------------------
//? Includes
#include "image.h"
//?------------------------------------------------------------------------------
//? Definitions

//?------------------------------------------------------------------------------
//? Function declarations

//?------------------------------------------------------------------------------
//? Global variables

//?------------------------------------------------------------------------------
//? Functions
/**
 * @brief:   二值化图像反相
 * @param:   - image:  图像地址
 * 			 - width:  图像宽度
 * 			 - height: 图像高度
 * @date:    2022-10-07
 */
void image_invert_binarized_image(image_t* image, int width, int height)
{
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			// 判断当前像素点是否为白点
			if (image[y * width + x] == (image_t)(~0))
			{
				image[y * width + x] = 0;
			}
			else
			{
				image[y * width + x] = (image_t)(~0);
			}
		}
	}
}

/**
 * @brief:   图像转换至灰度图像
 * @param:   - image:  图像地址
 * 			 - width:  图像宽度
 * 			 - height: 图像高度
 * @date:    2022-10-06
 */
void image_convert_into_gray(image_t* image, int width, int height)
{
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			// 获取当前像素点颜色值
			color_t color;
			color.hex = image[y * width + x];

			// 计算图像灰度
			int gray_level = color.argb.R * 0.299 + color.argb.G * 0.587 + color.argb.B * 0.114;

			// 将彩色图像转为（彩色格式）灰度图像
			color.argb.R = gray_level;
			color.argb.G = gray_level;
			color.argb.B = gray_level;
			image[y * width + x] = color.hex;
		}
	}
}

/**
 * @brief:   图像二值化
 * @param:   - image:		图像地址（需要传入灰度图像）
 * 			 - width:		图像宽度
 * 			 - height:		图像高度
 * 			 - threshold: 	二值化阈值
 * @date:    2022-10-07
 */
void image_binaryzation(image_t* image, int width, int height, int threshold)
{
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			// 获取当前像素点颜色值
			color_t color;
			int position = y * width + x; // 定位图像位置
			color.hex = image[position];

			// 根据阈值对整幅图像进行二值化
			if(color.argb.B > threshold)
			{
				// 超过阈值的部分设置为白色
				image[position] = MACR_COLORMAP_ABSOLUTE_WHITE;
			}
			else
			{
				// 低于等于阈值的部分设为黑色
				image[position] = MACR_COLORMAP_ABSOLUTE_BLACK;
			}
		}
	}
}

/**
 * @brief:   画十字线
 * @param:   - image:  图像地址
 * 			 - width:  图像宽度
 * 			 - height: 图像高度
 * 			 - node_x: 交点x轴位置
 * 			 - node_y: 交点y轴位置
 * 			 - color:  十字线颜色
 * @date:    2022-10-07
 */
void image_draw_cross_curve(image_t* image, int width, int height, int node_x, int node_y, color_t color)
{
	// 判断传入的交点x、y轴坐标是否超过图像限制
	if(node_x >= width || node_x < 0 || node_y >= height || node_y < 0)
	{
		// 设置的十字线交点超出图像范围，提前退出函数，不再绘制十字线
		return;
	}

	// 画横线
	for(int i = 0; i < width; i++)
	{
		image[node_y * width + i] = color.hex;
	}

	// 画竖线
	for(int i = 0; i < height; i++)
	{
		image[i * width + node_x] = color.hex;
	}
}

/**
 * @brief:   C#的调用入口
 * 			 :若useARGB为0，则传入的是二值化图像数组
 * 			 :若useARGB为1，则传入的是彩色图像或灰度图像
 * 			 使用MinGW-w64中GCC进行编译，编译基本指令如下: gcc image.c -shared -o image.dll
 * @param:   - image:	图像数组
 * 			 - width:	图像宽度
 * 			 - height:	图像高度
 * 			 - useARGB:	使用ARGB模式
 * @return:  供C#调用的图像数组指针
 * @date:    2022-10-06
 */
__declspec(dllexport) image_t* ImageHandler(image_t* image, int width, int height, int useARGB)
{
	enum en_image_type
	{
		BINARIZED_IMAGE_TYPE 	= 	0, // 二值化图像
		COLORFUL_IMAGE_TYPE 	= 	1, // 彩色图像
	};
	
	// 判断传入图像类型
	if(useARGB == BINARIZED_IMAGE_TYPE)
	{
		image_invert_binarized_image(image, width, height); // 对二值化图像做反相处理
	}
	else if(useARGB == COLORFUL_IMAGE_TYPE)
	{
		image_convert_into_gray(image, width, height); // 将图像转为灰度图像
		image_binaryzation(image, width, height, 127); // 对图像进行二值化
	}
	else
	{
		// nothing
	}

	// 画指定颜色的十字线
	color_t color;
	color.hex = 0xff26d0ce;
	image_draw_cross_curve(image, width, height, width/2, height/2, color);
	
	return image;
}
//?------------------------------------------------------------------------------
//? End of file
