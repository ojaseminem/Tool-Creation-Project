# AutoTools Hub — Push Git Tags for All Tools
# Run this once from the project root: .\push-tool-tags.ps1

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot

Write-Host "`n[AutoTools] Tagging all tools in: $projectRoot`n" -ForegroundColor Cyan

Set-Location $projectRoot

# ── Define all tags ────────────────────────────────────────────────────────
$tags = @(
    "tool/scene-manager",
    "tool/capture-screenshot",
    "tool/remove-missing-scripts",
    "tool/create-folders",
    "tool/shape-editor",
    "tool/mesh-combiner",
    "tool/camera-shake",
    "tool/draggable-object",
    "tool/procedural-recoil",
    "tool/procedural-island-gen",
    "tool/create-materials"
)

# ── Stage and commit all Hub files first ──────────────────────────────────
Write-Host "[1/3] Staging AutoTools Hub files..." -ForegroundColor Yellow

git add Assets/TurtleGameWorks/Editor/Hub/
git add Assets/TurtleGameWorks/Editor/IntroWindow.cs
git add Assets/TurtleGameWorks/package.json
git add Assets/TurtleGameWorks/AUTOTOOLS_README.md

$status = git status --porcelain
if ($status) {
    git commit -m "feat: AutoTools Hub — tag-based installer system

- ToolManifest.cs: static registry of all 11 tools with git tags, source paths, open modes
- ToolInstaller.cs: sparse git clone per tool, copies into InstalledTools/
- AutoToolsHub.cs: two-tab window (Setup = Install/Remove, Tools = Launch)
- AutoToolsInstaller.cs: first-run welcome dialog per project
- Cleaned up old adapter/interface files"
    Write-Host "  Committed Hub files." -ForegroundColor Green
} else {
    Write-Host "  Nothing new to commit — already up to date." -ForegroundColor Gray
}

# ── Create tags ───────────────────────────────────────────────────────────
Write-Host "`n[2/3] Creating tags..." -ForegroundColor Yellow

foreach ($tag in $tags) {
    $exists = git tag -l $tag
    if ($exists) {
        Write-Host "  [SKIP] $tag already exists" -ForegroundColor Gray
    } else {
        git tag $tag
        Write-Host "  [OK]   $tag" -ForegroundColor Green
    }
}

# ── Push everything ────────────────────────────────────────────────────────
Write-Host "`n[3/3] Pushing to origin (main + all tags)..." -ForegroundColor Yellow

git push origin main
git push origin --tags

Write-Host "`n[AutoTools] Done! All tags pushed to GitHub." -ForegroundColor Cyan
Write-Host "  Repo: https://github.com/ojaseminem/Tool-Creation-Project`n" -ForegroundColor Cyan
