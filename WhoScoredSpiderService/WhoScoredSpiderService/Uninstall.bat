@echo off
net stop "WhoScored Spider Service"
echo 卸载程序开始
echo WhoScoredSpiderService 正在卸载......
%windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe -u WhoScoredSpiderService.exe
echo WhoScoredSpiderService 卸载完成