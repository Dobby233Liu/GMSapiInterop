namespace GMSapiInterop;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

partial class DLLMain
{
    private static bool _enableDebugOutput = false;

    private static void DebugPrint(string message) {
        if (_enableDebugOutput) {
            Console.WriteLine(message);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_set_debug", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SetDebugOutputEnabled(GMBoolType enable) {
        // FIXME: do we care about the lock here
        _enableDebugOutput = enable == GMBool.True;
        return GMBool.True;
    }

    [UnmanagedCallersOnly(EntryPoint = "_sapi_cleanup", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType Cleanup() {
        return (CleanupSynthesis()) ? GMBool.True : GMBool.False;
    }
}