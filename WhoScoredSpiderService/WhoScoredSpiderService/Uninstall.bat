e@echo off
net stop "WhoScored Spider Service"
echo 卸载程序开始
echo 开始卸载 WhoScoredSpiderService.exe
%windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe -u WhoScoredSpiderService.exe
echo WhoScoredSpiderService.exe 卸载完成