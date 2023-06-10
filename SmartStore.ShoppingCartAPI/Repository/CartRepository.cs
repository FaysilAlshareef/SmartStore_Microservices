using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartStore.ShoppingCartAPI.Dtos;
using SmartStore.ShoppingCartAPI.Models;
using SmartStore.ShoppingCartAPI.ShoppingCartData;

namespace SmartStore.ShoppingCartAPI.Repository
{

    public class CartRepository : ICartRepository
    {
        private readonly ShoppingCartDbContext _context;
        private readonly IMapper _mapper;
        public CartRepository(ShoppingCartDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;
        }
        public async Task<CartDto> UpsertCartAsync(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);
            try
            {

                //check if product exists in database, if not create it!
                var prodInDb = await _context.Products.FirstOrDefaultAsync(u => u.Id == cartDto.CartDetails.FirstOrDefault()
                    .ProductId);
                if (prodInDb == null)
                {
                    _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                    await _context.SaveChangesAsync();
                }


                //check if header is null
                var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    _context.CartHeaders.Add(cart.CartHeader);
                    await _context.SaveChangesAsync();
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;

                    var details = cart.CartDetails.FirstOrDefault();
                    try
                    {
                        details.Product = null;
                        _context.CartDetails.Add(details);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        //create details
                        cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        try
                        {
                            var details = cart.CartDetails.FirstOrDefault();
                            _context.Entry(details).State = EntityState.Added;
                            _context.CartDetails.Add(details);
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cart.CartDetails.FirstOrDefault().CartDetailId = cartDetailsFromDb.CartDetailId;
                        //update the count / cart details
                        cart.CartDetails.FirstOrDefault().Product = null;
                        cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                        //cart.CartDetails.FirstOrDefault().CartDetailId = cartDetailsFromDb.CartDetailId;
                        cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetailsFromDb.CartHeaderId;

                        var details = cart.CartDetails.FirstOrDefault();
                        _context.Entry(details).State = EntityState.Modified;
                        _context.CartDetails.Update(details);

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _context.CartHeaders
              .FirstOrDefaultAsync(ch => ch.UserId == userId);
            if (cartHeaderFromDb != null)
            {
                _context.CartDetails.RemoveRange(_context.CartDetails
                    .Where(cd => cd.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _context.CartHeaders.Remove(cartHeaderFromDb);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders
               .FirstOrDefaultAsync(ch => ch.UserId == userId)
            };

            var CartDetails = _context.CartDetails
                .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId)
                .Include(cd => cd.Product);
            if (CartDetails.FirstOrDefault() != null)
            {
                cart.CartDetails = CartDetails;
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCartAsync(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _context.CartDetails
                    .FirstOrDefaultAsync(u => u.CartDetailId == cartDetailsId);

                int totlCountOfCartItems = _context.CartDetails
                    .Where(cd => cd.CartHeaderId == cartDetails.CartHeaderId)
                    .Count();

                _context.CartDetails.Remove(cartDetails);

                if (totlCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(ch =>
                        ch.CartHeaderId == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCountAsync(CountDetailsDto countDetailsDto)
        {
            CartDetails cartDetails = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartDetailId == countDetailsDto.CartDetailsId);
            if (cartDetails != null)
            {
                if (countDetailsDto.Action == "decrement")
                {
                    if (cartDetails.Count > 1)
                    {
                        cartDetails.Count -= countDetailsDto.Amount;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await RemoveFromCartAsync(countDetailsDto.CartDetailsId);
                    }
                }
                if (countDetailsDto.Action == "increment")
                {
                    cartDetails.Count += countDetailsDto.Amount;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //Using Same Method To Apply or remove Coupon
        public async Task<bool> ApplyAndRemoveCoupon(string userId, string couponCode = "")
        {

            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(ch => ch.UserId == userId);
            if (cartHeader == null) return false;

            if (couponCode == "")
                cartHeader.CouponCode = "";
            else
                cartHeader.CouponCode = couponCode;


            _context.CartHeaders.Update(cartHeader);
            await _context.SaveChangesAsync();
            return true;
        }

       
    }
}
