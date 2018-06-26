using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace SqlDbInsecureApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration config;

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public bool Failed { get; set; } = false;

        public IndexModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<IActionResult> OnPost()
        {
            using(var conn = new SqlConnection(config["DefaultConnection"]))
            {
                var user = await conn.QueryFirstOrDefaultAsync(
                    $"select * from [SalesLT].[Customer] where EmailAddress = '{Email}' and PasswordHash = '{Password}'");
                if (user != null)
                {
                    return this.Redirect("/about");
                }
                else
                {
                    Failed = true;
                    return this.Page();
                }
            }
        }
    }
}
