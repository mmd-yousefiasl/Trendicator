using Application.Dtos.Blog.AdminSide;
using Application.Dtos.Blog.ClientSide;
using Application.Dtos.BlogCategory.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.Blogs.Blog;
using Domain.Models.Blogs;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BlogService : IBlogService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IMediatorHandler _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVisitorService _visitorService;
        #endregion

        #region Ctor
        public BlogService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IHttpContextAccessor httpContextAccessor,
            IVisitorService visitorService)
        {
            _context = context;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _visitorService = visitorService;
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

            IQueryable<Blog> query = _context.Blogs;
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
                Writer = n.Writer,
                TimeToRead = n.TimeToRead
            }));

            return filter;

        }

        public async Task<Result<AdminSideUpdateBlogViewModel>> GetBlogForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateBlogViewModel>(Messages.OperationFailedError);

            var blog = await _context.Blogs
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateBlogViewModel()
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
                    TimeToRead = n.TimeToRead,
                    MetaDescription = n.MetaDescription
                }).FirstOrDefaultAsync();
            if (blog is null) return Result.Failure<AdminSideUpdateBlogViewModel>(Messages.OperationFailedError);

            return blog;
        }

        public async Task<Result> SoftDeleteBlog(int id)
        {
            if (id < 1)
                return Result.Failure(Messages.OperationFailedError);

            var blog = await _context.Blogs.FindAsync(id);

            if (blog is null)
                return Result.Failure(Messages.OperationFailedError);

            blog.IsDeleted = true;
            blog.LastUpdatedDate = DateTime.UtcNow;

            blog.FileName.RemoveFileFromServer(SiteTools.BlogBanner);

            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            BlogDeletedEvent BlogDeletedEvent = new BlogDeletedEvent(
                blog.Id);

            await _mediator.PublishEvent(BlogDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> CreateNewBlog(AdminSideCreateNewBlogViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            if (await _context.Blogs.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            if (!model.File.IsImage())
                return Result.Failure(Messages.InvalidFileFormatError);

            if (model.File.FileName.Length > SiteTools.MaxImageFileLength.CalculateMB())
                return Result.Failure(Messages.FileLengthError);

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
                TimeToRead = model.TimeToRead,
                MetaDescription = model.MetaDescription
            };

            _context.Blogs.Add(blog);
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
                blog.TimeToRead);

            await _mediator.PublishEvent(BlogCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlog(AdminSideUpdateBlogViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var blog = await _context.Blogs.FindAsync(model.Id);

            if (blog is null)
                return Result.Failure(Messages.OperationFailedError);

            if (await _context.Blogs.AnyAsync(n => n.Id != blog.Id && n.Slug == model.Slug.Replace(" ", "-")))
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
            blog.LastUpdatedDate = DateTime.UtcNow;
            blog.TimeToRead = model.TimeToRead;
            blog.MetaDescription = model.MetaDescription;

            _context.Blogs.Update(blog);
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
            blog.TimeToRead);

            await _mediator.PublishEvent(BlogUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideFilterBlogs(ClientSideFilterBlogsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterBlogsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Blog>();

            conditions.Add(n => n.IsPublished == true);

            if (!string.IsNullOrWhiteSpace(filter.CategorySlug))
            {
                var categoryResult = await ClientSideGetCategoryBySlug(filter.CategorySlug);
                if (categoryResult.IsSuccess)
                {
                    filter.CategoryMetaDescription = categoryResult.Value.MetaDescription;
                    filter.CategoryTitle = categoryResult.Value.Title;
                }

                conditions.Add(n => n.Category.Slug == filter.CategorySlug);
            }

            IQueryable<Blog> query = _context.Blogs;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Blog>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideBlogViewModel()
            {
                Id = n.Id,
                Slug = n.Slug,
                Title = n.Title,
                CreatedDate = n.CreatedDate,
                FileName = n.FileName,
                MetaDescription = n.MetaDescription,
                TimeToRead = n.TimeToRead,
                ViewsCount = n.ViewsCount
            }));

            return filter;
        }

        public async Task<Result<ClientSideBlogDetailsViewModel>> ClientSideGetBlogDetails(int id)
        {
            if (id < 1)
                return Result.Failure<ClientSideBlogDetailsViewModel>(Messages.OperationFailedError);

            var blog = await _context.Blogs
                .AsNoTracking()
                .Where(n => n.IsPublished && n.Id == id)
                .Select(n => new ClientSideBlogDetailsViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    Body = n.Body,
                    FileName = n.FileName,
                    CreatedDate = n.CreatedDate,
                    Writer = n.Writer,
                    Category = n.Category.Title,
                    CategoryId = n.BlogCategoryId,
                    CategorySlug = n.Category.Slug,
                    MetaDescription = n.MetaDescription,
                    TimeToRead = n.TimeToRead,
                    ViewsCount = n.ViewsCount,
                    Comments = n.Comments.Where(c => c.IsPublished)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new ClientSideBlogCommentDto()
                    {
                        Name = c.Name,
                        Comment = c.Comment,
                        CreatedDate = c.CreatedDate,
                        LastUpdatedDate = c.LastUpdatedDate,
                        Reply = c.Reply
                    }).Take(10)
                    .ToList()
                }).FirstOrDefaultAsync();
            if (blog is null) return Result.Failure<ClientSideBlogDetailsViewModel>(Messages.OperationFailedError);

            return blog;
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetLatestBlogs()
        {
            ClientSideFilterBlogsViewModel filter = new ClientSideFilterBlogsViewModel() { TakeEntity = 6 };

            var conditions = Filter.GenerateConditions<Blog>();
            conditions.Add(n => n.IsPublished);

            IQueryable<Blog> query = _context.Blogs;
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
                CreatedDate = n.CreatedDate,
                TimeToRead = n.TimeToRead,
                MetaDescription = n.MetaDescription,
                ViewsCount = n.ViewsCount,
            }));

            return filter;
        }

        public async Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetRelatedBlogs(int id, int categoryId)
        {
            ClientSideFilterBlogsViewModel filter = new ClientSideFilterBlogsViewModel() { TakeEntity = 3 };

            var conditions = Filter.GenerateConditions<Blog>();
            conditions.Add(n => n.IsPublished);
            conditions.Add(n => n.Id != id);
            conditions.Add(n => n.BlogCategoryId == categoryId);

            IQueryable<Blog> query = _context.Blogs;
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
                CreatedDate = n.CreatedDate,
                MetaDescription = n.MetaDescription,
                ViewsCount = n.ViewsCount,
                TimeToRead = n.TimeToRead,
            }));

            return filter;
        }

        public async Task<Result<List<AdminSideBlogForSiteMapViewModel>>> AdminSideGetBlogIdsForSiteMap()
        {
            var ids = await _context.Blogs
                .AsNoTracking()
                .Where(n => n.IsPublished)
                .Select(n => new AdminSideBlogForSiteMapViewModel()
                {
                    Id = n.Id,
                    Slug = n.Slug
                }).ToListAsync();

            if (!ids.Any())
                return Result.Failure<List<AdminSideBlogForSiteMapViewModel>>(Messages.OperationFailedError);

            return ids;
        }

        public async Task<Result> ClientSideAddBlogView(int blogId)
        {
            var visitorId = _visitorService.GetOrCreateVisitorId();
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var alreadyViewed = await _context.BlogViews
                .AsNoTracking()
                .AnyAsync(n => n.BlogId == blogId && n.VisitorId == visitorId);

            if (alreadyViewed)
                return Result.Success();

            var view = new BlogView()
            {
                BlogId = blogId,
                UserId = userId > 0 ? userId : null,
                UserIP = userIP,
                VisitorId = visitorId
            };

            _context.BlogViews.Add(view);
            await _context.SaveChangesAsync();

            try
            {
                await _context.Blogs
                    .Where(b => b.Id == blogId)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.ViewsCount, b => b.ViewsCount + 1));
            }
            catch
            {
                return Result.Failure(Messages.OperationFailedError);
            }

            return Result.Success();
        }

        public async Task<Result> ClientSideAddNewBlogComment(ClientSideAddCommentDto model)
        {
            if (model is null || model.BlogId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var comment = new BlogComment()
            {
                BlogId = model.BlogId,
                Name = model.Name,
                Email = model.Email,
                Comment = model.Comment,
                IsPublished = false,
                Reply = null,
                UserIP = userIP
            };

            if (userId > 0)
                comment.UserId = userId;


            _context.BlogComments.Add(comment);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.BlogCommentSentSuccessfully);
        }

        public async Task<Result<AdminSideFilterBlogCommentsDto>> AdminSideFilterComments(AdminSideFilterBlogCommentsDto filter)
        {
            if (filter.BlogId < 1)
                return Result.Failure<AdminSideFilterBlogCommentsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<BlogComment>();

            conditions.Add(n => n.BlogId == filter.BlogId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Name.Contains(filter.Search) || n.Email.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            if (filter.IsAnswered is not null)
            {
                if (filter.IsAnswered == true)
                {
                    conditions.Add(n => n.Reply != null);
                }
                else
                {
                    conditions.Add(n => n.Reply == null);
                }
            }

            IQueryable<BlogComment> query = _context.BlogComments;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<BlogComment>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBlogCommentDto()
            {
                Id = n.Id,
                Comment = n.Comment,
                Email = n.Email,
                IsAnswered = (n.Reply != null) ? true : false,
                IsPublished = n.IsPublished,
                Name = n.Name
            }));

            return filter;
        }

        public async Task<Result<int>> AdminSideDeleteBlogComment(int id)
        {
            if (id < 1)
                return Result.Failure<int>(Messages.OperationFailedError);

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment is null)
                return Result.Failure<int>(Messages.OperationFailedError);

            comment.IsDeleted = true;
            comment.LastUpdatedDate = DateTime.UtcNow;

            _context.BlogComments.Update(comment);
            await _context.SaveChangesAsync();

            return comment.BlogId;
        }

        public async Task<Result<AdminSideUpdateBlogCommentDto>> AdminSideGetBlogCommentForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateBlogCommentDto>(Messages.OperationFailedError);

            var comment = await _context.BlogComments
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateBlogCommentDto()
                {
                    Id = n.Id,
                    Comment = n.Comment,
                    IsPublished = n.IsPublished,
                    Reply = n.Reply,
                    BlogId = n.BlogId
                }).FirstOrDefaultAsync();

            if (comment is null)
                return Result.Failure<AdminSideUpdateBlogCommentDto>(Messages.OperationFailedError);

            return comment;
        }

        public async Task<Result<int>> AdminSideUpdateBlogComment(AdminSideUpdateBlogCommentDto model)
        {
            if (model is null)
                return Result.Failure<int>(Messages.OperationFailedError);

            var comment = await _context.BlogComments.FindAsync(model.Id);
            if (comment is null)
                return Result.Failure<int>(Messages.OperationFailedError);

            comment.Reply = model.Reply;
            comment.IsPublished = model.IsPublished;
            comment.LastUpdatedDate = DateTime.UtcNow;

            _context.BlogComments.Update(comment);
            await _context.SaveChangesAsync();

            return comment.BlogId;
        }

        public async Task<Result<ClientSideBlogCategoryViewModel>> ClientSideGetCategoryBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result.Failure<ClientSideBlogCategoryViewModel>(Messages.OperationFailedError);

            var categories = await _context.BlogCategories
                          .AsNoTracking()
                          .Where(n => n.IsPublished)
                          .OrderBy(n => n.Priority)
                          .Select(n => new ClientSideBlogCategoryViewModel()
                          {
                              Id = n.Id,
                              Title = n.Title,
                              Slug = n.Slug,
                              MetaDescription = n.MetaDescription
                          }).ToListAsync();

            if (!categories?.Any() ?? true)
                return Result.Failure<ClientSideBlogCategoryViewModel>(Messages.OperationFailedError);

            var category = categories!.FirstOrDefault(n => n.Slug == slug);
            if (category is null)
                return Result.Failure<ClientSideBlogCategoryViewModel>(Messages.OperationFailedError);

            return new ClientSideBlogCategoryViewModel()
            {
                Id = category.Id,
                Title = category.Title,
                Slug = category.Slug,
                MetaDescription = category.MetaDescription
            };
        }
    }
}