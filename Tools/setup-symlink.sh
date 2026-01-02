#!/bin/bash

set -e

cd "$(dirname "$0")/.."

SOURCE="External/descrio-dotnet/Descrio.Unity/Packages/com.nueruyu.descrio"
TARGET="Packages/com.nueruyu.descrio"

if [ ! -d "Packages" ]; then
    echo "Creating Packages directory..."
    mkdir -p Packages
fi

if [ -e "$TARGET" ] || [ -L "$TARGET" ]; then
    echo "Removing existing link/directory at $TARGET..."
    rm -rf "$TARGET"
fi

if [ ! -d "$SOURCE" ]; then
    echo "Error: Source directory not found: $SOURCE"
    exit 1
fi

echo "Creating symbolic link..."
echo "  Source: $SOURCE"
echo "  Target: $TARGET"

ln -s "../$SOURCE" "$TARGET"

echo "✓ Symbolic link created successfully!"
