#define _sapi_init_gml
global.__sapi__sapi_synthesizer_get_voice = external_define("GMSapiInteropNE.dll", "sapi_synthesizer_get_voice", dll_cdecl, ty_real, 1, ty_real);
global.__sapi__sapi_synthesizer_get_voices_next = external_define("GMSapiInteropNE.dll", "sapi_synthesizer_get_voices_next", dll_cdecl, ty_real, 0);

#define sapi_synthesizer_get_voice
return external_call(global.__sapi__sapi_synthesizer_get_voice, argument0);

#define sapi_synthesizer_get_voices_next
return external_call(global.__sapi__sapi_synthesizer_get_voices_next);

#define sapi_get_gender_and_age_word
return (argument1 << 8) + argument0;

#define _sapi_cleanup_gml
external_free(global.__sapi__sapi_synthesizer_get_voice);
external_free(global.__sapi__sapi_synthesizer_get_voices_next);