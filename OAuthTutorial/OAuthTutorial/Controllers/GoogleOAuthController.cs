using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthTutorial.Helpers;
using OAuthTutorial.Services;
using System;
using System.Threading.Tasks;

namespace OAuthTutorial.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private const string RedirectUrl = "https://localhost:5001/GoogleOAuth/Code";
        private const string YouTubeScope = "https://www.googleapis.com/auth/youtube";
        private const string PkceSessionKey = "codeVerifier";

        public IActionResult RedirectOnOAuthServer()
        {
            // PCKE.
            var codeVerifier = Guid.NewGuid().ToString();
            var codeChellange = Sha256Helper.ComputeHash(codeVerifier);

            HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

            var url = GoogleOAuthService.GenerateOAuthRequestUrl(YouTubeScope, RedirectUrl, codeChellange);
            return Redirect(url);
        }

        public async Task<IActionResult> CodeAsync(string code)
        {
            var codeVerifier = HttpContext.Session.GetString(PkceSessionKey);

            // Увага: токен оновлення надається лише при першій авторизації користувача! 
            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);

            var myChannelId = await YoutubeService.GetMyChannelIdAsync(tokenResult.AccessToken);

            var newDescription = "Цей канал гакнуто!";
            await YoutubeService.UpdateChannelDescriptionAsync(tokenResult.AccessToken, myChannelId, newDescription);

            // Почекаємо 3600 секунд
            // (саме стільки можна використовувати AccessToken, поки його термін придатності не спливе).
            
            // І оновлюємо Токен Доступу за допомогою Refresh-токена.
            var refreshedTokenResult = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);

            return Ok();
        }
    }
}
