using Mango.Web.Models;

namespace Mango.web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartByUserIdAsyns(string userId);
        Task<ResponseDto> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
    }
}
