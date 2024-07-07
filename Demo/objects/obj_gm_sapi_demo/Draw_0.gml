draw_text(40, 40, string(
    "state: {0}(Enter->Play/Stop,Space->Pause)  volume: {1}(A-/D+)  rate: {2}(W+/S-)",
    sapi_synthesizer_get_state(synth),
    volume,
    rate
    )
)
draw_text(40, 60, string(
    "{0}(gender={1}, age={2}, culture={3})",
    voice_name, gender, age, culture
    )
)
draw_text(40, 80, string(
    "R=hint T=name  G=text"
    )
)

draw_text_ext(40, 120, text, 20, room_width-40*2)
