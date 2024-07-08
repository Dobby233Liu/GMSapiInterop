namespace GMSapiInterop;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Linq;
using System.Globalization;

using SynthId = ObjectId;
// <remarks>
// <see cref="System.Speech.Synthesis.SynthesizerState" />
// </remarks>
using SynthState = GMReal;
// <summary>(age << 8) + gender</summary>
// <remarks>
// <see cref="System.Speech.Synthesis.VoiceGender" />
// <see cref="System.Speech.Synthesis.VoiceAge" />
// </remarks>
using GenderAndAge = GMReal;
// <remarks>
// <see cref="System.Speech.Synthesis.SynthesisTextFormat" />
// </remarks>
using PromptType = GMReal;

partial class DLLMain
{
    private static bool CleanupSynthesis() {
        lock (_synthesizersLock) {
            foreach (SpeechSynthesizer? synthesizer in _synthesizers) {
                synthesizer?.Dispose();
            }
            _synthesizers.Clear();

            if (_voiceEnumerator is not null)
                _voiceEnumerator.Dispose();
            _voiceEnumerator = null;
        }
        return true;
    }

    private readonly static List<SpeechSynthesizer?> _synthesizers = [];
    private readonly static object _synthesizersLock = new();


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_create", CallConvs = [typeof(CallConvCdecl)])]
    public static SynthId SynthesizerCreate() {
        lock (_synthesizersLock) {
            SpeechSynthesizer synthesizer = new();
            // TODO: events
            synthesizer.SetOutputToDefaultAudioDevice();
            _synthesizers.Add(synthesizer);

            return (SynthId)_synthesizers.Count - 1;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_exists", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerExists(SynthId id) {
        lock (_synthesizersLock) {
            return id >= 0 && _synthesizers.Count < (id + 1)
                && _synthesizers.ElementAtOrDefault(Convert.ToInt32(id)) is not null
                ? GMBool.False
                : GMBool.True;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_free", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerFree(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));

            if (synthesizer is not null) {
                // it's over...
                synthesizer.SpeakAsyncCancelAll();
                synthesizer.Dispose();
                synthesizer = null;
                
                return GMBool.True;
            }
            return GMBool.False;
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_volume", CallConvs = [typeof(CallConvCdecl)])]
    public static GMReal SynthesizerGetVolume(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return -1;
            return synthesizer.Volume;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_set_volume", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerSetVolume(SynthId id, GMReal volume) {
        lock (_synthesizersLock) {
            if (volume < 0 || volume > 100)
                return GMBool.False;

            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;

            synthesizer.Volume = Convert.ToInt32(volume);
            return GMBool.True;
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_rate", CallConvs = [typeof(CallConvCdecl)])]
    public static GMReal SynthesizerGetRate(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return -1;
            return synthesizer.Rate;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_set_rate", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerSetRate(SynthId id, GMReal rate) {
        lock (_synthesizersLock) {
            if (rate < -10 || rate > 10)
                return GMBool.False;

            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;

            synthesizer.Rate = Convert.ToInt32(rate);
            return GMBool.True;
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_state", CallConvs = [typeof(CallConvCdecl)])]
    public static SynthState SynthesizerGetState(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return -1;
            // TODO: enum
            return (double)synthesizer.State;
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_pause", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerPause(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;
            synthesizer.Pause();
            return GMBool.True;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_resume", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerResume(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;
            synthesizer.Resume();
            return GMBool.True;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_stop", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerStop(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.Resume();
            return GMBool.True;
        }
    }

    private static IEnumerator<InstalledVoice>? _voiceEnumerator;

    private static DSMap VoiceToDSMap(VoiceInfo voice) {
        DSMap map = new();
        map["id"] = voice.Id;
        map["name"] = voice.Name;
        map["gender"] = (double)(int)voice.Gender;
        map["age"] = (double)(int)voice.Age;
        map["culture"] = voice.Culture.Name;
        map["description"] = voice.Description;
        // you won't want to manually destroy another ds map
        //map["extra"] = voice.VoiceInfo.AdditionalInfo.ToDSMap();
        DebugPrint("dsmap id is " + ((double)map).ToString());
        return map;
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_voice", CallConvs = [typeof(CallConvCdecl)])]
    public static ObjectId SynthesizerGetVoice(SynthId id) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return -1;

            DebugPrint(synthesizer.Voice.Name);
            return VoiceToDSMap(synthesizer.Voice);
        }
    }

    private static ObjectId GetNextVoice() {
        if (_voiceEnumerator is null) {
            DebugPrint("GetNextVoice was called while an enumeration was not initialized");
            return -1;
        }

        try {
            if (_voiceEnumerator.MoveNext()) {
                InstalledVoice voice = _voiceEnumerator.Current;
                DebugPrint("next voice " + voice.VoiceInfo.Id);

                DSMap map = VoiceToDSMap(voice.VoiceInfo);
                map["enabled"] = voice.Enabled;
                return map;
            } else {
                DebugPrint("End of voice list");
                return -1;
            }
        } catch (Exception e) {
            DebugPrint("GetNextVoice failed");
            DebugPrint(e.Message ?? "");
            DebugPrint(e.StackTrace ?? "");
            return -1;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_voices_first", CallConvs = [typeof(CallConvCdecl)])]
    public static ObjectId SynthesizerGetVoicesFirst(SynthId id) {
        lock (_synthesizersLock) {
            if (_voiceEnumerator is not null) {
                DebugPrint("The voice list is already being enumerated");
                return -1;
            }

            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return -1;

            _voiceEnumerator = synthesizer.GetInstalledVoices().GetEnumerator();
            DebugPrint("Got enumerator");
            return GetNextVoice();
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_voices_next", CallConvs = [typeof(CallConvCdecl)])]
    public static ObjectId SynthesizerGetVoicesNext() {
        DebugPrint("get_voices_next called");
        lock (_synthesizersLock) {
            if (_voiceEnumerator is null) {
                DebugPrint("The voice list is not being enumerated");
                return -1;
            }

            DebugPrint("NEXT!");
            return GetNextVoice();
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_get_voices_close", CallConvs = [typeof(CallConvCdecl)])]
    public static ObjectId SynthesizerGetVoicesClose() {
        lock (_synthesizersLock) {
            if (_voiceEnumerator is null) {
                DebugPrint("The voice list was not enumerated");
                return GMBool.False;
            }

            DebugPrint("get_voices_close called");
            _voiceEnumerator.Dispose();
            _voiceEnumerator = null;
            return GMBool.True;
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_select_voice_by_name", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerSelectVoiceByName(SynthId id, GMU8StrPtr namePtr) {
        lock (_synthesizersLock) {
            string? name = Marshal.PtrToStringUTF8(namePtr);
            if (string.IsNullOrEmpty(name))
                return GMBool.False;

            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;

            try {
                synthesizer.SelectVoice(name);
            } catch (ArgumentException e) {
                DebugPrint(e.Message);
                return GMBool.False;
            }
            return GMBool.True;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_select_voice_by_hints", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerSelectVoiceByHints(SynthId id, GenderAndAge genderAndAge, GMReal altChoice, GMU8StrPtr cultureNamePtr) {
        lock (_synthesizersLock) {
            if (altChoice < 0) {
                DebugPrint("altChoice smaller than 0 is not allowed");
                return GMBool.False;
            }

            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;

            DebugPrint("selecting voice");
            // pieced together by GML-side sapi_get_gender_and_age_word. thanks YoYo
            VoiceGender gender = (VoiceGender)((int)genderAndAge & 0xFF);
            VoiceAge age = (VoiceAge)((int)genderAndAge >> 8);
            DebugPrint(gender.ToString());
            DebugPrint(age.ToString());
            CultureInfo? culture;
            string? cultureName = Marshal.PtrToStringUTF8(cultureNamePtr);
            if (string.IsNullOrEmpty(cultureName))
                culture = null;
            else
                culture = new CultureInfo(cultureName);
            if (culture is not null)
                DebugPrint(culture.Name);
            else
                DebugPrint("any culture (hopefully)");

            try {
                synthesizer.SelectVoiceByHints(gender, age, Convert.ToInt32(altChoice), culture);
            } catch (ArgumentException e) {
                DebugPrint($"Invalid '{e.ParamName}' passed");
                return GMBool.False;
            } catch (InvalidOperationException e) {
                DebugPrint(e.Message);
                return GMBool.False;
            }

            return GMBool.True;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sapi_synthesizer_speak", CallConvs = [typeof(CallConvCdecl)])]
    public static GMBoolType SynthesizerSpeak(SynthId id, GMU8StrPtr inputPtr, PromptType inputTypeRaw) {
        lock (_synthesizersLock) {
            SpeechSynthesizer? synthesizer = _synthesizers.ElementAtOrDefault(Convert.ToInt32(id));
            if (synthesizer is null)
                return GMBool.False;

            string? input = Marshal.PtrToStringUTF8(inputPtr);
            if (string.IsNullOrEmpty(input))
                return GMBool.False;

            SynthesisTextFormat inputType = (SynthesisTextFormat)inputTypeRaw;

            DebugPrint($"Speaking text ({inputType}): " + input);
            Prompt prompt = new(input, inputType);
            synthesizer.SpeakAsync(prompt);
            return GMBool.True;
        }
    }
}