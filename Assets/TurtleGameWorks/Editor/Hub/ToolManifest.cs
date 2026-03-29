using System.Collections.Generic;

namespace TurtleGameWorks.AutoTools
{
    // ── How a tool is opened ────────────────────────────────────────────────
    public enum OpenMode
    {
        EditorWindow,     // GetWindow<T>() — has a visible window
        MenuItem,         // Invoke a menu path via EditorApplication.ExecuteMenuItem
        SettingsProvider, // Open Project Settings at a path
        RuntimeComponent  // Runtime MonoBehaviour — no "open", just installs scripts
    }

    // ── One entry per tool ──────────────────────────────────────────────────
    public class ToolEntry
    {
        /// <summary>Friendly display name.</summary>
        public string Name;

        /// <summary>Short description for the Hub card.</summary>
        public string Description;

        /// <summary>Sidebar category.</summary>
        public string Category;

        /// <summary>Unity built-in icon name (EditorGUIUtility.IconContent).</summary>
        public string IconName;

        /// <summary>Git tag used to identify this tool in the remote repo.</summary>
        public string GitTag;

        /// <summary>
        /// Paths (relative to repo root) that belong to this tool.
        /// All of them get copied on install.
        /// </summary>
        public string[] SourcePaths;

        /// <summary>
        /// Where files land inside the target project.
        /// Mirrors the last folder segment of each SourcePath by default,
        /// but you can override per-tool here.
        /// </summary>
        public string InstallSubFolder; // relative to Assets/TurtleGameWorks/InstalledTools/

        /// <summary>How to invoke / open the tool once installed.</summary>
        public OpenMode OpenMode;

        /// <summary>
        /// For EditorWindow → fully-qualified type name.
        /// For MenuItem / SettingsProvider → the menu / settings path string.
        /// For RuntimeComponent → ignored.
        /// </summary>
        public string OpenTarget;
    }

    // ── Static catalogue of every tool in the repo ──────────────────────────
    public static class ToolManifest
    {
        public static readonly List<ToolEntry> All = new List<ToolEntry>
        {
            new ToolEntry
            {
                Name        = "Scene Manager",
                Description = "Manage and quick-load scenes. Supports custom names and saved scene layouts.",
                Category    = "Editor Windows",
                IconName    = "UnityEditor.SceneHierarchyWindow",
                GitTag      = "tool/scene-manager",
                SourcePaths = new[]
                {
                    "Assets/TurtleGameWorks/Editor/SceneManagerTool"
                },
                InstallSubFolder = "SceneManagerTool",
                OpenMode    = OpenMode.EditorWindow,
                OpenTarget  = "TurtleGameWorks.Editor.SceneManagerTool.SceneManagerTool"
            },

            new ToolEntry
            {
                Name        = "Capture Screenshot",
                Description = "Capture high-res screenshots via F11 shortcut. Configure output path in Project Settings.",
                Category    = "Editor Utilities",
                IconName    = "Camera Icon",
                GitTag      = "tool/capture-screenshot",
                SourcePaths = new[]
                {
                    "Assets/TurtleGameWorks/Editor/CaptureScreenshotTool"
                },
                InstallSubFolder = "CaptureScreenshotTool",
                OpenMode    = OpenMode.SettingsProvider,
                OpenTarget  = "TurtleGameWorks/Capture Screenshot"
            },

            new ToolEntry
            {
                Name        = "Remove Missing Scripts",
                Description = "Scan and strip all missing-script references from scene objects and prefabs.",
                Category    = "Editor Windows",
                IconName    = "console.warnicon",
                GitTag      = "tool/remove-missing-scripts",
                SourcePaths = new[]
                {
                    "Assets/TurtleGameWorks/Editor/RemoveMissingScriptsTool"
                },
                InstallSubFolder = "RemoveMissingScriptsTool",
                OpenMode    = OpenMode.EditorWindow,
                OpenTarget  = "TurtleGameWorks.Editor.RemoveMissingScriptsTool.RemoveMissingScriptsTool"
            },

            new ToolEntry
            {
                Name        = "Create Folders",
                Description = "One-click scaffold of a standard project folder structure under Assets.",
                Category    = "Editor Utilities",
                IconName    = "Folder Icon",
                GitTag      = "tool/create-folders",
                SourcePaths = new[]
                {
                    "Assets/TurtleGameWorks/Editor/CreateFoldersTool"
                },
                InstallSubFolder = "CreateFoldersTool",
                OpenMode    = OpenMode.MenuItem,
                OpenTarget  = "Assets/Create/TurtleGameWorks/CreateProjectFolders"
            },

            new ToolEntry
            {
                Name        = "Shape Editor",
                Description = "In-scene procedural 2D shape editor with vertex handles and mesh output.",
                Category    = "Editor Windows",
                IconName    = "EditCollider",
                GitTag      = "tool/shape-editor",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/ShapeEditorTool"
                },
                InstallSubFolder = "ShapeEditorTool",
                OpenMode    = OpenMode.EditorWindow,
                OpenTarget  = "ShapeEditor"
            },

            new ToolEntry
            {
                Name        = "Mesh Combiner",
                Description = "Combine multiple meshes into one draw call to optimise rendering at runtime.",
                Category    = "Runtime Components",
                IconName    = "Mesh Icon",
                GitTag      = "tool/mesh-combiner",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/MeshCombinerTool"
                },
                InstallSubFolder = "MeshCombinerTool",
                OpenMode    = OpenMode.RuntimeComponent,
                OpenTarget  = "Tools.MeshCombinerTool.MeshCombiner"
            },

            new ToolEntry
            {
                Name        = "Camera Shake",
                Description = "Procedural camera shake MonoBehaviour. Call Shake(duration, magnitude) from any script.",
                Category    = "Runtime Components",
                IconName    = "Camera Icon",
                GitTag      = "tool/camera-shake",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/CameraShakeTool"
                },
                InstallSubFolder = "CameraShakeTool",
                OpenMode    = OpenMode.RuntimeComponent,
                OpenTarget  = "CameraShake"
            },

            new ToolEntry
            {
                Name        = "Draggable Object",
                Description = "MonoBehaviour that makes any GameObject draggable by the mouse in the scene.",
                Category    = "Runtime Components",
                IconName    = "MoveTool",
                GitTag      = "tool/draggable-object",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/DraggableObjectTool"
                },
                InstallSubFolder = "DraggableObjectTool",
                OpenMode    = OpenMode.RuntimeComponent,
                OpenTarget  = "DraggableObject"
            },

            new ToolEntry
            {
                Name        = "Procedural Recoil System",
                Description = "Data-driven procedural recoil manager for weapons. Supports multiple recoil profiles.",
                Category    = "Runtime Components",
                IconName    = "d_PhysicsMaterial2D Icon",
                GitTag      = "tool/procedural-recoil",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/ProceduralRecoilSystem"
                },
                InstallSubFolder = "ProceduralRecoilSystem",
                OpenMode    = OpenMode.RuntimeComponent,
                OpenTarget  = "Tools.ProceduralRecoilSystem.ProceduralRecoilManager"
            },

            new ToolEntry
            {
                Name        = "Procedural Island Generator",
                Description = "Generate randomised island tilemaps procedurally at runtime using noise and seeds.",
                Category    = "Runtime Components",
                IconName    = "Terrain Icon",
                GitTag      = "tool/procedural-island-gen",
                SourcePaths = new[]
                {
                    "Assets/Scripts/Tools/ProceduralIslandGenerationTool"
                },
                InstallSubFolder = "ProceduralIslandGenerationTool",
                OpenMode    = OpenMode.RuntimeComponent,
                OpenTarget  = "ProceduralIslandGenerator"
            },

            new ToolEntry
            {
                Name        = "Create Materials for Textures",
                Description = "ScriptableWizard: bulk-create materials from selected textures using a chosen shader.",
                Category    = "Editor Utilities",
                IconName    = "d_Material Icon",
                GitTag      = "tool/create-materials",
                SourcePaths = new[]
                {
                    "Assets/Editor/Tools/CreateMaterialsForTextures.cs",
                    "Assets/Editor/Tools/CreateMaterialsForTextures.cs.meta"
                },
                InstallSubFolder = "CreateMaterialsForTextures",
                OpenMode    = OpenMode.MenuItem,
                OpenTarget  = "Tools/CreateMaterialsForTextures"
            },
        };
    }
}
