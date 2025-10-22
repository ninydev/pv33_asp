using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages;

public class Contact : PageModel
{
    /**
     * Код обработки запроса гет будет запущен
     * ДО построения самой страницы и может оработатать запрос
     * ПО сути - это МиниКонтроллер
     */
    public void OnGet()
    {
        
        var languages = Request.Headers["Accept-Language"].ToString();
        Console.WriteLine("User languages: " + languages);
        ViewData["LangHeader"] = languages;
        ViewData["BuildTime"] = DateTime.Now.ToLocalTime();
        
        var lang = languages.Split(",")[0];
        if (lang == "uk")
        {
            ViewData["Title"] = "Контакти";
            ViewData["Message"] = "Моя контактна сторінка";
        }
        else
        {
            ViewData["Title"] = "Contact";
            ViewData["Message"] = "Your contact page.";
        }

        
        
        
        

    }
}