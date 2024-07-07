var state = sapi_synthesizer_get_state(synth)

if state == GMSapi_SynthesizerState.Ready
    sapi_synthesizer_speak(synth, text, text_format)
else
    sapi_synthesizer_stop(synth)