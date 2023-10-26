using System.Runtime.InteropServices;

public class MessageLoop
{
    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
        uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

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

    private Action InitialAction { get; }
    private Thread? Thread { get; set; }

    public bool IsRunning { get; private set; }

    public MessageLoop(Action initialAction)
    {
        InitialAction = initialAction;
    }

    public void Start()
    {
        IsRunning = true;

        Thread = new Thread(() =>
        {
            InitialAction.Invoke();

            while (IsRunning)
            {
                var result = GetMessage(out var message, IntPtr.Zero, 0, 0);

                if (result <= 0)
                {
                    Stop();

                    continue;
                }

                TranslateMessage(ref message);
                DispatchMessage(ref message);
            }
        });

        Thread.Start();
    }

    public void Stop()
    {
        IsRunning = false;
       
    }
}