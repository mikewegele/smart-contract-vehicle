using var client = new HttpClient();

try
{
    var url = "https://contoso.com/";
    Console.WriteLine($"Sende GET-Anfrage an {url} ...");

    var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Antwort erhalten:");
    Console.WriteLine(content[..Math.Min(500, content.Length)] + "...");
}
catch (HttpRequestException e)
{
    Console.WriteLine($"Fehler bei der Anfrage: {e.Message}");
}
