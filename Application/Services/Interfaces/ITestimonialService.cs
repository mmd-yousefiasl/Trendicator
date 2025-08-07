using Application.ViewModels.Testimonial.AdminSide;
using Application.ViewModels.Testimonial.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ITestimonialService
    {
        Task<Result<AdminSideFilterTestimonialsViewModel>> AdminSideFilterTestimonials(AdminSideFilterTestimonialsViewModel filter);

        Task<Result> AdminSideCreateNewTestimonial(AdminSideCreateNewTestimonialViewModel model);

        Task<Result<AdminSideUpdateTestimonialViewModel>> AdminSideGetTestimonialForUpdate(int id);

        Task<Result> AdminSideUpdateTestimonial(AdminSideUpdateTestimonialViewModel model);

        Task<Result> AdminSideDeleteTestimonial(int id);

        Task<Result<List<ClientSideTestimonialViewModel>>> ClientSideGetTestimonials();
    }
}
