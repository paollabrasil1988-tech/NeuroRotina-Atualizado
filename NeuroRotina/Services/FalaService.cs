namespace NeuroRotina.Services;

/// <summary>Faz o aparelho falar em voz alta, em português do Brasil quando disponível.</summary>
public static class FalaService
{
    private static Locale? _localePtBr;
    private static bool _jaProcurou;

    public static async Task FalarAsync(string texto)
    {
        try
        {
            if (!_jaProcurou)
            {
                _jaProcurou = true;

                IEnumerable<Locale> idiomas = await TextToSpeech.Default.GetLocalesAsync();

                _localePtBr =
                    idiomas.FirstOrDefault(l => l.Language == "pt" && l.Country == "BR") ??
                    idiomas.FirstOrDefault(l => l.Language == "pt");
            }

            await TextToSpeech.Default.SpeakAsync(
                texto,
                new SpeechOptions
                {
                    Locale = _localePtBr,
                    Volume = 1.0f,
                    Pitch = 1.0f
                });
        }
        catch
        {
            // Aparelho sem voz instalada: o app continua funcionando sem som.
        }
    }
}
