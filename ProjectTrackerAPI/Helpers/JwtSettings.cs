namespace ProjectTrackerAPI.Helpers
{
    // JWT ayarlar�n� tutan model s�n�f�
    // Bu ayarlar appsettings.json'dan okunur ve JwtHelper taraf�ndan kullan�l�r
    public class JwtSettings
    {
        // JWT'nin imzalanmas� i�in kullan�lan gizli anahtar (En az 256 bit - 32 karakter)
        public string Key { get; set; } = string.Empty;

        // Token'� olu�turan taraf�n ad� (API'nin ad� sunucu)
        public string Issuer { get; set; } = string.Empty;

        // Token'� kullanacak olan uygulaman�n ad�n� Audience ile
        public string Audience { get; set; } = string.Empty;
    }
}
