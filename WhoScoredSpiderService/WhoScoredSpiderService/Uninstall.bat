@echo off
net stop "WhoScored Spider Service"
echo ж�س���ʼ
echo WhoScoredSpiderService ����ж��......
%windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe -u WhoScoredSpiderService.exe
echo WhoScoredSpiderService ж�����