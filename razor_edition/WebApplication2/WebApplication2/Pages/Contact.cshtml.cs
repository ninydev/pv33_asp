using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages;

public class Contact : PageModel
{
    
    [BindProperty]
    public ContactForm Form { get; set; }

    public void OnPost()
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model is not valid");
        }
        ViewData["Title"] = "Contact";
        ViewData["Name"] = Form.Name;
        ViewData["Email"] = Form.Email;
        ViewData["Message"] = Form.Message;
        ViewData["Age"] = Form.Age;
    }
    
    // [BindProperty]
    // public string Name { get; set; }
    //
    // [BindProperty]
    // public string Email { get; set; }
    //
    // [BindProperty]
    // public string Message { get; set; }
    //
    // [BindProperty]
    // public int Age { get; set; }
    //
    // public void OnPost()
    // {
    //     ViewData["Name"] = Name;
    //     ViewData["Email"] = Email;
    //     ViewData["Message"] = Message;
    //     ViewData["Age"] = Age;
    // }

    
    // public void OnPost(string name, int age, string email, string message)
    // {
    //     ViewData["Name"] = name;
    //     ViewData["Email"] = email;
    //     ViewData["Message"] = message;
    //     ViewData["Age"] = age;
    // }
    
    
    // Bad Practice
    // I read the form data from the request
    // public void OnPost()
    // {
    //     var name = Request.Form["name"];
    //     var email = Request.Form["email"];
    //     var message = Request.Form["message"];
    //     
    //     ViewData["Name"] = name;
    //     ViewData["Email"] = email;
    //     ViewData["Message"] = message;
    //     
    //     Console.WriteLine(name + " " + email);
    // }

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
            ViewData["PageTitle"] = "Контакти";
            ViewData["Message"] = "Моя контактна сторінка";
        }
        else
        {
            ViewData["PageTitle"] = "Contact";
            ViewData["Message"] = "Your contact page.";
        }

    }
}