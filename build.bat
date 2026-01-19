@echo off
chcp 65001 >nul
echo ========================================
echo   Windows Update Delayer 编译脚本
echo ========================================
echo.

REM 查找 MSBuild
set "MSBUILD="

REM 尝试 VS 2022
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)

REM 尝试 VS 2019
if "%MSBUILD%"=="" (
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
)

REM 尝试 .NET Framework 自带的 MSBuild
if "%MSBUILD%"=="" (
    if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
        set "MSBUILD=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
    )
)
if "%MSBUILD%"=="" (
    if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" (
        set "MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    )
)

if "%MSBUILD%"=="" (
    echo ❌ 错误: 未找到 MSBuild.exe
    echo 请安装 Visual Studio 2019/2022 或 .NET Framework 4.0+
    echo.
    echo 下载地址: https://visualstudio.microsoft.com/zh-hans/downloads/
    pause
    exit /b 1
)

echo ✅ 找到 MSBuild: %MSBUILD%
echo.

echo 🧹 清理旧的编译文件...
"%MSBUILD%" Windows-Update-Delayer.sln /t:Clean /p:Configuration=Release /v:quiet /nologo

echo.
echo 🔨 开始编译 Release 版本...
echo.

"%MSBUILD%" Windows-Update-Delayer.sln /t:Rebuild /p:Configuration=Release /v:minimal /nologo

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   ✅ 编译成功！
    echo ========================================
    echo.
    echo 📁 输出目录: bin\Release\
    echo 📦 可执行文件: Windows-Update-Delayer.exe
    echo.
    
    if exist "bin\Release\Windows-Update-Delayer.exe" (
        for %%A in ("bin\Release\Windows-Update-Delayer.exe") do (
            echo 文件大小: %%~zA 字节
        )
    )
    
    echo.
    echo 💡 提示: 运行程序需要以管理员身份运行！
    echo.
) else (
    echo.
    echo ========================================
    echo   ❌ 编译失败！
    echo ========================================
    echo.
    echo 请检查上面的错误信息
    pause
    exit /b 1
)

pause
