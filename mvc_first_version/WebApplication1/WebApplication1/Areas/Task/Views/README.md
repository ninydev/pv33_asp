Папка: Views (Area: Task)

Назначение
- Razor-представления для контроллеров области Task. Следуйте стандартной структуре MVC: `Areas/Task/Views/<Controller>/<Action>.cshtml`.

Базовые файлы в области
- `_ViewImports.cshtml` — общие импорты пространств имён и TagHelpers для всех представлений области.
- `_ViewStart.cshtml` — базовая настройка макета (`Layout`) для всех представлений области.

Рекомендации
- Группируйте представления по контроллерам (папки с именем контроллера без суффикса `Controller`).
- Общие частичные представления области храните в `Areas/Task/Views/Shared`.
- Если используется общий для всего приложения макет — укажите путь `~/Views/Shared/_Layout.cshtml` в `_ViewStart.cshtml`.

Пример маршрута и разрешения представлений
- URL: `/Task/Task/Index` → контроллер `TaskController` (в Area "Task"), представление `Areas/Task/Views/Task/Index.cshtml`.
