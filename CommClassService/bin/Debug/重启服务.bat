@ECHO OFF
@echo.       开始停止服务。
net stop "TESTSERVER"

@echo.       启动服务。
net start "TESTSERVER"