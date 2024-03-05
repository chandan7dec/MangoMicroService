using Mango.Web.Models;

namespace Mango.web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrder(CartDto cartDto);
    }
}
