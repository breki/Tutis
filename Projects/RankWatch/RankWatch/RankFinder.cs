﻿using System;
using System.Diagnostics;

namespace RankWatch
{
    public class RankFinder
    {
        public RankInfo FindRank(
            GoogleSearchRequestBuilder requestBuilder,
            string searchKeyword, 
            bool endOnFirstFind,
            int maxPages = 10)
        {
            Console.WriteLine("Finding rank for {0}", searchKeyword);

            RankInfo rankInfo = new RankInfo(searchKeyword);

            HtmlScraper scraper = new HtmlScraper ();
            //scraper.LoadFile(@"..\..\..\data\sample-serp.html");
            //scraper.Download (requestBuilder.ConstructUrl("vector map of USA", 0), requestBuilder.GetUserAgent());

            // http://www.google.si/search?gs_rn=19&gs_ri=psy-ab&pq=vector%20map%20of%20usa&cp=17&gs_id=g&xhr=t&q=vector+map+of+usa&es_nrs=true&pf=p&sclient=psy-ab&oq=vector+map+of+usa&gs_l=&pbx=1&biw=1600&bih=1083&cad=cbv&sei=q9_jUayzL8vEtAbcjIHIAQ
            // http://www.google.si/search?q=vector+map+of+usa&safe=off&ei=rt_jUfWEHMnNON6WgZgP&sqi=2&start=10&sa=N&biw=1600&bih=1083

            //scraper.SaveHtml(@"..\..\..\data\sample-serp.html");

            int pageNumber = 0;
            int resultCount = 0;

            bool shouldFinish = false;
            while (!shouldFinish && pageNumber < maxPages)
            {
                requestBuilder.WaitForNextPageQuery ();
                scraper.Download (
                    requestBuilder.ConstructUrl (searchKeyword, pageNumber),
                    requestBuilder.GetUserAgent ());
                scraper.SaveHtml (@"output.html");

                while (true)
                {
                    int foundIndex = scraper.FindNextOneOf ("<div class=\"kv kva", "<div class=\"f kv");
                    if (foundIndex == -1)
                        break;

                    if (foundIndex == 0)
                        continue;

                    // <div class="kv kva ads-visurl">
                    // <div class="f kv" style="white-space: nowrap;">
                    if (scraper.FindNext("<cite"))
                    {
                        if (!scraper.FindNext (">"))
                            throw new InvalidOperationException ("BUG 1");

                        string resultUrl = scraper.ExtractUntil ("</cite>");
                        resultUrl = resultUrl.Replace ("<b>", string.Empty);
                        resultUrl = resultUrl.Replace ("</b>", string.Empty);

                        Console.WriteLine ("{0}: {1}", resultCount + 1, resultUrl);

                        resultCount++;

                        if (resultUrl.StartsWith("scalablemaps.com"))
                        {
                            rankInfo.Ranks.Add(resultCount);
                            if (endOnFirstFind)
                            {
                                shouldFinish = true;
                                break;
                            }
                        }
                    }
                }

                pageNumber++;
            }

            return rankInfo;
        }
    }
}