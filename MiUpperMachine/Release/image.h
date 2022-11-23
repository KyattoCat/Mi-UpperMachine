#ifndef IMAGE_H
#define IMAGE_H

//?------------------------------------------------------------------------------
//? Includes
#include "stdint.h"
//?------------------------------------------------------------------------------
//? Definitions
#define COLORMAP_RED 256
#define COLORMAP_BLUE 257
#define COLORMAP_GREEN 258

#define MACR_COLORMAP_ABSOLUTE_BLACK 0xFF000000		 // 纯黑颜色（alpha通道得是255
#define MACR_COLORMAP_ABSOLUTE_WHITE ((image_t)(~0)) // 纯白颜色

/**
 * @brief: ARGB颜色结构体（注意字节序
 */
typedef union
{
	struct argb_s
	{
		uint32_t B	: 8;		///< Blue	- 蓝色
		uint32_t G	: 8;		///< Green	- 黄色
		uint32_t R	: 8;		///< Red	- 红色
		uint32_t A	: 8;		///< alpha	- 阿尔法通道，对应透明度
	} argb;
	uint32_t hex;				///< 16进制ARGB值
} color_t;

typedef int image_t; // 图像类型
//?------------------------------------------------------------------------------
//? Function declarations

#endif

//?------------------------------------------------------------------------------
//? End of file
