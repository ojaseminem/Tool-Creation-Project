# AutoTools Hub - TurtleGameWorks

A unified hub for all your custom Unity editor tools.  
Open it via: **Tools > TurtleGameWorks > AutoTools Hub**

---

## Installing into a New Project via GitHub (UPM)

1. Open **Window > Package Manager**
2. Click the **+** button → **Add package from Git URL**
3. Paste:
   ```
   https://github.com/YOUR_USERNAME/AutoTools.git#v1.0.0
   ```
4. The Hub will auto-open on first install. Done!

To always have it in new projects, add it to your **Unity project template**.

---

## Adding a New Tool to the Hub

Create a class implementing `IAutoTool` anywhere in an `Editor` folder:

```csharp
using TurtleGameWorks.AutoTools;
using UnityEditor;

public class MyNewToolAdapter : IAutoTool
{
    public string ToolName    => "My New Tool";
    public string Description => "What this tool does.";
    public string Category    => "Utilities";          // Groups tools in the sidebar
    public string IconName    => "cs Script Icon";     // Any Unity built-in icon name

    public void Open() => EditorWindow.GetWindow<MyNewToolWindow>().Show();
}
```

That's it — **no registration needed**. The Hub auto-discovers it via reflection.

---

## Releasing a New Version to GitHub

```bash
git add .
git commit -m "Add MyNewTool"
git tag v1.1.0
git push origin main --tags
```

Users update by changing the Git URL tag in Package Manager:
```
https://github.com/YOUR_USERNAME/AutoTools.git#v1.1.0
```

---

## Categories Used

| Category       | Tools                        |
|----------------|------------------------------|
| Scene          | Scene Manager                |
| Utilities      | Screenshot, Remove Missing Scripts |
| Project Setup  | Create Folders               |

Add more by setting `Category` in your `IAutoTool` implementation.
