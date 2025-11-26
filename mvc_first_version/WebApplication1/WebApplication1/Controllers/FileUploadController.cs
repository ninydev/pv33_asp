using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class FileUploadController : Controller
{
    
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile avatar ,IWebHostEnvironment appEnvironment)
    {
        // 1. Проверка: прислали ли вообще файл?
        if (avatar == null || avatar.Length == 0)
        {
            return View();
        }

        // 2. Формируем путь к папке (wwwroot/images)
        // WebRootPath указывает на физический путь к папке wwwroot
        string uploadsFolder = Path.Combine(appEnvironment.WebRootPath, "images");
            
        // Если папки нет, создадим её
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // 3. Придумываем уникальное имя файла
        // Почему нельзя оставить avatar.FileName? 
        // Потому что два юзера могут загрузить файл "image.jpg", и один перезатрет другой.
        // Используем Guid для уникальности.
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatar.FileName;
            
        // Полный путь к файлу на сервере (C:\Projects\Site\wwwroot\images\guid_cat.jpg)
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // 4. Самое главное: копируем поток (Stream) файла на диск
        // ASP.NET Core дает нам поток (Stream) через avatar.OpenReadStream(), но удобнее CopyToAsync
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(fileStream);
        }

        // 5. Формируем ссылку для БД
        // В базу мы запишем относительный путь, чтобы браузер мог его открыть
        string dbPath = "/images/" + uniqueFileName;

        // ... ТУТ КОД СОХРАНЕНИЯ dbPath В БАЗУ ДАННЫХ ...
        return View();
    }
    
}