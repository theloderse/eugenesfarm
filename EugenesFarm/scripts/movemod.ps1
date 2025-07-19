# Move KingCobraJFS to Stardew Valley Mods folder
$source = "C:\Users\milkg\OneDrive\Desktop\eugenesfarm\EugenesFarm\KingCobraJFS"
$dest = "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\KingCobraJFS"

# Remove old folder if it exists
if (Test-Path $dest) {
    Remove-Item $dest -Recurse -Force
}

# Copy new folder
Copy-Item $source $dest -Recurse

Write-Host "KingCobraJFS mod copied to Stardew Valley Mods folder."
