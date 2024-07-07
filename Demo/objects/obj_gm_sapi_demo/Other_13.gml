var voice_info = sapi_synthesizer_get_voice(synth)
if ds_exists(voice_info, ds_type_map) {
    gender = voice_info[? "gender"]
    age = voice_info[? "age"]
    culture = voice_info[? "culture"]
    voice_name = voice_info[? "name"]
}
ds_map_destroy(voice_info)