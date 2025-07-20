# moves required image assets to Stardew Valley Mods folder

# Move KingCobraJFS to Stardew Valley Mods folder
$source = "C:\Users\milkg\OneDrive\Desktop\eugenesfarm\EugenesFarm\KingCobraJFS"
$dest = "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\KingCobraJFS"

if (Test-Path $dest) {
    Remove-Item $dest -Recurse -Force
}

Copy-Item $source $dest -Recurse

Write-Host "KingCobraJFS mod copied to Stardew Valley Mods folder."

# ----------------------------------------------------------------------------------
# Move galaxyTrailAssets to Stardew Valley Mods folder for EugenesFarm mod

$source = "C:\Users\milkg\OneDrive\Desktop\EugenesFarm\EugenesFarm\Player\galaxyTrailAssets"
$dest = "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\EugenesFarm\galaxyTrailAssets"

if (!(Test-Path $dest)) {
    New-Item -ItemType Directory -Path $dest | Out-Null
}

Get-ChildItem $dest -File | Remove-Item -Force

Copy-Item "$source\*" $dest -Force

Write-Host "galaxyTrailAssets copied to EugenesFarm mod folder in Stardew Valley Mods."