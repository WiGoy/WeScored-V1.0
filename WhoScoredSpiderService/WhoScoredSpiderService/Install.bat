@echo off
echo ��ʼ��װ����
if exist %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe -i WhoScoredSpiderService.exe
if not exist %windir%/Microsoft.NET/Framework64/v4.0.30319/InstallUtil.exe
echo �밲װ.Net Framework v4.0.30319
echo WhoScoredSpiderService.exe ��װ���