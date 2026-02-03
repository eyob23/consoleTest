#!/bin/bash
# Script to build and package the AzureApplicationInsightsLogger NuGet package

set -e

echo "=== AzureApplicationInsightsLogger NuGet Package Builder ==="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
PROJECT_FILE="ConsoleApp.csproj"
OUTPUT_DIR="./bin/Release"
PACKAGE_NAME="AzureApplicationInsightsLogger"

# Check if project file exists
if [ ! -f "$PROJECT_FILE" ]; then
    echo -e "${RED}Error: $PROJECT_FILE not found${NC}"
    exit 1
fi

echo -e "${YELLOW}Step 1: Cleaning previous builds...${NC}"
dotnet clean -c Release

echo -e "${YELLOW}Step 2: Restoring dependencies...${NC}"
dotnet restore

echo -e "${YELLOW}Step 3: Building the project...${NC}"
dotnet build -c Release

echo -e "${YELLOW}Step 4: Creating NuGet package...${NC}"
dotnet pack -c Release -o "$OUTPUT_DIR"

# Find the created .nupkg file
NUPKG=$(find "$OUTPUT_DIR" -name "${PACKAGE_NAME}*.nupkg" | head -n 1)

if [ -z "$NUPKG" ]; then
    echo -e "${RED}Error: NuGet package not created${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}✓ Package created successfully!${NC}"
echo -e "${GREEN}Package location: $NUPKG${NC}"
echo ""

# Get package version
VERSION=$(basename "$NUPKG" | sed "s/${PACKAGE_NAME}\.\(.*\)\.nupkg/\1/")
echo -e "${GREEN}Package version: $VERSION${NC}"
echo ""

# Instructions
echo -e "${YELLOW}Next steps:${NC}"
echo ""
echo "1. To test the package locally:"
echo "   dotnet add package --source \"$OUTPUT_DIR\" $PACKAGE_NAME"
echo ""
echo "2. To publish to NuGet.org:"
echo "   dotnet nuget push \"$NUPKG\" --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.json"
echo ""
echo "3. To publish to a local NuGet feed:"
echo "   cp \"$NUPKG\" YOUR_LOCAL_NUGET_FEED_PATH/"
echo ""
