@ECHO OFF
set b=%~dp0 
cd %b%
@echo.       ��ʼע�����
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil /LogFile J:\Project\DotNet\CommClass\CommClassService\bin\Debug\CommClassService.exe
@echo.       ����ע��ɹ���

@echo.       ��������
net start "TESTSERVER"
pause