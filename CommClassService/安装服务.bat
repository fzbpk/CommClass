@ECHO OFF
set b=%~dp0 
cd %b%
@echo.       开始注册服务。
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil /LogFile J:\Project\DotNet\CommClass\CommClassService\bin\Debug\CommClassService.exe
@echo.       服务注册成功。

@echo.       启动服务。
net start "TESTSERVER"
pause