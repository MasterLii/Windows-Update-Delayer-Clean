# Windows Update Delayer 编译脚本
# 使用 MSBuild 编译项目

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Windows Update Delayer 编译脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 查找 MSBuild
$msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"

if (-not (Test-Path $msbuildPath)) {
    # 尝试查找其他版本的 MSBuild
    $possiblePaths = @(
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $msbuildPath = $path
            break
        }
    }
}

if (-not (Test-Path $msbuildPath)) {
    Write-Host "❌ 错误: 未找到 MSBuild.exe" -ForegroundColor Red
    Write-Host "请安装 Visual Studio 2019/2022 或 Build Tools" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "下载地址:" -ForegroundColor Yellow
    Write-Host "Visual Studio: https://visualstudio.microsoft.com/zh-hans/downloads/" -ForegroundColor Cyan
    exit 1
}

Write-Host "✅ 找到 MSBuild: $msbuildPath" -ForegroundColor Green
Write-Host ""

# 编译项目
Write-Host "🔨 开始编译项目..." -ForegroundColor Yellow
Write-Host ""

$solutionFile = "Windows-Update-Delayer.sln"

# 清理旧的编译文件
Write-Host "🧹 清理旧的编译文件..." -ForegroundColor Cyan
& $msbuildPath $solutionFile /t:Clean /p:Configuration=Release /v:quiet

Write-Host ""
Write-Host "🔨 编译 Release 版本..." -ForegroundColor Cyan
Write-Host ""

# 编译 Release 版本
& $msbuildPath $solutionFile /t:Rebuild /p:Configuration=Release /v:minimal /p:WarningLevel=0

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✅ 编译成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📁 输出目录: bin\Release\" -ForegroundColor Cyan
    Write-Host "📦 可执行文件: Windows-Update-Delayer.exe" -ForegroundColor Cyan
    Write-Host ""
    
    # 显示生成的文件
    if (Test-Path "bin\Release\Windows-Update-Delayer.exe") {
        $exeInfo = Get-Item "bin\Release\Windows-Update-Delayer.exe"
        Write-Host "文件大小: $([math]::Round($exeInfo.Length / 1KB, 2)) KB" -ForegroundColor Yellow
        Write-Host "创建时间: $($exeInfo.CreationTime)" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "💡 提示: 运行程序需要以管理员身份运行！" -ForegroundColor Yellow
    
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  ❌ 编译失败！" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查上面的错误信息" -ForegroundColor Yellow
    exit 1
}
