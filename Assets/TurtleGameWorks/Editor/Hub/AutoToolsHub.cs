using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.AutoTools
{
    /// <summary>
    /// AutoTools Hub — TurtleGameWorks
    /// Two tabs:
    ///   • Setup  — install / remove individual tools from the remote repo
    ///   • Tools  — launch installed tools
    ///
    /// Open via: Tools > TurtleGameWorks > AutoTools Hub
    /// </summary>
    public class AutoToolsHub : EditorWindow
    {
        // ── Layout ───────────────────────────────────────────────────────────
        private const float SidebarWidth   = 168f;
        private const float CardHeight     = 84f;
        private const float HeaderHeight   = 58f;
        private const float TabBarHeight   = 34f;

        // ── Tab ──────────────────────────────────────────────────────────────
        private enum Tab { Setup, Tools }
        private Tab _activeTab = Tab.Setup;

        // ── Data ─────────────────────────────────────────────────────────────
        private List<ToolEntry>  _allTools;
        private List<string>     _categories;
        private string           _selectedCategory = "All";
        private string           _searchQuery      = "";
        private Vector2          _scrollPos;

        // ── Install state ─────────────────────────────────────────────────────
        // Per-tool: null = idle, string = in-progress message
        private Dictionary<string, string>  _progressMessages = new Dictionary<string, string>();
        private Dictionary<string, bool>    _operationFailed  = new Dictionary<string, bool>();

        // ── Styles ───────────────────────────────────────────────────────────
        private bool     _stylesReady;
        private GUIStyle _headerLabel;
        private GUIStyle _subLabel;
        private GUIStyle _tabActive;
        private GUIStyle _tabInactive;
        private GUIStyle _categoryBtn;
        private GUIStyle _categoryBtnSel;
        private GUIStyle _toolName;
        private GUIStyle _toolDesc;
        private GUIStyle _tagLabel;
        private GUIStyle _actionBtn;
        private GUIStyle _removeBtn;
        private GUIStyle _openBtn;
        private GUIStyle _runtimeNote;

        // ── Palette ───────────────────────────────────────────────────────────
        private static readonly Color Accent     = new Color(0.18f, 0.62f, 0.85f);
        private static readonly Color AccentDim  = new Color(0.18f, 0.62f, 0.85f, 0.18f);
        private static readonly Color SidebarBg  = new Color(0.14f, 0.14f, 0.14f);
        private static readonly Color CardBg     = new Color(0.21f, 0.21f, 0.21f);
        private static readonly Color HeaderBg   = new Color(0.12f, 0.12f, 0.12f);
        private static readonly Color TagColor   = new Color(0.55f, 0.55f, 0.55f);
        private static readonly Color Green      = new Color(0.28f, 0.76f, 0.43f);
        private static readonly Color Red        = new Color(0.85f, 0.30f, 0.30f);

        // ─────────────────────────────────────────────────────────────────────
        [MenuItem("Tools/TurtleGameWorks/AutoTools Hub", priority = 0)]
        public static void ShowWindow()
        {
            var w = GetWindow<AutoToolsHub>("AutoTools Hub");
            w.minSize = new Vector2(700, 460);
            w.Show();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            _allTools   = ToolManifest.All;
            _categories = new List<string> { "All" };
            _categories.AddRange(_allTools.Select(t => t.Category).Distinct().OrderBy(c => c));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Style init
        // ─────────────────────────────────────────────────────────────────────
        private void InitStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _headerLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18, alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };
            _subLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10, normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };
            _tabActive = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 12, fontStyle = FontStyle.Bold,
                fixedHeight = TabBarHeight,
                normal = { textColor = Accent }
            };
            _tabInactive = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 12, fixedHeight = TabBarHeight,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };
            _categoryBtn = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12, alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(12, 8, 3, 3),
                normal = { textColor = new Color(0.72f, 0.72f, 0.72f) }
            };
            _categoryBtnSel = new GUIStyle(_categoryBtn)
            {
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Accent }
            };
            _toolName = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13, normal = { textColor = Color.white }
            };
            _toolDesc = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11, wordWrap = true,
                normal = { textColor = new Color(0.70f, 0.70f, 0.70f) }
            };
            _tagLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = TagColor }
            };
            _actionBtn = new GUIStyle("miniButton")
            {
                fontSize = 11, fontStyle = FontStyle.Bold,
                fixedWidth = 76, fixedHeight = 24,
                alignment = TextAnchor.MiddleCenter
            };
            _removeBtn = new GUIStyle(_actionBtn);
            _openBtn   = new GUIStyle(_actionBtn) { fixedWidth = 60 };
            _runtimeNote = new GUIStyle(EditorStyles.miniLabel)
            {
                fontStyle = FontStyle.Italic,
                normal    = { textColor = new Color(0.55f, 0.55f, 0.55f) }
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        //  OnGUI
        // ─────────────────────────────────────────────────────────────────────
        private void OnGUI()
        {
            InitStyles();
            DrawHeader();
            DrawTabBar();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSidebar();
                if (_activeTab == Tab.Setup) DrawSetupPanel();
                else                         DrawToolsPanel();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Header
        // ─────────────────────────────────────────────────────────────────────
        private void DrawHeader()
        {
            var r = EditorGUILayout.BeginHorizontal(GUILayout.Height(HeaderHeight));
            EditorGUI.DrawRect(r, HeaderBg);

            GUILayout.Space(14);
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("⚙  AutoTools Hub", _headerLabel);
                GUILayout.Label("TurtleGameWorks · tag-based tool installer", _subLabel);
                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();

            // Search (only in Setup/Tools panels)
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true), GUILayout.Width(200)))
            {
                GUILayout.FlexibleSpace();
                _searchQuery = EditorGUILayout.TextField(_searchQuery,
                    EditorStyles.toolbarSearchField, GUILayout.Width(192));
                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true), GUILayout.Width(28)))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"),
                        GUILayout.Width(26), GUILayout.Height(26)))
                    Refresh();
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();

            // Accent rule
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(position.width, 2), Accent);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Tab bar
        // ─────────────────────────────────────────────────────────────────────
        private void DrawTabBar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar,
                       GUILayout.Height(TabBarHeight)))
            {
                if (GUILayout.Button("⬇  Setup  (Install / Remove)",
                        _activeTab == Tab.Setup ? _tabActive : _tabInactive, GUILayout.Width(220)))
                    _activeTab = Tab.Setup;

                if (GUILayout.Button("🛠  Tools  (Launch)",
                        _activeTab == Tab.Tools ? _tabActive : _tabInactive, GUILayout.Width(180)))
                    _activeTab = Tab.Tools;

                GUILayout.FlexibleSpace();

                // Installed count badge
                int installedCount = _allTools.Count(t => ToolInstaller.IsInstalled(t));
                GUILayout.Label($"{installedCount} / {_allTools.Count} installed",
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = installedCount > 0 ? Green : TagColor },
                        alignment = TextAnchor.MiddleRight
                    }, GUILayout.Width(120));
                GUILayout.Space(8);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Sidebar (shared)
        // ─────────────────────────────────────────────────────────────────────
        private void DrawSidebar()
        {
            var r = EditorGUILayout.BeginVertical(
                GUILayout.Width(SidebarWidth), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(r, SidebarBg);

            GUILayout.Space(8);
            GUILayout.Label("  CATEGORIES",
                new GUIStyle(EditorStyles.miniLabel)
                { normal = { textColor = new Color(0.45f, 0.45f, 0.45f) } });
            GUILayout.Space(4);

            foreach (var cat in _categories)
            {
                bool sel  = _selectedCategory == cat;
                var  item = EditorGUILayout.BeginHorizontal(GUILayout.Height(26));
                if (sel) EditorGUI.DrawRect(item, AccentDim);

                if (GUILayout.Button(cat, sel ? _categoryBtnSel : _categoryBtn,
                        GUILayout.Height(26), GUILayout.ExpandWidth(true)))
                    _selectedCategory = cat;

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            int vis = GetFiltered().Count;
            GUILayout.Label($"  {vis} tool{(vis != 1 ? "s" : "")}",
                new GUIStyle(EditorStyles.miniLabel)
                { normal = { textColor = new Color(0.45f, 0.45f, 0.45f) } });
            GUILayout.Space(8);
            EditorGUILayout.EndVertical();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  SETUP PANEL
        // ─────────────────────────────────────────────────────────────────────
        private void DrawSetupPanel()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Space(8);

            var filtered = GetFiltered();
            if (filtered.Count == 0)
            {
                GUILayout.Space(60);
                GUILayout.Label("No tools match.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                string lastCat = null;
                foreach (var tool in filtered)
                {
                    if (_selectedCategory == "All" && tool.Category != lastCat)
                    {
                        lastCat = tool.Category;
                        GUILayout.Space(4);
                        GUILayout.Label(tool.Category.ToUpper(),
                            new GUIStyle(EditorStyles.miniLabel)
                            { normal = { textColor = Accent }, fontStyle = FontStyle.Bold });
                        EditorGUI.DrawRect(GUILayoutUtility.GetRect(
                            position.width - SidebarWidth - 16, 1), new Color(0.32f, 0.32f, 0.32f));
                        GUILayout.Space(4);
                    }
                    DrawSetupCard(tool);
                }
            }

            GUILayout.Space(12);
            EditorGUILayout.EndScrollView();
        }

        private void DrawSetupCard(ToolEntry tool)
        {
            bool installed = ToolInstaller.IsInstalled(tool);
            _progressMessages.TryGetValue(tool.GitTag, out string progressMsg);
            bool busy = progressMsg != null;

            var cardR = EditorGUILayout.BeginHorizontal(GUILayout.Height(CardHeight));
            EditorGUI.DrawRect(cardR, CardBg);

            // Installed indicator strip (left edge)
            EditorGUI.DrawRect(new Rect(cardR.x, cardR.y, 3, cardR.height),
                installed ? Green : new Color(0.3f, 0.3f, 0.3f));

            GUILayout.Space(14);

            // Icon
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(34), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();
                var icon = string.IsNullOrEmpty(tool.IconName)
                    ? EditorGUIUtility.IconContent("cs Script Icon")
                    : EditorGUIUtility.IconContent(tool.IconName);
                GUILayout.Label(icon, GUILayout.Width(30), GUILayout.Height(30));
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(8);

            // Text block
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(tool.Name, _toolName);
                GUILayout.Label(tool.Description, _toolDesc);
                GUILayout.Label($"tag: {tool.GitTag}  ·  {tool.OpenMode}", _tagLabel);
                GUILayout.FlexibleSpace();
            }

            // Action buttons
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(90), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();

                if (busy)
                {
                    GUILayout.Label(progressMsg, new GUIStyle(EditorStyles.miniLabel)
                    { wordWrap = true, normal = { textColor = Accent } }, GUILayout.Width(84));
                }
                else if (installed)
                {
                    // Remove button
                    GUI.backgroundColor = Red;
                    if (GUILayout.Button("Remove", _removeBtn))
                    {
                        if (EditorUtility.DisplayDialog("Remove Tool",
                                $"Remove '{tool.Name}' from this project?\n\nFiles will be deleted from {ToolInstaller.InstallRoot}/{tool.InstallSubFolder}",
                                "Remove", "Cancel"))
                        {
                            ToolInstaller.Remove(tool, (ok, msg) =>
                            {
                                _operationFailed[tool.GitTag] = !ok;
                                EditorUtility.DisplayDialog(ok ? "Done" : "Error", msg, "OK");
                                Repaint();
                            });
                        }
                    }
                    GUI.backgroundColor = Color.white;

                    // Status dot
                    GUILayout.Label("✔ Installed", new GUIStyle(EditorStyles.miniLabel)
                    { normal = { textColor = Green }, alignment = TextAnchor.MiddleCenter });
                }
                else
                {
                    // Install button
                    GUI.backgroundColor = Accent;
                    if (GUILayout.Button("Install", _actionBtn))
                    {
                        _progressMessages[tool.GitTag] = "Starting…";
                        _operationFailed[tool.GitTag]  = false;
                        Repaint();

                        // Run on a background thread so the editor doesn't freeze
                        var capturedTool = tool;
                        System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                        {
                            ToolInstaller.Install(capturedTool,
                                onProgress: msg =>
                                {
                                    _progressMessages[capturedTool.GitTag] = msg;
                                    // Trigger repaint from background thread
                                    EditorApplication.delayCall += Repaint;
                                },
                                onDone: (ok, msg) =>
                                {
                                    _progressMessages.Remove(capturedTool.GitTag);
                                    _operationFailed[capturedTool.GitTag] = !ok;
                                    EditorApplication.delayCall += () =>
                                    {
                                        EditorUtility.DisplayDialog(ok ? "Installed!" : "Error", msg, "OK");
                                        Repaint();
                                    };
                                });
                        });
                    }
                    GUI.backgroundColor = Color.white;
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  TOOLS PANEL (launch installed tools)
        // ─────────────────────────────────────────────────────────────────────
        private void DrawToolsPanel()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Space(8);

            var filtered  = GetFiltered().Where(t => ToolInstaller.IsInstalled(t)).ToList();

            if (filtered.Count == 0)
            {
                GUILayout.Space(60);
                GUILayout.Label(
                    _allTools.Any(t => ToolInstaller.IsInstalled(t))
                        ? "No installed tools match your search / category."
                        : "No tools installed yet.\nSwitch to the Setup tab to install tools.",
                    new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 12, wordWrap = true });
            }
            else
            {
                string lastCat = null;
                foreach (var tool in filtered)
                {
                    if (_selectedCategory == "All" && tool.Category != lastCat)
                    {
                        lastCat = tool.Category;
                        GUILayout.Space(4);
                        GUILayout.Label(tool.Category.ToUpper(),
                            new GUIStyle(EditorStyles.miniLabel)
                            { normal = { textColor = Accent }, fontStyle = FontStyle.Bold });
                        EditorGUI.DrawRect(GUILayoutUtility.GetRect(
                            position.width - SidebarWidth - 16, 1), new Color(0.32f, 0.32f, 0.32f));
                        GUILayout.Space(4);
                    }
                    DrawToolCard(tool);
                }
            }

            GUILayout.Space(12);
            EditorGUILayout.EndScrollView();
        }

        private void DrawToolCard(ToolEntry tool)
        {
            var cardR = EditorGUILayout.BeginHorizontal(GUILayout.Height(CardHeight));
            EditorGUI.DrawRect(cardR, CardBg);
            EditorGUI.DrawRect(new Rect(cardR.x, cardR.y, 3, cardR.height), Green);

            GUILayout.Space(14);

            // Icon
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(34), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();
                var icon = string.IsNullOrEmpty(tool.IconName)
                    ? EditorGUIUtility.IconContent("cs Script Icon")
                    : EditorGUIUtility.IconContent(tool.IconName);
                GUILayout.Label(icon, GUILayout.Width(30), GUILayout.Height(30));
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(8);

            // Text
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(tool.Name, _toolName);
                GUILayout.Label(tool.Description, _toolDesc);
                GUILayout.FlexibleSpace();
            }

            // Open / note
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(90), GUILayout.Height(CardHeight)))
            {
                GUILayout.FlexibleSpace();

                if (tool.OpenMode == OpenMode.RuntimeComponent)
                {
                    GUILayout.Label("Runtime\nComponent", _runtimeNote, GUILayout.Width(84));
                }
                else
                {
                    GUI.backgroundColor = Accent;
                    if (GUILayout.Button("Open", _openBtn))
                        OpenTool(tool);
                    GUI.backgroundColor = Color.white;
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Open dispatch
        // ─────────────────────────────────────────────────────────────────────
        private static void OpenTool(ToolEntry tool)
        {
            switch (tool.OpenMode)
            {
                case OpenMode.EditorWindow:
                {
                    var type = System.Type.GetType(tool.OpenTarget) ??
                               FindTypeAcrossAssemblies(tool.OpenTarget);
                    if (type != null)
                        GetWindow(type).Show();
                    else
                        Debug.LogError($"[AutoTools] Could not find type '{tool.OpenTarget}'. Is the tool installed?");
                    break;
                }
                case OpenMode.MenuItem:
                    EditorApplication.ExecuteMenuItem(tool.OpenTarget);
                    break;

                case OpenMode.SettingsProvider:
                    SettingsService.OpenProjectSettings(tool.OpenTarget);
                    break;

                case OpenMode.RuntimeComponent:
                    // Nothing to open — show a hint
                    EditorUtility.DisplayDialog(tool.Name,
                        $"This is a runtime MonoBehaviour component.\n\n" +
                        $"Add it to a GameObject in your scene:\n'{tool.OpenTarget}'",
                        "OK");
                    break;
            }
        }

        private static System.Type FindTypeAcrossAssemblies(string typeName)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(typeName);
                if (t != null) return t;
            }
            return null;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Filter
        // ─────────────────────────────────────────────────────────────────────
        private List<ToolEntry> GetFiltered()
        {
            var q = _searchQuery?.Trim().ToLowerInvariant() ?? "";
            return _allTools.Where(t =>
            {
                bool catOk    = _selectedCategory == "All" || t.Category == _selectedCategory;
                bool searchOk = string.IsNullOrEmpty(q)
                    || t.Name.ToLower().Contains(q)
                    || t.Description.ToLower().Contains(q)
                    || t.Category.ToLower().Contains(q)
                    || t.GitTag.ToLower().Contains(q);
                return catOk && searchOk;
            }).ToList();
        }
    }
}
