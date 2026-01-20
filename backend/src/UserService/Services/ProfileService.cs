using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Services;

public class ProfileService : IProfileService
{
    private readonly UserDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(UserDbContext context, IConfiguration configuration, ILogger<ProfileService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadProfileImageAsync(Guid userId, Stream imageStream, string fileName)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            throw new KeyNotFoundException($"Profile not found for user {userId}");

        var config = new AmazonS3Config
        {
            ServiceURL = _configuration["AWS:ServiceURL"],
            ForcePathStyle = true
        };

        using var s3Client = new AmazonS3Client("test", "test", config);
        var bucketName = _configuration["AWS:S3BucketName"];
        var key = $"profiles/{userId}/{Guid.NewGuid()}-{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = imageStream,
            ContentType = GetContentType(fileName)
        };

        await s3Client.PutObjectAsync(request);

        var imageUrl = $"{config.ServiceURL}/{bucketName}/{key}";
        profile.ProfileImageUrl = imageUrl;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Uploaded profile image for user {UserId}", userId);

        return imageUrl;
    }

    public async Task<AddressDto> AddAddressAsync(Guid userId, AddressDto addressDto)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw new KeyNotFoundException($"Profile not found for user {userId}");

        if (addressDto.IsPrimary)
        {
            foreach (var addr in profile.Addresses)
            {
                addr.IsPrimary = false;
            }
        }

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Street = addressDto.Street,
            City = addressDto.City,
            PostalCode = addressDto.PostalCode,
            Country = addressDto.Country,
            IsPrimary = addressDto.IsPrimary
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        addressDto.Id = address.Id;
        return addressDto;
    }

    public async Task<AddressDto> UpdateAddressAsync(Guid userId, Guid addressId, AddressDto addressDto)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw new KeyNotFoundException($"Profile not found for user {userId}");

        var address = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new KeyNotFoundException($"Address {addressId} not found");

        if (addressDto.IsPrimary && !address.IsPrimary)
        {
            foreach (var addr in profile.Addresses)
            {
                addr.IsPrimary = false;
            }
        }

        address.Street = addressDto.Street;
        address.City = addressDto.City;
        address.PostalCode = addressDto.PostalCode;
        address.Country = addressDto.Country;
        address.IsPrimary = addressDto.IsPrimary;

        await _context.SaveChangesAsync();

        return addressDto;
    }

    public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return false;

        var address = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null) return false;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return true;
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
