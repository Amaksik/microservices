using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Polly;

namespace KnowYourPostView.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public IndexModel(ILogger<IndexModel> logger, 
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IAsyncPolicy<HttpResponseMessage> policy)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _policy = policy;
    }

    [FromQuery(Name = "id")]
    public string? Chosen { get; set; }

    public string GetChosenString 
    {
        get {
            return $"{CountryOrigin}-{CountryDestination}";
        }
    }

    [BindProperty]
    public int CountryOrigin { get; set; }
    public SelectList CountriesOrigin { get; set; }

    [BindProperty]
    public int CountryDestination { get; set; }
    public SelectList CountriesDestination { get; set; }

    [BindProperty]
    public string PostalService { get; set; }
    public SelectList PostalServices { get; set; }

    [BindProperty]
    public double Weight { get; set; }

    [BindProperty(SupportsGet = true)]
    public double PriceProp { get; set; }

    public async Task OnGetAsync()
    {
        var countries = await GetCountries();
        CountriesOrigin = new SelectList(countries, "Id", "Name");
        CountriesDestination = new SelectList(countries, "Id", "Name");
    }

    public async Task<IActionResult> OnPostGetCompaniesAsync()
    {
        await OnGetAsync();

        try 
        {
            var data = await GetCompanies(CountryOrigin, CountryDestination);
            PostalServices = new SelectList(data, "DataString", "Name");
            return Page();
        }
        catch
        {
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostCalculateAsync()
    {
        try 
        {
            var TaxServiceUrl = Environment.GetEnvironmentVariable("TAX_SERVICE_URL") ?? "http://localhost:5000";
            using var client = _clientFactory.CreateClient();
            var response = await _policy.ExecuteAsync(() => client.GetAsync($"{TaxServiceUrl}/tax"));

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error calling service 2");

            var content = await response.Content.ReadAsStringAsync();
            var taxData = JsonSerializer.Deserialize<List<TaxModel>>(content);

            var data = PostalService.Split('-');
            var companyId = int.Parse(data[0]);
            var rate = double.Parse(data[1]);
            var originId = int.Parse(data[2]);
            var destId = int.Parse(data[3]);


            var countries = await GetCountries();
            var country = countries!.First(x => x.Id == destId).Name;

            var tax = taxData!.First(x => x.Name.ToLower() == country.ToLower());

            PriceProp = rate * Weight * (1 + tax.Tax);

            return Page();
        }
        catch
        {
            return RedirectToPage("/Index");
        }
    }

    private async Task<List<CountryModel>?> GetCountries(int? id = null)
    {
        var RateServiceUrl = Environment.GetEnvironmentVariable("RATE_SERVICE_URL") ?? "http://localhost:5000";
        using var client = _clientFactory.CreateClient();
        var url = $"{RateServiceUrl}/Countries";
        if (id != null) url += $"/{id}";
        Console.WriteLine("url1 = " + url);

        var response = await _policy.ExecuteAsync(() => client.GetAsync(url));

        if (!response.IsSuccessStatusCode)
            throw new Exception("Error calling service 2");

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<CountryModel>>(content);
    }

    private async Task<List<CompanyModel>> GetCompanies(int originId, int destId)
    {
        var RateServiceUrl = Environment.GetEnvironmentVariable("RATE_SERVICE_URL") ?? "http://localhost:5000";
        using var client = _clientFactory.CreateClient();
        var url = $"{RateServiceUrl}/Services/{originId}/{destId}";
        Console.WriteLine("url2 = " + url);

        var response = await _policy.ExecuteAsync(() => client.GetAsync(url));

        if (!response.IsSuccessStatusCode)
            throw new Exception("Error calling service 2");

        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<CompanyModel>>(content)!;

        foreach (var item in res)
        {
            item.Data = new CompanyCountryRate
            {
                Id = item.Id,
                Rate = item.Rate,
                OriginId = originId,
                DestinationId = destId
            };
        }

        return res;
    }
}


public class CountryModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class CompanyModel
{
    [JsonPropertyName("companyID")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("rate")]
    public double Rate { get; set; }

    public CompanyCountryRate Data { get; set; }
    public string DataString => Data.ToString();
}

public class CompanyCountryRate
{
    public int Id { get; set; }
    public double Rate { get; set; }
    public int OriginId { get; set; }
    public int DestinationId { get; set; }

    public override string ToString()
    {
        return $"{Id}-{Rate}-{OriginId}-{DestinationId}";
    }
}

public class TaxModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tax")]
    public double Tax { get; set; }
}