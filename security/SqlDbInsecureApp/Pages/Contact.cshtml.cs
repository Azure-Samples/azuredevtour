using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace SqlDbInsecureApp.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IConfiguration config;

        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string GenerateLoad { get; set; }

        public ContactModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task OnGet()
        {
            Message = "Your contact page.";
            if (string.IsNullOrWhiteSpace(GenerateLoad)) return;
            
            // run a query to try to trigger an automatic tuning recommendation (didn't work)

            using (var conn = new SqlConnection(config["DefaultConnection"]))
            {
                await conn.OpenAsync();
                await Task.WhenAll(Enumerable
                    .Range(0, 50)
                    .Select(_ => conn.ExecuteReaderAsync("select * from SalesLT.Customer where Title='Ms.'")));
                Message = "OK";
            }
        }
    }
}
