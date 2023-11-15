using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace Broker.Common
{
    public class TextPlainInputFormatter : InputFormatter
    {
        private const string ContentType = "text/plain";

        public TextPlainInputFormatter() 
        { 
            SupportedMediaTypes.Add(ContentType);
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var content = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            }
        }

        public override bool CanRead(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;

            return contentType.StartsWith(contentType);
        }
    }
}
