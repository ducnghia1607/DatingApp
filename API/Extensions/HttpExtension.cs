using System.Text.Json;
using System.Text.Json.Serialization;

namespace API;

public static class HttpExtension
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        //JsonSerializer.Serialize(header,jsonOptions):  Convert from object PaginationHeader to string with CamelCase Name Policy
        // Add header name Pagination 
        response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
        // Specify Pagination to access from client side 
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

    }
}
