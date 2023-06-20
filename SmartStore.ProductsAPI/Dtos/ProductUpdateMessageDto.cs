using SmartStore.MessageBus;
using SmartStore.ProductsAPI.Dto;

namespace SmartStore.ProductsAPI.Dtos
{
    public class ProductUpdateMessageDto : BaseMessage
    {
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }

    }
}
