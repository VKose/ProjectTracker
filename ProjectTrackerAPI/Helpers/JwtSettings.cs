namespace ProjectTrackerAPI.Helpers
{
    // JWT ayarlarýný tutan model sýnýfý
    // Bu ayarlar appsettings.json'dan okunur ve JwtHelper tarafýndan kullanýlýr
    public class JwtSettings
    {
        // JWT'nin imzalanmasý için kullanýlan gizli anahtar (En az 256 bit - 32 karakter)
        public string Key { get; set; } = string.Empty;

        // Token'ý oluþturan tarafýn adý (API'nin adý sunucu)
        public string Issuer { get; set; } = string.Empty;

        // Token'ý kullanacak olan uygulamanýn adýný Audience ile
        public string Audience { get; set; } = string.Empty;
    }
}
