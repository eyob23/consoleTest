# Script to build and package the AzureApplicationInsightsLogger NuGet package
# Usage: .\build-package.ps1

$ErrorActionPreference = "Stop"

Write-Host "=== AzureApplicationInsightsLogger NuGet Package Builder ===" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ProjectFile = "ConsoleApp.csproj"
$OutputDir = ".\bin\Release"
$PackageName = "AzureApplicationInsightsLogger"

# Check if project file exists
if (-not (Test-Path $ProjectFile)) {
    Write-Host "Error: $ProjectFile not found" -ForegroundColor Red
    exit 1
}

try {
    Write-Host "Step 1: Cleaning previous builds..." -ForegroundColor Yellow
    dotnet clean -c Release

    Write-Host "Step 2: Restoring dependencies..." -ForegroundColor Yellow
    dotnet restore

    Write-Host "Step 3: Building the project..." -ForegroundColor Yellow
    dotnet build -c Release

    Write-Host "Step 4: Creating NuGet package..." -ForegroundColor Yellow
    dotnet pack -c Release -o $OutputDir

    # Find the created .nupkg file
    $NupkgFiles = Get-ChildItem -Path $OutputDir -Filter "$PackageName*.nupkg" -ErrorAction SilentlyContinue
    
    if ($NupkgFiles.Count -eq 0) {
        Write-Host "Error: NuGet package not created" -ForegroundColor Red
        exit 1
    }

    $Nupkg = $NupkgFiles[0].FullName
    $Version = $NupkgFiles[0].BaseName -replace "^$PackageName\.", ""

    Write-Host ""
    Write-Host "✓ Package created successfully!" -ForegroundColor Green
    Write-Host "Package location: $Nupkg" -ForegroundColor Green
    Write-Host "Package version: $Version" -ForegroundColor Green
    Write-Host ""

    # Instructions
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. To test the package locally:"
    Write-Host "   dotnet add package --source ""$OutputDir"" $PackageName"
    Write-Host ""
    Write-Host "2. To publish to NuGet.org:"
    Write-Host "   dotnet nuget push ""$Nupkg"" --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.json"
    Write-Host ""
    Write-Host "3. To publish to a local NuGet feed:"
    Write-Host "   Copy-Item ""$Nupkg"" -Destination YOUR_LOCAL_NUGET_FEED_PATH"
    Write-Host ""
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
