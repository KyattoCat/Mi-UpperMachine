@echo 尝试编译image.dll文件!

gcc image.c -shared -o image.dll

@echo 1秒后自动关闭...
@choice /t 1 /d y /n >nul