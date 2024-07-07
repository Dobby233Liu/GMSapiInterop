var state = sapi_synthesizer_get_state(synth)

if state == GMSapi_SynthesizerState.Speaking
    sapi_synthesizer_pause(synth)
else if state == GMSapi_SynthesizerState.Paused
    sapi_synthesizer_resume(synth)