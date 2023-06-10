using SmartStore.UI.Dtos;
using SmartStore.UI.Dtos.Products;
using SmartStore.UI.Models;

namespace SmartStore.UI.Services.Interfaces
{

    public interface IBaseService : IDisposable
    {
        public ResponseDto ResponseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
