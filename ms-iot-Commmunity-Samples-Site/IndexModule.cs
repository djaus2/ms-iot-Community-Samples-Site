namespace msiotCommunitySamples
{
    using Nancy;
    using Kiwi.Markdown;
    using Kiwi.Markdown.ContentProviders;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Reflection;

    public class IndexModule : NancyModule
    {
        private const string DBSep = "---";
        private const string dirr = @"C:\Users\david\Documents\Visual Studio 2015\Projects\NancBlog2\NancBlog2";
        private const string jsonFile = "Data.json";
        private const string jsonDirr = @"\Json\";
        private const string MDDB = dirr + jsonDirr + jsonFile;
        private const string MD2 = dirr + @"\MD2\";
        private const string MD = dirr + @"\MD\";
        private const string jsonDir = dirr + @"\Json\";

        public IndexModule()
        {
            Get["/"] = _ =>
            {
                //var contentProvider = new FileContentProvider(jsonDirr, null);
                //var converter = new MarkdownService(contentProvider);
                //var document = contentProvider.GetContent("DB");
                string[] files1 = Directory.GetFiles(dirr+ jsonDirr, jsonFile);
                if (files1.Length != 1)
                    return View["IndexList"];
                string document = "";
                document= File.ReadAllText(MDDB);

                JsonSerializerSettings set = new JsonSerializerSettings();
                set.MissingMemberHandling = MissingMemberHandling.Ignore;
                Objects.BlogPost[] md = JsonConvert.DeserializeObject<Objects.BlogPost[]>(document, set);
                //var document = converter.GetDocument("DB.json");
                //var jsonBytes = Encoding.UTF8.GetBytes(document);
                //return new Response
                //{
                //    ContentType = "application/json",
                //    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                //};
                var mdd = from n in md select n;
                //Objects.BlogPost.BlogPostz = md.Select(data => data)).ToList()
                Objects.BlogPost.BlogPostz = md.ToList<Objects.BlogPost>();
                Objects.BlogPost.ResetBlogPostz();
                return View["IndexList"];
            };

            Get["/login"] = _ => {
                return View["login"];
            };
            Get["/convert"] = _ =>
            {
                string[] files0= Directory.GetFiles(dirr + jsonDirr, "*.*");

                foreach (string file in files0)
                {
                    File.Delete(file);
                }

                string[] files1 = Directory.GetFiles(MD2, "*.*");

                foreach (string file in files1)
                {
                    File.Delete(file);
                }

                char[] lineSep = new char[] { '\r', '\n' };
                string[] files = Directory.GetFiles(MD, "*.MD");
                //File.AppendAllText(MDDB, "[\r\n");

                int count = files.Length;
                Objects.BlogPost.ClearBlogPostz();
                foreach (string file in files)
                {
                    
                    try {
                        string filename = Path.GetFileNameWithoutExtension(file);
                        count--;
                        string fileTxt = File.ReadAllText(file);
                        int startIndex = fileTxt.IndexOf(DBSep);
                        if (startIndex < 0)
                            startIndex = 0;
                        int endIndex = fileTxt.IndexOf(DBSep, startIndex + DBSep.Length);
                        string DB2 = fileTxt.Substring(startIndex, endIndex - startIndex + DBSep.Length) + "\r\n";
                        string DB = fileTxt.Substring(startIndex + DBSep.Length, endIndex - startIndex - DBSep.Length).Trim();
                        string[] lines = DB.Split(lineSep);
                        //string db3 = "\"filename\":\"" + filename + "\",\r\n" ;
                        Objects.BlogPost blogpost = new Objects.BlogPost();
                        blogpost.filename = filename;
                        foreach (string line in lines)
                        {
                            string newLine = line.Trim();
                            if (newLine != "")
                            {
                                string[] parts = newLine.Split(new char[] { ':' });
                                string vname = parts[0].Trim();
                                string vvalue = parts[1].Trim();
                                try
                                {
                                blogpost.lang = "\"" + vvalue + "\"";
                                Type type = typeof(Objects.BlogPost);
                                //FieldInfo[] fields = type.GetFields(BindingFlags.)
                                    var fields = typeof(Objects.BlogPost).GetFields(
    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                var field = from n in fields where n.Name.Substring(1).Replace(">k__BackingField", "") == vname select n;
                                if (field.Count()==1)
                                {
                                    field.First().SetValue(blogpost, vvalue);
                                }
                                //if (vname != "samplelink")
                                //    vvalue = parts[1].Trim();
                                //else if (newLine.Contains("http"))
                                //    vvalue = newLine.Substring(newLine.IndexOf("http")).Trim();
                                //else
                                //    vvalue = parts[1].Trim();
                                //newLine = "\"" + vname + "\":\"" + vvalue + "\",";
                            }
                                catch (Exception ex)
                                {
                                }

                                //newLine = "\"" + newLine.Replace(":", "\":\"") + "\",";
                                //db3 += newLine + "\r\n";
                            }


                    }
                        ////Remove trailing comma
                        //db3 = db3.Substring(0, db3.Length - "/r/n".Length + 1);
                        //if (count != 0)
                        //    db3 = "{\r\n" + db3 + "\r\n},\r\n";
                        //else
                        //    db3 = "{\r\n" + db3 + "\r\n}";

                        //File.AppendAllText(MDDB, db3);
                        //fileTxt = fileTxt.Replace(DB2, "");
                        //string name = Path.GetFileName(file);
                        //File.WriteAllText(MD2 + name, fileTxt);
                        //System.Diagnostics.Debug.WriteLine(DB);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //File.AppendAllText(MDDB, "\r\n]\r\n");
                Objects.BlogPost.ResetBlogPostz();
                string json = JsonConvert.SerializeObject(Objects.BlogPost.BlogPostz);
                
                File.AppendAllText(MDDB, json);
                return View["IndexList"];
            };

            Get["/display/{name}"] = parameters =>
            {
                //string startOfDB = "<p>";
                //string endOfDB = "</h2>";
                var contentProvider = new FileContentProvider(MD2, null);
                var converter = new MarkdownService(contentProvider);
                var document = converter.GetDocument(parameters.name);
                //int start = document.Content.IndexOf(startOfDB);
                //int end = document.Content.IndexOf(endOfDB);
                //string DB = document.Content.Substring(start, end - start + endOfDB.Length);
                //document.Content = document.Content.Replace(DB, "");
                //System.Diagnostics.Debug.WriteLine(DB);
                return document.Content;
            };

            Get["/json"] = parameters =>
            {
                Objects.BlogPost.ClearBlogPostz();
                //    //var blogPost = new Objects.BlogPost
                //    //{
                //    //    Id = 2,
                //    //    Title = "dFrom ASP.NET MVC to Nancy - Part 2",
                //    //    Summary = "Lorem ipsum...vvvvvvvvvvvvvvvvvvvvvv Lorem ipsum...vvvvvvvvvvvvvvvvvvvvvv Lorem ipsum...vvvvvvvvvvvvvvvvvvvvvv",
                //    //    Language = "en-us",
                //    //    CodeLanguages = { "C" },
                //    //    Tags = { "c#", "Native", "nancypants" }
                //    //};

                //    //var blogPost2 = new Objects.BlogPost
                //    //{
                //    //    Id = 1,
                //    //    Title = "From ASP.NET MVC to Nancy - Part 3",
                //    //    Summary = "Lorem ipsum...",
                //    //    Language = "en-aus",
                //    //    CodeLanguages = { "C#","C++" },
                //    //    Tags = { "c#", ".NET", "nancy" }
                //    //};
                //    //var blogPost3 = new Objects.BlogPost
                //    //{
                //    //    Id = 3,
                //    //    Title = "aFrom ASP.NET MVC to Nancy - Part 3",
                //    //    Summary = "Lorem ipsum...1234567890123456789012345678901234567890",
                //    //    Language = "en-aus",
                //    //    CodeLanguages = { "C++" },
                //    //    Tags = { "c#", "aspnetmvc", "nance" }
                //    //};

                //    //var blogs = new List<Objects.BlogPost>();
                //    //blogs.Add(blogPost);
                //    //blogs.Add(blogPost2);
                //    //blogs.Add(blogPost3);


                return View["IndexList"];
            };

            Get["/Home"] = parameters =>
            {
                Objects.BlogPost.ResetBlogPostz();
                return View["IndexList"];
            };

            Get["/Sort/{field}"] = parameters =>
            {
                string sortString = parameters.field;
                Objects.BlogPost.Sort(sortString);
                return View["IndexList"];
            };
            Get["/Show/{id}"] = parameters =>
            {
                string id = parameters.id;
                Objects.BlogPost blogPost = Objects.BlogPost.Get(id);
                if (blogPost != null)
                    return View["Index", blogPost];
                else
                    return View["IndexList"];
            };
            Get["/Reset"] = _ =>
            {
                Objects.BlogPost.ResetBlogPostz();
                return View["IndexList"];
            };
            Get["/Clear"] = _ =>
            {
                Objects.BlogPost.ClearBlogPostz();
                return View["IndexList"];
            };
            Get["/List"] = _ =>
            {
                return View["IndexList"];
            };
            Get["/Filter"] = _ =>
            {
                return View["IndexList"];
            };
            Get["/Filter/{filter1}"] = parameters =>
            {
                string filter1 = parameters.filter1;
                return View["IndexList"];
            };
            Get["/Filter/{filter1}/{filter2}"] = parameters =>
            {
                string filter1 = parameters.filter1;
                string filter2 = parameters.filter2;
                return View["IndexList"];
            };
            Get["/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}"] = parameters =>
            {
                return View["IndexList"];
            };
            Get["/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}/{tagsfilter}/{tagsfilter2}"] = parameters =>
            {
                char[] sep = new char[] { '~' };
                string[] tupl;
                var filters = new List<Tuple<string, string>>();
                
                string filter = parameters.idfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                            if (tupl[1] != "")
                                filters.Add(new Tuple<string, string>(tupl[0],tupl[1]));                               
                }
                filter = parameters.titlefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.summaryfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.codefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.tagsfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.tagsfilter2;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Objects.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                if (filters.Count != 0)

                    Objects.BlogPost.Filter(filters);

                return View["IndexList"];
            };
        }
    }
}