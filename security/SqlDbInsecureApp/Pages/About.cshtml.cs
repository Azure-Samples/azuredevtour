using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace SqlDbInsecureApp.Pages
{
    public class AboutModel : PageModel
    {
        private readonly IConfiguration config;

        public string Message { get; set; }

        public dynamic Records { get; set; }

        public AboutModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task OnGet()
        {
            using (var conn = new SqlConnection(config["DefaultConnection"]))
            {
                Records = await conn.QueryAsync("select top(20) * from SalesLT.Customer");
            }
        }
    }
}
