// See https://aka.ms/new-console-template for more information

using IDASupport;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

IntPtr _hookID = IntPtr.Zero;

const int WH_KEYBOARD_LL = 13;
const int WM_KEYDOWN = 0x0100;
const int WM_KEYUP = 0x0101;
const int MOUSEEVENTF_LEFTDOWN = 0x02;
const int MOUSEEVENTF_LEFTUP = 0x04;
const int MOUSEEVENTF_MOVE = 0x0001;
var isCtrlPress = false;
var isCKeyPress = false;
var isReset=false;
var classDic=new Dictionary<string, ClassInformation>();
var stop = false;
var path = "ClassDefined.json";
if (File.Exists(path))
{
    classDic=JsonConvert.DeserializeObject<Dictionary<string, ClassInformation>>(File.ReadAllText(path));
    PrintClassStatistic();
}

IntPtr idaProcess = IntPtr.Zero;
Process p = Process.GetProcessesByName("ida64").FirstOrDefault();
if (p != null)
{
    idaProcess = p.MainWindowHandle;
    SetForegroundWindow(idaProcess);
}

LowLevelKeyboardProc _proc = HookCallback;
_hookID = SetHook(_proc);
//var loop = new MessageLoop(() => {
//    _hookID = SetHook(_proc);
//});      
//var t = new Thread(() =>
//{
    var m= GetMessage(out var message, IntPtr.Zero, 0, 0);
//});
//t.Start();
//loop.Start();

Console.Read();    
;
//loop.Stop();  
    
static IntPtr SetHook(LowLevelKeyboardProc proc)
{
    using (Process curProcess = Process.GetCurrentProcess())

    using (ProcessModule curModule = curProcess.MainModule)
    {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
            GetModuleHandle(curModule.ModuleName), 0);
    }
}

IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
{
    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
    {
        int vkCode = Marshal.ReadInt32(lParam);
        switch (vkCode)
        {
            case 162:
                isCtrlPress = true;
                break;
            case 67:

                isCKeyPress = true;
                break;
        }
    }
    if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
    {
        int vkCode = Marshal.ReadInt32(lParam);
        //Console.WriteLine(vkCode);
        switch (vkCode)
        {
            case 162:
                isCtrlPress = false;
                break;
            case 67:
                isCKeyPress = false;
                isReset=true;
                break;
            case 32:
                SendIDADecompiler();
                SendCopy();
                break;
            case 0x53:
                stop = true;
                break;
            
        }
    }

    if (isCtrlPress && isReset)
    {
        isReset=false;
        Thread staThread = new Thread(
            GetClipboard);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();
    }
    return CallNextHookEx(_hookID, nCode, wParam, lParam);
}
void SendKeys(string key)
{
    System.Windows.Forms.SendKeys.SendWait(key);
}
void SendIDADecompiler()
{
    SendKeys("%1");
    Thread.Sleep(100);
    SendKeys("{DOWN}");
    Thread.Sleep(100);
    SendKeys("~");
}
void SendCopy()
{
    // Thread.Sleep(500);
    Thread.Sleep(100);
    SendKeys("%4");

    Thread.Sleep(100);
    SendKeys("^a");
    Thread.Sleep(100);
    SendKeys("^c");
    // Console.WriteLine("Send Copy");
}

void SendIDA(bool nextFunction = true)
{
    if (stop)
    {
        Console.WriteLine("END");
        return;
    }
    SetForegroundWindow(idaProcess);
    var t = new Thread(() =>
    {
        if (nextFunction)
            SendIDADecompiler();
        SendCopy();

    });
    t.Start();
    // t.Join();
}
void GetClipboard()
{
    string clipboardText = Clipboard.GetText(TextDataFormat.Text);
    Clipboard.Clear();
    if (clipboardText.Contains("RowData__OnLanguageSwitch")
        || clipboardText.Contains("RowData___ctor"))
    {
        SendIDA();
        return;
    }
    Console.Clear();
    var classAndFieldName = GetClassInfo(clipboardText);
    var fieldIndex = GetFieldIndex(clipboardText);
    var isTranslateField = GetTranslate(clipboardText);
   // Console.WriteLine(clipboardText);
    Console.WriteLine("====================");
    Console.WriteLine($"class info: {classAndFieldName}");
    Console.WriteLine($"field index: {fieldIndex}");
    Console.WriteLine($"is translate field: {isTranslateField}");
    Console.WriteLine("====================");
    if (fieldIndex != -1)
    {
        if (!classDic.TryGetValue(classAndFieldName.Item2, out var classInfo))
        {
            classInfo = new ClassInformation();
            classDic[classAndFieldName.Item2] = classInfo;
            classInfo.ClassName=classAndFieldName.Item2;
        }

        if (!classInfo.Fields.TryGetValue(classAndFieldName.Item3, out var field))
        {
            field = new ClassFieldInformation()
            {
                FieldName = classAndFieldName.Item3,
                FieldType = classAndFieldName.Item1,
                FileIndex = fieldIndex,
                IsTranslate = isTranslateField
            };
            classInfo.Fields[classAndFieldName.Item3] = field;
        }

        File.WriteAllText(path, JsonConvert.SerializeObject(classDic, Formatting.Indented));
        PrintClassStatistic();
        SendIDA();
    }
    else
    {
        if (string.IsNullOrEmpty(clipboardText))
        {
            DoMouseClick();
            // send decompiler again
            SendIDA(false);
            return;
        }
        Console.WriteLine(clipboardText);
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Parse Error!");
        Console.ForegroundColor = color;
    }
    
}

void PrintClassStatistic()
{
    Console.Title = $"Class: {classDic.Count}";
}

Tuple<string,string,string> GetClassInfo(string str)
{
    var match = Regex.Match(str, @"([a-zA-Z0-9_]+)[.*\s]+__stdcall ([a-zA-Z_]+)_RowData__get_([a-zA-Z0-9_]+)");
    if (match.Success)
    {
        return new Tuple<string, string, string>(
            match.Groups[1].Value
                .Replace("_o","")
                .Replace("_t","")
                .Replace("_","."), 
            match.Groups[2].Value.Replace("_","."), 
            match.Groups[3].Value
            );
    }

    return null;
}

int GetFieldIndex(string str)
{
    var match = Regex.Match(str, @"RowDataPtr_k__BackingField,[\r\n\s]+([a-zA-Z0-9]+),");
    if (match.Success)
    {
        var val = match.Groups[1].Value;
        val = val.Replace("u", "");
        if (val.StartsWith("0x"))
        {
            return Convert.ToInt32(val, 16);
        }

        return int.Parse(val);
    }
    return -1;
}

bool GetTranslate(string str)
{
    var match = Regex.Match(str, @"MoonClient_StringPoolManager__GetString\(\(MoonClient_StringPoolManager_o \*\)singleton, ValueUInt");
    return match.Success;
}
void DoMouseClick()
{
    
    //perform click            
    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
}

[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]

static extern bool UnhookWindowsHookEx(IntPtr hhk);

[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

static extern IntPtr GetModuleHandle(string lpModuleName);

[DllImport("user32.dll")]
static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
    uint wMsgFilterMax);

[DllImport("user32.dll")]
static extern bool TranslateMessage([In] ref MSG lpMsg);

[DllImport("user32.dll")]
static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
[DllImport("User32.dll")]
static extern int SetForegroundWindow(IntPtr point);

[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
[StructLayout(LayoutKind.Sequential)]
public struct MSG
{
    IntPtr hwnd;
    uint message;
    UIntPtr wParam;
    IntPtr lParam;
    int time;
    POINT pt;
    int lPrivate;
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator System.Drawing.Point(POINT p)
    {
        return new System.Drawing.Point(p.X, p.Y);
    }

    public static implicit operator POINT(System.Drawing.Point p)
    {
        return new POINT(p.X, p.Y);
    }

    public override string ToString()
    {
        return $"X: {X}, Y: {Y}";
    }
}