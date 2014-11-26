using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

using AppStudio.Data;

namespace AppStudio.Services
{
    public static class SpeechServices
    {
        const VoiceGender VOICE_GENDER = VoiceGender.Female;

        static private MediaElement _soundPlayer = null;
        static private SpeechSynthesizer _speech = null;

        static public async Task StartTextToSpeech(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    if (_speech != null)
                    {
                        StopTextToSpeech();
                    }

                    var voice = GetSpeechVoice();
                    if (voice != null)
                    {
                        _speech = new SpeechSynthesizer();
                        _speech.Voice = voice;

                        SpeechSynthesisStream speechStream = await _speech.SynthesizeTextToStreamAsync(Utility.DecodeHtml(text));
                        _soundPlayer = new MediaElement();
                        _soundPlayer.SetSource(speechStream, speechStream.ContentType);
                        _soundPlayer.Play();
                    }
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("SpeechServices", ex);
                }
            }
        }

        static public void StopTextToSpeech()
        {
            if (_speech != null)
            {
                _soundPlayer.Stop();
                _soundPlayer = null;

                _speech.Dispose();
                _speech = null;
            }
        }

        static private VoiceInformation GetSpeechVoice()
        {
            string language = CultureInfo.CurrentCulture.ToString();
            var voices = SpeechSynthesizer.AllVoices.Where(v => v.Language == language);
            var voice = voices.FirstOrDefault(v => v.Gender == VOICE_GENDER);
            if (voice == null)
            {
                voice = voices.FirstOrDefault();
            }
            return voice;
        }
    }
}
