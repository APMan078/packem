using Packem.Domain.Entities;
using Packem.Domain.Models;
using System.IO;

namespace Packem.Domain.Common.ExtensionMethods
{
    public static class ItemExtension
    {
        public static byte[] ToImagByte(this Item item, CustomerDeviceTokenAuthModel state, int vendorId, string rootPath)
        {
            var path = Path.Combine(rootPath,
                "images", "items", state.CustomerId.ToString(), state.CustomerLocationId.ToString(), vendorId.ToString());

            Directory.CreateDirectory(path);

            // images/items/{CustomerId}/{CustomerLocationId}/{VendorId}/{ItemId}.jpeg
            var filePath = Path.Combine(path, $"{item.ItemId}.jpg");

            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                var notFoundPath = Path.Combine(rootPath, "images", "items", "no_image.jpg");
                return File.ReadAllBytes(notFoundPath);
            }
        }
    }
}
