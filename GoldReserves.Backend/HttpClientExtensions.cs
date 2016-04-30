using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public static class HttpClientExtensions
    {


        internal static async Task<HtmlDocument> GetHtmlDocumentAsync(this HttpClient @this, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 1));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            HttpResponseMessage response;
            try
            {
                response = await @this.SendAsync(request);
            }
            catch (AggregateException ex1)
            {
                List<Exception> ex1InnerEXList_changed = new List<Exception>();
                var ex1InnerEXList = ex1.InnerExceptions;
                bool ex1InnerEXList_containsTimeoutException = false;
                bool ex1InnerEXList_didChange = false;
                for (int i = 0; i < ex1InnerEXList.Count; i++)
                {
                    if (ex1InnerEXList[i] is TimeoutException)
                    {
                        ex1InnerEXList_containsTimeoutException = true;
                        break;
                    }
                }
                for (int i = 0; i < ex1InnerEXList.Count; i++) 
                {
                    var ex1InnerEX = ex1InnerEXList[i];
                    if (ex1InnerEX is TaskCanceledException) 
                    {
                        if (!ex1InnerEXList_containsTimeoutException)
                        {
                            ex1InnerEXList_changed.Add(new TimeoutException());
                            ex1InnerEXList_containsTimeoutException = true;
                        }
                        ex1InnerEXList_didChange = true;
                        continue;
                    }
                    ex1InnerEXList_changed.Add(ex1InnerEX);
                }
                if (ex1InnerEXList_didChange) throw new AggregateException(ex1InnerEXList_changed.ToArray());
                throw;
            }
            catch (Exception ex1)
            {
                if (ex1 is TaskCanceledException)
                {
                    throw new TimeoutException();
                }
                throw;
            }
            response.EnsureSuccessStatusCode();
            Stream stream1 = null;
            try
            {
                var contentType = response.Content.Headers.ContentType;
                if (contentType.MediaType != "text/html")
                {
                    throw new NotImplementedException();
                }
                Encoding contentCharSet = null;
                if (contentType.CharSet != null)
                {
                    if ("UTF-8".Equals(contentType.CharSet, StringComparison.OrdinalIgnoreCase))
                    {
                        contentCharSet = Encoding.UTF8;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                var contentEncoding = response.Content.Headers.ContentEncoding.SingleOrDefault();
                stream1 = await response.Content.ReadAsStreamAsync();
                if (contentEncoding != null)
                {
                    switch (contentEncoding)
                    {
                        case "gzip":
                            stream1 = new GZipStream(stream1, CompressionMode.Decompress, false);
                            break;
                        case "deflate":
                            stream1 = new DeflateStream(stream1, CompressionMode.Decompress, false);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(stream1, contentCharSet, false);
                return htmlDoc;
            }
            finally
            {
                stream1?.Dispose();
            }
        }
    }
}
