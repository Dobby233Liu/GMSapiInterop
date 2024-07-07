sapi_set_debug(true)

synth = sapi_synthesizer_create()

gender = GMSapi_VoiceGender.Male
age = GMSapi_VoiceAge.Adult
culture = "en-US"
choice_offset = 0
voice_name = ""
event_user(0)

volume = 100 // 0-100
event_user(1)
rate = 3 // -10-10
event_user(2)

text = "I think I speak English pretty well"
text_format = GMSapi_SynthesisTextFormat.Text