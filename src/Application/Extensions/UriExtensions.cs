using Domain.Common;

namespace Application.Extensions;

public static class UriExtensions
{
    public static ProviderEnum GetProviderEnum(this Uri uri)
    {
        var leftPart = uri.GetLeftPart(UriPartial.Authority);
        if (leftPart.Contains("detik.com"))
        {
            return ProviderEnum.Detik;
        }

        if (leftPart.Contains("kompas.com"))
        {
            return ProviderEnum.Kompas;
        }

        return ProviderEnum.Default;
    }
}