# win32

The [win32](https://github.com/sharp-apps/win32) Sharp App contains an examples dashboard of invoking different native Win32 functions:

[![](https://raw.githubusercontent.com/ServiceStack/docs/master/docs/images/release-notes/v5.9/win32.png)](https://github.com/sharp-apps/win32)

You can run this Gist Desktop App via URL Scheme from:

<strong><p><a name="app://win32" href="app://win32">app://win32</a></p></strong>

Or via command-line:

    $ app open win32

### Kiosk Mode

This app starts in full-screen **Kiosk** mode, enabled in its [app.settings](https://github.com/sharp-apps/win32/blob/master/scripts/deploy/app.settings) by:

    CefConfig { Kiosk:true }

The main source code of this component is in [Win32/index.ts](https://github.com/sharp-apps/win32/blob/master/src/components/Win32/index.ts),
which makes use of the built in TypeScript APIs below from `@servicestack/desktop`:

```ts
start('%USERPROFILE%\\\\.sharp-apps')

openUrl('https://google.com')

messageBox('The Title', 'Caption', MessageBoxType.YesNo | MessageBoxType.IconInformation)

await openFile(  {
    title: 'Pick Images',
    filter: "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*",
    initialDir: await expandEnvVars('%USERPROFILE%\\\\Pictures'),
    defaultExt: '*.png',
})

openFile({ isFolderPicker: true })

deviceScreenResolution()

primaryMonitorInfo()

windowSetPosition(x, y)

windowSetSize(width, height)
```

### Custom Win32 API

You're also not limited to calling the built-in Win32 APIs above as calling custom APIs just involves wrapping the C# inside
your preferred [#Script method](/docs/methods) that you would like to make it available to JS as, 
e.g. here's the **win32** implementation for launching [Win32's Color Dialog Box](https://docs.microsoft.com/en-us/windows/win32/dlgbox/color-dialog-box)
and returning the selected color in HTML Color format:

```csharp
public class CustomMethods : ScriptMethods
{
    [DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
    internal static extern int CommDlgExtendedError();

    [DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
    internal static extern bool ChooseColor(ref ChooseColor cc);
    
    private int[] customColors = new int[16] {
        0x00FFFFFF, 0x00C0C0C0, 0x00808080, 0x00000000,
        0x00FF0000, 0x00800000, 0x00FFFF00, 0x00808000,
        0x0000FF00, 0x00008000, 0x0000FFFF, 0x00008080,
        0x000000FF, 0x00000080, 0x00FF00FF, 0x00800080,
    };

    public string chooseColor(ScriptScopeContext scope) => chooseColor(scope, "#ffffff");

    public string chooseColor(ScriptScopeContext scope, string defaultColor) => scope.DoWindow(w => {
        var cc = new ChooseColor();
        cc.lStructSize = Marshal.SizeOf(cc);
        var lpCustColors = Marshal.AllocCoTaskMem(16 * sizeof(int));
        try
        {
            Marshal.Copy(customColors, 0, lpCustColors,16);
            cc.hwndOwner = w;
            cc.lpCustColors = lpCustColors;
            cc.Flags = ChooseColorFlags.FullOpen | ChooseColorFlags.RgbInit;
            var c = ColorTranslator.FromHtml(defaultColor);
            cc.rgbResult = ColorTranslator.ToWin32(c);

            if (!ChooseColor(ref cc)) 
                return (string) null;
        
            c = ColorTranslator.FromWin32(cc.rgbResult);
            return ColorTranslator.ToHtml(c);
        }
        finally
        {
            Marshal.FreeCoTaskMem(lpCustColors);
        }
    });
}
```

ServiceStack.Desktop's IPC takes care of invoking the `#Script` [JS-compatible expression](/docs/expression-viewer) and returning the result:

```ts
var selectedColor = await evaluateCode('chooseColor(`#336699`)')
```

[![](https://raw.githubusercontent.com/ServiceStack/docs/master/docs/images/release-notes/v5.9/win32-choosecolor.png)](https://github.com/sharp-apps/win32)

The `scope.DoWindow()` extension method supports expressions being invoked in-process when launched by `app.exe` as well as when 
invoked during development in "detached mode" if electing to run the .NET Core backend as a stand-alone Web App.

If your App calls your custom APIs a lot you can wrap it in a first-class TypeScript method that mirrors the server #Script method:

```ts
function chooseColor(defaultColor?:string) {
    return defaultColor
        ? evaluateCode(`chooseColor(${quote(defaultColor)})`)
        : evaluateCode(`chooseColor()`);
}
```

Where it can be called using the same syntax in JS and #Script:

```ts
var selectedColor = await chooseColor(`#336699`)
```

## Highly productive live-reloading Development experience

If it weren't for the productivity possible for being able to only needing to develop for Chrome's state-of-the-art rendering engine where you can use advanced features like CSS grid along with the productivity of high-level productive Reactive UI frameworks like Vue, the effort into create a Desktop App like ServiceStack Studio wouldn't be justifiable. 

After the next release we'll create pre-packaged project templates for **vue-desktop** and **react-desktop** Desktop Apps to make it easy develop Vue & React Desktop Apps along with scripts to bundle it & publish it to gist. If preferred `app.exe` also lets you deploy the published app to your own private repo & limit access to only users accessible with a GitHub token which they can open with from a URL with:

    app://user/repo?token={GITHUB_TOKEN}

Or on the command line with:

    $ app user/repo -token $GITHUB_TOKEN

> Or without a token by setting it in the `GITHUB_TOKEN` Environment variable

For offline deployments the published `/dist` folder can be copied and launched with `app` (or `x`) in the app's folder:

    $ app

For better Desktop integration this (or custom command-line arguments) can be wrapped in a new Windows Shortcut:

    $ app shortcut

For a look at example Desktop App projects built using this development model can checkout [ServiceStack/Studio](https://github.com/ServiceStack/Studio) or [NetCoreApps/SharpData](https://github.com/NetCoreApps/SharpData).