using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Blog.AdminSide;
using Application.ViewModels.Blog.ClientSide;
using Application.ViewModels.BlogTag.ClientSide;
using Data.Context;
using Domain.Events.Blogs.Blog;
using Domain.Models.Blogs;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BlogService : IBlogService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Blog> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public BlogService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Blog>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterBlogsViewModel>> AdminSideFilterBlogs(AdminSideFilterBlogsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterBlogsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Blog>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Slug.Contains(filter.Search) ||
                n.Writer.Contains(filter.Search) ||
                n.Category.Title.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<Blog> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Blog>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBlogViewModel()
            {
                Id = n.Id,
                Category = n.Category.Title,
                Slug = n.Slug,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Title = n.Title,
                Writer = n.Writer
            }));

            return filter;

        }

        public async Task<Result<AdminSideUpdateBlogViewModel>> GetBlogForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateBlogViewModel>(Messages.OperationFailedError);

            var blog = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateBlogViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                Slug = n.Slug,
                FileName = n.FileName,
                Writer = n.Writer,
                CategoryId = n.BlogCategoryId,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Tags = n.Tags.Select(n => n.TagId).ToList(),
            }).AsNoTracking().FirstOrDefaultAsync();
            if (blog is null) return Result.Failure<AdminSideUpdateBlogViewModel>(Messages.OperationFailedError);

            return blog;
        }

        public async Task<Result> SoftDeleteBlog(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var blog = await _dbSet.Where(n => n.Id == id).Include(n => n.Tags).FirstOrDefaultAsync();
            if (blog is null) return Result.Failure(Messages.OperationFailedError);

            blog.IsDeleted = true;
            blog.Tags = new List<BlogTagMapping>();
            blog.LastUpdatedDate = DateTime.Now;

            blog.FileName.RemoveFileFromServer(SiteTools.BlogBanner);

            _dbSet.Update(blog);
            await _context.SaveChangesAsync();

            BlogDeletedEvent BlogDeletedEvent = new BlogDeletedEvent(
                blog.Id);

            await _mediator.PublishEvent(BlogDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> CreateNewBlog(AdminSideCreateNewBlogViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _dbSet.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.File.FileName.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            model.File.UploadFileToServer(fileName, SiteTools.BlogBanner);

            Blog blog = new Blog()
            {
                Title = model.Title,
                Slug = model.Slug.Replace(" ", "-"),
                Body = model.Body,
                BlogCategoryId = model.CategoryId,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                Writer = model.Writer,
                FileName = fileName,
                Tags = model.Tags?.Select(n => new BlogTagMapping()
                {
                    TagId = n
                }).ToList()
            };

            await _dbSet.AddAsync(blog);
            await _context.SaveChangesAsync();

            BlogCreatedEvent BlogCreatedEvent = new BlogCreatedEvent(
                blog.FileName,
                blog.Title,
                blog.Slug,
                blog.Body,
                blog.Writer,
                blog.Priority,
                blog.IsPublished,
                blog.BlogCategoryId,
                blog.Tags?.Select(n => n.TagId).ToList());

            await _mediator.PublishEvent(BlogCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlog(AdminSideUpdateBlogViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var blog = await _dbSet.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == model.Id);
            if (blog is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.Id != blog.Id && n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            if (model.File is not null)
            {
                if (!model.File.IsImage()) return Result.Failure(Messages.OperationFailedError);
                if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                model.File.UploadFileToServer(fileName, SiteTools.BlogBanner);
                blog.FileName.RemoveFileFromServer(SiteTools.BlogBanner);
                blog.FileName = fileName;
            }

            blog.Title = model.Title;
            blog.Body = model.Body;
            blog.Priority = model.Priority;
            blog.Slug = model.Slug.Replace(" ", "-");
            blog.IsPublished = model.IsPublished;
            blog.Writer = model.Writer;
            blog.BlogCategoryId = model.CategoryId;

            blog.Tags = new List<BlogTagMapping>();

            blog.Tags = model.Tags?.Select(n => new BlogTagMapping()
            {
                BlogId = blog.Id,
                TagId = n
            }).ToList();

            _dbSet.Update(blog);
            await _context.SaveChangesAsync();


            BlogUpdatedEvent BlogUpdatedEvent = new BlogUpdatedEvent(
            blog.Id,
            blog.FileName,
            blog.Title,
            blog.Slug,
            blog.Body,
            blog.Writer,
            blog.Priority,
            blog.IsPublished,
            blog.BlogCategoryId,
            blog.Tags?.Select(n => n.TagId).ToList());

            await _mediator.PublishEvent(BlogUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideFilterBlogs(ClientSideFilterBlogsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterBlogsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Blog>();

            conditions.Add(n => n.IsPublished);

            if (filter.TagId is not null)
            {
                conditions.Add(n => n.Tags.Any(n => n.TagId == filter.TagId));
            }

            if (filter.CategoryId is not null)
            {
                conditions.Add(n => n.BlogCategoryId == filter.CategoryId);
            }

            IQueryable<Blog> query = _dbSet;
            query = query.OrderBy(n => n.Priority);

            foreach (var item in conditions ?? new FilterConditions<Blog>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideBlogViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                FileName = n.FileName,
                CreatedDate = n.CreatedDate,
                Writer = n.Writer,
                CategoryId = n.BlogCategoryId,
                Category = n.Category.Title
            }));

            return filter;
        }

        public async Task<Result<ClientSideBlogDetailsViewModel>> ClientSideGetBlogDetails(int id)
        {
            if (id < 1)
                return Result.Failure<ClientSideBlogDetailsViewModel>(Messages.OperationFailedError);

            var blog = await _dbSet.Where(n => n.IsPublished && n.Id == id)
                .Select(n => new ClientSideBlogDetailsViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    FileName = n.FileName,
                    CreatedDate = n.CreatedDate,
                    Writer = n.Writer,
                    Category = n.Category.Title,
                    CategoryId = n.BlogCategoryId,
                    Tags = n.Tags.Select(n => new ClientSideBlogTagViewModel()
                    {
                        Id = n.TagId,
                        Title = n.Tag.Title
                    }).ToList()
                }).AsNoTracking().FirstOrDefaultAsync();
            if (blog is null) return Result.Failure<ClientSideBlogDetailsViewModel>(Messages.OperationFailedError);

            return blog;
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetLatestBlogs()
        {
            ClientSideFilterBlogsViewModel filter = new ClientSideFilterBlogsViewModel() { TakeEntity = 6 };

            var conditions = Filter.GenerateConditions<Blog>();
            conditions.Add(n => n.IsPublished);

            IQueryable<Blog> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Blog>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideBlogViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                FileName = n.FileName,
                Category = n.Category.Title,
                CreatedDate = n.CreatedDate,
                Writer = n.Writer,
                CategoryId = n.BlogCategoryId,
            }));

            return filter;
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetRelatedBlogs(int id, int categoryId)
        {
            ClientSideFilterBlogsViewModel filter = new ClientSideFilterBlogsViewModel() { TakeEntity = 2 };

            var conditions = Filter.GenerateConditions<Blog>();
            conditions.Add(n => n.IsPublished);
            conditions.Add(n => n.Id != id);
            conditions.Add(n => n.BlogCategoryId == categoryId);

            IQueryable<Blog> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Blog>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideBlogViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                FileName = n.FileName,
                Category = n.Category.Title,
                CreatedDate = n.CreatedDate,
                Writer = n.Writer,
                CategoryId = n.BlogCategoryId,
            }));

            return filter;
        }
    }
}
