using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtension
    {
        public static void AddPaginationHeader(this HttpResponse response, int CurrentPage,
           int ItemsPerPage, int TotalItems, int TotalPages
        ){
            var PaginationHeaders =  new PaginationHeaders(CurrentPage,ItemsPerPage,TotalItems,TotalPages);

           var options = new JsonSerializerOptions
           {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
           };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(PaginationHeaders,options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}