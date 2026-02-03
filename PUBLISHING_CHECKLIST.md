# Pre-Publishing Checklist

Complete this checklist before publishing your NuGet package to ensure quality and professionalism.

## Code Quality

- [ ] All C# code follows naming conventions and best practices
- [ ] XML documentation comments are complete and accurate
- [ ] No compiler warnings or errors
- [ ] All public methods have documentation
- [ ] Code compiles successfully in Release mode

```bash
dotnet build -c Release
dotnet build -c Debug
```

## Package Metadata

- [ ] `<PackageId>` is unique and appropriate
- [ ] `<Version>` follows semantic versioning (MAJOR.MINOR.PATCH)
- [ ] `<Authors>` is updated with your name
- [ ] `<Description>` clearly explains the package purpose
- [ ] `<PackageProjectUrl>` points to valid GitHub repository
- [ ] `<RepositoryUrl>` matches your actual repository
- [ ] `<PackageLicenseExpression>` is appropriate (MIT/Apache/etc.)
- [ ] `<PackageTags>` are relevant and searchable

### Update these fields in ConsoleApp.csproj:
```xml
<Authors>Your Name Here</Authors>
<PackageProjectUrl>https://github.com/yourusername/AzureApplicationInsightsLogger</PackageProjectUrl>
<RepositoryUrl>https://github.com/yourusername/AzureApplicationInsightsLogger</RepositoryUrl>
```

## Testing

- [ ] Sample code in documentation runs without errors
- [ ] All provided examples are tested and working
- [ ] Package installs correctly via NuGet
- [ ] IntelliSense works in Visual Studio
- [ ] No runtime errors when using the package

```bash
# Test local installation
dotnet add package --source ./bin/Release AzureApplicationInsightsLogger
```

## Documentation

- [ ] README in NUGET_README.md is clear and complete
- [ ] Installation instructions are accurate
- [ ] Code examples are correct and tested
- [ ] All public APIs are documented
- [ ] Troubleshooting section covers common issues
- [ ] Dependencies are clearly listed

## Repository Setup (Optional but Recommended)

- [ ] Repository is public on GitHub
- [ ] README.md is comprehensive
- [ ] LICENSE file is included (MIT or your choice)
- [ ] .gitignore is configured for C# projects
- [ ] Documentation is in the repository root or /docs

## NuGet.org Account Setup (First Time Only)

- [ ] Account created at https://www.nuget.org
- [ ] Email verified
- [ ] API Key generated and stored securely
- [ ] NuGet CLI is installed (included with .NET SDK)

```bash
dotnet nuget --version
```

## Pre-Publishing Testing

```bash
# Step 1: Build the package
./build-package.sh  # macOS/Linux
# OR
.\build-package.ps1  # Windows

# Step 2: List package details
dotnet nuget search AzureApplicationInsightsLogger --source ./bin/Release

# Step 3: Test installation in a temporary project
mkdir temp-test
cd temp-test
dotnet new console -n TestApp
cd TestApp
dotnet add package --source ../bin/Release AzureApplicationInsightsLogger
```

## Version Numbering Guide

Use semantic versioning: `MAJOR.MINOR.PATCH`

- **MAJOR** (1.0.0): Breaking changes, major features
- **MINOR** (1.1.0): New features, backward compatible
- **PATCH** (1.0.1): Bug fixes, backward compatible

Examples:
- Initial release: `1.0.0` ✓
- Add new helper method: `1.1.0` ✓
- Fix a bug: `1.0.1` ✓
- Change API signature: `2.0.0` (breaking change)

## Publishing to NuGet.org

### Step 1: Verify Everything
- [ ] All checklist items above are complete
- [ ] Package builds successfully
- [ ] Local testing passes
- [ ] API key is ready

### Step 2: Build Release Package
```bash
./build-package.sh  # macOS/Linux
# OR
.\build-package.ps1  # Windows
```

### Step 3: Publish
```bash
dotnet nuget push ./bin/Release/AzureApplicationInsightsLogger.1.0.0.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

### Step 4: Verify Publication
- [ ] Package appears on https://www.nuget.org/packages/AzureApplicationInsightsLogger/
- [ ] All metadata displays correctly
- [ ] Versions are listed in "Version History"
- [ ] Package can be installed via dotnet CLI

## After Publishing

- [ ] Create a GitHub release with release notes
- [ ] Share on social media / developer communities
- [ ] Update version for next development cycle
- [ ] Monitor issues and feedback
- [ ] Plan updates and improvements

## Troubleshooting

### Package Already Exists
```
Error: Conflict PUT ...
```
**Solution**: Increment version number and rebuild
```xml
<Version>1.0.1</Version>
```

### Invalid API Key
```
Error: Invalid API key
```
**Solution**: Verify API key on nuget.org > Settings > API Keys

### Package Not Found After Publishing
**Solution**: Wait 5-10 minutes for NuGet CDN to update

### Issues During Install
```
Error: Could not find package
```
**Solution**: Rebuild package, verify `.nupkg` file exists and is valid
```bash
dotnet nuget push ./bin/Release/*.nupkg -s https://api.nuget.org/v3/index.json --api-key YOUR_KEY
```

## File Checklist

Ensure these files are in your repository:

- [ ] `ConsoleApp.csproj` - Project file with NuGet metadata
- [ ] `ApplicationInsightsLogger.cs` - Core library classes
- [ ] `ServiceCollectionExtensions.cs` - DI extensions
- [ ] `Program.cs` - Example usage
- [ ] `NUGET_README.md` - Package documentation
- [ ] `PUBLISHING_GUIDE.md` - Publishing instructions
- [ ] `WEBAPI_SAMPLE.cs` - Web API integration sample
- [ ] `build-package.sh` - Build script (Unix)
- [ ] `build-package.ps1` - Build script (Windows)
- [ ] `PROJECT_SUMMARY.md` - Project overview
- [ ] `README.md` - Repository README
- [ ] `LICENSE` - License file (if publishing to public repo)

## Resources

- [NuGet Best Practices](https://learn.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)
- [Semantic Versioning](https://semver.org/)
- [OpenTelemetry Documentation](https://opentelemetry.io/)
- [Azure Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)

---

**Ready to publish? You're all set! 🚀**

For help, see `PUBLISHING_GUIDE.md`
