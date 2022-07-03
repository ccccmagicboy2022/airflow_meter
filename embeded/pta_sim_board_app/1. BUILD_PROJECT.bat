@echo off
::Keil执行文件位置
set UV=d:\cccc2022\TOOL\Keil_v5\UV4\UV4.exe
::查找uvprojx工程文件
for /f "usebackq delims=" %%j in (`dir /s /b %cd%\*.uvprojx`) do (
if exist %%j (
set UV_PRO_PATH="%%j"))
echo ---------------------------------------------------------------
echo Author: cccc
echo Init building ...
echo >build_log0.txt
echo >build_log1.txt
%UV% -j0 -b %UV_PRO_PATH% -t "flashcode" -l %cd%\build_log0.txt
type build_log0.txt
%UV% -j0 -b %UV_PRO_PATH% -t "ramcode" -l %cd%\build_log1.txt
type build_log1.txt
echo Done.
pause


