using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TranslationResponse
{
	[JsonPropertyName("source-language")]
	public string SourceLanguage { get; set; }
	[JsonPropertyName("source-text")]
	public string SourceText { get; set; }
	[JsonPropertyName("destination-language")]
	public string DestinationLanguage { get; set; }
	[JsonPropertyName("destination-text")]
	public string DestinationText { get; set; }
	public JsonElement Translations { get; set; }
	public JsonElement Pronunciation { get; set; }
	public JsonElement Definitions { get; set; }
	[JsonPropertyName("see-also")]
	public JsonElement SeeAlso { get; set; }
}
