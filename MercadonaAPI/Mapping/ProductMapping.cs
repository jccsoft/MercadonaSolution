using MercadonaAPI.Models;
using MercadonaAPI.Shared.Models;
using System.Globalization;

namespace MercadonaAPI.Mapping;

public static class ProductMapping
{
    public static ProductDto ToProductDto(this ProductModel src)
    {
        var product = new ProductDto()
        {
            Id = src.id ?? "",
            Name = src.display_name ?? "",
            ShareUrl = src.share_url ?? "",
            Thumbnail = src.thumbnail ?? ""
        };
        if (src.price_instructions != null)
        {
            if (decimal.TryParse(src.price_instructions.unit_price, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal unitPrice))
                product.UnitPrice = unitPrice;

            product.UnitPriceString = $"{string.Format(GetCultureInfo(), "{0:0.00}", product.UnitPrice)} €/{(src.price_instructions.is_pack ? "pack" : "ud.")}";

            product.Packaging = GetPackaging(src.price_instructions.unit_size ?? 0, src.price_instructions.size_format ?? ""
                , src.price_instructions.total_units ?? 0, src.price_instructions.unit_name ?? ""
                , src.price_instructions.reference_format ?? "", src.price_instructions.drained_weight ?? 0
                , src.price_instructions.is_pack, src.packaging ?? "");

            if (string.IsNullOrWhiteSpace(src.price_instructions.reference_price) == false &&
                decimal.TryParse(src.price_instructions.unit_price, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal refPrice))
            {
                product.ReferencePrice = refPrice;
            }

            if (product.ReferencePrice > 0 && string.IsNullOrWhiteSpace(src.price_instructions.reference_format) == false)
            {
                product.ReferencePriceString = $"{string.Format(GetCultureInfo(), "{0:0.00}", product.ReferencePrice)} €/{src.price_instructions.reference_format}";
            }
        }

        return product;
    }


    private static string GetPackaging(decimal unitSize, string sizeFormat, int totalUnits, string unitName, string referenceFormat,
                                       decimal drainedWeight, bool isPack, string packaging)
    {
        // ud, L, ml, 100 ml, kg, 100 g, dc, lv: lavados, m

        string output = "";
        if (unitSize > 0 && string.IsNullOrWhiteSpace(sizeFormat) == false)
        {
            if (totalUnits > 0 && unitName.Length > 0 && unitSize > 0 && sizeFormat.Length > 0)
            {
                output = $"{totalUnits} {unitName}{GetUnitSizeFormat(unitSize, sizeFormat, true)}";
            }
            else
            {
                output = GetUnitSizeFormat(unitSize, sizeFormat, false);
            }

            if (output == "" && unitSize > 1)
            {
                output = $"{string.Format(GetCultureInfo(), "{0}", unitSize)} {referenceFormat}";
            }
        }
        else if (drainedWeight > 0)
        {
            if (drainedWeight < 1)
            {
                output = $"{string.Format(GetCultureInfo(), "{0:0}", drainedWeight * 1000)} g escurrido";
            }
            else
            {
                output = $"{string.Format(GetCultureInfo(), "{0:0}", drainedWeight)} kg escurrido";
            }
        }
        output = (packaging == null || isPack ? "" : packaging + " ") + output;
        return output;
    }

    private static string GetUnitSizeFormat(decimal unitSize, string sizeFormat, bool parenthesis)
    {
        string output = "";

        if (unitSize < 1 && (sizeFormat.ToLower().Last() == 'g' || sizeFormat.ToLower().Last() == 'l'))
        {
            output = $"{string.Format(GetCultureInfo(), "{0:0}", unitSize * 1000)} {sizeFormat.ToLower().Last()}";
        }
        else if (unitSize > 0)
        {
            output = $"{string.Format(GetCultureInfo(), "{0:0.#}", unitSize)} {sizeFormat}";
        }

        if (output.Length > 0 && parenthesis) output = " (" + output + ")";


        return output;
    }

    private static CultureInfo GetCultureInfo()
    {
        return new CultureInfo("es-ES");
    }
}
