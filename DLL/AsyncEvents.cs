namespace GMSapiInterop;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

static partial class AsyncEvents
{
#pragma warning disable CS8618 // RegisterCallbacks is our init code, kinda
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PerformAsyncEvent(GMDSMapIdInt map, EventType eventType);
    internal static PerformAsyncEvent _performAsyncEvent;
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    internal delegate GMDSMapIdInt CreateDSMapX(int n);
    internal static CreateDSMapX _createDSMapX;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate GMReal SetDSMapReal(GMDSMapIdInt map, string key, GMReal value);
    internal static SetDSMapReal _setDSMapReal;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate GMReal SetDSMapString(GMDSMapIdInt map, string key, string value);
    internal static SetDSMapString _setDSMapString;
#pragma warning restore CS8618

    [UnmanagedCallersOnly(EntryPoint = "RegisterCallbacks", CallConvs = [typeof(CallConvCdecl)])]
    public static GMReal RegisterCallbacks(
        IntPtr performAsyncEvent, IntPtr createDSMapX,
        IntPtr setDSMapReal, IntPtr setDSMapString
    ) {
        _performAsyncEvent = Marshal.GetDelegateForFunctionPointer<PerformAsyncEvent>(performAsyncEvent);
        _createDSMapX = Marshal.GetDelegateForFunctionPointer<CreateDSMapX>(createDSMapX);
        _setDSMapReal = Marshal.GetDelegateForFunctionPointer<SetDSMapReal>(setDSMapReal);
        _setDSMapString = Marshal.GetDelegateForFunctionPointer<SetDSMapString>(setDSMapString);

        return GMBool.True;
    }
}

public class DSMap {
    internal readonly GMDSMapIdInt _mapPtr;

    public DSMap() {
        _mapPtr = AsyncEvents._createDSMapX(0);
    }

    public DSMap(GMDSMapIdInt map) {
        _mapPtr = map;
    }

    public static implicit operator GMDSMapIdInt(DSMap map) {
        return map._mapPtr;
    }

    public object this[string key] {
        set {
            if (value is double real) {
                Set(key, real);
            } else if (value is DSMap map) {
                Set(key, map);
            } else if (value is bool b) {
                Set(key, b ? GMBool.True : GMBool.False);
            } else if (value is string str) {
                Set(key, str);
            } else if (value is null) {
                Set(key, "");
            } else {
                throw new InvalidOperationException(value.ToString());
            }
        }
    }

    public bool Set(string key, GMReal value) {
        return AsyncEvents._setDSMapReal(_mapPtr, key, value) == GMBool.True;
    }

    public bool Set(string key, string value) {
        return AsyncEvents._setDSMapString(_mapPtr, key, value) == GMBool.True;
    }
}

static partial class AsyncEvents
{
    public enum EventType : int {
        System = 75
    }

    public class AsyncEvent(EventType eventType)
    {
        public EventType EventType = eventType;
        public readonly DSMap Map = new();

        public object this[string key] {
            set {
                Map[key] = value;
            }
        }

        public void Perform() {
            _performAsyncEvent(Map, EventType);
        }
    }
}

public static class DSMapFromIDictionaryExtension
{
    public static DSMap ToDSMap<T>(this IDictionary<string, T> dict) {
        DSMap map = new();
        foreach (KeyValuePair<string, T> pair in dict) {
            // We do handle value being null
#pragma warning disable CS8601
            map[pair.Key] = pair.Value;
#pragma warning restore CS8601
        }
        return map;
    }
}