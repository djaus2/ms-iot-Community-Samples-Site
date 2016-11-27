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

        //Running from filesystem
        private const string dirr = @"C:\Users\david\Documents\Visual Studio 2015\Projects\NancBlog2\NancBlog2";
        private const string jsonFile = @"Data.json";
        private const string jsonDirr = dirr + @"\Json\";
        private const string MDDB = jsonDirr + jsonFile;
        private const string MD2 = dirr + @"\MD2\";
        private const string MD = dirr + @"\MD\";

        //When deployed on server
        //private const string jsonFile = @"Data.json";
        //private const string jsonDirr = @"~/Json/";
        //private const string MDDB = jsonDirr + jsonFile;
        //private const string MD2 = @"~/MD2/";
        //private const string MD = @"~/MD/";
 

        public IndexModule()
        {
           

            Models.Errors errorMsg = new Models.Errors();
            Get["/"] = _ =>
            {
                return View["default"];
            };
            Get["/ms_iot_Community_Samples"] = _ =>
            {
                bool getList = false;
                if (Models.BlogPost.BlogPostz == null)
                    getList = true;
                else if (Models.BlogPost.BlogPostz.Count() == 0)
                    getList = true;
                if (getList)
                {
                    string[] files1 = Directory.GetFiles( jsonDirr, jsonFile);
                    if (files1.Length != 1)
                        return View["IndexList"];
                    string document = "";
                    document = File.ReadAllText(MDDB);

                    JsonSerializerSettings set = new JsonSerializerSettings();
                    set.MissingMemberHandling = MissingMemberHandling.Ignore;
                    Models.BlogPost[] md = JsonConvert.DeserializeObject<Models.BlogPost[]>(document, set);
                    //var document = converter.GetDocument("DB.json");
                    //var jsonBytes = Encoding.UTF8.GetBytes(document);
                    //return new Response
                    //{
                    //    ContentType = "application/json",
                    //    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                    //};
                    var mdd = from n in md select n;
                    //Objects.BlogPost.BlogPostz = md.Select(data => data)).ToList()
                    Models.BlogPost.BlogPostz = md.ToList<Models.BlogPost>();
                    Models.BlogPost.ResetBlogPostz();
                }
                return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
            };
            Get["/ms_iot_Community_Samples/load"] = _ =>
            {
                //var contentProvider = new FileContentProvider(jsonDirr, null);
                //var converter = new MarkdownService(contentProvider);
                //var document = contentProvider.GetContent("DB");
                string[] files1 = Directory.GetFiles( jsonDirr, jsonFile);
                if (files1.Length != 1)
                    return View["IndexList"];
                string document = "";
                document= File.ReadAllText(MDDB);

                JsonSerializerSettings set = new JsonSerializerSettings();
                set.MissingMemberHandling = MissingMemberHandling.Ignore;
                Models.BlogPost[] md = JsonConvert.DeserializeObject<Models.BlogPost[]>(document, set);
                //var document = converter.GetDocument("DB.json");
                //var jsonBytes = Encoding.UTF8.GetBytes(document);
                //return new Response
                //{
                //    ContentType = "application/json",
                //    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                //};
                var mdd = from n in md select n;
                //Objects.BlogPost.BlogPostz = md.Select(data => data)).ToList()
                Models.BlogPost.BlogPostz = md.ToList<Models.BlogPost>();
                Models.BlogPost.ResetBlogPostz();
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/default"] = _ => {
                return View["default"];
            };
            Get["/ms_iot_Community_Samples/login"] = _ => {
                return View["/ms_iot_Community_Samples/login"];
            };
            Get["/ms_iot_Community_Samples/logout"] = _ => {
                Models.Errors.LoggedInStatus = false;
                return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
            };
            Get["/ms_iot_Community_Samples/onlogin/{user}/{pwd}"] = parameters => {
                string user = parameters.user;
                string pwd = parameters.pwd;
                user = user.Trim();
                pwd = pwd.Trim();
                if ((user == "a") && (pwd == "b"))
                {
                    Models.Errors.LoggedInStatus = true;
                }
                else
                {
                    Models.Errors.LoggedInStatus = false;
                    errorMsg.Message = "Login failed!";
                    errorMsg.Source = "/OnLogin";
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }
                return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
            };
            Get["/ms_iot_Community_Samples/convert"] = _ =>
            {
                if (!Models.Errors.LoggedInStatus)
                {
                    errorMsg.Message = "Not logged in!";
                    errorMsg.Source = "/Convert";
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }
                string[] files0= Directory.GetFiles(jsonDirr, "*.*");

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
                Models.BlogPost.ClearBlogPostz();
                foreach (string file in files)
                {
                    
                    try {
                        string filename = Path.GetFileNameWithoutExtension(file);
                        count--;
                        string fileTxt = File.ReadAllText(file);

                        //Get database between 1st and second lines of ---
                        int startIndex = fileTxt.IndexOf(DBSep,0);
                        if (startIndex < 0)
                            continue;

                        int endIndex = fileTxt.IndexOf(DBSep, startIndex + DBSep.Length);
                        if (endIndex < 0)
                            continue;

                        string DB2 = fileTxt.Substring(startIndex, endIndex - startIndex + DBSep.Length) + "\r\n";
                        string DB = fileTxt.Substring(startIndex + DBSep.Length, endIndex - startIndex - DBSep.Length).Trim();
                        fileTxt = fileTxt.Substring(endIndex+ DBSep.Length );
                        string[] lines = DB.Split(lineSep);
                        Models.BlogPost blogpost = new Models.BlogPost();
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
                                Type type = typeof(Models.BlogPost);
                                //FieldInfo[] fields = type.GetFields(BindingFlags.)
                                    var fields = typeof(Models.BlogPost).GetFields(
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
                        string name = Path.GetFileName(file);
                        File.WriteAllText(MD2 + name, fileTxt);
                        //System.Diagnostics.Debug.WriteLine(DB);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //File.AppendAllText(MDDB, "\r\n]\r\n");
                Models.BlogPost.ResetBlogPostz();
                string json = JsonConvert.SerializeObject(Models.BlogPost.BlogPostz);
                
                File.AppendAllText(MDDB, json);
                return View["/ms_iot_Community_Samples/IndexList"];
            };

            Get["/ms_iot_Community_Samples/display/{name}"] = parameters =>
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

            Get["/ms_iot_Community_Samples/json"] = parameters =>
            {
                Models.BlogPost.ClearBlogPostz();
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


                return View["/ms_iot_Community_Samples/IndexList"];
            };

            Get["/ms_iot_Community_Samples/Home"] = parameters =>
            {
                Models.BlogPost.ResetBlogPostz();
                return View["/ms_iot_Community_Samples/IndexList"];
            };

            Get["/ms_iot_Community_Samples/Sort/{field}"] = parameters =>
            {
                string sortString = parameters.field;
                Models.BlogPost.Sort(sortString);
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/Show/{id}"] = parameters =>
            {
                string id = parameters.id;
                Models.BlogPost blogPost = Models.BlogPost.Get(id);
                if (blogPost != null)
                    return View["/ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/reset"] = _ =>
            {
                Models.BlogPost.ResetBlogPostz();
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/clear"] = _ =>
            {
                Models.BlogPost.ClearBlogPostz();
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/list"] = _ =>
            {
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/Filter"] = _ =>
            {
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/Filter/{filter1}"] = parameters =>
            {
                string filter1 = parameters.filter1;
                return View["IndexList"];
            };
            Get["/ms_iot_Community_Samples/Filter/{filter1}/{filter2}"] = parameters =>
            {
                string filter1 = parameters.filter1;
                string filter2 = parameters.filter2;
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}"] = parameters =>
            {
                return View["/ms_iot_Community_Samples/IndexList"];
            };
            Get["/ms_iot_Community_Samples/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}/{tagsfilter}/{tagsfilter2}"] = parameters =>
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
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
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
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
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
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.codefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.tagsfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
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
                            if (Models.BlogPost.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                if (filters.Count != 0)

                    Models.BlogPost.Filter(filters);

                return View["/ms_iot_Community_Samples/IndexList"];
            };
        }
    }
}