using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace DO_NOT_REMEMBER
{
    public static class Detector
    {
        public delegate void InputCallBack(bool button);
        public delegate void BindShortcutCallBack(int idx, bool isAuto);
        public delegate void ChangeShortcutCallBack(int idx);
        public delegate void ExecuteShortcutCallBack(int idx);

        public static InputCallBack inputCallBack;
        public static BindShortcutCallBack bindShortcutCallBack;
        public static ChangeShortcutCallBack changeShortcutCallBack;
        public static ExecuteShortcutCallBack executeShortcutCallBack;

        public static IKeyboardMouseEvents global = Hook.GlobalEvents();

        private static void MouseEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                inputCallBack(false);
            else if (e.Button == MouseButtons.Left)
                inputCallBack(true);
        }

        private static void KeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        changeShortcutCallBack(0);
                        break;
                    case Keys.D2:
                        changeShortcutCallBack(1);
                        break;
                    case Keys.D3:
                        changeShortcutCallBack(2);
                        break;
                    case Keys.D4:
                        changeShortcutCallBack(3);
                        break;
                    case Keys.D5:
                        changeShortcutCallBack(4);
                        break;
                    case Keys.D6:
                        changeShortcutCallBack(5);
                        break;
                    case Keys.D7:
                        changeShortcutCallBack(6);
                        break;
                    case Keys.D8:
                        changeShortcutCallBack(7);
                        break;
                    case Keys.D9:
                        changeShortcutCallBack(8);
                        break;
                    case Keys.D0:
                        changeShortcutCallBack(9);
                        break;
                }
            }
            else if (e.Shift && e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        executeShortcutCallBack(0);
                        break;
                    case Keys.D2:
                        executeShortcutCallBack(1);
                        break;
                    case Keys.D3:
                        executeShortcutCallBack(2);
                        break;
                    case Keys.D4:
                        executeShortcutCallBack(3);
                        break;
                    case Keys.D5:
                        executeShortcutCallBack(4);
                        break;
                    case Keys.D6:
                        executeShortcutCallBack(5);
                        break;
                    case Keys.D7:
                        executeShortcutCallBack(6);
                        break;
                    case Keys.D8:
                        executeShortcutCallBack(7);
                        break;
                    case Keys.D9:
                        executeShortcutCallBack(8);
                        break;
                    case Keys.D0:
                        executeShortcutCallBack(9);
                        break;
                }
            }
            else if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        bindShortcutCallBack(0, false);
                        break;
                    case Keys.D2:
                        bindShortcutCallBack(1, false);
                        break;
                    case Keys.D3:
                        bindShortcutCallBack(2, false);
                        break;
                    case Keys.D4:
                        bindShortcutCallBack(3, false);
                        break;
                    case Keys.D5:
                        bindShortcutCallBack(4, false);
                        break;
                    case Keys.D6:
                        bindShortcutCallBack(5, false);
                        break;
                    case Keys.D7:
                        bindShortcutCallBack(6, false);
                        break;
                    case Keys.D8:
                        bindShortcutCallBack(7, false);
                        break;
                    case Keys.D9:
                        bindShortcutCallBack(8, false);
                        break;
                    case Keys.D0:
                        bindShortcutCallBack(9, false);
                        break;
                }
            }
            else if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        bindShortcutCallBack(0, true);
                        break;
                    case Keys.D2:
                        bindShortcutCallBack(1, true);
                        break;
                    case Keys.D3:
                        bindShortcutCallBack(2, true);
                        break;
                    case Keys.D4:
                        bindShortcutCallBack(3, true);
                        break;
                    case Keys.D5:
                        bindShortcutCallBack(4, true);
                        break;
                    case Keys.D6:
                        bindShortcutCallBack(5, true);
                        break;
                    case Keys.D7:
                        bindShortcutCallBack(6, true);
                        break;
                    case Keys.D8:
                        bindShortcutCallBack(7, true);
                        break;
                    case Keys.D9:
                        bindShortcutCallBack(8, true);
                        break;
                    case Keys.D0:
                        bindShortcutCallBack(9, true);
                        break;
                }
            }
        }
        public static void ListenForEvents()
        {
            global.MouseDown += MouseEvent;
            global.KeyDown += KeyEvent;
        }
    }
}
