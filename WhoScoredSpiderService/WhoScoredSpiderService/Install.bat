@echo off
echo 开始安装服务
if exist %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe -i WhoScoredSpiderService.exe
if not exist %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe
echo 请安装.Net Framework v4.0.30319
echo WhoScoredSpiderService.exe 安装完成