/// Enumerates values for the state of the a synthesizer, as retrieved from sapi_synthesizer_get_state
enum GMSapi_SynthesizerState {
    /// Indicates that the synthesizer is ready to generate speech from a prompt.
    Ready = 0,
    /// Indicates that the synthesizer is speaking.
    Speaking = 1,
    /// Indicates that the synthesizer is paused.
    Paused = 2
}

// Gender and age is to be combined by sapi_get_gender_and_age_word(gender, age)
// due to GM limitations

/// Defines the values for the gender of a synthesized voice.
enum GMSapi_VoiceGender {
    /// Indicates no voice gender specification.
    NotSet = 0,
    /// Indicates a male voice.
    Male = 1,
    /// Indicates a female voice.
    Female = 2,
    /// Indicates a gender-neutral voice.
    Neutral = 3
}

/// Defines the values for the age of a synthesized voice.
enum GMSapi_VoiceAge {
    /// Indicates that no voice age is specified.
    NotSet = 0,
    /// Indicates a child voice (age 10).
    Child = 10,
    /// Indicates a teenage voice (age 15).
    Teen = 15,
    /// Indicates an adult voice (age 30).
    Adult = 30,
    /// Indicates a senior voice (age 65).
    Senior = 65
}

/// Enumerates the types of text formats that may be spoken by a synthesizer.
enum GMSapi_SynthesisTextFormat {
    /// Indicates that the input is in plain text format.
    Text = 0,
    /// Indicates that the input is in SSML (Speech Synthesis Markup Language) format.
    Ssml = 1
}