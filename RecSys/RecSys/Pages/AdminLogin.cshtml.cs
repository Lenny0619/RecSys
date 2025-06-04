using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class AdminLoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        if (Username == "admin" && Password == "admin0619")
        {
            HttpContext.Session.SetString("IsAdminLoggedIn", "true");
            return RedirectToPage("/AdminDashboard");
        }
        else
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}