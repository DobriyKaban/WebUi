using System;
using System.Reflection;

namespace WebUi
{
    public static class Log
    {
        private static object? _sawmill;
        private static MethodInfo? _infoMethod;
        private static MethodInfo? _warningMethod;
        private static MethodInfo? _errorMethod;
        private static MethodInfo? _debugMethod;
        
        static Log()
        {
            try
            {
                // Пытаемся найти логгер игры (Robust.Shared.Log.Logger)
                var loggerType = Type.GetType("Robust.Shared.Log.Logger, Robust.Shared");
                if (loggerType != null)
                {
                    var getSawmillMethod = loggerType.GetMethod("GetSawmill", new[] { typeof(string) });
                    if (getSawmillMethod != null)
                    {
                        // Создаем Sawmill с именем "WebUI"
                        _sawmill = getSawmillMethod.Invoke(null, new object[] { "WebUI" });
                        
                        if (_sawmill != null)
                        {
                            var sawmillType = _sawmill.GetType();
                            _infoMethod = sawmillType.GetMethod("Info", new[] { typeof(string) });
                            _warningMethod = sawmillType.GetMethod("Warning", new[] { typeof(string) });
                            _errorMethod = sawmillType.GetMethod("Error", new[] { typeof(string) });
                            _debugMethod = sawmillType.GetMethod("Debug", new[] { typeof(string) });
                        }
                    }
                }
            }
            catch
            {
                _sawmill = null;
            }
        }
        
        public static void Info(string message)
        {
            if (_sawmill != null && _infoMethod != null)
                _infoMethod.Invoke(_sawmill, new object[] { message });
            else
                Console.WriteLine($"[WebUI][INFO] {message}");
        }

        public static void Warn(string message)
        {
            if (_sawmill != null && _warningMethod != null)
                _warningMethod.Invoke(_sawmill, new object[] { message });
            else
                Console.WriteLine($"[WebUI][WARN] {message}");
        }

        public static void Error(string message)
        {
            if (_sawmill != null && _errorMethod != null)
                _errorMethod.Invoke(_sawmill, new object[] { message });
            else
                Console.WriteLine($"[WebUI][ERROR] {message}");
        }

        public static void Debug(string message)
        {
            if (_sawmill != null && _debugMethod != null)
                _debugMethod.Invoke(_sawmill, new object[] { message });
            else
                Console.WriteLine($"[WebUI][DEBUG] {message}");
        }
    }
}