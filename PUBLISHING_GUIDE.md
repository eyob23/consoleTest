# Publishing AzureApplicationInsightsLogger to NuGet

This guide walks you through building and publishing your NuGet package.

## Prerequisites

- .NET 8.0 SDK or later
- NuGet CLI (included with .NET SDK)
- A NuGet.org account (for public publishing)

## Building the Package

### On macOS/Linux:

```bash
chmod +x build-package.sh
./build-package.sh
```

### On Windows:

```powershell
.\build-package.ps1
```

### Manual Build:

```bash
dotnet clean -c Release
dotnet restore
dotnet build -c Release
dotnet pack -c Release -o ./bin/Release
```

The `.nupkg` file will be created in `./bin/Release/`

## Publishing Options

### Option 1: Publish to NuGet.org (Public)

#### Step 1: Create a NuGet Account
1. Go to [nuget.org](https://www.nuget.org/)
2. Click "Sign up" and create an account
3. Verify your email

#### Step 2: Create API Key
1. Log in to your NuGet.org account
2. Go to Settings → API Keys
3. Click "Create"
4. Copy the generated API key
5. Keep it secure (this is like a password)

#### Step 3: Push the Package
```bash
dotnet nuget push ./bin/Release/AzureApplicationInsightsLogger.1.0.0.nupkg \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

#### Step 4: Verify
- Visit `https://www.nuget.org/packages/AzureApplicationInsightsLogger/`
- Your package should appear within a few minutes

### Option 2: Publish to Azure Artifacts (Private)

#### Step 1: Create Azure DevOps Project
1. Go to [dev.azure.com](https://dev.azure.com)
2. Create a new project
3. Create an artifact feed

#### Step 2: Get Feed URL
1. In Azure Artifacts, click "Connect to feed"
2. Select "NuGet.exe"
3. Copy the feed URL

#### Step 3: Configure NuGet.config
Create or update `NuGet.config` in your repository:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Azure DevOps Feed" value="YOUR_FEED_URL" />
  </packageSources>
  <packageSourceCredentials>
    <Azure DevOps Feed>
      <add key="Username" value="anything" />
      <add key="ClearTextPassword" value="YOUR_PAT_TOKEN" />
    </Azure DevOps Feed>
  </packageSourceCredentials>
</configuration>
```

#### Step 4: Push the Package
```bash
dotnet nuget push ./bin/Release/AzureApplicationInsightsLogger.1.0.0.nupkg \
  -s "Azure DevOps Feed"
```

### Option 3: Local Feed (For Testing)

#### Step 1: Create Local Folder
```bash
mkdir ~/nuget-feed
```

#### Step 2: Copy Package
```bash
cp ./bin/Release/AzureApplicationInsightsLogger.1.0.0.nupkg ~/nuget-feed/
```

#### Step 3: Add to NuGet Sources
```bash
dotnet nuget add source ~/nuget-feed --name local-feed
```

#### Step 4: Install Locally
```bash
# In another project
dotnet add package AzureApplicationInsightsLogger --source local-feed
```

## Version Management

Update the version in `ConsoleApp.csproj`:

```xml
<PropertyGroup>
  <Version>1.0.1</Version>
  ...
</PropertyGroup>
```

When publishing a new version:
1. Update version number
2. Rebuild the package
3. Push the new `.nupkg` file

NuGet prevents re-publishing the same version.

## Package Metadata Best Practices

Before publishing, ensure your `.csproj` has:

```xml
<PropertyGroup>
  <PackageId>AzureApplicationInsightsLogger</PackageId>
  <Version>1.0.0</Version>
  <Authors>Your Name</Authors>
  <Description>A reusable NuGet package for Azure Application Insights integration...</Description>
  <PackageProjectUrl>https://github.com/yourusername/repo</PackageProjectUrl>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <RepositoryUrl>https://github.com/yourusername/repo</RepositoryUrl>
  <PackageTags>azure;logging;opentelemetry</PackageTags>
</PropertyGroup>
```

## Troubleshooting

### "Package already exists"
- Increment the version number in `.csproj`
- Rebuild and push again

### "Invalid API key"
- Verify your NuGet.org API key is correct
- Check that the key hasn't expired
- Regenerate if needed

### "Source not found"
- Verify the feed URL is correct
- Check that you have authentication configured
- Run `dotnet nuget list source` to verify registered sources

### "File not found"
- Ensure you're in the correct directory
- Verify the `.nupkg` file exists in `./bin/Release/`
- Check the exact filename

## Documentation

Once published to NuGet.org:
1. Your package will be searchable on nuget.org
2. Users can install via: `dotnet add package AzureApplicationInsightsLogger`
3. Documentation from XML comments will appear in Visual Studio IntelliSense

## Next Steps

1. ✅ Build the package
2. ✅ Test locally
3. ✅ Publish to NuGet.org or your chosen feed
4. 📢 Announce the package in relevant communities
5. 📝 Create GitHub repository for source code
6. 🔄 Maintain version updates and bug fixes

## Resources

- [NuGet Documentation](https://learn.microsoft.com/en-us/nuget/)
- [dotnet nuget push](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-push)
- [Creating and Publishing a Package](https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli)
