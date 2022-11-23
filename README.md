# Mi-UpperMachine
提供一个打开和显示图片的窗口，并调用指定方法执行用户的写的图片处理代码

---

本项目依赖于：

- .NET 6.0运行时 -> 启动程序所必须
- MinGW-W64（或者其他能编译64位dll的东西） -> 用于编译64位的dll给上位机使用

---

仓库内有两个解决方案：
1. <del>上位机v0<del>，现已删除
2. MiUpperMachine

<del>上位机v0是旧版本的，蛮久以前写的，稍稍好看一点，功能也比较全，但是用户代码是要用c#来写，而且需要用vs2022开启，比较不符合现在实验室的开发环境。
之后也不会再去更新这个解决方案，当然弄下来自己改一改也可以用。<del>

MiUpperMachine是现在在重写的项目，刚写，没有什么界面美观可言，也欢迎提出建议。
我会在闲暇时间更新这个项目，目标是能够让用户不需要vs启动，可以用c语言写图像处理的代码（主要思路是编译成dll提供给上位机调用）。

<p align="right">@KyattoCat 2022年9月19日</p>
