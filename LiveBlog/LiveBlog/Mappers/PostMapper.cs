using LiveBlog.Models.Media;
using Microsoft.AspNetCore.Http;

namespace LiveBlog.Models.Posts;

/// <summary>
/// Мапер для перетворення між запитами/відповідями та сутністю допису.
/// Розміщено у просторі імен LiveBlog.Models.Posts згідно з вимогою.
/// </summary>
public static class PostMapper
{
    /// <summary>
    /// Створює <see cref="PostEntity"/> із <see cref="CreatePostRequest"/>.
    /// Імена файлів беруться з <see cref="IFormFile.FileName"/>.
    /// </summary>
    public static PostEntity FromCreateRequest(CreatePostRequest request)
    {
        var entity = new PostEntity
        {
            UserId = request.UserId?.Trim() ?? string.Empty,
            Slug = request.Slug?.Trim() ?? string.Empty,
            Content = request.Content?.Trim() ?? string.Empty,
        };

        if (request.Files is { Count: > 0 })
        {
            foreach (var file in request.Files)
            {
                if (file == null) continue;
                var name = file.FileName;
                if (string.IsNullOrWhiteSpace(name)) continue;

                entity.MediaFiles.Add(new PostMediaFileEntity
                {
                    FileName = name,
                    UserId = entity.UserId
                });
            }
        }

        return entity;
    }

    /// <summary>
    /// Застосовує зміни з <see cref="UpdatePostRequest"/> до наявної сутності <see cref="PostEntity"/>.
    /// Правила для файлів:
    /// - Files == null: не змінювати вкладення;
    /// - Files.Count == 0: очистити вкладення;
    /// - Files має елементи: замінити колекцію на нові імена файлів.
    /// </summary>
    public static void ApplyUpdate(PostEntity entity, UpdatePostRequest request)
    {
        if (request.Slug is not null)
        {
            entity.Slug = request.Slug.Trim();
        }

        if (request.Content is not null)
        {
            entity.Content = request.Content.Trim();
        }

        if (request.Files is null)
        {
            // Не змінюємо вкладення
            return;
        }

        // Якщо передано порожній список — очищаємо вкладення
        if (request.Files.Count == 0)
        {
            entity.MediaFiles.Clear();
            return;
        }

        // Інакше — замінюємо поточну колекцію на нові елементи
        entity.MediaFiles.Clear();
        foreach (var file in request.Files)
        {
            if (file == null) continue;
            var name = file.FileName;
            if (string.IsNullOrWhiteSpace(name)) continue;

            entity.MediaFiles.Add(new PostMediaFileEntity
            {
                FileName = name
            });
        }
    }

    /// <summary>
    /// Проєцює <see cref="PostEntity"/> у коротку відповідь <see cref="SmallPostResponse"/>.
    /// </summary>
    public static SmallPostResponse ToSmallResponse(PostEntity entity)
    {
        var response = new SmallPostResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Slug = entity.Slug,
            Content = entity.Content,
        };

        if (entity.MediaFiles is { Count: > 0 })
        {
            foreach (var mf in entity.MediaFiles)
            {
                if (string.IsNullOrWhiteSpace(mf.FileName)) continue;
                response.FileNames.Add(mf.FileName!);
            }
        }

        return response;
    }
}
