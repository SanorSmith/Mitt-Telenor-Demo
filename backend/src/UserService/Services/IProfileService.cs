using UserService.Models;

namespace UserService.Services;

public interface IProfileService
{
    Task<string> UploadProfileImageAsync(Guid userId, Stream imageStream, string fileName);
    Task<AddressDto> AddAddressAsync(Guid userId, AddressDto addressDto);
    Task<AddressDto> UpdateAddressAsync(Guid userId, Guid addressId, AddressDto addressDto);
    Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);
}
