using System.Runtime.InteropServices;
using Hexa.NET.ImGui;

public static class InputUtils
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static bool IsKeyDown(ImGuiKey key)
    {
        int vKey = 0;

        // Маппинг основных клавиш (добавьте другие по необходимости)
        switch (key)
        {
            case ImGuiKey.None: return false;
            case ImGuiKey.Tab: vKey = 0x09; break;
            case ImGuiKey.LeftArrow: vKey = 0x25; break;
            case ImGuiKey.RightArrow: vKey = 0x27; break;
            case ImGuiKey.UpArrow: vKey = 0x26; break;
            case ImGuiKey.DownArrow: vKey = 0x28; break;
            case ImGuiKey.PageUp: vKey = 0x21; break;
            case ImGuiKey.PageDown: vKey = 0x22; break;
            case ImGuiKey.Home: vKey = 0x24; break;
            case ImGuiKey.End: vKey = 0x23; break;
            case ImGuiKey.Insert: vKey = 0x2D; break;
            case ImGuiKey.Delete: vKey = 0x2E; break;
            case ImGuiKey.Backspace: vKey = 0x08; break;
            case ImGuiKey.Space: vKey = 0x20; break;
            case ImGuiKey.Enter: vKey = 0x0D; break;
            case ImGuiKey.Escape: vKey = 0x1B; break;
            case ImGuiKey.LeftCtrl: vKey = 0xA2; break;
            case ImGuiKey.LeftShift: vKey = 0xA0; break;
            case ImGuiKey.LeftAlt: vKey = 0xA4; break;
            case ImGuiKey.MouseLeft: vKey = 0x01; break;
            case ImGuiKey.MouseRight: vKey = 0x02; break;
            case ImGuiKey.MouseMiddle: vKey = 0x04; break;
            case ImGuiKey.MouseX1: vKey = 0x05; break; // Боковая кнопка мыши 1
            case ImGuiKey.MouseX2: vKey = 0x06; break; // Боковая кнопка мыши 2
            
            // Буквы A-Z (в WinAPI совпадают с ASCII)
            default:
                if (key >= ImGuiKey.A && key <= ImGuiKey.Z)
                {
                    vKey = (int)key - (int)ImGuiKey.A + 0x41;
                }
                else if (key >= ImGuiKey.Key0 && key <= ImGuiKey.Key9)
                {
                    vKey = (int)key - (int)ImGuiKey.Key0 + 0x30;
                }
                break;
        }

        if (vKey == 0) return false;
        
        // Проверяем старший бит (клавиша зажата в данный момент)
        return (GetAsyncKeyState(vKey) & 0x8000) != 0;
    }
}