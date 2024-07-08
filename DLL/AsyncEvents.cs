namespace GMSapiInterop;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

static partial class AsyncEvents
{
#pragma warning disable CS8618 // RegisterCallbacks is our init code, kinda
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PerformAsyncEventProc(GMDSMapIdInt map, EventType eventType);
    internal static PerformAsyncEventProc PerformAsyncEvent;
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    internal delegate GMDSMapIdInt CreateDSMapXProc(int n, IntPtr varArgs);
    internal static CreateDSMapXProc CreateDSMapX;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate GMReal SetDSMapRealProc(GMDSMapIdInt map, string key, GMReal value);
    internal static SetDSMapRealProc SetDSMapReal;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate GMReal SetDSMapStringProc(GMDSMapIdInt map, string key, string value);
    internal static SetDSMapStringProc SetDSMapString;
#pragma warning restore CS8618

    [UnmanagedCallersOnly(EntryPoint = "RegisterCallbacks", CallConvs = [typeof(CallConvCdecl)])]
    public static GMReal RegisterCallbacks(
        IntPtr performAsyncEvent, IntPtr createDSMapX,
        IntPtr setDSMapReal, IntPtr setDSMapString
    ) {
        PerformAsyncEvent = Marshal.GetDelegateForFunctionPointer<PerformAsyncEventProc>(performAsyncEvent);
        CreateDSMapX = Marshal.GetDelegateForFunctionPointer<CreateDSMapXProc>(createDSMapX);
        SetDSMapReal = Marshal.GetDelegateForFunctionPointer<SetDSMapRealProc>(setDSMapReal);
        SetDSMapString = Marshal.GetDelegateForFunctionPointer<SetDSMapStringProc>(setDSMapString);

        return GMBool.True;
    }
}

public class DSMap {
    internal readonly GMDSMapIdInt _mapPtr;

    public DSMap() {
        // Apparently that if a previously created map was destroyed,
        // this will take the ID of that
        _mapPtr = AsyncEvents.CreateDSMapX(0, (nint)null);
        DLLMain.DebugPrint($"Created DSMap {_mapPtr}");
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
                throw new ArgumentException(value.ToString(), nameof(value));
            }
        }
    }

    public bool Set(string key, GMReal value) {
        return AsyncEvents.SetDSMapReal(_mapPtr, key, value) == GMBool.True;
    }

    public bool Set(string key, string value) {
        return AsyncEvents.SetDSMapString(_mapPtr, key, value) == GMBool.True;
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
            PerformAsyncEvent(Map, EventType);
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