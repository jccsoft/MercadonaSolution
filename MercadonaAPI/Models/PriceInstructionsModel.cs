namespace MercadonaAPI.Models;

#pragma warning disable IDE1006 // Naming Styles

public class PriceInstructionsModel
{
    public int iva { get; set; }
    public bool is_new { get; set; }
    public bool is_pack { get; set; }
    public decimal? pack_size { get; set; }
    public string? unit_name { get; set; }
    public decimal? unit_size { get; set; }
    public string? bulk_price { get; set; }
    public string? unit_price { get; set; }
    public bool approx_size { get; set; }
    public string? size_format { get; set; }
    public int? total_units { get; set; }
    public bool unit_selector { get; set; }
    public bool bunch_selector { get; set; }
    public decimal? drained_weight { get; set; }
    public int selling_method { get; set; }
    public bool price_decreased { get; set; }
    public string? reference_price { get; set; }
    public decimal? min_bunch_amount { get; set; }
    public string? reference_format { get; set; }
    public object? previous_unit_price { get; set; }
    public decimal? increment_bunch_amount { get; set; }
}

