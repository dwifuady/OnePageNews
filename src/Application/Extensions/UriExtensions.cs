using Domain.Common;

namespace Application.Extensions;

public static class UriExtensions
{
    public static ProviderEnum GetProviderEnum(this Uri uri)
    {
        var provider = ProviderEnum.General;
        var leftPart = uri.GetLeftPart(UriPartial.Authority);
        if (leftPart.Contains("detik.com"))
        {
            provider = ProviderEnum.Detik;
        }
        else if (leftPart.Contains("kompas.com"))
        {
            provider = ProviderEnum.Kompas;
        }
        else if (leftPart.Contains("okezone.com"))
        {
            provider = ProviderEnum.Okezone;
        }
        else if (leftPart.Contains("cnbcindonesia.com"))
        {
            provider = ProviderEnum.CnbcIndonesia;
        }

        
        return provider;
    }
}