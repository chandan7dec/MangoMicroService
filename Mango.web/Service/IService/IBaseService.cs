using Mango.web.Models;
using Mango.Web.Models;

namespace Mango.web.Service.IService
{
    public interface IBaseService
    {
      Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
