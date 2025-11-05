using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Dto;
using WebApplication1.Entities;
using WebApplication1.ViewModel;

namespace WebApplication1.Mappers
{
    /// <summary>
    /// PostMapper — спеціалізований клас для конвертації між різними представленнями поста.
    /// Призначення:
    /// - DTO -> Entity: під час створення/оновлення з форми або API у сутність БД.
    /// - Entity -> ViewModel: для відображення на сторінках (Views) у зручному для UI форматі.
    /// </summary>
    public static class PostMapper
    {
        /// <summary>
        /// Перетворює PostCreateDto у PostEntity для збереження в БД.
        /// </summary>
        /// <param name="dto">Вхідні дані з форми створення поста</param>
        /// <param name="authorId">Ідентифікатор поточного користувача-автора</param>
        /// <returns>Новий екземпляр PostEntity</returns>
        public static PostEntity ToEntity(PostCreateDto dto, string authorId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(authorId)) throw new ArgumentException("AuthorId is required", nameof(authorId));

            return new PostEntity
            {
                Title = dto.Title?.Trim(),
                Slug = dto.Slug?.Trim(),
                Content = dto.Content?.Trim(),
                AuthorId = authorId,
                // CreatedAt ініціалізується у конструкторі PostEntity або може бути заповнений БД
            };
        }

        /// <summary>
        /// Перетворює PostEntity у PostViewModel для відображення у представленнях.
        /// </summary>
        public static PostViewModel ToViewModel(PostEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new PostViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Content = entity.Content,
                Author = entity.Author,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Tags = entity.Tags?.ToList() ?? new List<TagModel>()
            };
        }

        /// <summary>
        /// Мапінг колекції сутностей у колекцію в’юмоделей.
        /// </summary>
        public static IEnumerable<PostViewModel> ToViewModels(IEnumerable<PostEntity> entities)
        {
            if (entities == null) yield break;
            foreach (var e in entities)
            {
                yield return ToViewModel(e);
            }
        }
    }
}
